// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
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
            _loggerFactory = outputHelper.ToLoggerFactory();
        }

        [Fact]
        public void Constructor_Validates_Parameters()
        {
            // Arrange
            var options = new SqlLocalDbOptions();

            Assert.Throws<ArgumentNullException>("options", () => new SqlLocalDbApi(null, _loggerFactory));
            Assert.Throws<ArgumentNullException>("loggerFactory", () => new SqlLocalDbApi(null));
            Assert.Throws<ArgumentNullException>("loggerFactory", () => new SqlLocalDbApi(options, null));
            Assert.Throws<ArgumentNullException>("registry", () => new SqlLocalDbApi(options, null, _loggerFactory));
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

        [WindowsOnlyFact]
        public void Can_Start_And_Stop_Tracing()
        {
            // Arrange
            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                // Act (no Assert)
                api.StartTracing();
                api.StopTracing();
            }
        }

        [WindowsOnlyFact]
        public void Throws_InvalidOperationException_If_SQL_LocalDB_Not_Installed()
        {
            // Arrange
            var options = new SqlLocalDbOptions();
            var registry = Mock.Of<Interop.IRegistry>();

            using (var actual = new SqlLocalDbApi(options, registry, _loggerFactory))
            {
                // Act and Assert
                Assert.Throws<InvalidOperationException>(() => actual.CreateInstance("name"));
                Assert.Throws<InvalidOperationException>(() => actual.DeleteInstance("name"));
                Assert.Throws<InvalidOperationException>(() => actual.DeleteUserInstances());
                Assert.Throws<InvalidOperationException>(() => actual.GetDefaultInstance());
                Assert.Throws<InvalidOperationException>(() => actual.GetInstanceInfo("name"));
                Assert.Throws<InvalidOperationException>(() => actual.GetInstanceNames());
                Assert.Throws<InvalidOperationException>(() => actual.GetInstances());
                Assert.Throws<InvalidOperationException>(() => actual.GetOrCreateInstance("name"));
                Assert.Throws<InvalidOperationException>(() => actual.GetVersionInfo("name"));
                Assert.Throws<InvalidOperationException>(() => actual.InstanceExists("name"));
                Assert.Throws<InvalidOperationException>(() => actual.LatestVersion);
                Assert.Throws<InvalidOperationException>(() => actual.ShareInstance("name", "sharedName"));
                Assert.Throws<InvalidOperationException>(() => actual.StartInstance("name"));
                Assert.Throws<InvalidOperationException>(() => actual.StartTracing());
                Assert.Throws<InvalidOperationException>(() => actual.StopInstance("name"));
                Assert.Throws<InvalidOperationException>(() => actual.StopTracing());
                Assert.Throws<InvalidOperationException>(() => actual.UnshareInstance("name"));
            }
        }

        [WindowsOnlyFact]
        public void StopTimeout_Throws_If_Value_Is_Negative()
        {
            // Arrange
            TimeSpan value = TimeSpan.Zero.Add(TimeSpan.FromTicks(-1));

            using (var actual = new SqlLocalDbApi(_loggerFactory))
            {
                // Act and Assert
                var exception = Assert.Throws<ArgumentOutOfRangeException>("value", () => actual.StopTimeout = value);
                exception.ActualValue.ShouldBe(value);
            }
        }

        [WindowsOnlyFact]
        public void Methods_Validate_Parameters()
        {
            // Arrange
            TimeSpan timeout = TimeSpan.Zero.Add(TimeSpan.FromTicks(-1));

            using (var actual = new SqlLocalDbApi(_loggerFactory))
            {
                // Act and Assert
                Assert.Throws<ArgumentNullException>("instanceName", () => actual.CreateInstance(null, "version"));
                Assert.Throws<ArgumentNullException>("version", () => actual.CreateInstance("instanceName", null));
                Assert.Throws<ArgumentNullException>("instanceName", () => actual.DeleteInstance(null));
                Assert.Throws<ArgumentNullException>("instanceName", () => actual.GetInstanceInfo(null));
                Assert.Throws<ArgumentNullException>("version", () => actual.GetVersionInfo(null));
                Assert.Throws<ArgumentNullException>("ownerSid", () => actual.ShareInstance(null, "instanceName", "sharedInstanceName"));
                Assert.Throws<ArgumentNullException>("instanceName", () => actual.ShareInstance("ownerSid", null, "sharedInstanceName"));
                Assert.Throws<ArgumentNullException>("sharedInstanceName", () => actual.ShareInstance("ownerSid", "instanceName", null));
                Assert.Throws<ArgumentException>("instanceName", () => actual.ShareInstance("sid", string.Empty, "sharedInstanceName"));
                Assert.Throws<ArgumentNullException>("instanceName", () => actual.StartInstance(null));
                Assert.Throws<ArgumentNullException>("instanceName", () => actual.StopInstance(null, TimeSpan.Zero));
                Assert.Throws<ArgumentOutOfRangeException>("timeout", () => actual.StopInstance("instanceName", timeout)).ActualValue.ShouldBe(timeout);
                Assert.Throws<ArgumentNullException>("instanceName", () => actual.UnshareInstance(null));
            }
        }

        [WindowsOnlyFact]
        public void DeleteInstanceInternal_Returns_False_If_ThrownIfNotFound_Is_False_And_Instance_Does_Not_Exist()
        {
            // Arrange
            TimeSpan timeout = TimeSpan.Zero.Add(TimeSpan.FromTicks(-1));

            using (var actual = new SqlLocalDbApi(_loggerFactory))
            {
                // Act and Assert
                actual.DeleteInstanceInternal("NotARealInstance", throwIfNotFound: false).ShouldBeFalse();
            }
        }

        [WindowsOnlyFact]
        public async Task Can_Manage_SqlLocalDB_Instances()
        {
            // Arrange
            using (var actual = new SqlLocalDbApi(_loggerFactory))
            {
                string instanceName = Guid.NewGuid().ToString();

                // Act
                ISqlLocalDbInstanceInfo instance = actual.GetInstanceInfo(instanceName);

                // Assert
                instance.ShouldNotBeNull();
                instance.Name.ShouldBe(instanceName);
                instance.Exists.ShouldBeFalse();
                instance.IsRunning.ShouldBeFalse();

                // Act and Assert
                actual.InstanceExists(instanceName).ShouldBeFalse();

                // Act
                instance = actual.CreateInstance(instanceName);

                // Assert
                instance = actual.GetInstanceInfo(instanceName);
                instance.ShouldNotBeNull();
                instance.Name.ShouldBe(instanceName);
                instance.Exists.ShouldBeTrue();
                instance.IsRunning.ShouldBeFalse();

                // Act
                string namedPipe = actual.StartInstance(instanceName);

                // Assert
                namedPipe.ShouldNotBeNullOrWhiteSpace();

                var builder = new SqlConnectionStringBuilder() { DataSource = namedPipe };

                using (var connection = new SqlConnection(builder.ConnectionString))
                {
                    await connection.OpenAsync();
                }

                // Act
                instance = actual.GetInstanceInfo(instanceName);

                // Assert
                instance.ShouldNotBeNull();
                instance.Name.ShouldBe(instanceName);
                instance.Exists.ShouldBeTrue();
                instance.IsRunning.ShouldBeTrue();

                // Act and Assert
                actual.InstanceExists(instanceName).ShouldBeTrue();

                // Act
                actual.StopInstance(instanceName);

                // Assert
                instance = actual.GetInstanceInfo(instanceName);
                instance.ShouldNotBeNull();
                instance.Name.ShouldBe(instanceName);
                instance.Exists.ShouldBeTrue();
                instance.IsRunning.ShouldBeFalse();

                // Act
                actual.DeleteInstance(instanceName);

                // Assert
                instance = actual.GetInstanceInfo(instanceName);
                instance.ShouldNotBeNull();
                instance.Name.ShouldBe(instanceName);
                instance.Exists.ShouldBeFalse();
                instance.IsRunning.ShouldBeFalse();

                // Act and Assert
                actual.InstanceExists(instanceName).ShouldBeFalse();

                // Act (no Assert)
                actual.DeleteInstanceFiles(instanceName);
            }
        }

        [WindowsOnlyFact]
        public void Can_Create_SqlLocalDB_Instances_With_Different_Versions()
        {
            // Arrange
            using (var actual = new SqlLocalDbApi(_loggerFactory))
            {
                foreach (string version in actual.Versions)
                {
                    // Act
                    ISqlLocalDbVersionInfo versionInfo = actual.GetVersionInfo(version);

                    // Assert
                    versionInfo.ShouldNotBeNull();
                    versionInfo.Name.ShouldStartWith(version.Split('.').First());
                    versionInfo.Exists.ShouldBeTrue();
                    versionInfo.Version.ShouldNotBeNull();
                    versionInfo.Version.ShouldNotBe(new Version());

                    string instanceName = Guid.NewGuid().ToString();

                    // Act
                    ISqlLocalDbInstanceInfo instanceInfo = actual.CreateInstance(instanceName, version);

                    // Assert
                    instanceInfo.ShouldNotBeNull();
                    instanceInfo.Name.ShouldBe(instanceName);
                    instanceInfo.Exists.ShouldBeTrue();
                    instanceInfo.IsRunning.ShouldBeFalse();
                    instanceInfo.LocalDbVersion.ShouldBe(versionInfo.Version);

                    // Act (no Assert)
                    actual.DeleteInstance(instanceName);
                    actual.DeleteInstanceFiles(instanceName);
                }
            }
        }

        [Fact]
        public void SqlLocalDbApi_Is_ISqlLocalDbApiAdapter()
        {
            // Arrange
            using (var instance = new SqlLocalDbApi(_loggerFactory))
            {
                // Act
                ISqlLocalDbApiAdapter adapter = instance;

                // Assert
                adapter.LocalDb.ShouldBeSameAs(instance);
            }
        }

        [WindowsOnlyFact]
        public void SqlLocalDbApi_Throws_Exception_For_Native_Errors()
        {
            // Arrange
            string instanceName = new string('$', 10000);

            using (var actual = new SqlLocalDbApi(_loggerFactory))
            {
                // Act
                var exception = Assert.Throws<SqlLocalDbException>(() => actual.CreateInstance(instanceName));

                // Assert
                exception.ErrorCode.ShouldBe(SqlLocalDbErrors.InvalidParameter);
                exception.Message.ShouldStartWith("The parameter for the LocalDB Instance API method is incorrect. Consult the API documentation.");
                exception.InstanceName.ShouldBe(instanceName);
            }
        }
    }
}
