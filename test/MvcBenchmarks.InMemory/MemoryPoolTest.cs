using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.MemoryPool;
using Xunit;

namespace MvcBenchmarks.InMemory
{
    public class MemoryPoolTest
    {
        [Theory]
        public void AllocatingPool(Type type)
        {
            Console.WriteLine($"Benchmarking {type} with Threads");

            //if (type == typeof(RoundRobinCASArraySegmentPool<>))
            //{
            //    Console.WriteLine("Ready");
            //    Console.ReadLine();
            //}

            var iterations = 100000;
            var concurrency = 64;

            var timer = new Stopwatch();

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);

            var charPool = (IArraySegmentPool<char>)Activator.CreateInstance(type.MakeGenericType(typeof(char)));
            var bytePool = (IArraySegmentPool<byte>)Activator.CreateInstance(type.MakeGenericType(typeof(byte)));

            var threads = new Thread[concurrency];
            for (var i = 0; i < concurrency; i++)
            {
                var id = i;
                threads[i] = new Thread(() =>
                {
                    var text = "The quick brown fox jumps over the lazy brown dog.";
                    for (var j = 0; j < iterations; j++)
                    {
                        using (var writer = new HttpResponseStreamWriter(
                            Stream.Null,
                            Encoding.UTF8,
                            charPool.Lease(4096),
                            bytePool.Lease(4096)))
                        {
                            for (var k = 0; k < id + 10; k++)
                            {
                                writer.Write(text);
                            }
                        }
                    }
                });
            }

            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);
            GC.WaitForPendingFinalizers();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, blocking: true);

            timer.Start();
            for (var i = 0; i < concurrency; i++)
            {
                threads[i].Start();
            }

            for (var i = 0; i < concurrency; i++)
            {
                threads[i].Join();
            }

            timer.Stop();
            Console.WriteLine($"{type} - Time elapsed: {timer.Elapsed}");
        }
    }
}
