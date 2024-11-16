using HassClient.Helpers;
using HassClient.Models;
using HassClient.WS.Tests.Mocks;
using NUnit.Framework;
using System.Threading.Tasks;
using static HassClient.WS.Tests.Mocks.MockEventListener;

namespace HassClient.WS.Tests
{
    public class StateChangedEventListenerTests : BaseHassWSApiTest
    {
        private const string testLightEntityId1 = "light.ceiling_lights";

        private const string testLightEntityId2 = "light.office_rgbw_lights";

        private const string testSwitchEntityId1 = "switch.decorative_lights";

        private const string testSwitchEntityId2 = "switch.ac";

        private async Task<EventData<StateChangedEvent>> ForceStateChangedAndGetEventData(string entityId, MockEventListener listener)
        {
            var domain = entityId.GetDomain();
            var update = await this.hassWSApi.Services.CallForEntitiesAsync(domain, "toggle", entityId);
            Assert.NotNull(update, "SetUp failed. Service call failed");

            var eventData = await listener.WaitFirstEventWithTimeoutAsync<StateChangedEvent>(millisecondsTimeout: 500);
            return eventData;
        }

        [Test]
        public async Task SubscribeDomainStatusChanged_WhenEntityInDomainChanges_NotifiesCorrectly()
        {
            // Arrange
            var listener = new MockEventListener();
            var testDomain = testLightEntityId1.GetDomain();
            this.hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged(testDomain, listener.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act
            var eventData = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener);

            Assert.NotZero(listener.HitCount);
            Assert.NotNull(eventData);
            Assert.AreEqual(eventData.Sender, this.hassWSApi.StateChangedEventListener);
            Assert.AreEqual(eventData.Args.EntityId.GetDomain(), testDomain);
            Assert.NotNull(eventData.Args.NewState.State);
        }

        [Test]
        public async Task SubscribeDomainStatusChanged_MultipleSubscriptions_NotifiesCorrectly()
        {
            // Arrange
            var listener1 = new MockEventListener();
            var listener2 = new MockEventListener();
            var testDomain1 = testLightEntityId1.GetDomain();
            var testDomain2 = testSwitchEntityId1.GetDomain();
            this.hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged(testDomain1, listener1.Handle);
            this.hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged(testDomain2, listener2.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act
            var eventData1 = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener1);
            var eventData2 = await this.ForceStateChangedAndGetEventData(testSwitchEntityId1, listener2);

            Assert.NotZero(listener1.HitCount);
            Assert.NotZero(listener2.HitCount);
            Assert.NotNull(eventData1);
            Assert.NotNull(eventData2);
            Assert.AreEqual(eventData1.Sender, this.hassWSApi.StateChangedEventListener);
            Assert.AreEqual(eventData2.Sender, this.hassWSApi.StateChangedEventListener);
            Assert.AreEqual(eventData1.Args.EntityId.GetDomain(), testDomain1);
            Assert.AreEqual(eventData2.Args.EntityId.GetDomain(), testDomain2);
            Assert.NotNull(eventData1.Args.NewState.State);
            Assert.NotNull(eventData2.Args.NewState.State);
        }

        [Test]
        public async Task SubscribeDomainStatusChanged_WhenEntityNotInDomainChanges_DoesNotNotify()
        {
            // Arrange
            var listener = new MockEventListener();
            var testDomain = testLightEntityId1.GetDomain();
            this.hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged(testDomain, listener.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act
            var otherDomainEntityId = testSwitchEntityId1;
            var eventData = await this.ForceStateChangedAndGetEventData(otherDomainEntityId, listener);

            Assert.Zero(listener.HitCount);
            Assert.Null(eventData);
        }
        
        [Test]
        public async Task SubscribeMultipleListenersSameDomain_WhenDomainEntityChanges_NotifiesAllListeners()
        {
            // Arrange
            var listener1 = new MockEventListener();
            var listener2 = new MockEventListener();
            var listener3 = new MockEventListener();
            var testDomain = testLightEntityId1.GetDomain();
            
            this.hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged(testDomain, listener1.Handle);
            this.hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged(testDomain, listener2.Handle);
            this.hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged(testDomain, listener3.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act
            var eventData = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener1);

            // Assert
            Assert.NotNull(eventData);
            Assert.NotZero(listener1.HitCount);
            Assert.NotZero(listener2.HitCount);
            Assert.NotZero(listener3.HitCount);
        }

        [Test]
        public async Task UnsubscribeDomainStatusChanged_WhenEntityInDomainChanges_DoesNotNotify()
        {
            // Arrange
            var listener = new MockEventListener();
            var testDomain = testLightEntityId1.GetDomain();
            this.hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged(testDomain, listener.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act - First change should be received
            var eventData = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener);
            Assert.NotNull(eventData, "First event should be received");

            // Unsubscribe and try again
            this.hassWSApi.StateChangedEventListener.UnsubscribeDomainStatusChanged(testDomain, listener.Handle);
            
            listener.Reset();
            var secondEventData = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener);

            // Assert
            Assert.Zero(listener.HitCount, "Should not receive events after unsubscribe");
            Assert.Null(secondEventData);
        }

        [Test]
        public async Task UnsubscribeOneOfMultipleDomainListeners_WhenDomainEntityChanges_OnlyNotifiesRemainingListeners()
        {
            // Arrange
            var listener1 = new MockEventListener();
            var listener2 = new MockEventListener();
            var testDomain = testLightEntityId1.GetDomain();
            
            this.hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged(testDomain, listener1.Handle);
            this.hassWSApi.StateChangedEventListener.SubscribeDomainStatusChanged(testDomain, listener2.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act
            this.hassWSApi.StateChangedEventListener.UnsubscribeDomainStatusChanged(testDomain, listener1.Handle);
            
            var eventData = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener2);

            // Assert
            Assert.NotNull(eventData);
            Assert.Zero(listener1.HitCount);
            Assert.NotZero(listener2.HitCount);
        }

