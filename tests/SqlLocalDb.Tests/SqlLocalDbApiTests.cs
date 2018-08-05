// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace MartinCostello.SqlLocalDb
{
    public class SqlLocalDbApiTests
    {
        private readonly ILoggerFactory _loggerFactory;

        public SqlLocalDbApiTests(ITestOutputHelper outputHelper)
        {
            _loggerFactory = outputHelper.AsLoggerFactory();
        }

        [Fact]
        public void Constructor_Validates_Parameters()
        {
            // Arrange
            var options = new SqlLocalDbOptions();

            Assert.Throws<ArgumentNullException>("options", () => new SqlLocalDbApi(null, _loggerFactory));
            Assert.Throws<ArgumentNullException>("loggerFactory", () => new SqlLocalDbApi(null));
            Assert.Throws<ArgumentNullException>("loggerFactory", () => new SqlLocalDbApi(options, null));
        }

        [WindowsOnlyFact]
        public void Constructor_Initializes_Instance()
        {
            // Arrange
            var options = new SqlLocalDbOptions()
            {
                AutomaticallyDeleteInstanceFiles = true,
                Language = CultureInfo.GetCultureInfo("de-DE"),
                NativeApiOverrideVersion = "11.0",
                StopOptions = StopInstanceOptions.NoWait,
                StopTimeout = TimeSpan.FromSeconds(30),
            };

            using (var actual = new SqlLocalDbApi(options, _loggerFactory))
            {
                actual.AutomaticallyDeleteInstanceFiles.ShouldBe(options.AutomaticallyDeleteInstanceFiles);
                actual.DefaultInstanceName.ShouldNotBeNull();
                actual.LanguageId.ShouldBeGreaterThan(0);
                actual.LatestVersion.ShouldNotBeNull();
                actual.LoggerFactory.ShouldNotBeNull();
                actual.StopOptions.ShouldBe(options.StopOptions);
                actual.StopTimeout.ShouldBe(options.StopTimeout);
                actual.Versions.ShouldNotBeNull();
                actual.Versions.ShouldNotBeEmpty();
                actual.Versions.ShouldBeUnique();
            }
        }
    }
}
