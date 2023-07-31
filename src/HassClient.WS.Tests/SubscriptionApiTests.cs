using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Messages;
using HassClient.WS.Tests.Mocks;
using NUnit.Framework;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class SubscriptionApiTests : BaseHassWSApiTest
    {
        private const string testEntitytId = "light.ceiling_lights";

        private async Task<StateChangedEvent> ForceStateChangedAndGetEventData(MockEventSubscriber subscriber)
        {
            string domain = testEntitytId.GetDomain();
            bool update = await this.hassWSApi.CallServiceForEntitiesAsync(domain, "toggle", testEntitytId);
            Assert.NotNull(update, "SetUp failed");

            EventResultInfo eventResultInfo = await subscriber.WaitFirstEventArgWithTimeoutAsync<EventResultInfo>(
                                            (x) => HassSerializer.TryGetEnumFromSnakeCase<KnownEventTypes>(x.EventType, out KnownEventTypes knownEventType) &&
                                                   knownEventType == KnownEventTypes.StateChanged,
                                            500);

            Assert.NotNull(eventResultInfo, "SetUp failed");

            return eventResultInfo.DeserializeData<StateChangedEvent>();
        }

        [Test]
        public async Task AddMultipleEventHandlerSubscriptionForAnyEvent()
        {
            MockEventHandler<EventResultInfo> testEventHandler1 = new();
            MockEventHandler<EventResultInfo> testEventHandler2 = new();
            MockEventSubscriber subscriber1 = new();
            MockEventSubscriber subscriber2 = new();
            testEventHandler1.Event += subscriber1.Handle;
            testEventHandler2.Event += subscriber2.Handle;
            bool result1 = await this.hassWSApi.AddEventHandlerSubscriptionAsync(testEventHandler1.EventHandler);
            bool result2 = await this.hassWSApi.AddEventHandlerSubscriptionAsync(testEventHandler2.EventHandler);

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);

            StateChangedEvent eventData = await this.ForceStateChangedAndGetEventData(subscriber1);

            Assert.NotZero(subscriber1.HitCount);
            Assert.AreEqual(subscriber1.HitCount, subscriber2.HitCount);
            Assert.IsTrue(eventData.EntityId == testEntitytId);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForAnyEvent()
        {
            MockEventHandler<EventResultInfo> testEventHandler = new();
            MockEventSubscriber subscriber = new();
            testEventHandler.Event += subscriber.Handle;
            bool result = await this.hassWSApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler);

            Assert.IsTrue(result);

            await this.ForceStateChangedAndGetEventData(subscriber);

            Assert.NotZero(subscriber.HitCount);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForStateChangedEvents()
        {
            MockEventHandler<EventResultInfo> testEventHandler = new();
            MockEventSubscriber subscriber = new();
            testEventHandler.Event += subscriber.Handle;
            bool result = await this.hassWSApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler, KnownEventTypes.StateChanged);

            Assert.IsTrue(result);

            StateChangedEvent eventData = await this.ForceStateChangedAndGetEventData(subscriber);

            Assert.NotZero(subscriber.HitCount);
            Assert.IsTrue(eventData.EntityId == testEntitytId);
            Assert.NotNull(eventData.NewState.State);
        }

        [Test]
        public async Task StateChagedEventListenerShouldSubscribeEntityStatusChanged()
        {
            MockEventHandler<StateChangedEvent> testEventHandler = new();
            MockEventSubscriber subscriber = new();
            testEventHandler.Event += subscriber.Handle;
            this.hassWSApi.StateChagedEventListener.SubscribeEntityStatusChanged(testEntitytId, testEventHandler.EventHandler);

            StateChangedEvent eventData = await this.ForceStateChangedAndGetEventData(subscriber);

            Assert.NotZero(subscriber.HitCount);
            Assert.IsTrue(eventData.EntityId == testEntitytId);
            Assert.NotNull(eventData.NewState.State);
        }
    }
}