        [Test]
        public async Task SubscribeEntityStatusChanged_WhenEntityChanges_NotifiesCorrectly()
        {
            // Arrange
            var listener = new MockEventListener();
            this.hassWSApi.StateChangedEventListener
                .SubscribeEntityStatusChanged(testLightEntityId1, listener.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act
            var eventData = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener);

            Assert.NotZero(listener.HitCount);
            Assert.NotNull(eventData);
            Assert.AreEqual(eventData.Sender, this.hassWSApi.StateChangedEventListener);
            Assert.IsTrue(eventData.Args.EntityId == testLightEntityId1);
            Assert.NotNull(eventData.Args.NewState.State);
        }

        [Test]
        public async Task SubscribeEntityStatusChanged_WhenOtherEntityChanges_DoesNotNotify()
        {
            // Arrange
            var listener = new MockEventListener();
            this.hassWSApi.StateChangedEventListener
                .SubscribeEntityStatusChanged(testLightEntityId1, listener.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act
            var otherEntityId = testSwitchEntityId1;
            var eventData = await this.ForceStateChangedAndGetEventData(otherEntityId, listener);

            Assert.Zero(listener.HitCount);
            Assert.Null(eventData);
        }

        [Test]
        public async Task SubscribeEntityStatusChanged_MultipleSubscriptions_NotifiesCorrectly()
        {
            // Arrange
            var listener1 = new MockEventListener();
            var listener2 = new MockEventListener();
            this.hassWSApi.StateChangedEventListener
                .SubscribeEntityStatusChanged(testLightEntityId1, listener1.Handle);
            this.hassWSApi.StateChangedEventListener
                .SubscribeEntityStatusChanged(testSwitchEntityId2, listener2.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act
            var eventData1 = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener1);
            var eventData2 = await this.ForceStateChangedAndGetEventData(testSwitchEntityId2, listener2);

            Assert.NotZero(listener1.HitCount);
            Assert.NotZero(listener2.HitCount);
            Assert.NotNull(eventData1);
            Assert.NotNull(eventData2);
            Assert.AreEqual(eventData1.Sender, this.hassWSApi.StateChangedEventListener);
            Assert.AreEqual(eventData2.Sender, this.hassWSApi.StateChangedEventListener);
            Assert.IsTrue(eventData1.Args.EntityId == testLightEntityId1);
            Assert.IsTrue(eventData2.Args.EntityId == testSwitchEntityId2);
            Assert.NotNull(eventData1.Args.NewState.State);
            Assert.NotNull(eventData2.Args.NewState.State);
        }

        [Test]
        public async Task SubscribeMultipleListenersToSameEntity_WhenEntityChanges_NotifiesAllListeners()
        {
            // Arrange
            var listener1 = new MockEventListener();
            var listener2 = new MockEventListener();
            var listener3 = new MockEventListener();
            
            this.hassWSApi.StateChangedEventListener
                .SubscribeEntityStatusChanged(testLightEntityId1, listener1.Handle);
            this.hassWSApi.StateChangedEventListener
                .SubscribeEntityStatusChanged(testLightEntityId1, listener2.Handle);
            this.hassWSApi.StateChangedEventListener
                .SubscribeEntityStatusChanged(testLightEntityId1, listener3.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act
            var eventData = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener1);

            // Assert
            Assert.NotNull(eventData);
            Assert.NotZero(listener1.HitCount);
            Assert.NotZero(listener2.HitCount);
            Assert.NotZero(listener3.HitCount);
        }

        [Test]
        public async Task UnsubscribeEntityStatusChanged_WhenEntityChanges_DoesNotNotify()
        {
            // Arrange
            var listener = new MockEventListener();
            this.hassWSApi.StateChangedEventListener.SubscribeEntityStatusChanged(testLightEntityId1, listener.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act - First change should be received
            var eventData = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener);
            Assert.NotNull(eventData, "First event should be received");

            // Unsubscribe and try again
            this.hassWSApi.StateChangedEventListener.UnsubscribeEntityStatusChanged(testLightEntityId1, listener.Handle);
            
            listener.Reset();
            var secondEventData = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener);

            // Assert
            Assert.Zero(listener.HitCount, "Should not receive events after unsubscribe");
            Assert.Null(secondEventData);
        }

        [Test]
        public async Task UnsubscribeOneOfMultipleListeners_WhenEntityChanges_OnlyNotifiesRemainingListeners()
        {
            // Arrange
            var listener1 = new MockEventListener();
            var listener2 = new MockEventListener();
            
            this.hassWSApi.StateChangedEventListener
                .SubscribeEntityStatusChanged(testLightEntityId1, listener1.Handle);
            this.hassWSApi.StateChangedEventListener
                .SubscribeEntityStatusChanged(testLightEntityId1, listener2.Handle);
            await this.hassWSApi.StateChangedEventListener.WaitForSubscriptionCompletedAsync();

            // Act
            this.hassWSApi.StateChangedEventListener
                .UnsubscribeEntityStatusChanged(testLightEntityId1, listener1.Handle);
            
            var eventData = await this.ForceStateChangedAndGetEventData(testLightEntityId1, listener2);

            // Assert
            Assert.NotNull(eventData);
            Assert.Zero(listener1.HitCount);
            Assert.NotZero(listener2.HitCount);
        }
    }
}
