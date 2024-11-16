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
            Assert.NotNull(update, "SetUp failed. Service call failed");

            var eventData = await listener.WaitFirstEventWithTimeoutAsync<EventResultInfo>(
                    (x) => HassSerializer.TryGetEnumFromSnakeCase<KnownEventTypes>(x.EventType, out var knownEventType) &&
                            knownEventType == KnownEventTypes.StateChanged,
                    millisecondsTimeout: 500);

            if (eventData == null)
            {
                return null;
            }

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

        [Test]
        public async Task RemoveEventHandlerSubscription_RemovesSingleHandler()
        {
            var listener = new MockEventListener();
            var result = await this.hassWSApi.AddEventHandlerSubscriptionAsync(listener.Handle);
            Assert.IsTrue(result);

            var removeResult = await this.hassWSApi.RemoveEventHandlerSubscriptionAsync(listener.Handle);
            Assert.IsTrue(removeResult);

            var eventsHitAfterUnsubscribe = listener.HitCount;

            // Force a state change and verify the listener doesn't receive it
            await this.ForceStateChangedAndGetEventData(new MockEventListener());
            Assert.AreEqual(listener.HitCount, eventsHitAfterUnsubscribe);
        }

        [Test]
        public async Task RemoveEventHandlerSubscription_RemovesSpecificEventTypeHandler()
        {
            var listener = new MockEventListener();
            var result = await this.hassWSApi.AddEventHandlerSubscriptionAsync(listener.Handle, KnownEventTypes.StateChanged);
            Assert.IsTrue(result);

            var removeResult = await this.hassWSApi.RemoveEventHandlerSubscriptionAsync(listener.Handle, KnownEventTypes.StateChanged);
            Assert.IsTrue(removeResult);

            // Force a state change and verify the listener doesn't receive it
            await this.ForceStateChangedAndGetEventData(new MockEventListener());
            Assert.Zero(listener.HitCount);
        }

        [Test]
        public async Task RemoveEventHandlerSubscription_RemovesOneOfMultipleHandlers()
        {
            var listener1 = new MockEventListener();
            var listener2 = new MockEventListener();
            await this.hassWSApi.AddEventHandlerSubscriptionAsync(listener1.Handle);
            await this.hassWSApi.AddEventHandlerSubscriptionAsync(listener2.Handle);

            var removeResult = await this.hassWSApi.RemoveEventHandlerSubscriptionAsync(listener1.Handle);
            Assert.IsTrue(removeResult);

            // Force a state change and verify only listener2 receives it
            var eventData = await this.ForceStateChangedAndGetEventData(listener2);
            Assert.Zero(listener1.HitCount);
            Assert.NotZero(listener2.HitCount);
        }

        [Test]
        public async Task RemoveEventHandlerSubscription_ReturnsFalseForNonexistentHandler()
        {
            var listener = new MockEventListener();
            var removeResult = await this.hassWSApi.RemoveEventHandlerSubscriptionAsync(listener.Handle);
            Assert.IsFalse(removeResult);
        }
    }
}
