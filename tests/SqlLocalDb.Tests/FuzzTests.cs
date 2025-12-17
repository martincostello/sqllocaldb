// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using FsCheck;
using FsCheck.Xunit;
using MartinCostello.SqlLocalDb.Interop;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace MartinCostello.SqlLocalDb;

public class FuzzTests : IAsyncLifetime
{
    private LocalDbInstanceApi? _target;

    public ValueTask InitializeAsync()
    {
        _target = CreateLocalDbApi();

#if NETFRAMEWORK
        return default;
#else
        return ValueTask.CompletedTask;
#endif
    }

    public ValueTask DisposeAsync()
    {
        _target?.Dispose();
        GC.SuppressFinalize(this);

#if NETFRAMEWORK
        return default;
#else
        return ValueTask.CompletedTask;
#endif
    }

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
        using var target = CreateLocalDbApi(apiVersion);

        // Assert
        target.ShouldNotBeNull();
    }

    [Property]
    public void LocalDbInstanceApi_CreateInstance_Handles_Arbitrary_Strings(
        NonNull<string> version,
        NonNull<string> instanceName)
    {
        // Act and Assert
        Should.NotThrow(() => _target!.CreateInstance(version.Get, instanceName.Get, 0));
    }

    [Property]
    public void LocalDbInstanceApi_DeleteInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Act and Assert
        Should.NotThrow(() => _target!.DeleteInstance(instanceName.Get, 0));
    }

    [Property]
    public void LocalDbInstanceApi_GetInstanceInfo_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Act and Assert
        Should.NotThrow(() => _target!.GetInstanceInfo(instanceName.Get, IntPtr.Zero, 0));
    }

    [Property]
    public void LocalDbInstanceApi_GetVersionInfo_Handles_Arbitrary_Strings(NonNull<string> versionName)
    {
        // Act and Assert
        Should.NotThrow(() => _target!.GetVersionInfo(versionName.Get, IntPtr.Zero, 0));
    }

    [Property]
    public void LocalDbInstanceApi_ShareInstance_Handles_Arbitrary_Strings(
        NonNull<string> privateName,
        NonNull<string> sharedName)
    {
        // Act and Assert
        Should.NotThrow(() => _target!.ShareInstance(IntPtr.Zero, privateName.Get, sharedName.Get, 0));
    }

    [Property]
    public void LocalDbInstanceApi_StartInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Arrange
        var buffer = new StringBuilder(261);
        int size = buffer.Capacity;

        // Act and Assert
        Should.NotThrow(() => _target!.StartInstance(instanceName.Get, 0, buffer, ref size));
    }

    [Property]
    public void LocalDbInstanceApi_StopInstance_Handles_Arbitrary_Strings(
        NonNull<string> instanceName,
        NonNegativeInt timeout)
    {
        // Act and Assert
        Should.NotThrow(() => _target!.StopInstance(instanceName.Get, StopInstanceOptions.None, timeout.Get));
    }

    [Property]
    public void LocalDbInstanceApi_UnshareInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        // Act and Assert
        Should.NotThrow(() => _target!.UnshareInstance(instanceName.Get, 0));
    }

    [Property]
    public void LocalDbInstanceApi_GetLocalDbError_Handles_Arbitrary_Error_Codes(int errorCode, int languageId)
    {
        // Arrange
        var buffer = new StringBuilder(261);
        int size = buffer.Capacity;

        // Act and Assert
        Should.NotThrow(() => _target!.GetLocalDbError(errorCode, languageId, buffer, ref size));
    }

    [Property]
    public void LocalDbInstanceApi_Disposal_Is_Idempotent(PositiveInt callCount)
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
    public void LocalDbInstanceApi_TryGetLocalDbApiPath_Handles_Arbitrary_Version_Strings(string apiVersion)
    {
        // Arrange
        using var target = CreateLocalDbApi(apiVersion);

        // Act
        Should.NotThrow(() => target.TryGetLocalDbApiPath(out _));
    }

    [Property]
    public void LocalDbInstanceApi_GetInstanceNames_Handles_Arbitrary_Counts()
    {
        // Arrange
        int count = 0;

        // Act and Assert
        Should.NotThrow(() => _target!.GetInstanceNames(IntPtr.Zero, ref count));
    }

    [Property]
    public void LocalDbInstanceApi_GetVersions_Handles_Arbitrary_Counts()
    {
        // Arrange
        int count = 0;

        // Act and Assert
        Should.NotThrow(() => _target!.GetVersions(IntPtr.Zero, ref count));
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
