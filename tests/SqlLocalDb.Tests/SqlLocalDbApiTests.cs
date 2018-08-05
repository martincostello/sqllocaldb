// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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

        [WindowsOnlyFact]
        public void Can_Get_Instances_From_Names()
        {
            // Arrange
            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                IReadOnlyList<string> names = api.GetInstanceNames();

                foreach (string name in names)
                {
                    // Act
                    ISqlLocalDbInstanceInfo info = api.GetInstanceInfo(name);

                    // Assert
                    info.ShouldNotBeNull();
                }

                // Arrange
                string instanceName = Guid.NewGuid().ToString();

                // Act
                bool actual = api.InstanceExists(instanceName);

                // Assert
                actual.ShouldBeFalse();
            }
        }

        [WindowsOnlyFact]
        public void Can_Test_Whether_Instances_Exist()
        {
            // Arrange
            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                // Start the default instance to ensure it exists
                api.StartInstance(api.DefaultInstanceName);

                try
                {
                    // Arrange
                    string instanceName = api.DefaultInstanceName;

                    // Act
                    bool actual = api.InstanceExists(instanceName);

                    // Assert
                    actual.ShouldBeTrue();

                    // Arrange
                    instanceName = Guid.NewGuid().ToString();

                    // Act
                    actual = api.InstanceExists(instanceName);

                    // Assert
                    actual.ShouldBeFalse();
                }
                finally
                {
                    api.StopInstance(api.DefaultInstanceName);
                }
            }
        }

        [NotWindowsFact]
        public void Does_Not_Throw_PlatformNotSupportedException()
        {
            // Arrange
            using (var actual = new SqlLocalDbApi(_loggerFactory))
            {
                // Act and Assert
                actual.DefaultInstanceName.ShouldBe(string.Empty);
                actual.IsLocalDBInstalled().ShouldBeFalse();
                actual.Versions.ShouldBeEmpty();
            }
        }

        [NotWindowsFact]
        public void Throws_PlatformNotSupportedException()
        {
            // Arrange
            using (var actual = new SqlLocalDbApi(_loggerFactory))
            {
                // Act and Assert
                Assert.Throws<PlatformNotSupportedException>(() => actual.CreateInstance("name"));
                Assert.Throws<PlatformNotSupportedException>(() => actual.DeleteInstance("name"));
                Assert.Throws<PlatformNotSupportedException>(() => actual.DeleteUserInstances());
                Assert.Throws<PlatformNotSupportedException>(() => actual.GetDefaultInstance());
                Assert.Throws<PlatformNotSupportedException>(() => actual.GetInstanceInfo("name"));
                Assert.Throws<PlatformNotSupportedException>(() => actual.GetInstanceNames());
                Assert.Throws<PlatformNotSupportedException>(() => actual.GetInstances());
                Assert.Throws<PlatformNotSupportedException>(() => actual.GetOrCreateInstance("name"));
                Assert.Throws<PlatformNotSupportedException>(() => actual.GetVersionInfo("name"));
                Assert.Throws<PlatformNotSupportedException>(() => actual.InstanceExists("name"));
                Assert.Throws<PlatformNotSupportedException>(() => actual.LatestVersion);
                Assert.Throws<PlatformNotSupportedException>(() => actual.ShareInstance("name", "sharedName"));
                Assert.Throws<PlatformNotSupportedException>(() => actual.ShareInstance("sid", "name", "sharedName"));
                Assert.Throws<PlatformNotSupportedException>(() => actual.StartInstance("name"));
                Assert.Throws<PlatformNotSupportedException>(() => actual.StartTracing());
                Assert.Throws<PlatformNotSupportedException>(() => actual.StopInstance("name"));
                Assert.Throws<PlatformNotSupportedException>(() => actual.StopTracing());
                Assert.Throws<PlatformNotSupportedException>(() => actual.UnshareInstance("name"));
            }
        }
    }
}
