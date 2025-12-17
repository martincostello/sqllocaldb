// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using FsCheck;
using FsCheck.Xunit;
using MartinCostello.SqlLocalDb.Interop;

namespace MartinCostello.SqlLocalDb;

#if NETFRAMEWORK
[Collection(FuzzCollection.Name)]
#else
[Collection<FuzzCollection>]
#endif
public class FuzzTests(LocalDbFixture fixture)
{
    [Property]
    public void MarshalString_Handles_Arbitrary_Byte_Arrays(byte[] bytes)
    {
        // Arrange
        if (bytes == null || bytes.Length > 10000)
        {
            return;
        }

        // Act
        string result = LocalDbInstanceApi.MarshalString(bytes);

        // Assert
        result.ShouldNotBeNull();
    }

    [Property]
    public void MarshalString_Handles_Unicode_Strings(string input)
    {
        // Arrange
        if (input == null)
        {
            return;
        }

        byte[] bytes = Encoding.Unicode.GetBytes(input);

        // Act
        string result = LocalDbInstanceApi.MarshalString(bytes);

        // Assert
        result.ShouldNotBeNull();
        result.ShouldNotEndWith("\0");
    }

    [Property]
    public void LocalDbInstanceApi_Constructor_Handles_Arbitrary_Version_Strings(string apiVersion)
    {
        // Act
        using var target = LocalDbFixture.CreateLocalDbApi(apiVersion);

        // Assert
        target.ShouldNotBeNull();
    }

    [Property]
    public void LocalDbInstanceApi_CreateInstance_Handles_Arbitrary_Strings(
        NonNull<string> version,
        NonNull<string> instanceName)
    {
        // Act and Assert
        Should.NotThrow(() => fixture.Target.CreateInstance(version.Get, instanceName.Get, 0));
    }

    [Property]
    public void LocalDbInstanceApi_DeleteInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Act and Assert
        Should.NotThrow(() => fixture.Target.DeleteInstance(instanceName.Get, 0));
    }

    [Property]
    public void LocalDbInstanceApi_GetInstanceInfo_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Act and Assert
        Should.NotThrow(() => fixture.Target.GetInstanceInfo(instanceName.Get, IntPtr.Zero, 0));
    }

    [Property]
    public void LocalDbInstanceApi_GetVersionInfo_Handles_Arbitrary_Strings(NonNull<string> versionName)
    {
        // Act and Assert
        Should.NotThrow(() => fixture.Target.GetVersionInfo(versionName.Get, IntPtr.Zero, 0));
    }

    [Property]
    public void LocalDbInstanceApi_ShareInstance_Handles_Arbitrary_Strings(
        NonNull<string> privateName,
        NonNull<string> sharedName)
    {
        // Act and Assert
        Should.NotThrow(() => fixture.Target.ShareInstance(IntPtr.Zero, privateName.Get, sharedName.Get, 0));
    }

    [Property]
    public void LocalDbInstanceApi_StartInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Arrange
        var buffer = new StringBuilder(261);
        int size = buffer.Capacity;

        // Act and Assert
        Should.NotThrow(() => fixture.Target.StartInstance(instanceName.Get, 0, buffer, ref size));
    }

    [Property]
    public void LocalDbInstanceApi_StopInstance_Handles_Arbitrary_Strings(
        NonNull<string> instanceName,
        NonNegativeInt timeout)
    {
        // Act and Assert
        Should.NotThrow(() => fixture.Target.StopInstance(instanceName.Get, StopInstanceOptions.None, timeout.Get));
    }

    [Property]
    public void LocalDbInstanceApi_UnshareInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Act and Assert
        Should.NotThrow(() => fixture.Target.UnshareInstance(instanceName.Get, 0));
    }

    [Property]
    public void LocalDbInstanceApi_GetLocalDbError_Handles_Arbitrary_Error_Codes(int errorCode, int languageId)
    {
        // Arrange
        var buffer = new StringBuilder(261);
        int size = buffer.Capacity;

        // Act and Assert
        Should.NotThrow(() => fixture.Target.GetLocalDbError(errorCode, languageId, buffer, ref size));
    }

    [Property]
    public void LocalDbInstanceApi_Disposal_Is_Idempotent(PositiveInt callCount)
    {
        // Arrange
        using var target = LocalDbFixture.CreateLocalDbApi();

        // Act
        for (int i = 0; i < Math.Min(callCount.Get, 100); i++)
        {
            target.Dispose();
        }

        // Assert
        target.ShouldNotBeNull();
    }

    [Property]
    public void LocalDbInstanceApi_TryGetLocalDbApiPath_Handles_Arbitrary_Version_Strings(string apiVersion)
    {
        // Arrange
        using var target = LocalDbFixture.CreateLocalDbApi(apiVersion);

        // Act
        Should.NotThrow(() => target.TryGetLocalDbApiPath(out _));
    }

    [Property]
    public void LocalDbInstanceApi_GetInstanceNames_Handles_Arbitrary_Counts()
    {
        // Arrange
        int count = 0;

        // Act and Assert
        Should.NotThrow(() => fixture.Target.GetInstanceNames(IntPtr.Zero, ref count));
    }

    [Property]
    public void LocalDbInstanceApi_GetVersions_Handles_Arbitrary_Counts()
    {
        // Arrange
        int count = 0;

        // Act and Assert
        Should.NotThrow(() => fixture.Target.GetVersions(IntPtr.Zero, ref count));
    }
}
