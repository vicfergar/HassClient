using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HassClient.WS.Tests.Mocks
{
    public class MockEventSubscriber
    {
        private ConcurrentQueue<object> receivedEventArgs = new ConcurrentQueue<object>();

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
            this.receivedEventArgs.Enqueue(u);
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
            return this.WaitConditionAsync(() => this.ReceivedEventArgs.Contains(eventArg));
        }

        public Task<bool> WaitEventArgWithTimeoutAsync(object eventArg, int millisecondsTimeout)
        {
            return this.WaitConditionWithTimeoutAsync(() => this.ReceivedEventArgs.Contains(eventArg), millisecondsTimeout);
        }

        public async Task<T> WaitFirstEventArgAsync<T>(Func<T, bool> predicate)
            where T : class
        {
            T result = default;
            var aa = await this.WaitConditionAsync(() =>
            {
                result = this.ReceivedEventArgs.FirstOrDefault(x => x is T y && predicate(y)) as T;
                return result != null;
            });
            return result;
        }

        public async Task<T> WaitFirstEventArgWithTimeoutAsync<T>(Func<T, bool> predicate, int millisecondsTimeout)
            where T : class
        {
            T result = default;
            var aa = await this.WaitConditionWithTimeoutAsync(() =>
            {
                result = this.ReceivedEventArgs.FirstOrDefault(x => x is T y && predicate(y)) as T;
                return result != null;
            }, millisecondsTimeout);
            return result;
        }

        public Task<T> WaitFirstEventArgAsync<T>()
            where T : class
        {
            return WaitFirstEventArgAsync<T>(x => true);
        }

        public Task<T> WaitFirstEventArgWithTimeoutAsync<T>(int millisecondsTimeout)
            where T : class
        {
            return WaitFirstEventArgWithTimeoutAsync<T>(x => true, millisecondsTimeout);
        }
    }
}
