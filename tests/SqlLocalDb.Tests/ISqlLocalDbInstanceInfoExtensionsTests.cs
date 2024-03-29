﻿// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Data.SqlClient;
using NSubstitute;

namespace MartinCostello.SqlLocalDb;

public static class ISqlLocalDbInstanceInfoExtensionsTests
{
    [Fact]
    public static void CreateConnection_Throws_If_Instance_Is_Null()
    {
        // Arrange
        ISqlLocalDbInstanceInfo? instance = null;

        // Act and Assert
        Assert.Throws<ArgumentNullException>("instance", () => instance!.CreateConnection());
    }

    [Fact]
    public static void CreateConnectionStringBuilder_Throws_If_Instance_Is_Null()
    {
        // Arrange
        ISqlLocalDbInstanceInfo? instance = null;

        // Act and Assert
        Assert.Throws<ArgumentNullException>("instance", () => instance!.CreateConnectionStringBuilder());
    }

    [Fact]
    public static void CreateConnectionStringBuilder_Throws_If_The_Instance_Is_Not_Running()
    {
        // Arrange
        var instance = Substitute.For<ISqlLocalDbInstanceInfo>();

        instance.IsRunning.Returns(false);
        instance.Name.Returns("MyInstance");
        instance.NamedPipe.Returns("MyNamedPipe");

        // Act and Assert
        var exception = Assert.Throws<InvalidOperationException>(instance.CreateConnectionStringBuilder);
        exception.Message.ShouldBe("The SQL LocalDB instance 'MyInstance' is not running.");
    }

    [Fact]
    public static void CreateConnectionStringBuilder_Initializes_Connection_String_Builder()
    {
        // Arrange
        var instance = Substitute.For<ISqlLocalDbInstanceInfo>();

        instance.IsRunning.Returns(true);
        instance.NamedPipe.Returns("MyNamedPipe");

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
        var instance = Substitute.For<ISqlLocalDbInstanceInfo>();

        instance.IsRunning.Returns(true);
        instance.NamedPipe.Returns("MyNamedPipe");

        // Act
        string actual = instance.GetConnectionString();

        // Assert
        actual.ShouldBe("Data Source=MyNamedPipe");
    }

    [Fact]
    public static void Manage_Throws_If_Instance_Is_Null()
    {
        // Arrange
        ISqlLocalDbInstanceInfo? instance = null;

        // Act and Assert
        Assert.Throws<ArgumentNullException>("instance", () => instance!.Manage());
    }

    [Fact]
    public static void Manage_Throws_If_Instance_Does_Not_Implement_ISqlLocalDbApiAdapter()
    {
        // Arrange
        var instance = Substitute.For<ISqlLocalDbInstanceInfo>();

        // Act and Assert
        Assert.Throws<ArgumentException>("instance", instance.Manage).Message.ShouldStartWith("The specified instance of ISqlLocalDbInstanceInfo does not implement the ISqlLocalDbApiAdapter interface.");
    }

    [Fact]
    public static void Manage_Returns_An_ISqlLocalDbInstanceManager()
    {
        // Arrange
        var api = Substitute.For<ISqlLocalDbApi>();
        var instance = Substitute.For<ISqlLocalDbInstanceInfo, ISqlLocalDbApiAdapter>();

        ((ISqlLocalDbApiAdapter)instance).LocalDb.Returns(api);

        // Act
        ISqlLocalDbInstanceManager actual = instance.Manage();

        // Assert
        actual.ShouldNotBeNull();

        var adapter = actual as ISqlLocalDbApiAdapter;

        adapter.ShouldNotBeNull();
        adapter!.LocalDb.ShouldBeSameAs(api);
    }
}
