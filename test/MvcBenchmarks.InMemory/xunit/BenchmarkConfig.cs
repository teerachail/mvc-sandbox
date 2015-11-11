﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace MvcBenchmarks
{
    public class BenchmarkConfig
    {
        private static Lazy<BenchmarkConfig> _instance = new Lazy<BenchmarkConfig>(() =>
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();

            return new BenchmarkConfig
            {
            };
        });

        private BenchmarkConfig()
        { }

        public static BenchmarkConfig Instance
        {
            get { return _instance.Value; }
        }

        public bool RunIterations { get; private set; }
        public IEnumerable<string> ResultDatabases { get; private set; }
        public string BenchmarkDatabaseInstance { get; private set; }
        public string ProductReportingVersion { get; private set; }
    }
}