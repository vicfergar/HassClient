using System;

namespace HassClient.WS.Tests.Mocks
{
    public class MockEventHandler<T>
    {
        public event EventHandler<T> Event;

        public EventHandler<T> EventHandler => this.Event;
    }
}
