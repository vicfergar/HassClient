using Ninja.WebSockets;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace HassClient.WS.Tests.Mocks.HassServer
{
    public abstract class MockServerWebSocket : IDisposable
    {
        private TcpListener _listener;
        private bool _isDisposed = false;

        private readonly IWebSocketServerFactory _webSocketServerFactory;

        private Task socketListenerTask;

        private TaskCompletionSource<bool> startTCS;

        public Task StartTask => this.startTCS?.Task;

        public Uri ServerUri { get; private set; }

        public bool IsStarted => this.startTCS?.Task.IsCompleted == true && this.startTCS.Task.Result;

        public MockServerWebSocket()
        {
            _webSocketServerFactory = new WebSocketServerFactory();

            var port = RandomNumberGenerator.GetInt32(8000, ushort.MaxValue);
            this.ServerUri = new Uri($"ws://127.0.0.1:{port}");
        }

        public void Start()
        {
            this.startTCS = new TaskCompletionSource<bool>();
            this.socketListenerTask = Task.Factory.StartNew(
                async () =>
                {
                    try
                    {
                        _listener = new TcpListener(IPAddress.Any, this.ServerUri.Port);
                        _listener.Start();

                        this.startTCS.SetResult(true);

                        //_logger.LogInformation($"Server started listening on port {port}");
                        while (true)
                        {
                            TcpClient tcpClient = await _listener.AcceptTcpClientAsync();
                            ProcessTcpClient(tcpClient);
                        }
                    }
                    catch (SocketException ex)
                    {
                        string message = string.Format("Error listening on port {0}. Make sure IIS or another application is not running and consuming your port.", this.ServerUri.Port);
                        throw new Exception(message, ex);
                    }
                    finally
                    {
                        this.startTCS.SetResult(false);
                    }
                });
        }

        private void ProcessTcpClient(TcpClient tcpClient)
        {
            Task.Run(() => ProcessTcpClientAsync(tcpClient));
        }

        private async Task ProcessTcpClientAsync(TcpClient tcpClient)
        {
            CancellationTokenSource source = new CancellationTokenSource();

            try
            {
                if (_isDisposed)
                {
                    return;
                }

                // this worker thread stays alive until either of the following happens:
                // Client sends a close conection request OR
                // An unhandled exception is thrown OR
                // The server is disposed
                //_logger.LogInformation("Server: Connection opened. Reading Http header from stream");

                // get a secure or insecure stream
                var stream = tcpClient.GetStream();
                WebSocketHttpContext context = await _webSocketServerFactory.ReadHttpHeaderFromStreamAsync(stream);
                if (context.IsWebSocketRequest)
                {
                    var options = new WebSocketServerOptions() { KeepAliveInterval = TimeSpan.MaxValue };
                    //_logger.LogInformation("Http header has requested an upgrade to Web Socket protocol. Negotiating Web Socket handshake");

                    WebSocket webSocket = await _webSocketServerFactory.AcceptWebSocketAsync(context, options);

                    //_logger.LogInformation("Web Socket handshake response sent. Stream ready.");
                    await RespondToWebSocketRequestAsync(webSocket, source.Token);
                }
                else
                {
                    //_logger.LogInformation("Http header contains no web socket upgrade request. Ignoring");
                }

                //_logger.LogInformation("Server: Connection closed");
            }
            catch (ObjectDisposedException)
            {
                // do nothing. This will be thrown if the Listener has been stopped
            }
            catch (Exception)
            {
                //_logger.LogError(ex.ToString());
            }
            finally
            {
                try
                {
                    tcpClient.Client.Close();
                    tcpClient.Close();
                    source.Cancel();
                }
                catch (Exception)
                {
                    //_logger.LogError($"Failed to close TCP connection: {ex}");
                }
            }
        }

        protected abstract Task RespondToWebSocketRequestAsync(WebSocket webSocket, CancellationToken token);

        public void Dispose()
        {
            if (!_isDisposed)
            {
                _isDisposed = true;

                // safely attempt to shut down the listener
                try
                {
                    if (_listener != null)
                    {
                        if (_listener.Server != null)
                        {
                            _listener.Server.Close();
                        }

                        _listener.Stop();
                    }
                }
                catch (Exception)
                {
                    //_logger.LogError(ex.ToString());
                }

                //_logger.LogInformation("Web Server disposed");
            }
        }
    }
}
