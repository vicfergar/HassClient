using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests.Mocks
{
    public class MockEventSubscriber
    {
        private List<object> receivedEventArgs = new List<object>();

        public int HitCount { get; private set; }

        public IEnumerable<object> ReceivedEventArgs => this.receivedEventArgs;

        public MockEventSubscriber()
        {
            this.Reset();
        }

        public void Reset()
        {
            this.HitCount = 0;
            this.receivedEventArgs.Clear();
        }

        public void Handle()
        {
            this.HitCount++;
        }

        public void Handle<T>(T _)
        {
            this.Handle();
        }

        public void Handle<T, U>(T _, U u)
        {
            this.receivedEventArgs.Add(u);
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

        public async Task<bool> WaitConditionWithTimeoutAsync(Func<bool> condition, int millisecondsTimeout)
        {
            var checkTask = this.WaitConditionAsync(condition);
            var waitTask = Task.Delay(millisecondsTimeout);
            await Task.WhenAny(waitTask, checkTask);
            return checkTask.IsCompleted && await checkTask;
        }

        public Task<bool> WaitHitAsync(int minHitsCount = 1)
        {
            return this.WaitConditionAsync(() => this.HitCount >= minHitsCount);
        }

        public Task<bool> WaitHitWithTimeoutAsync(int millisecondsTimeout, int minHitsCount = 1)
        {
            return this.WaitConditionWithTimeoutAsync(() => this.HitCount >= minHitsCount, millisecondsTimeout);
        }

        public Task<bool> WaitEventArgAsync(object eventArg)
        {
            return this.WaitConditionAsync(() => this.receivedEventArgs.Contains(eventArg));
        }

        public Task<bool> WaitEventArgWithTimeoutAsync(object eventArg, int millisecondsTimeout)
        {
            return this.WaitConditionWithTimeoutAsync(() => this.receivedEventArgs.Contains(eventArg), millisecondsTimeout);
        }

        public async Task<T> WaitFirstEventArgAsync<T>()
        {
            T result = default;
            await this.WaitConditionAsync(() => this.receivedEventArgs.FirstOrDefault(x => x is T result) != null);
            return result;
        }

        public async Task<T> WaitFirstEventArgWithTimeoutAsync<T>(int millisecondsTimeout)
        {
            T result = default;
            await this.WaitConditionWithTimeoutAsync(() => this.receivedEventArgs.FirstOrDefault(x => x is T result) != null, millisecondsTimeout);
            return result;
        }
    }
}
