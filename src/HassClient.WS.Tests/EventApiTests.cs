using HassClient.Helpers;
using HassClient.Models;
using HassClient.WS.Messages;
using HassClient.WS.Tests.Mocks;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace HassClient.WS.Tests
{
    public class EventApiTests : BaseHassWSApiTest
    {
        private Task<MockEventSubscriber> CreateSubscriberAsync(KnownEventTypes eventType)
        {
            return this.CreateSubscriberAsync(eventType.ToEventTypeString());
        }

        private async Task<MockEventSubscriber> CreateSubscriberAsync(string eventType)
        {
            var subscriber = new MockEventSubscriber();
            var testEventHandler = new MockEventHandler<EventResultInfo>();
            testEventHandler.Event += subscriber.Handle;
            var result = await this.hassWSApi.AddEventHandlerSubscriptionAsync(testEventHandler.EventHandler, eventType);
            Assert.IsTrue(result, "Error while creating the subscriber");

            return subscriber;
        }

        private async Task WaitForEventAsync<T>(MockEventSubscriber subscriber, string eventType, Func<T, bool> predicate = null)
        {
            var eventResultInfo = await subscriber.WaitFirstEventArgWithTimeoutAsync<EventResultInfo>(
                                            (x) => x.EventType == eventType &&
                                                   (predicate?.Invoke(x.DeserializeData<T>()) ?? true),
                                            500);

            Assert.NotNull(eventResultInfo, "Event not received");
            Assert.AreEqual(1, subscriber.HitCount, "Event not received");
        }

        [Test]
        public async Task FireKnownEventType()
        {
            var testEventType = KnownEventTypes.LovelaceUpdated;
            var subscriber = await CreateSubscriberAsync(testEventType);

            var result = await hassWSApi.FireEventAsync(testEventType);

            Assert.IsTrue(result);

            await this.WaitForEventAsync<object>(subscriber, testEventType.ToEventTypeString());
        }

        [Test]
        public async Task FireKnownEventTypeWithData()
        {
            var testEventType = KnownEventTypes.StateChanged;
            var subscriber = await CreateSubscriberAsync(testEventType);

            var testEntityId = "testdomain.entity";
            var result = await this.hassWSApi.FireEventAsync(testEventType, new
            {
                EntityId = testEntityId,
                OldState = (StateModel)null,
                NewState = (StateModel)null,

            });

            Assert.IsTrue(result);

            await this.WaitForEventAsync<StateChangedEvent>(subscriber, testEventType.ToEventTypeString(), x => x.EntityId == testEntityId);
        }

        [Test]
        public async Task FireCustomEventType()
        {
            var testEventType = "mydomain_event";
            var subscriber = await CreateSubscriberAsync(testEventType);

            var result = await this.hassWSApi.FireEventAsync(testEventType);

            await this.WaitForEventAsync<object>(subscriber, testEventType);
        }

        class TestEventData
        {
            public string DeviceId;

            public string Type;
        }

        [Test]
        public async Task FireCustomEventTypeWithData()
        {
            var testEventType = "mydomain_event";
            var subscriber = await CreateSubscriberAsync(testEventType);

            var testDeviceId = "my-device-id";
            var testType = "motion_detected";
            var result = await this.hassWSApi.FireEventAsync(
                testEventType,
                new TestEventData
                {
                    DeviceId = testDeviceId,
                    Type = testType
                });

            Assert.IsTrue(result);

            await this.WaitForEventAsync<TestEventData>(subscriber, testEventType, x => x.DeviceId == testDeviceId && x.Type == testType);
        }

        [Test]
        public async Task AddEventHandlerSubscriptionForAnyEvent()
        {
            var subscriber = await this.CreateSubscriberAsync(KnownEventTypes.Any);

            var testEventType = "mydomain_event";
            var result = await this.hassWSApi.FireEventAsync(testEventType);

            Assert.True(result);

            await this.WaitForEventAsync<object>(subscriber, testEventType);

            Assert.NotZero(subscriber.HitCount);
        }

        [Test]
        public async Task AddMultipleEventHandlerSubscriptionForAnyEvent()
        {
            var subscriber1 = await this.CreateSubscriberAsync(KnownEventTypes.Any);
            var subscriber2 = await this.CreateSubscriberAsync(KnownEventTypes.Any);

            var testEventType = "mydomain_event";
            var result = await this.hassWSApi.FireEventAsync(testEventType);

            Assert.True(result);

            await this.WaitForEventAsync<object>(subscriber1, testEventType);

            Assert.NotZero(subscriber1.HitCount);
            Assert.AreEqual(subscriber1.HitCount, subscriber2.HitCount);
        }
    }
}
