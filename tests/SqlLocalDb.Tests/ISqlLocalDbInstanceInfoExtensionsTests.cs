// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using Microsoft.Data.SqlClient;
using Moq;
using Shouldly;
using Xunit;

namespace MartinCostello.SqlLocalDb
{
    public static class ISqlLocalDbInstanceInfoExtensionsTests
    {
        [Fact]
        public static void CreateConnection_Throws_If_Instance_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstanceInfo instance = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("instance", () => instance.CreateConnection());
        }

        [Fact]
        public static void CreateConnectionStringBuilder_Throws_If_Instance_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstanceInfo instance = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("instance", () => instance.CreateConnectionStringBuilder());
        }

        [Fact]
        public static void CreateConnectionStringBuilder_Throws_If_The_Instance_Is_Not_Running()
        {
            // Arrange
            var mock = new Mock<ISqlLocalDbInstanceInfo>();

            mock.Setup((p) => p.IsRunning).Returns(false);
            mock.Setup((p) => p.Name).Returns("MyInstance");
            mock.Setup((p) => p.NamedPipe).Returns("MyNamedPipe");

            ISqlLocalDbInstanceInfo instance = mock.Object;

            // Act and Assert
            var exception = Assert.Throws<InvalidOperationException>(() => instance.CreateConnectionStringBuilder());
            exception.Message.ShouldBe("The SQL LocalDB instance 'MyInstance' is not running.");
        }

        [Fact]
        public static void CreateConnectionStringBuilder_Initializes_Connection_String_Builder()
        {
            // Arrange
            var mock = new Mock<ISqlLocalDbInstanceInfo>();

            mock.Setup((p) => p.IsRunning).Returns(true);
            mock.Setup((p) => p.NamedPipe).Returns("MyNamedPipe");

            ISqlLocalDbInstanceInfo instance = mock.Object;

            // Act
            SqlConnectionStringBuilder actual = instance.CreateConnectionStringBuilder();

            // Assert
            actual.ShouldNotBeNull();
            actual.DataSource.ShouldBe("MyNamedPipe");
        }

        [Fact]
        public static void GetConnectionString_Returns_Sql_Connection_String()
        {
            // Arrange
            var mock = new Mock<ISqlLocalDbInstanceInfo>();

            mock.Setup((p) => p.IsRunning).Returns(true);
            mock.Setup((p) => p.NamedPipe).Returns("MyNamedPipe");

            ISqlLocalDbInstanceInfo instance = mock.Object;

            // Act
            string actual = instance.GetConnectionString();

            // Assert
            actual.ShouldBe("Data Source=MyNamedPipe");
        }

        [Fact]
        public static void Manage_Throws_If_Instance_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstanceInfo instance = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("instance", () => instance.Manage());
        }

        [Fact]
        public static void Manage_Throws_If_Instance_Does_Not_Implement_ISqlLocalDbApiAdapter()
        {
            // Arrange
            var instance = Mock.Of<ISqlLocalDbInstanceInfo>();

            // Act and Assert
            Assert.Throws<ArgumentException>("instance", () => instance.Manage()).Message.ShouldStartWith("The specified instance of ISqlLocalDbInstanceInfo does not implement the ISqlLocalDbApiAdapter interface.");
        }

        [Fact]
        public static void Manage_Returns_An_ISqlLocalDbInstanceManager()
        {
            // Arrange
            var api = Mock.Of<ISqlLocalDbApi>();
            var mock = new Mock<ISqlLocalDbInstanceInfo>();

            mock.As<ISqlLocalDbApiAdapter>()
                .Setup((p) => p.LocalDb).Returns(api);

            ISqlLocalDbInstanceInfo instance = mock.Object;

            // Act
            ISqlLocalDbInstanceManager actual = instance.Manage();

            // Assert
            actual.ShouldNotBeNull();

            var adapter = actual as ISqlLocalDbApiAdapter;

            adapter.ShouldNotBeNull();
            adapter.LocalDb.ShouldBeSameAs(api);
        }
    }
}
