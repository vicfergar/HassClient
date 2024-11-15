using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Tests.Mocks;
using NUnit.Framework;
using System.Threading.Tasks;
using static HassClient.WS.Tests.Mocks.MockEventListener;

namespace HassClient.WS.Tests
{
    public class SubscriptionApiTests : BaseHassWSApiTest
    {
        private const string testEntityId = "light.ceiling_lights";

        private async Task<EventData<StateChangedEvent>> ForceStateChangedAndGetEventData(MockEventListener listener)
        {
            var domain = testEntityId.GetDomain();
            var update = await this.hassWSApi.Services.CallForEntitiesAsync(domain, "toggle", testEntityId);
            Assert.NotNull(update, "SetUp failed");

            var eventData = await listener.WaitFirstEventWithTimeoutAsync<EventResultInfo>(
                                            (x) => HassSerializer.TryGetEnumFromSnakeCase<KnownEventTypes>(x.EventType, out var knownEventType) &&
                                                   knownEventType == KnownEventTypes.StateChanged,
                                            500);

            Assert.NotNull(eventData, "SetUp failed");

            var args = eventData.Args.DeserializeData<StateChangedEvent>();
            return new EventData<StateChangedEvent>(eventData.Sender, args);
        }

        [Test]
        public async Task AddMultipleEventHandlerSubscriptionForAnyEvent()
        {
            var listener1 = new MockEventListener();
            var listener2 = new MockEventListener();
            var result1 = await this.hassWSApi.AddEventHandlerSubscriptionAsync(listener1.Handle);
            var result2 = await this.hassWSApi.AddEventHandlerSubscriptionAsync(listener2.Handle);

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);

            var eventData = await this.ForceStateChangedAndGetEventData(listener1);

            Assert.NotZero(listener1.HitCount);
            Assert.AreEqual(listener1.HitCount, listener2.HitCount);
            Assert.NotNull(eventData);
            Assert.AreEqual(eventData.Sender, this.hassWSApi.WebSocket);
            Assert.IsTrue(eventData.Args.EntityId == testEntityId);
            Assert.NotNull(eventData.Args.NewState.State);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForAnyEvent()
        {
            var listener = new MockEventListener();
            var result = await this.hassWSApi.AddEventHandlerSubscriptionAsync(listener.Handle);

            Assert.IsTrue(result);

            var eventData = await this.ForceStateChangedAndGetEventData(listener);

            Assert.NotZero(listener.HitCount);
            Assert.NotNull(eventData);
            Assert.AreEqual(eventData.Sender, this.hassWSApi.WebSocket);
            Assert.IsTrue(eventData.Args.EntityId == testEntityId);
            Assert.NotNull(eventData.Args.NewState.State);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForStateChangedEvents()
        {
            var listener = new MockEventListener();
            var result = await this.hassWSApi.AddEventHandlerSubscriptionAsync(listener.Handle, KnownEventTypes.StateChanged);

            Assert.IsTrue(result);

            var eventData = await this.ForceStateChangedAndGetEventData(listener);

            Assert.NotZero(listener.HitCount);
            Assert.NotNull(eventData);
            Assert.AreEqual(eventData.Sender, this.hassWSApi.WebSocket);
            Assert.IsTrue(eventData.Args.EntityId == testEntityId);
            Assert.NotNull(eventData.Args.NewState.State);
        }
    }
}
