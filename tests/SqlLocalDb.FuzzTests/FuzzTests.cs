// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using FsCheck;
using FsCheck.Xunit;
using MartinCostello.SqlLocalDb.Interop;

namespace MartinCostello.SqlLocalDb;

[Collection(FuzzCollection.Name)]
public class FuzzTests(LocalDbFixture fixture, ITestOutputHelper outputHelper) : IAsyncLifetime
{
    private readonly ConcurrentBag<string> _instanceNames = [];

    [Property]
    public void MarshalString_Handles_Arbitrary_Byte_Arrays(byte[] bytes)
    {
        // Arrange
        if (bytes is null || bytes.Length > 10_000)
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
        if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
        {
            return;
        }

        outputHelper.WriteLine("Creating instance with name: {0}", instanceNameValue);

        // Act and Assert
        Should.NotThrow(() => fixture.Target.CreateInstance(version.Get, instanceNameValue, 0));
    }

    [Property]
    public void LocalDbInstanceApi_DeleteInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
        {
            return;
        }

        outputHelper.WriteLine("Deleting instance with name: {0}", instanceNameValue);

        // Act and Assert
        Should.NotThrow(() => fixture.Target.DeleteInstance(instanceNameValue, 0));
    }

    [Property]
    public void LocalDbInstanceApi_GetInstanceInfo_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
        {
            return;
        }

        outputHelper.WriteLine("Getting instance with name: {0}", instanceNameValue);

        // Act and Assert
        Should.NotThrow(() => fixture.Target.GetInstanceInfo(instanceNameValue, IntPtr.Zero, 0));
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
        if (!SanitizeInstanceName(privateName, out string privateNameValue))
        {
            return;
        }

        outputHelper.WriteLine("Sharing instance with name: {0}", privateNameValue);

        // Act and Assert
        Should.NotThrow(() => fixture.Target.ShareInstance(IntPtr.Zero, privateNameValue, sharedName.Get, 0));
    }

    [Property]
    public void LocalDbInstanceApi_StartInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
        {
            return;
        }

        outputHelper.WriteLine("Starting instance with name: {0}", instanceNameValue);

        // Arrange
        var buffer = new StringBuilder(261);
        int size = buffer.Capacity;

        // Act and Assert
        Should.NotThrow(() => fixture.Target.StartInstance(instanceNameValue, 0, buffer, ref size));
    }

    [Property(MaxTest = 100)] // Otherwise it seems to crash
    public void LocalDbInstanceApi_StopInstance_Handles_Arbitrary_Strings(
        NonNull<string> instanceName,
        NonNegativeInt timeout)
    {
        if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
        {
            return;
        }

        outputHelper.WriteLine("Stopping instance with name: {0}", instanceNameValue);

        // Act and Assert
        Should.NotThrow(() => fixture.Target.StopInstance(instanceNameValue, StopInstanceOptions.NoWait, timeout.Get));
    }

    [Property]
    public void LocalDbInstanceApi_UnshareInstance_Handles_Arbitrary_Strings(NonNull<string> instanceName)
    {
        if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
        {
            return;
        }

        outputHelper.WriteLine("Unsharing instance with name: {0}", instanceNameValue);

        // Act and Assert
        Should.NotThrow(() => fixture.Target.UnshareInstance(instanceNameValue, 0));
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
    public void LocalDbInstanceApi_GetInstanceNames_Handles_Arbitrary_Counts(NonNegativeInt value)
    {
        // Arrange
        int count = value.Get;

        // Act and Assert
        Should.NotThrow(() => fixture.Target.GetInstanceNames(IntPtr.Zero, ref count));
    }

    [Property]
    public void LocalDbInstanceApi_GetVersions_Handles_Arbitrary_Counts(NonNegativeInt value)
    {
        // Arrange
        int count = value.Get;

        // Act and Assert
        Should.NotThrow(() => fixture.Target.GetVersions(IntPtr.Zero, ref count));
    }

    public ValueTask InitializeAsync()
    {
#if NETFRAMEWORK
        return default;
#else
        return ValueTask.CompletedTask;
#endif
    }

    public ValueTask DisposeAsync()
    {
        foreach (var name in _instanceNames)
        {
            try
            {
                _ = fixture.Target.DeleteInstance(name, 0);
            }
            catch (Exception)
            {
                // Ignore
            }
        }

        GC.SuppressFinalize(this);

#if NETFRAMEWORK
        return default;
#else
        return ValueTask.CompletedTask;
#endif
    }

    private bool SanitizeInstanceName(NonNull<string> instanceName, out string value)
    {
        value = instanceName.Get;

        // See https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/sql-server-express-localdb-reference-instance-apis#named-instance-naming-rules
        HashSet<char> invalid =
        [
            .. Path.GetInvalidFileNameChars(),
            .. Path.GetInvalidPathChars(),
            '\'',
            '$',
            '%',
            '&',
            '[',
            ']',
            '.',
            ' ',
            '_',
        ];

        bool isValid = value.Any(invalid.Contains);

        if (isValid)
        {
            _instanceNames.Add(value);
        }

        return isValid;
    }
}
