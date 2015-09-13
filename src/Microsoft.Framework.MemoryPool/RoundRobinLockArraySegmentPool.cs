using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Microsoft.Framework.MemoryPool
{
    public class RoundRobinCASArraySegmentPool<T> : IArraySegmentPool<T>
    {
        public const int BlockSize = 4096;

        public static readonly int PoolCount = Environment.ProcessorCount;
        public static readonly int GrowthThreshold = Environment.ProcessorCount * 2;
        public static readonly int GrowthFactor = Environment.ProcessorCount * 4;

        private Pool[] _pools = new Pool[PoolCount];

        private int _remaining;
        private volatile int _growing;

        public RoundRobinCASArraySegmentPool()
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

                    while (Interlocked.CompareExchange(ref pool.Locked, 1, 0) != 0)
                    {
                        // Spin
                    }

                    if (pool.Remaining < blockCount)
                    {
                        // While we were waiting, someone took the blocks.
                        Interlocked.Decrement(ref pool.Locked);
                        continue;
                    }


                    Interlocked.Decrement(ref pool.Remaining);
                    var segment = pool.Items.Dequeue();
                    Debug.Assert(segment != null);
                    Interlocked.Decrement(ref pool.Locked);

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
            while (Interlocked.CompareExchange(ref pool.Locked, 1, 0) != 0)
            {
                // Spin
            }

            pool.Items.Enqueue(segment);
            Interlocked.Increment(ref pool.Remaining);
            Interlocked.Decrement(ref pool.Locked);

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
                        while (Interlocked.CompareExchange(ref pool.Locked, 1, 0) != 0)
                        {
                            // Spin
                        }

                        for (var j = i; j < GrowthFactor; j += PoolCount)
                        {
                            pool.Items.Enqueue(new RoundRobinCASLeasedArraySegment<T>(new ArraySegment<T>(buffer, BlockSize * j, BlockSize), this, pool));
                            Interlocked.Increment(ref pool.Remaining);
                        }

                        Interlocked.Decrement(ref pool.Locked);
                    }

                    Interlocked.Add(ref _remaining, GrowthFactor);
                    Interlocked.Decrement(ref _growing);
                }
            }
        }

        private class Pool
        {
            public volatile int Locked;

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