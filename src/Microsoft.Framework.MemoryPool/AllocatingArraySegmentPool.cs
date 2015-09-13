using System;

namespace Microsoft.Framework.MemoryPool
{
    public class AllocatingArraySegmentPool<T> : IArraySegmentPool<T>
    {
        public LeasedArraySegment<T> Lease(int size)
        {
            var actualSize = (size / 1024 + 1) * 1024;
            return new LeasedArraySegment<T>(new ArraySegment<T>(new T[actualSize]), this);
        }

        public void Return(LeasedArraySegment<T> buffer)
        {
            ((ILeasedLifetime)buffer).Destroy();
        }
    }
}
