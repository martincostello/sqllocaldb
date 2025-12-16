// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using FsCheck;
using FsCheck.Xunit;
using MartinCostello.SqlLocalDb.Interop;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace MartinCostello.SqlLocalDb;

public static class FuzzTests
{
    [Property]
    public static void MarshalString_Handles_Arbitrary_Byte_Arrays(byte[] bytes)
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
    public static void MarshalString_Handles_Unicode_Strings(string input)
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
    public static void LocalDbInstanceApi_Constructor_Handles_Arbitrary_Version_Strings(string apiVersion)
    {
        // Arrange
        var registry = Substitute.For<IRegistry>();
        var logger = NullLogger<LocalDbInstanceApi>.Instance;

        // Act
        using var target = new LocalDbInstanceApi(apiVersion ?? string.Empty, registry, logger);

        // Assert
        target.ShouldNotBeNull();
    }

    [Property]
    public static void LocalDbInstanceApi_CreateInstance_Handles_Arbitrary_Strings(
        NonNull<string> version,
        NonNull<string> instanceName)
    {
        // Arrange
        using var target = CreateLocalDbApi();

        // Act and Assert
        Should.NotThrow(() => target.CreateInstance(version.Get, instanceName.Get, 0));
    }

    [Property]
    public static void LocalDbInstanceApi_DeleteInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Arrange
        using var target = CreateLocalDbApi();

        // Act and Assert
        Should.NotThrow(() => target.DeleteInstance(instanceName.Get, 0));
    }

    [Property]
    public static void LocalDbInstanceApi_GetInstanceInfo_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Arrange
        using var target = CreateLocalDbApi();

        // Act
        int result = target.GetInstanceInfo(instanceName.Get, IntPtr.Zero, 0);

        // Assert
        result.ShouldNotBe(0);
    }

    [Property]
    public static void LocalDbInstanceApi_GetVersionInfoHandlesArbitraryStrings(NonNull<string> versionName)
    {
        // Arrange
        using var target = CreateLocalDbApi();

        // Act
        int result = target.GetVersionInfo(versionName.Get, IntPtr.Zero, 0);

        // Assert
        result.ShouldNotBe(0);
    }

    [Property]
    public static void LocalDbInstanceApi_ShareInstance_Handles_Arbitrary_Strings(
        NonNull<string> privateName,
        NonNull<string> sharedName)
    {
        // Arrange
        using var target = CreateLocalDbApi();

        // Act
        int result = target.ShareInstance(IntPtr.Zero, privateName.Get, sharedName.Get, 0);

        // Assert
        result.ShouldNotBe(0);
    }

    [Property]
    public static void LocalDbInstanceApi_StartInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Arrange
        using var target = CreateLocalDbApi();

        var buffer = new StringBuilder(261);
        int size = buffer.Capacity;

        // Act and Assert
        Should.NotThrow(() => target.StartInstance(instanceName.Get, 0, buffer, ref size));
    }

    [Property]
    public static void LocalDbInstanceApi_StopInstance_Handles_Arbitrary_Strings(
        NonNull<string> instanceName,
        NonNegativeInt timeout)
    {
        // Arrange
        using var target = CreateLocalDbApi();

        // Act and Assert
        Should.NotThrow(() => target.StopInstance(instanceName.Get, StopInstanceOptions.None, timeout.Get));
    }

    [Property]
    public static void LocalDbInstanceApi_UnshareInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Arrange
        using var target = CreateLocalDbApi();

        // Act
        int result = target.UnshareInstance(instanceName.Get, 0);

        // Assert
        result.ShouldNotBe(0);
    }

    [Property]
    public static void LocalDbInstanceApi_GetLocalDbError_HandlesArbitrary_Error_Codes(int errorCode, int languageId)
    {
        // Arrange
        using var target = CreateLocalDbApi();

        var buffer = new StringBuilder(261);
        int size = buffer.Capacity;

        // Act and Assert
        Should.NotThrow(() => target.GetLocalDbError(errorCode, languageId, buffer, ref size));
    }

    [Property]
    public static void LocalDbInstanceApi_Disposal_Is_Idempotent(PositiveInt callCount)
    {
        // Arrange
        using var target = CreateLocalDbApi();

        // Act
        for (int i = 0; i < Math.Min(callCount.Get, 100); i++)
        {
            target.Dispose();
        }

        // Assert
        target.ShouldNotBeNull();
    }

    [Property]
    public static void LocalDbInstanceApi_TryGetLocalDbApiPath_Handles_Arbitrary_Version_Strings(string apiVersion)
    {
        // Arrange
        using var target = CreateLocalDbApi(apiVersion);

        // Act
        bool result = target.TryGetLocalDbApiPath(out string? fileName);

        // Assert
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            result.ShouldBeTrue();
            fileName.ShouldNotBeNullOrWhiteSpace();
        }
        else
        {
            result.ShouldBeFalse();
            fileName.ShouldBeNull();
        }
    }

    [Property]
    public static void LocalDbInstanceApi_GetInstanceNames_Handles_Arbitrary_Counts()
    {
        // Arrange
        int count = 0;

        using var target = CreateLocalDbApi();

        // Act
        int result = target.GetInstanceNames(IntPtr.Zero, ref count);

        // Assert
        result.ShouldNotBe(0);
    }

    [Property]
    public static void LocalDbInstanceApi_GetVersions_Handles_Arbitrary_Counts()
    {
        // Arrange
        int count = 0;

        using var target = CreateLocalDbApi();

        // Act
        int result = target.GetVersions(IntPtr.Zero, ref count);

        // Assert
        result.ShouldNotBe(0);
    }

    private static LocalDbInstanceApi CreateLocalDbApi(string? apiVersion = default)
    {
        IRegistry registry =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            new WindowsRegistry() :
            Substitute.For<IRegistry>();

        var logger = NullLogger<LocalDbInstanceApi>.Instance;

        return new(apiVersion ?? string.Empty, registry, logger);
    }
}
