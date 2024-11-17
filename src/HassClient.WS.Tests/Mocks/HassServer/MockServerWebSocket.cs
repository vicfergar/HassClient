using Ninja.WebSockets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public abstract class MockServerWebSocket : IDisposable
    {
        private readonly IWebSocketServerFactory webSocketServerFactory = new WebSocketServerFactory();

        private readonly List<TcpClient> activeClients = new List<TcpClient>();

        private TcpListener listener;

        private bool isDisposed = false;

        private TaskCompletionSource<bool> startTCS;

        private Task socketListenerTask;

        public Uri ServerUri { get; private set; }

        public bool IsStarted => this.startTCS?.Task.IsCompleted == true ? this.startTCS.Task.Result : false;

        public MockServerWebSocket()
        {
            var port = this.GetAvailablePortNumber();
            this.ServerUri = new Uri($"ws://127.0.0.1:{port}");
        }

        private int GetAvailablePortNumber()
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            var range = Enumerable.Range(8000, ushort.MaxValue);
            var portsInUse = tcpConnInfoArray.Select(x => x.Port);

            return range.Except(portsInUse).FirstOrDefault();
        }

        public Task StartAsync()
        {
            lock (this.ServerUri)
            {
                if (this.startTCS != null &&
                    (this.startTCS.Task.Status == TaskStatus.Running || this.IsStarted))
                {
                    return this.startTCS.Task;
                }
            }

            this.startTCS = new TaskCompletionSource<bool>();
            this.socketListenerTask = Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        this.listener = new TcpListener(IPAddress.Any, this.ServerUri.Port);
                        this.listener.Start();

                        this.startTCS.SetResult(true);

                        Trace.TraceInformation($"Server started listening on port {this.ServerUri.Port}");
                        while (true)
                        {
                            TcpClient tcpClient = await this.listener.AcceptTcpClientAsync();
                            ProcessTcpClient(tcpClient);
                        }
                    }
                    catch (SocketException ex)
                    {
                        this.startTCS.SetResult(false);

                        string message = string.Format("Error listening on port {0}. Make sure IIS or another application is not running and consuming your port.", this.ServerUri.Port);
                        throw new Exception(message, ex);
                    }
                });

            return this.startTCS.Task;
        }

        private void ProcessTcpClient(TcpClient tcpClient)
        {
            Task.Run(() => this.ProcessTcpClientAsync(tcpClient));
        }

        private async Task ProcessTcpClientAsync(TcpClient tcpClient)
        {
            var source = new CancellationTokenSource();
            this.activeClients.Add(tcpClient);

            try
            {
                if (this.isDisposed)
                {
                    return;
                }

                // this worker thread stays alive until either of the following happens:
                // Client sends a close connection request OR
                // An unhandled exception is thrown OR
                // The server is disposed
                Trace.TraceInformation("Server: Connection opened. Reading HTTP header from stream");

                // get a secure or insecure stream
                var stream = tcpClient.GetStream();
                WebSocketHttpContext context = await this.webSocketServerFactory.ReadHttpHeaderFromStreamAsync(stream);
                if (context.IsWebSocketRequest)
                {
                    var options = new WebSocketServerOptions() { KeepAliveInterval = TimeSpan.MaxValue };
                    Trace.TraceInformation("HTTP header has requested an upgrade to Web Socket protocol. Negotiating Web Socket handshake");

                    var webSocket = await this.webSocketServerFactory.AcceptWebSocketAsync(context, options);

                    Trace.TraceInformation("Web Socket handshake response sent. Stream ready.");
                    await RespondToWebSocketRequestAsync(webSocket, source.Token);
                }
                else
                {
                    Trace.TraceInformation("HTTP header contains no web socket upgrade request. Ignoring");
                }

                Trace.TraceInformation("Server: Connection closed");
            }
            catch (ObjectDisposedException)
            {
                // Do nothing. This will be thrown if the Listener has been stopped
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
            finally
            {
                this.activeClients.Remove(tcpClient);

                try
                {
                    tcpClient.Client.Close();
                    tcpClient.Close();
                    source.Cancel();
                }
                catch (Exception ex)
                {
                    Trace.TraceError($"Failed to close TCP connection: {ex}");
                }
            }
        }

        protected abstract Task RespondToWebSocketRequestAsync(WebSocket webSocket, CancellationToken token);

        /// <summary>
        /// Simulates a server shutdown by closing all active clients.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task CloseActiveClientsAsync()
        {
            foreach (var client in this.activeClients.ToArray())
            {
                client.Close();
                this.activeClients.Remove(client);
            }

            // Wait to clients notices disconnection
            return Task.Delay(1000);
        }

        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;

                // safely attempt to shut down the listener
                try
                {
                    if (this.listener != null)
                    {
                        if (this.listener.Server != null)
                        {
                            this.listener.Server.Close();
                        }

                        this.listener.Stop();
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }

                Trace.TraceInformation("Web Server disposed");
            }
        }
    }
}
