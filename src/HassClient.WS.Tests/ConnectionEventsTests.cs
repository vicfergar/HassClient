using HassClient.Models;
using HassClient.WS.Tests.Mocks;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class ConnectionEventsTests : BaseHassWSApiTest
    {
        private MockEventListener connectionChangedListener;

        public ConnectionEventsTests()
        {
            this.connectionChangedListener = new MockEventListener();
            this.hassWSApi.ConnectionStateChanged += this.connectionChangedListener.Handle;
        }
        
        [Test, Order(1)]
        public void ConnectionStatusChangedRaisedWhenConnecting()
        {
            Assert.AreEqual(ConnectionStates.Connected, this.hassWSApi.ConnectionState);

            Assert.AreEqual(3, this.connectionChangedListener.HitCount);
            Assert.AreEqual(new[] { ConnectionStates.Connecting, ConnectionStates.Authenticating, ConnectionStates.Connected }, this.connectionChangedListener.ReceivedEventArgs);
            Assert.IsTrue(this.connectionChangedListener.ReceivedEvents.All(e => this.hassWSApi.WebSocket == e.Sender));
        }

        [Test, Order(2)]
        public async Task ConnectionStatusChangedRaisedWhenClosing()
        {
            Assert.AreEqual(ConnectionStates.Connected, this.hassWSApi.ConnectionState);
            this.connectionChangedListener.Reset();
            await this.hassWSApi.CloseAsync();
            Assert.AreEqual(ConnectionStates.Disconnected, this.hassWSApi.ConnectionState);

            Assert.AreEqual(1, this.connectionChangedListener.HitCount);
            Assert.AreEqual(ConnectionStates.Disconnected, this.connectionChangedListener.ReceivedEventArgs.FirstOrDefault());
            Assert.AreEqual(this.hassWSApi.WebSocket, connectionChangedListener.ReceivedEvents.FirstOrDefault().Sender);
        }
    }
}
