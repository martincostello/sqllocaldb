// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using NSubstitute;

namespace MartinCostello.SqlLocalDb;

public class ISqlLocalDbApiExtensionsTests(ITestOutputHelper outputHelper)
{
    private readonly ILoggerFactory _loggerFactory = outputHelper.ToLoggerFactory();

    [Theory]
    [InlineData(unchecked((int)0x89c50112))]
    [InlineData(unchecked((int)0x89c50108))]
    public static void TemporaryInstance_Ignores_Exception_If_Delete_Fails(int errorCode)
    {
        // Arrange
        Assert.SkipWhen(SqlLocalDbApi.IsWindows, "Not expected to fail on Windows.");

        // Arrange
        var api = Substitute.For<ISqlLocalDbApi>();

        api.LatestVersion.Returns("v99.9");
        api.CreateInstance(Arg.Any<string>(), "v99.9");
        api.StartInstance(Arg.Any<string>());
        api.StopInstance(Arg.Any<string>(), null);

        api.When((p) => p.DeleteInstance(Arg.Any<string>()))
           .Do((_) => throw new SqlLocalDbException("Error", errorCode));

        // Act and Assert
        Should.NotThrow(() =>
        {
            using var target = api.CreateTemporaryInstance();
            target.GetInstanceInfo();
        });
    }

    [WindowsOnlyFact]
    public static void ShareInstance_Uses_SID_For_Current_User()
    {
        // Arrange
        string instanceName = "SomeName";
        string sharedInstanceName = "SomeSharedName";

        var api = Substitute.For<ISqlLocalDbApi>();

        // Act
        api.ShareInstance(instanceName, sharedInstanceName);

        // Assert
        api.Received(1).ShareInstance(Arg.Is<string>((p) => p != null), instanceName, sharedInstanceName);
    }

    [Fact]
    public void CreateTemporaryInstance_Throws_If_Api_Is_Null()
    {
        // Arrange
        ISqlLocalDbApi? api = null;

        // Act and Assert
        Assert.Throws<ArgumentNullException>("api", () => api!.CreateTemporaryInstance());
    }

    [Fact]
    public void TemporaryInstance_Throws_If_Used_After_Disposal()
    {
        // Arrange
        var api = Substitute.For<ISqlLocalDbApi>();

        TemporarySqlLocalDbInstance instance = api.CreateTemporaryInstance();

        // Act
        instance.Dispose();

        // Assert
        Assert.Throws<ObjectDisposedException>(() => instance.Name);
        Assert.Throws<ObjectDisposedException>(instance.GetInstanceInfo);
    }

    [Fact]
    public void TemporaryInstance_Disposes_Cleanly_If_Not_Used()
    {
        // Arrange
        var api = Substitute.For<ISqlLocalDbApi>();

        // Act and Assert
        using TemporarySqlLocalDbInstance instance = api.CreateTemporaryInstance();
        instance.Dispose();
    }

    [Fact]
    public void TemporaryInstance_Is_ISqlLocalDbApiAdapter()
    {
        // Arrange
        var api = Substitute.For<ISqlLocalDbApi>();

        using TemporarySqlLocalDbInstance instance = api.CreateTemporaryInstance();

        // Act
        ISqlLocalDbApiAdapter adapter = instance;

        // Assert
        adapter.LocalDb.ShouldBeSameAs(api);
    }

    [Fact]
    public void TemporaryInstance_Deletes_Instance_If_Start_Fails()
    {
        // Arrange
        var api = Substitute.For<ISqlLocalDbApi>();

        api.LatestVersion.Returns("v99.9");

        api.When((p) => p.StartInstance(Arg.Any<string>()))
           .Do((_) => throw new PlatformNotSupportedException());

        using (TemporarySqlLocalDbInstance target = api.CreateTemporaryInstance())
        {
            // Act and Assert
            Assert.Throws<PlatformNotSupportedException>(target.GetInstanceInfo);
        }

        // Assert
        api.Received(1).CreateInstance(Arg.Any<string>(), "v99.9");
        api.Received(1).DeleteInstance(Arg.Any<string>());
    }

    [Theory]
    [InlineData(unchecked((int)0x89c50108))]
    [InlineData(unchecked((int)0x89c50107))]
    public void TemporaryInstance_Ignores_Exception_If_Stop_Fails(int errorCode)
    {
        // Arrange
        var api = Substitute.For<ISqlLocalDbApi>();

        api.LatestVersion.Returns("v99.9");

        api.When((p) => p.StopInstance(Arg.Any<string>(), null))
           .Do((_) => throw new SqlLocalDbException("Error", errorCode));

        // Act
        using (var target = api.CreateTemporaryInstance())
        {
            target.GetInstanceInfo();
        }

        // Assert
        api.Received(1).CreateInstance(Arg.Any<string>(), "v99.9");
        api.Received(1).StartInstance(Arg.Any<string>());
        api.Received(1).DeleteInstance(Arg.Any<string>());
    }

    [Fact]
    public void GetDefaultInstance_Throws_If_Api_Is_Null()
    {
        // Arrange
        ISqlLocalDbApi? api = null;

        // Act and Assert
        Assert.Throws<ArgumentNullException>("api", () => api!.GetDefaultInstance());
    }

