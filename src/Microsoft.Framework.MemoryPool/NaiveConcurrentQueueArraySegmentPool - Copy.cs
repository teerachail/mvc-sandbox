using System;
using System.Collections.Concurrent;

namespace Microsoft.Framework.MemoryPool
{
    public class NaiveConcurrentQueueArraySegmentPool<T> : IArraySegmentPool<T>
    {
        private readonly ConcurrentQueue<LeasedArraySegment<T>> _segments = new ConcurrentQueue<LeasedArraySegment<T>>();

        public LeasedArraySegment<T> Lease(int size)
        {
            LeasedArraySegment<T> segment;
            if (!_segments.TryDequeue(out segment))
            {
                segment = new LeasedArraySegment<T>(new ArraySegment<T>(new T[size]), this);
            }

            return segment;
        }

        public void Return(LeasedArraySegment<T> buffer)
        {
            _segments.Enqueue(buffer);
        }
    }
}
