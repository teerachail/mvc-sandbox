// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;
using System.Diagnostics;
using Microsoft.Extensions.PlatformAbstractions;

#if DNXCORE50 || DNX451
using XunitDiagnosticMessage = Xunit.DiagnosticMessage;
#else
using XunitDiagnosticMessage = Xunit.Sdk.DiagnosticMessage;
#endif

namespace MvcBenchmarks
{
    public class BenchmarkTestCaseRunner : XunitTestCaseRunner
    {
        private static string _machineName = GetMachineName();
        private static string _framework = GetFramework();
        private readonly IMessageSink _diagnosticMessageSink;

        public BenchmarkTestCaseRunner(
                BenchmarkTestCase testCase,
                string displayName,
                string skipReason,
                object[] constructorArguments,
                object[] testMethodArguments,
                IMessageBus messageBus,
                ExceptionAggregator aggregator,
                CancellationTokenSource cancellationTokenSource,
                IMessageSink diagnosticMessageSink)
            : base(
                  testCase,
                  displayName,
                  skipReason,
                  constructorArguments,
                  testMethodArguments,
                  messageBus,
                  aggregator,
                  cancellationTokenSource)
        {
            TestCase = testCase;
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public new BenchmarkTestCase TestCase { get; private set; }

        protected override async Task<RunSummary> RunTestAsync()
        {
            var runSummary = new BenchmarkRunSummary
            {
                TestClassFullName = TestCase.TestMethod.TestClass.Class.Name,
                TestClass = TestCase.TestMethod.TestClass.Class.Name.Split('.').Last(),
                TestMethod = TestCase.DisplayName,
                Variation = TestCase.Variation,
                ProductReportingVersion = BenchmarkConfig.Instance.ProductReportingVersion,
                RunStarted = DateTime.UtcNow,
                MachineName = _machineName,
                Framework = _framework,
                WarmupIterations = TestCase.WarmupIterations,
                Iterations = TestCase.Iterations,
            };

            for (int i = 0; i < TestCase.WarmupIterations; i++)
            {
                var runner = CreateRunner(i + 1, TestCase.WarmupIterations, TestCase.Variation, warmup: true);
                runSummary.Aggregate(await runner.RunAsync());
            }

            var stopwatch = new Stopwatch();
            for (int i = 0; i < TestCase.Iterations; i++)
            {
                TestCase.MetricCollector.Reset();
                var runner = CreateRunner(i + 1, TestCase.Iterations, TestCase.Variation, warmup: false);

                stopwatch.Start();

                // Running the actual test
                var result = await runner.RunAsync();

                stopwatch.Stop();
                runSummary.Aggregate(result);
            }

            runSummary.TimeElapsed = stopwatch.Elapsed;
            _diagnosticMessageSink.OnMessage(new XunitDiagnosticMessage(runSummary.ToString()));
            Console.WriteLine(runSummary.ToString());

            return runSummary;
        }

        private XunitTestRunner CreateRunner(int iteration, int totalIterations, string variation, bool warmup)
        {
            var name = $"{DisplayName} [Stage: {(warmup ? "Warmup" : "Collection")}] [Iteration: {iteration}/{totalIterations}] [Variation: {variation}]";

            return new XunitTestRunner(
                new XunitTest(TestCase, name),
                MessageBus,
                TestClass,
                ConstructorArguments,
                TestMethod,
                TestMethodArguments,
                SkipReason,
                BeforeAfterAttributes,
                Aggregator,
                CancellationTokenSource);
        }

        private static string GetFramework()
        {
#if DNX451 || DNXCORE50
            return "DNX." + PlatformServices.Default.Runtime.RuntimeType;
#else
            return ".NETFramework";
#endif
        }

        private static string GetMachineName()
        {
#if DNXCORE50
            var config = new ConfigurationBuilder(".")
                .AddEnvironmentVariables()
                .Build();

            return config["computerName"];
#else
            return Environment.MachineName;
#endif
        }
    }
}