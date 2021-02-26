using System.Collections.Generic;

namespace HassClient.Helpers
{
    internal class Map<T1, T2>
    {
        private Dictionary<T1, T2> forward = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> reverse = new Dictionary<T2, T1>();

        public IReadOnlyDictionary<T1, T2> Forward => this.forward;

        public IReadOnlyDictionary<T2, T1> Reverse => this.reverse;

        public void Add(T1 t1, T2 t2)
        {
            this.forward.Add(t1, t2);
            this.reverse.Add(t2, t1);
        }
    }
}
