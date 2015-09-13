using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Framework.MemoryPool
{
    public class RoundRobinConcurrentQueueArraySegmentPool<T> : IArraySegmentPool<T>
    {
        public const int BlockSize = 4096;

        public static readonly int PoolCount = Environment.ProcessorCount;
        public static readonly int GrowthThreshold = Environment.ProcessorCount * 2;
        public static readonly int GrowthFactor = Environment.ProcessorCount * 4;

        private Pool[] _pools = new Pool[PoolCount];

        private int _remaining;
        private volatile int _growing;

        public RoundRobinConcurrentQueueArraySegmentPool()
        {
            _pools = new Pool[PoolCount];
            for (var i = 0; i < PoolCount; i++)
            {
                _pools[i] = new Pool();
            }
        }

        public LeasedArraySegment<T> Lease(int size)
        {
            var offset = Thread.CurrentThread.ManagedThreadId % _pools.Length;
            var blockCount = (int)Math.Ceiling(size / (double)BlockSize);

            if (_remaining < GrowthThreshold)
            {
                Grow();
            }

            while (true)
            {
                for (var i = 0; i < _pools.Length; i++)
                {
                    var pool = _pools[(i + offset) % PoolCount];

                    Segment<T> segment;
                    if (!pool.TryDequeue(out segment))
                    {
                        // While we were waiting, someone took the blocks.
                        continue;
                    }

                    Interlocked.Decrement(ref _remaining);
                    return segment;
                }

                if (_remaining < GrowthFactor)
                {
                    Grow();
                }
            }
        }

        public void Return(LeasedArraySegment<T> buffer)
        {
            var segment = (Segment<T>)buffer;
            var pool = segment.Pool;

            pool.Enqueue(segment);

            Interlocked.Increment(ref _remaining);
        }

        private void Grow()
        {
            if (_growing == 0)
            {
                var lockTaken = false;
                while (true)
                {
                    if (Interlocked.CompareExchange(ref _growing, 1, 0) == 0)
                    {
                        lockTaken = true;
                        break;
                    }
                    else if (_remaining > GrowthThreshold)
                    {
                        // Someone else grew the pool, go get a block.
                        break;
                    }
                }

                if (lockTaken)
                {
                    var buffer = new T[BlockSize * GrowthFactor];

                    for (var i = 0; i < PoolCount; i++)
                    {
                        var pool = _pools[i];
                        for (var j = i; j < GrowthFactor; j += PoolCount)
                        {
                            pool.Enqueue(new Segment<T>(new ArraySegment<T>(buffer, BlockSize * j, BlockSize), this, pool));
                        }
                    }

                    Interlocked.Add(ref _remaining, GrowthFactor);
                    Interlocked.Decrement(ref _growing);
                }
            }
        }

        private class Pool : ConcurrentQueue<Segment<T>>
        {
        }

        private class Segment<T> : LeasedArraySegment<T>
        {
            public Segment(ArraySegment<T> segment, IArraySegmentPool<T> owner, Pool pool)
                : base(segment, owner)
            {
                Pool = pool;
            }

            public Pool Pool { get; }
        }
    }
}