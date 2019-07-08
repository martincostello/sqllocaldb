// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace MartinCostello.SqlLocalDb
{
    public class ISqlLocalDbApiExtensionsTests
    {
        private readonly ILoggerFactory _loggerFactory;

        public ISqlLocalDbApiExtensionsTests(ITestOutputHelper outputHelper)
        {
            _loggerFactory = outputHelper.ToLoggerFactory();
        }

        [Theory]
        [InlineData(SqlLocalDbErrors.InstanceBusy)]
        [InlineData(SqlLocalDbErrors.InternalError)]
        public static void TemporaryInstance_Ignores_Exception_If_Delete_Fails(int errorCode)
        {
            // Arrange
            if (!SqlLocalDbApi.IsWindows)
            {
                // HACK Theories dont seem to work correctly with subclasses now
                // so cannot make a derived class for a "Windows-only" theory.
                return;
            }

            // Arrange
            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.LatestVersion)
                .Returns("v99.9");

            mock.Setup((p) => p.CreateInstance(It.IsAny<string>(), "v99.9"))
                .Verifiable();

            mock.Setup((p) => p.StartInstance(It.IsAny<string>()))
                .Verifiable();

            mock.Setup((p) => p.StopInstance(It.IsAny<string>(), null))
                .Verifiable();

            mock.Setup((p) => p.DeleteInstance(It.IsAny<string>()))
                .Throws(new SqlLocalDbException("Error", errorCode))
                .Verifiable();

            ISqlLocalDbApi api = mock.Object;

            // Act
            using (TemporarySqlLocalDbInstance target = api.CreateTemporaryInstance())
            {
                target.GetInstanceInfo();
            }

            // Assert
            mock.Verify();
        }

        [WindowsOnlyFact]
        public static void ShareInstance_Uses_SID_For_Current_User()
        {
            // Arrange
            string instanceName = "SomeName";
            string sharedInstanceName = "SomeSharedName";

            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.ShareInstance(It.IsNotNull<string>(), instanceName, sharedInstanceName))
                .Verifiable();

            ISqlLocalDbApi api = mock.Object;

            // Act
            api.ShareInstance(instanceName, sharedInstanceName);

            // Assert
            mock.Verify();
        }

        [Fact]
        public void CreateTemporaryInstance_Throws_If_Api_Is_Null()
        {
            // Arrange
            ISqlLocalDbApi api = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("api", () => api.CreateTemporaryInstance());
        }

        [Fact]
        public void TemporaryInstance_Throws_If_Used_After_Disposal()
        {
            // Arrange
            var api = Mock.Of<ISqlLocalDbApi>();

            TemporarySqlLocalDbInstance instance = api.CreateTemporaryInstance();

            // Act
            instance.Dispose();

            // Assert
            Assert.Throws<ObjectDisposedException>(() => instance.Name);
            Assert.Throws<ObjectDisposedException>(() => instance.GetInstanceInfo());
        }

        [Fact]
        public void TemporaryInstance_Disposes_Cleanly_If_Not_Used()
        {
            // Arrange
            var api = Mock.Of<ISqlLocalDbApi>();

            // Act and Assert
            using (TemporarySqlLocalDbInstance instance = api.CreateTemporaryInstance())
            {
                instance.Dispose();
            }
        }

        [Fact]
        public void TemporaryInstance_Is_ISqlLocalDbApiAdapter()
        {
            // Arrange
            var api = Mock.Of<ISqlLocalDbApi>();

            using (TemporarySqlLocalDbInstance instance = api.CreateTemporaryInstance())
            {
                // Act
                ISqlLocalDbApiAdapter adapter = instance;

                // Assert
                adapter.LocalDb.ShouldBeSameAs(api);
            }
        }

        [Fact]
        public void TemporaryInstance_Deletes_Instance_If_Start_Fails()
        {
            // Arrange
            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.LatestVersion)
                .Returns("v99.9");

            mock.Setup((p) => p.CreateInstance(It.IsAny<string>(), "v99.9"))
                .Verifiable();

            mock.Setup((p) => p.StartInstance(It.IsAny<string>()))
                .Throws<PlatformNotSupportedException>();

            mock.Setup((p) => p.DeleteInstance(It.IsAny<string>()))
                .Verifiable();

            ISqlLocalDbApi api = mock.Object;

            using (TemporarySqlLocalDbInstance target = api.CreateTemporaryInstance())
            {
                // Act and Assert
                Assert.Throws<PlatformNotSupportedException>(() => target.GetInstanceInfo());
            }

            mock.Verify();
        }

        [Theory]
        [InlineData(SqlLocalDbErrors.InternalError)]
        [InlineData(SqlLocalDbErrors.UnknownInstance)]
        public void TemporaryInstance_Ignores_Exception_If_Stop_Fails(int errorCode)
        {
            // Arrange
            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.LatestVersion)
                .Returns("v99.9");

            mock.Setup((p) => p.CreateInstance(It.IsAny<string>(), "v99.9"))
                .Verifiable();

            mock.Setup((p) => p.StartInstance(It.IsAny<string>()))
                .Verifiable();

            mock.Setup((p) => p.StopInstance(It.IsAny<string>(), null))
                .Throws(new SqlLocalDbException("Error", errorCode))
                .Verifiable();

            mock.Setup((p) => p.DeleteInstance(It.IsAny<string>()))
                .Verifiable();

            ISqlLocalDbApi api = mock.Object;

            // Act
            using (TemporarySqlLocalDbInstance target = api.CreateTemporaryInstance())
            {
                target.GetInstanceInfo();
            }

            // Assert
            mock.Verify();
        }

        [Fact]
        public void GetDefaultInstance_Throws_If_Api_Is_Null()
        {
            // Arrange
            ISqlLocalDbApi api = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("api", () => api.GetDefaultInstance());
        }

        [WindowsOnlyFact]
        public void GetDefaultInstance_Returns_The_Default_Instance()
        {
            // Arrange
            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                // Act
                ISqlLocalDbInstanceInfo actual = api.GetDefaultInstance();

                // Assert
                actual.ShouldNotBeNull();
                actual.IsAutomatic.ShouldBeTrue();
                actual.Name.ShouldBe(api.DefaultInstanceName);
            }
        }

        [Fact]
        public void GetInstances_Throws_If_Api_Is_Null()
        {
            // Arrange
            ISqlLocalDbApi api = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("api", () => api.GetInstances());
        }

        [WindowsOnlyFact]
        public void GetInstances_Returns_All_The_Named_Instances()
        {
            // Arrange
            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                // Act
                IReadOnlyList<ISqlLocalDbInstanceInfo> actual = api.GetInstances();

                // Assert
                actual.ShouldNotBeNull();
                actual.Count.ShouldBeGreaterThanOrEqualTo(1);
                actual.ShouldBeUnique();
                actual.ShouldAllBe((p) => p != null);
                actual.ShouldContain((p) => p.Name == api.DefaultInstanceName);
                actual.ShouldContain((p) => p.IsAutomatic);
            }
        }

        [Fact]
        public void GetInstances_Returns_Empty_If_No_Instances()
        {
            // Arrange
            var api = Mock.Of<ISqlLocalDbApi>();

            // Act
            IReadOnlyList<ISqlLocalDbInstanceInfo> actual = api.GetInstances();

            // Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeEmpty();
        }

        [Fact]
        public void GetVersions_Throws_If_Api_Is_Null()
        {
            // Arrange
            ISqlLocalDbApi api = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("api", () => api.GetVersions());
        }

        [WindowsOnlyFact]
        public void GetVersions_Returns_All_The_Installed_Versions()
        {
            // Arrange
            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                // Act
                IReadOnlyList<ISqlLocalDbVersionInfo> actual = api.GetVersions();

                // Assert
                actual.ShouldNotBeNull();
                actual.Count.ShouldBeGreaterThanOrEqualTo(1);
                actual.ShouldBeUnique();
                actual.ShouldAllBe((p) => p != null);
                actual.ShouldContain((p) => p.Exists);
            }
        }

        [Fact]
        public void GetVersions_Returns_Empty_If_No_Versions()
        {
            // Arrange
            var api = Mock.Of<ISqlLocalDbApi>();

            // Act
            IReadOnlyList<ISqlLocalDbVersionInfo> actual = api.GetVersions();

            // Assert
            actual.ShouldNotBeNull();
            actual.ShouldBeEmpty();
        }

        [Fact]
        public void GetOrCreateInstance_Throws_If_Api_Is_Null()
        {
            // Arrange
            ISqlLocalDbApi api = null;
            string instanceName = "SomeName";

            // Act and Assert
            Assert.Throws<ArgumentNullException>("api", () => api.GetOrCreateInstance(instanceName));
        }

        [Fact]
        public void GetOrCreateInstance_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            ISqlLocalDbApi api = Mock.Of<ISqlLocalDbApi>();
            string instanceName = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("instanceName", () => api.GetOrCreateInstance(instanceName));
        }

        [Fact]
        public void GetOrCreateInstance_Returns_The_Default_Instance_If_It_Exists_With_The_Default_Name()
        {
            // Arrange
            string instanceName = "Default";

            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.DefaultInstanceName)
                .Returns(instanceName);

            mock.Setup((p) => p.GetInstanceInfo(instanceName))
                .Returns(CreateInstanceInfo(exists: true));

            ISqlLocalDbApi api = mock.Object;

            // Act
            ISqlLocalDbInstanceInfo actual = api.GetOrCreateInstance(instanceName);

            // Assert
            actual.ShouldNotBeNull();
            actual.Exists.ShouldBeTrue();
        }

        [Theory]
        [InlineData("v11.0")]
        [InlineData("MSSQLLocalDB")]
        public void GetOrCreateInstance_Returns_The_Default_Instance_If_It_Exists(string instanceName)
        {
            // Arrange
            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.DefaultInstanceName)
                .Returns("Blah");

            mock.Setup((p) => p.GetInstanceInfo(instanceName))
                .Returns(CreateInstanceInfo(exists: true));

            ISqlLocalDbApi api = mock.Object;

            // Act
            ISqlLocalDbInstanceInfo actual = api.GetOrCreateInstance(instanceName);

            // Assert
            actual.ShouldNotBeNull();
            actual.Exists.ShouldBeTrue();
        }

        [Fact]
        public void GetOrCreateInstance_Returns_An_Instance_If_It_Exists()
        {
            // Arrange
            string instanceName = "MyInstance";

            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.DefaultInstanceName)
                .Returns("Blah");

            mock.Setup((p) => p.InstanceExists(instanceName))
                .Returns(true);

            mock.Setup((p) => p.GetInstanceInfo(instanceName))
                .Returns(CreateInstanceInfo(exists: true));

            ISqlLocalDbApi api = mock.Object;

            // Act
            ISqlLocalDbInstanceInfo actual = api.GetOrCreateInstance(instanceName);

            // Assert
            actual.ShouldNotBeNull();
        }

        [Fact]
        public void GetOrCreateInstance_Returns_An_Instance_If_It_Does_Not_Exist()
        {
            // Arrange
            string instanceName = "MyInstance";

            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.DefaultInstanceName)
                .Returns("Blah");

            mock.Setup((p) => p.LatestVersion)
                .Returns("v99.0");

            mock.Setup((p) => p.CreateInstance(instanceName, "v99.0"))
                .Returns(CreateInstanceInfo(exists: false));

            ISqlLocalDbApi api = mock.Object;

            // Act
            ISqlLocalDbInstanceInfo actual = api.GetOrCreateInstance(instanceName);

            // Assert
            actual.ShouldNotBeNull();
        }

        [Fact]
        public void ShareInstance_Throws_If_Api_Is_Null()
        {
            // Arrange
            ISqlLocalDbApi api = null;
            string instanceName = "SomeName";
            string sharedInstanceName = "SomeSharedName";

            // Act and Assert
            Assert.Throws<ArgumentNullException>("api", () => api.ShareInstance(instanceName, sharedInstanceName));
        }

        [WindowsOnlyFact]
        public void CreateTemporaryInstance_Creates_Starts_And_Deletes_An_Instance_If_Files_Not_Deleted()
        {
            CreateTemporaryInstance_Creates_Starts_And_Deletes_An_Instance(deleteFiles: false);
        }

        [WindowsOnlyFact]
        public void CreateTemporaryInstance_Creates_Starts_And_Deletes_An_Instance_If_Files_Deleted()
        {
            CreateTemporaryInstance_Creates_Starts_And_Deletes_An_Instance(deleteFiles: true);
        }

        [RunAsAdminFact]
        public void ShareInstance_Shares_Instance_For_Current_User()
        {
            // Arrange
            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                using (TemporarySqlLocalDbInstance target = api.CreateTemporaryInstance(deleteFiles: true))
                {
                    target.GetInstanceInfo().IsShared.ShouldBeFalse();

                    // Act
                    api.ShareInstance(target.Name, Guid.NewGuid().ToString());

                    // Assert
                    target.GetInstanceInfo().IsShared.ShouldBeTrue();
                }
            }
        }

        private static ISqlLocalDbInstanceInfo CreateInstanceInfo(bool exists)
        {
            var mock = new Mock<ISqlLocalDbInstanceInfo>();

            mock.Setup((p) => p.Exists).Returns(exists);

            return mock.Object;
        }

        private void CreateTemporaryInstance_Creates_Starts_And_Deletes_An_Instance(bool deleteFiles)
        {
            // Arrange
            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                ISqlLocalDbInstanceInfo info;
                string name;

                // Act
                using (TemporarySqlLocalDbInstance target = api.CreateTemporaryInstance(deleteFiles))
                {
                    // Assert
                    target.ShouldNotBeNull();
                    target.Name.ShouldNotBeNull();
                    target.Name.ShouldNotBeEmpty();

                    Guid.TryParse(target.Name, out Guid nameAsGuid).ShouldBeTrue();
                    nameAsGuid.ShouldNotBe(Guid.Empty);

                    // Act
                    info = target.GetInstanceInfo();

                    // Assert
                    info.ShouldNotBeNull();
                    info.Exists.ShouldBeTrue();
                    info.IsRunning.ShouldBeTrue();

                    name = target.Name;
                }

                // Act
                info = api.GetInstanceInfo(name);

                // Assert
                info.ShouldNotBeNull();
                info.Exists.ShouldBeFalse();
            }
        }
    }
}
