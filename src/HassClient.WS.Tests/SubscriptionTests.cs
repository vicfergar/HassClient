using HassClient.Helpers;
using HassClient.Models;
using HassClient.Serialization;
using HassClient.WS.Tests.Mocks;
using HassClient.WS.Messages;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    [TestFixture(true, TestName = nameof(SubscriptionTests) + "WithFakeServer")]
    [TestFixture(false, TestName = nameof(SubscriptionTests) + "WithRealServer")]
    public class SubscriptionTests : BaseHassWSApiTest
    {
        private const string testEntitytId = "light.ceiling_lights";

        public SubscriptionTests(bool useFakeHassServer)
            : base(useFakeHassServer)
        {
        }

        private async Task<StateChangedEvent> ForceStateChangedAndGetEventData(MockEventSubscriber subscriber)
        {
            if (this.hassServer != null)
            {
                var succeed = await this.hassServer.RaiseStateChangedEventAsync(testEntitytId);
                Assert.IsTrue(succeed, "SetUp failed");
            }
            else
            {
                var domain = testEntitytId.GetDomain();
                var update = await this.hassWSApi.CallServiceForEntitiesAsync(domain, "toggle", testEntitytId);
                Assert.NotNull(update, "SetUp failed");
            }

            var result = await subscriber.WaitHitWithTimeoutAsync(500);

            if (!result)
            {
                return null;
            }

            var eventInfo = subscriber.ReceivedEventArgs.FirstOrDefault(
                x => x is EventResultInfo erv &&
                HassSerializer.TryGetEnumFromSnakeCase<KnownEventTypes>(erv.EventType, out var knownEventType) &&
                knownEventType == KnownEventTypes.StateChanged) as EventResultInfo;

            Assert.NotNull(eventInfo, "SetUp failed");

            return eventInfo.DeserializeData<StateChangedEvent>();
        }

        [Test]
        public async Task AddMultipleEventHandlerSubscriptionForAnyEvent()
        {
            var testEventHandler1 = new MockEventHandler<EventResultInfo>();
            var testEventHandler2 = new MockEventHandler<EventResultInfo>();
            var subscriber1 = new MockEventSubscriber();
            var subscriber2 = new MockEventSubscriber();
            testEventHandler1.Event += subscriber1.Handle;
            testEventHandler2.Event += subscriber2.Handle;
            var result1 = await this.hassWSApi.AddEventHandlerSubscriptionAsync(testEventHandler1.EventHandler);
            var result2 = await this.hassWSApi.AddEventHandlerSubscriptionAsync(testEventHandler2.EventHandler);

            Assert.IsTrue(result1);
            Assert.IsTrue(result2);

            var eventData = await this.ForceStateChangedAndGetEventData(subscriber1);

            Assert.NotZero(subscriber1.HitCount);
            Assert.AreEqual(subscriber1.HitCount, subscriber2.HitCount);
            Assert.IsTrue(eventData.EntityId == testEntitytId);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForAnyEvent()
        {
            var testEventHandler = new MockEventHandler<EventResultInfo>();
            var subscriber = new MockEventSubscriber();
            testEventHandler.Event += subscriber.Handle;
            var result = await this.hassWSApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler);

            Assert.IsTrue(result);

            await this.ForceStateChangedAndGetEventData(subscriber);

            Assert.NotZero(subscriber.HitCount);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForStateChangedEvents()
        {
            var testEventHandler = new MockEventHandler<EventResultInfo>();
            var subscriber = new MockEventSubscriber();
            testEventHandler.Event += subscriber.Handle;
            var result = await this.hassWSApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler, KnownEventTypes.StateChanged);

            Assert.IsTrue(result);

            var eventData = await this.ForceStateChangedAndGetEventData(subscriber);

            Assert.NotZero(subscriber.HitCount);
            Assert.IsTrue(eventData.EntityId == testEntitytId);
            Assert.NotNull(eventData.NewState.State);
        }
    }
}