    [WindowsOnlyFact]
    public void GetDefaultInstance_Returns_The_Default_Instance()
    {
        // Arrange
        using var api = new SqlLocalDbApi(_loggerFactory);

        // Act
        ISqlLocalDbInstanceInfo actual = api.GetDefaultInstance();

        // Assert
        actual.ShouldNotBeNull();
        actual.IsAutomatic.ShouldBeTrue();
        actual.Name.ShouldBe(api.DefaultInstanceName);
    }

    [Fact]
    public void GetInstances_Throws_If_Api_Is_Null()
    {
        // Arrange
        ISqlLocalDbApi? api = null;

        // Act and Assert
        Assert.Throws<ArgumentNullException>("api", () => api!.GetInstances());
    }

    [WindowsOnlyFact]
    public void GetInstances_Returns_All_The_Named_Instances()
    {
        // Arrange
        using var api = new SqlLocalDbApi(_loggerFactory);

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

    [Fact]
    public void GetInstances_Returns_Empty_If_No_Instances()
    {
        // Arrange
        var api = Substitute.For<ISqlLocalDbApi>();

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
        ISqlLocalDbApi? api = null;

        // Act and Assert
        Assert.Throws<ArgumentNullException>("api", () => api!.GetVersions());
    }

    [WindowsOnlyFact]
    public void GetVersions_Returns_All_The_Installed_Versions()
    {
        // Arrange
        using var api = new SqlLocalDbApi(_loggerFactory);

        // Act
        IReadOnlyList<ISqlLocalDbVersionInfo> actual = api.GetVersions();

        // Assert
        actual.ShouldNotBeNull();
        actual.Count.ShouldBeGreaterThanOrEqualTo(1);
        actual.ShouldBeUnique();
        actual.ShouldAllBe((p) => p != null);
        actual.ShouldContain((p) => p.Exists);
    }

    [Fact]
    public void GetVersions_Returns_Empty_If_No_Versions()
    {
        // Arrange
        var api = Substitute.For<ISqlLocalDbApi>();

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
        ISqlLocalDbApi? api = null;
        string instanceName = "SomeName";

        // Act and Assert
        Assert.Throws<ArgumentNullException>("api", () => api!.GetOrCreateInstance(instanceName));
    }

    [Fact]
    public void GetOrCreateInstance_Throws_If_InstanceName_Is_Null()
    {
        // Arrange
        ISqlLocalDbApi api = Substitute.For<ISqlLocalDbApi>();
        string? instanceName = null;

        // Act and Assert
        Assert.Throws<ArgumentNullException>("instanceName", () => api.GetOrCreateInstance(instanceName!));
    }

    [Fact]
    public void GetOrCreateInstance_Returns_The_Default_Instance_If_It_Exists_With_The_Default_Name()
    {
        // Arrange
        string instanceName = "Default";

        var instance = CreateInstanceInfo(exists: true);
        var api = Substitute.For<ISqlLocalDbApi>();

        api.DefaultInstanceName
           .Returns(instanceName);

        api.GetInstanceInfo(instanceName)
           .Returns(instance);

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
        var instance = CreateInstanceInfo(exists: true);
        var api = Substitute.For<ISqlLocalDbApi>();

        api.DefaultInstanceName
           .Returns("Blah");

        api.GetInstanceInfo(instanceName)
           .Returns(instance);

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

        var instance = CreateInstanceInfo(exists: true);
        var api = Substitute.For<ISqlLocalDbApi>();

        api.DefaultInstanceName
           .Returns("Blah");

        api.InstanceExists(instanceName)
           .Returns(true);

        api.GetInstanceInfo(instanceName)
           .Returns(instance);

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

        var instance = CreateInstanceInfo(exists: false);
        var api = Substitute.For<ISqlLocalDbApi>();

        api.DefaultInstanceName
           .Returns("Blah");

        api.LatestVersion
           .Returns("v99.0");

        api.CreateInstance(instanceName, "v99.0")
           .Returns(instance);

        // Act
        ISqlLocalDbInstanceInfo actual = api.GetOrCreateInstance(instanceName);

        // Assert
        actual.ShouldNotBeNull();
    }

    [Fact]
    public void ShareInstance_Throws_If_Api_Is_Null()
    {
        // Arrange
        ISqlLocalDbApi? api = null;
        string instanceName = "SomeName";
        string sharedInstanceName = "SomeSharedName";

        // Act and Assert
        Assert.Throws<ArgumentNullException>("api", () => api!.ShareInstance(instanceName, sharedInstanceName));
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
        using var api = new SqlLocalDbApi(_loggerFactory);
        using TemporarySqlLocalDbInstance target = api.CreateTemporaryInstance(deleteFiles: true);

        target.GetInstanceInfo().IsShared.ShouldBeFalse();

        // Act
        api.ShareInstance(target.Name, Guid.NewGuid().ToString());

        // Assert
        target.GetInstanceInfo().IsShared.ShouldBeTrue();
    }

    private static ISqlLocalDbInstanceInfo CreateInstanceInfo(bool exists)
    {
        var instance = Substitute.For<ISqlLocalDbInstanceInfo>();

        instance.Exists.Returns(exists);

        return instance;
    }

    private void CreateTemporaryInstance_Creates_Starts_And_Deletes_An_Instance(bool deleteFiles)
    {
        // Arrange
        using var api = new SqlLocalDbApi(_loggerFactory);

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
