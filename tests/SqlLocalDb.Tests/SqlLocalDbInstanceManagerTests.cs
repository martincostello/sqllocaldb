// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace MartinCostello.SqlLocalDb
{
    public class SqlLocalDbInstanceManagerTests
    {
        private readonly ILoggerFactory _loggerFactory;

        public SqlLocalDbInstanceManagerTests(ITestOutputHelper outputHelper)
        {
            _loggerFactory = outputHelper.ToLoggerFactory();
        }

        [WindowsOnlyFact]
        public static void Share_Shares_Instance()
        {
            // Act
            string sharedName = Guid.NewGuid().ToString();

            var mock = new Mock<ISqlLocalDbApi>();
            ISqlLocalDbInstanceInfo instance = CreateInstance();
            ISqlLocalDbApi api = mock.Object;

            var target = new SqlLocalDbInstanceManager(instance, api);

            // Act
            target.Share(sharedName);

            // Assert
            mock.Verify((p) => p.ShareInstance(It.IsNotNull<string>(), "Name", sharedName), Times.Once());
        }

        [WindowsOnlyFact]
        public static void Share_Throws_If_SqlLocalDbEception_Is_Thrown()
        {
            // Act
            var innerException = new SqlLocalDbException(
                "It broke",
                123,
                "Name");

            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.ShareInstance(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(innerException);

            ISqlLocalDbInstanceInfo instance = CreateInstance();
            ISqlLocalDbApi api = mock.Object;

            var target = new SqlLocalDbInstanceManager(instance, api);

            var exception = Assert.Throws<SqlLocalDbException>(() => target.Share("SharedName"));

            exception.ErrorCode.ShouldBe(123);
            exception.InstanceName.ShouldBe("Name");
            exception.Message.ShouldBe("Failed to share SQL LocalDB instance 'Name'.");
            exception.InnerException.ShouldBeSameAs(innerException);
        }

        [Fact]
        public void Constructor_Validates_Arguments()
        {
            // Act
            ISqlLocalDbInstanceInfo instance = CreateInstance();
            var api = Mock.Of<ISqlLocalDbApi>();

            // Act and Assert
            Assert.Throws<ArgumentNullException>("instance", () => new SqlLocalDbInstanceManager(null, api));
            Assert.Throws<ArgumentNullException>("api", () => new SqlLocalDbInstanceManager(instance, null));
        }

        [Fact]
        public void Constructor_Initializes_Properties()
        {
            // Act
            ISqlLocalDbInstanceInfo instance = CreateInstance();
            var api = Mock.Of<ISqlLocalDbApi>();

            // Act
            var actual = new SqlLocalDbInstanceManager(instance, api);

            // Assert
            actual.Name.ShouldBe("Name");
            actual.NamedPipe.ShouldBe("NamedPipe");
        }

        [Fact]
        public void Share_Throws_If_SharedName_Is_Null()
        {
            // Act
            ISqlLocalDbInstanceInfo instance = CreateInstance();
            var api = Mock.Of<ISqlLocalDbApi>();
            var target = new SqlLocalDbInstanceManager(instance, api);

            Assert.Throws<ArgumentNullException>("sharedName", () => target.Share(null));
        }

        [Fact]
        public void Start_Starts_Instance()
        {
            // Act
            var mock = new Mock<ISqlLocalDbApi>();
            ISqlLocalDbInstanceInfo instance = CreateInstance();
            ISqlLocalDbApi api = mock.Object;

            var target = new SqlLocalDbInstanceManager(instance, api);

            // Act
            target.Start();

            // Assert
            mock.Verify((p) => p.StartInstance("Name"), Times.Once());
        }

        [Fact]
        public void Start_Throws_If_SqlLocalDbEception_Is_Thrown()
        {
            // Act
            var innerException = new SqlLocalDbException(
                "It broke",
                123,
                "Name");

            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.StartInstance(It.IsAny<string>()))
                .Throws(innerException);

            ISqlLocalDbInstanceInfo instance = CreateInstance();
            ISqlLocalDbApi api = mock.Object;

            var target = new SqlLocalDbInstanceManager(instance, api);

            var exception = Assert.Throws<SqlLocalDbException>(() => target.Start());

            exception.ErrorCode.ShouldBe(123);
            exception.InstanceName.ShouldBe("Name");
            exception.Message.ShouldBe("Failed to start SQL LocalDB instance 'Name'.");
            exception.InnerException.ShouldBeSameAs(innerException);
        }

        [Fact]
        public void Stop_Stops_Instance()
        {
            // Act
            var mock = new Mock<ISqlLocalDbApi>();
            ISqlLocalDbInstanceInfo instance = CreateInstance();
            ISqlLocalDbApi api = mock.Object;

            var target = new SqlLocalDbInstanceManager(instance, api);

            // Act
            target.Stop();

            // Assert
            mock.Verify((p) => p.StopInstance("Name", null), Times.Once());
        }

        [Fact]
        public void Stop_Throws_If_SqlLocalDbEception_Is_Thrown()
        {
            // Act
            var innerException = new SqlLocalDbException(
                "It broke",
                123,
                "Name");

            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.StopInstance(It.IsAny<string>(), It.IsAny<TimeSpan?>()))
                .Throws(innerException);

            ISqlLocalDbInstanceInfo instance = CreateInstance();
            ISqlLocalDbApi api = mock.Object;

            var target = new SqlLocalDbInstanceManager(instance, api);

            var exception = Assert.Throws<SqlLocalDbException>(() => target.Stop());

            exception.ErrorCode.ShouldBe(123);
            exception.InstanceName.ShouldBe("Name");
            exception.Message.ShouldBe("Failed to stop SQL LocalDB instance 'Name'.");
            exception.InnerException.ShouldBeSameAs(innerException);
        }

        [Fact]
        public void Unshare_Unshares_Instance()
        {
            // Act
            var mock = new Mock<ISqlLocalDbApi>();
            ISqlLocalDbInstanceInfo instance = CreateInstance();
            ISqlLocalDbApi api = mock.Object;

            var target = new SqlLocalDbInstanceManager(instance, api);

            // Act
            target.Unshare();

            // Assert
            mock.Verify((p) => p.UnshareInstance("Name"), Times.Once());
        }

        [Fact]
        public void Unshare_Throws_If_SqlLocalDbEception_Is_Thrown()
        {
            // Act
            var innerException = new SqlLocalDbException(
                "It broke",
                123,
                "Name");

            var mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.UnshareInstance(It.IsAny<string>()))
                .Throws(innerException);

            ISqlLocalDbInstanceInfo instance = CreateInstance();
            ISqlLocalDbApi api = mock.Object;

            var target = new SqlLocalDbInstanceManager(instance, api);

            var exception = Assert.Throws<SqlLocalDbException>(() => target.Unshare());

            exception.ErrorCode.ShouldBe(123);
            exception.InstanceName.ShouldBe("Name");
            exception.Message.ShouldBe("Failed to stop sharing SQL LocalDB instance 'Name'.");
            exception.InnerException.ShouldBeSameAs(innerException);
        }

        [RunAsAdminFact]
        public void Manager_Shares_And_Unshares_Instance()
        {
            // Act
            string instanceName = Guid.NewGuid().ToString();
            string sharedName = Guid.NewGuid().ToString();

            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                api.CreateInstance(instanceName);

                try
                {
                    api.StartInstance(instanceName);

                    try
                    {
                        var instance = api.GetInstanceInfo(instanceName);

                        var manager = new SqlLocalDbInstanceManager(instance, api);

                        // Act
                        manager.Share(sharedName);

                        // Assert
                        instance.IsShared.ShouldBeTrue();

                        // Act
                        manager.Unshare();

                        // Assert
                        instance.IsShared.ShouldBeFalse();
                    }
                    catch (Exception)
                    {
                        api.StopInstance(instanceName);
                        throw;
                    }
                }
                catch (Exception)
                {
                    api.DeleteInstanceInternal(instanceName, throwIfNotFound: false, deleteFiles: true);
                    throw;
                }
            }
        }

        private static ISqlLocalDbInstanceInfo CreateInstance()
        {
            var mock = new Mock<ISqlLocalDbInstanceInfo>();

            mock.Setup((p) => p.Name).Returns("Name");
            mock.Setup((p) => p.NamedPipe).Returns("NamedPipe");

            return mock.Object;
        }
    }
}
