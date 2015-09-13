using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Microsoft.Framework.MemoryPool
{
    public class NaiveTLSConcurrentQueueArraySegmentPool<T> : IArraySegmentPool<T>
    {
        private readonly ConcurrentQueue<LeasedArraySegment<T>> _segments = new ConcurrentQueue<LeasedArraySegment<T>>();

        [ThreadStatic]
        private static LeasedArraySegment<T> _tls;

        public LeasedArraySegment<T> Lease(int size)
        {
            LeasedArraySegment<T> segment;
            if (_tls != null)
            {
                segment = _tls;
                _tls = null;
                return segment;
            }

            if (!_segments.TryDequeue(out segment))
            {
                segment = new LeasedArraySegment<T>(new ArraySegment<T>(new T[size]), this);
            }

            return segment;
        }

        public void Return(LeasedArraySegment<T> buffer)
        {
            if (_tls == null)
            {
                _tls = buffer;
                return;
            }

            _segments.Enqueue(buffer);
        }
    }
}
