// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace MvcBenchmarks
{
    public class BenchmarkTestCaseDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _diagnosticMessageSink;

        public BenchmarkTestCaseDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _diagnosticMessageSink = diagnosticMessageSink;
        }

        public virtual IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            var variations = testMethod.Method
                .GetCustomAttributes(typeof(BenchmarkVariationAttribute))
                .ToDictionary(
                    a => a.GetNamedArgument<string>(nameof(BenchmarkVariationAttribute.VariationName)),
                    a => a.GetNamedArgument<object[]>(nameof(BenchmarkVariationAttribute.Data)));

            if (!variations.Any())
            {
                variations.Add("Default", new object[0]);
            }

            var tests = new List<IXunitTestCase>();
            foreach (var variation in variations)
            {
                tests.Add(new BenchmarkTestCase(
                    factAttribute.GetNamedArgument<int>(nameof(BenchmarkAttribute.Iterations)),
                    factAttribute.GetNamedArgument<int>(nameof(BenchmarkAttribute.WarmupIterations)),
                    variation.Key,
                    _diagnosticMessageSink,
                    testMethod,
                    variation.Value));
            }

            return tests;
        }
    }
}