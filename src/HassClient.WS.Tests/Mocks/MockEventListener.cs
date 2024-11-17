using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests.Mocks
{
    public class MockEventListener
    {
        public record EventData(object Sender, object Args);

        public record EventData<TArgs>(object Sender, TArgs Args);

        private ConcurrentQueue<EventData> receivedEvents = new ConcurrentQueue<EventData>();

        public int HitCount { get; private set; }

        public IEnumerable<EventData> ReceivedEvents => this.receivedEvents.AsEnumerable();
        
        public IEnumerable<object> ReceivedEventArgs => this.receivedEvents.Select(e => e.Args);

        public MockEventListener()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.HitCount = 0;
            this.receivedEvents.Clear();
        }

        public void Handle()
        {
            this.HitCount++;
        }

        public void Handle<TArgs>(TArgs args)
        {
            this.receivedEvents.Enqueue(new EventData(null, args));
            this.Handle();
        }

        public void Handle<TSender, TArgs>(TSender sender, TArgs args)
        {
            this.receivedEvents.Enqueue(new EventData(sender, args));
            this.Handle();
        }

        public async Task<bool> WaitConditionAsync(Func<bool> condition)
        {
            var checkTask = Task.Run(async () =>
            {
                while (!condition())
                {
                    await Task.Delay(25);
                }

                return true;
            });

            return await checkTask;
        }

        public Task<bool> WaitEventArgWithTimeoutAsync(object eventArg, int millisecondsTimeout)
        {
            return this.WaitConditionWithTimeoutAsync(() => this.ReceivedEventArgs.Contains(eventArg), millisecondsTimeout);
        }

        public async Task<bool> WaitConditionWithTimeoutAsync(Func<bool> condition, int millisecondsTimeout)
        {
            var checkTask = this.WaitConditionAsync(condition);
            var waitTask = Task.Delay(millisecondsTimeout);
            await Task.WhenAny(waitTask, checkTask);
            return checkTask.IsCompleted && await checkTask;
        }

        public async Task<EventData<TArgs>> WaitFirstEventWithTimeoutAsync<TArgs>(Func<TArgs, bool> predicate, int millisecondsTimeout)
            where TArgs : class
        {
            EventData<TArgs> result = default;
            var aa = await this.WaitConditionWithTimeoutAsync(() =>
            {
                var matchingEvent = this.ReceivedEvents.FirstOrDefault(x => x.Args is TArgs y && predicate(y));
                if (matchingEvent != null)
                {
                    result = new EventData<TArgs>(matchingEvent.Sender, (TArgs)matchingEvent.Args);
                    return true;
                }

                return false;
            }, millisecondsTimeout);
            return result;
        }

        public Task<EventData<TArgs>> WaitFirstEventWithTimeoutAsync<TArgs>(int millisecondsTimeout)
            where TArgs : class
        {
            return WaitFirstEventWithTimeoutAsync<TArgs>(x => true, millisecondsTimeout);
        }
    }
}
