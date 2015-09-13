using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Framework.MemoryPool
{
    public class RoundRobinLockArraySegmentPool<T> : IArraySegmentPool<T>
    {
        public const int BlockSize = 4096;

        public static readonly int PoolCount = Environment.ProcessorCount;
        public static readonly int GrowthThreshold = Environment.ProcessorCount * 2;
        public static readonly int GrowthFactor = Environment.ProcessorCount * 4;

        private Pool[] _pools = new Pool[PoolCount];

        private volatile int _remaining;
        private volatile int _growing;
        private volatile int _size;

        public RoundRobinLockArraySegmentPool()
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
                    if (pool.Remaining < blockCount)
                    {
                        continue;
                    }

                    RoundRobinCASLeasedArraySegment<T> segment;
                    lock (pool.Lock)
                    {
                        if (pool.Remaining < blockCount)
                        {
                            // While we were waiting, someone took the blocks.
                            continue;
                        }

                        pool.Remaining--;
                        segment = pool.Items.Dequeue();
                        Debug.Assert(segment != null);
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
            var segment = (RoundRobinCASLeasedArraySegment<T>)buffer;
            var pool = segment.Pool;
            lock (pool.Lock)
            {
                pool.Items.Enqueue(segment);
                pool.Remaining++;
            }

            Interlocked.Increment(ref _remaining);
        }

        private void Grow()
        {
            if (_growing == 0)
            {
                var lockTaken = false;

                if (Interlocked.CompareExchange(ref _growing, 1, 0) == 0)
                {
                    if (_remaining < GrowthFactor)
                    {
                        lockTaken = true;
                    }
                }

                if (lockTaken)
                {
                    var buffer = new T[BlockSize * GrowthFactor];

                    for (var i = 0; i < PoolCount; i++)
                    {
                        var pool = _pools[i];
                        lock (pool.Lock)
                        {
                            for (var j = i; j < GrowthFactor; j += PoolCount)
                            {
                                pool.Items.Enqueue(new RoundRobinCASLeasedArraySegment<T>(new ArraySegment<T>(buffer, BlockSize * j, BlockSize), this, pool));
                                pool.Remaining++;
                            }
                        }
                    }
                    
                    Interlocked.Add(ref _remaining, GrowthFactor);
                    Interlocked.Decrement(ref _growing);
                }
            }
        }

        private class Pool
        {
            public object Lock = new object();

            public volatile int Remaining;

            public Queue<RoundRobinCASLeasedArraySegment<T>> Items = new Queue<RoundRobinCASLeasedArraySegment<T>>();
        }

        private class RoundRobinCASLeasedArraySegment<T> : LeasedArraySegment<T>
        {
            public RoundRobinCASLeasedArraySegment(ArraySegment<T> segment, IArraySegmentPool<T> owner, Pool pool)
                : base(segment, owner)
            {
                Pool = pool;
            }

            public Pool Pool { get; }
        }
    }
}