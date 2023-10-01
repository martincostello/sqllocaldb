// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NSubstitute;

namespace MartinCostello.SqlLocalDb;

public class SqlLocalDbInstanceManagerTests(ITestOutputHelper outputHelper)
{
    private readonly ILoggerFactory _loggerFactory = outputHelper.ToLoggerFactory();

    [WindowsOnlyFact]
    public static void Share_Shares_Instance()
    {
        // Act
        string sharedName = Guid.NewGuid().ToString();

        var api = Substitute.For<ISqlLocalDbApi>();
        ISqlLocalDbInstanceInfo instance = CreateInstance();

        var target = new SqlLocalDbInstanceManager(instance, api);

        // Act
        target.Share(sharedName);

        // Assert
        api.Received(1).ShareInstance(Arg.Is<string>((p) => p != null), "Name", sharedName);
    }

    [WindowsOnlyFact]
    public static void Share_Throws_If_SqlLocalDbException_Is_Thrown()
    {
        // Act
        var innerException = new SqlLocalDbException(
            "It broke",
            123,
            "Name");

        var api = Substitute.For<ISqlLocalDbApi>();

        api.When((p) => p.ShareInstance(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>()))
            .Do((_) => throw innerException);

        ISqlLocalDbInstanceInfo instance = CreateInstance();

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
        var api = Substitute.For<ISqlLocalDbApi>();

        // Act and Assert
        Assert.Throws<ArgumentNullException>("instance", () => new SqlLocalDbInstanceManager(null!, api));
        Assert.Throws<ArgumentNullException>("api", () => new SqlLocalDbInstanceManager(instance, null!));
    }

    [Fact]
    public void Constructor_Initializes_Properties()
    {
        // Act
        ISqlLocalDbInstanceInfo instance = CreateInstance();
        var api = Substitute.For<ISqlLocalDbApi>();

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
        var api = Substitute.For<ISqlLocalDbApi>();
        var target = new SqlLocalDbInstanceManager(instance, api);

        Assert.Throws<ArgumentNullException>("sharedName", () => target.Share(null!));
    }

    [Fact]
    public void Start_Starts_Instance()
    {
        // Act
        var api = Substitute.For<ISqlLocalDbApi>();
        ISqlLocalDbInstanceInfo instance = CreateInstance();

        var target = new SqlLocalDbInstanceManager(instance, api);

        // Act
        target.Start();

        // Assert
        api.Received(1).StartInstance("Name");
    }

    [Fact]
    public void Start_Throws_If_SqlLocalDbException_Is_Thrown()
    {
        // Act
        var innerException = new SqlLocalDbException(
            "It broke",
            123,
            "Name");

        var api = Substitute.For<ISqlLocalDbApi>();

        api.When((p) => p.StartInstance(Arg.Any<string>()))
           .Do((_) => throw innerException);

        ISqlLocalDbInstanceInfo instance = CreateInstance();

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
        var api = Substitute.For<ISqlLocalDbApi>();
        ISqlLocalDbInstanceInfo instance = CreateInstance();

        var target = new SqlLocalDbInstanceManager(instance, api);

        // Act
        target.Stop();

        // Assert
        api.Received(1).StopInstance("Name", null);
    }

    [Fact]
    public void Stop_Throws_If_SqlLocalDbException_Is_Thrown()
    {
        // Act
        var innerException = new SqlLocalDbException(
            "It broke",
            123,
            "Name");

        var api = Substitute.For<ISqlLocalDbApi>();

        api.When((p) => p.StopInstance(Arg.Any<string>(), Arg.Any<TimeSpan?>()))
           .Do((_) => throw innerException);

        ISqlLocalDbInstanceInfo instance = CreateInstance();

        var target = new SqlLocalDbInstanceManager(instance, api);

        var exception = Assert.Throws<SqlLocalDbException>(target.Stop);

        exception.ErrorCode.ShouldBe(123);
        exception.InstanceName.ShouldBe("Name");
        exception.Message.ShouldBe("Failed to stop SQL LocalDB instance 'Name'.");
        exception.InnerException.ShouldBeSameAs(innerException);
    }

    [Fact]
    public void Unshare_Unshares_Instance()
    {
        // Act
        var api = Substitute.For<ISqlLocalDbApi>();
        ISqlLocalDbInstanceInfo instance = CreateInstance();

        var target = new SqlLocalDbInstanceManager(instance, api);

        // Act
        target.Unshare();

        // Assert
        api.Received(1).UnshareInstance("Name");
    }

    [Fact]
    public void Unshare_Throws_If_SqlLocalDbException_Is_Thrown()
    {
        // Act
        var innerException = new SqlLocalDbException(
            "It broke",
            123,
            "Name");

        var api = Substitute.For<ISqlLocalDbApi>();

        api.When((p) => p.UnshareInstance(Arg.Any<string>()))
           .Do((_) => throw innerException);

        ISqlLocalDbInstanceInfo instance = CreateInstance();

        var target = new SqlLocalDbInstanceManager(instance, api);

        var exception = Assert.Throws<SqlLocalDbException>(target.Unshare);

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

        using var api = new SqlLocalDbApi(_loggerFactory);
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

    private static ISqlLocalDbInstanceInfo CreateInstance()
    {
        var instance = Substitute.For<ISqlLocalDbInstanceInfo>();

        instance.Name.Returns("Name");
        instance.NamedPipe.Returns("NamedPipe");

        return instance;
    }
}
