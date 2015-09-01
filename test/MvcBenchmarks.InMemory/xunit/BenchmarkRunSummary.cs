// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;

namespace MvcBenchmarks
{
    public class BenchmarkRunSummary : RunSummary
    {
        // Dimensions
        public string TestClassFullName { get; set; }
        public string TestClass { get; set; }
        public string TestMethod { get; set; }
        public string Variation { get; set; }
        public string MachineName { get; set; }
        public string ProductReportingVersion { get; set; }
        public string Framework { get; set; }
        public string CustomData { get; set; }
        public DateTime RunStarted { get; set; }
        public int WarmupIterations { get; set; }
        public int Iterations { get; set; }

        // Metrics
        public TimeSpan TimeElapsed { get; set; }
        public long TimeElapsedAverage { get; private set; }
        public long TimeElapsedPercentile99 { get; private set; }
        public long TimeElapsedPercentile95 { get; private set; }
        public long TimeElapsedPercentile90 { get; private set; }
        public double TimeElapsedStandardDeviation { get; private set; }

        public long MemoryDeltaAverage { get; private set; }
        public long MemoryDeltaPercentile99 { get; private set; }
        public long MemoryDeltaPercentile95 { get; private set; }
        public long MemoryDeltaPercentile90 { get; private set; }
        public double MemoryDeltaStandardDeviation { get; private set; }

        public override string ToString()
        {
            return $@"{TestClass}.{TestMethod} (Variation={Variation})
    Warmup Iterations: {WarmupIterations}
    Collection Iterations: {Iterations}
    Total Time Elapsed: {TimeElapsed}
    Requests per Second: {Iterations * ((double)TimeSpan.FromSeconds(1).Ticks / (double)TimeElapsed.Ticks)}
    Iteration Time Elapsed (95th Percentile): {TimeElapsedPercentile95}ms
    Iteration Memory Delta (95th Percentile): {MemoryDeltaPercentile95}";
        }

        private static long Percentile(IEnumerable<long> results, double percentile)
        {
            return results.OrderBy(r => r).ElementAt((int)(results.Count() * percentile));
        }

        private static double StandardDeviation(IEnumerable<long> results, long average)
        {
            return Math.Sqrt(results
                .Select(r => r - average)
                .Select(r => r * r)
                .Sum()
                / results.Count());
        }
    }
}