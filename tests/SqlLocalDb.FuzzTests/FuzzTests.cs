// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using FsCheck;
using FsCheck.Fluent;
using MartinCostello.SqlLocalDb.Interop;

namespace MartinCostello.SqlLocalDb;

[Collection(FuzzCollection.Name)]
public class FuzzTests(LocalDbFixture fixture) : IAsyncLifetime
{
    // See https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/sql-server-express-localdb-reference-instance-apis#named-instance-naming-rules
    private static readonly HashSet<char> InvalidNameChars =
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

    private readonly ConcurrentBag<string> _instanceNames = [];

    [Fact]
    public void MarshalString_Handles_Arbitrary_Byte_Arrays()
    {
        PropertyCheck.Run<byte[]>((bytes) =>
        {
            // Arrange
            if (bytes is null || bytes.Length > 10_000)
            {
                return true;
            }

            // Act
            string result = LocalDbInstanceApi.MarshalString(bytes);

            // Assert
            result.ShouldNotBeNull();
            return true;
        });
    }

    [Fact]
    public void MarshalString_Handles_Unicode_Strings()
    {
        PropertyCheck.Run<string>((input) =>
        {
            // Arrange
            if (input == null)
            {
                return true;
            }

            byte[] bytes = Encoding.Unicode.GetBytes(input);

            // Act
            string result = LocalDbInstanceApi.MarshalString(bytes);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldNotEndWith("\0");
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_Constructor_Handles_Arbitrary_Version_Strings()
    {
        PropertyCheck.Run<string>((apiVersion) =>
        {
            // Act
            using var target = LocalDbFixture.CreateLocalDbApi(apiVersion);

            // Assert
            target.ShouldNotBeNull();
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_CreateInstance_Handles_Arbitrary_Strings()
    {
        PropertyCheck.Run<NonNull<string>, NonNull<string>>((version, instanceName) =>
        {
            if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
            {
                return true;
            }

            // Act and Assert
            Should.NotThrow(() => fixture.Target.CreateInstance(version.Get, instanceNameValue, 0));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_DeleteInstance_Handles_Arbitrary_Strings()
    {
        PropertyCheck.Run<NonEmptyString>((instanceName) =>
        {
            if (string.IsNullOrWhiteSpace(instanceName.Get))
            {
                // An empty name causes the SQL LocalDB Instance API to internally use the
                // default "MSSQLLocalDB" instance, which we do not want to delete.
                return true;
            }

            if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
            {
                return true;
            }

            // Act and Assert
            Should.NotThrow(() => fixture.Target.DeleteInstance(instanceNameValue, 0));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_GetInstanceInfo_Handles_Arbitrary_Strings()
    {
        PropertyCheck.Run<NonNull<string>>((instanceName) =>
        {
            if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
            {
                return true;
            }

            // Act and Assert
            Should.NotThrow(() => fixture.Target.GetInstanceInfo(instanceNameValue, IntPtr.Zero, 0));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_GetVersionInfo_Handles_Arbitrary_Strings()
    {
        PropertyCheck.Run<NonNull<string>>((versionName) =>
        {
            // Act and Assert
            Should.NotThrow(() => fixture.Target.GetVersionInfo(versionName.Get, IntPtr.Zero, 0));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_ShareInstance_Handles_Arbitrary_Strings()
    {
        PropertyCheck.Run<NonNull<string>, NonNull<string>>((privateName, sharedName) =>
        {
            if (!SanitizeInstanceName(privateName, out string privateNameValue))
            {
                return true;
            }

            if (!SanitizeInstanceName(sharedName, out string sharedNameValue))
            {
                return true;
            }

            // Act and Assert
            Should.NotThrow(() => fixture.Target.ShareInstance(IntPtr.Zero, privateNameValue, sharedNameValue, 0));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_StartInstance_Handles_Arbitrary_Strings()
    {
        PropertyCheck.Run<NonNull<string>>((instanceName) =>
        {
            if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
            {
                return true;
            }

            // Arrange
            var buffer = new StringBuilder(261);
            int size = buffer.Capacity;

            // Act and Assert
            Should.NotThrow(() => fixture.Target.StartInstance(instanceNameValue, 0, buffer, ref size));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_StopInstance_Handles_Arbitrary_Strings()
    {
        PropertyCheck.Run<NonEmptyString, NonNegativeInt, NonNegativeInt>((instanceName, options, timeout) =>
        {
            if (string.IsNullOrWhiteSpace(instanceName.Get))
            {
                // An empty name causes the SQL LocalDB Instance API to internally use the
                // default "MSSQLLocalDB" instance, which then may cause the test process
                // to crash if the right (unknown) sequence of events occurs. Stack trace:
                //
                // ucrtbase.dll!_invoke_watson()
                // ucrtbase.dll!_invalid_parameter()
                // ucrtbase.dll!_invalid_parameter_noinfo()
                // ucrtbase.dll!_ultow_s()
                // SqlUserInstance.dll!LocalDBLogWinError(unsigned long,wchar_t const *,unsigned short,unsigned long,wchar_t const *)
                // SqlUserInstance.dll!CSqlUserInstance::ShutdownUserInstance(wchar_t const *,unsigned long,int)
                // SqlUserInstance.dll!LocalDBStopInstance()
                // MartinCostello.SqlLocalDb.dll!MartinCostello.SqlLocalDb.Interop.LocalDbInstanceApi.StopInstance(...)
                return true;
            }

            if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
            {
                return true;
            }

            // Act and Assert
            Should.NotThrow(() => fixture.Target.StopInstance(instanceNameValue, (StopInstanceOptions)options.Get, timeout.Get));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_UnshareInstance_Handles_Arbitrary_Strings()
    {
        PropertyCheck.Run<NonNull<string>>((instanceName) =>
        {
            if (!SanitizeInstanceName(instanceName, out string instanceNameValue))
            {
                return true;
            }

            // Act and Assert
            Should.NotThrow(() => fixture.Target.UnshareInstance(instanceNameValue, 0));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_GetLocalDbError_Handles_Arbitrary_Error_Codes()
    {
        PropertyCheck.Run<int, int>((errorCode, languageId) =>
        {
            // Arrange
            var buffer = new StringBuilder(261);
            int size = buffer.Capacity;

            // Act and Assert
            Should.NotThrow(() => fixture.Target.GetLocalDbError(errorCode, languageId, buffer, ref size));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_Disposal_Is_Idempotent()
    {
        PropertyCheck.Run<PositiveInt>((callCount) =>
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
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_TryGetLocalDbApiPath_Handles_Arbitrary_Version_Strings()
    {
        PropertyCheck.Run<string>((apiVersion) =>
        {
            // Arrange
            using var target = LocalDbFixture.CreateLocalDbApi(apiVersion);

            // Act
            Should.NotThrow(() => target.TryGetLocalDbApiPath(out _));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_GetInstanceNames_Handles_Arbitrary_Counts()
    {
        PropertyCheck.Run<NonNegativeInt>((value) =>
        {
            // Arrange
            int count = value.Get;

            // Act and Assert
            Should.NotThrow(() => fixture.Target.GetInstanceNames(IntPtr.Zero, ref count));
            return true;
        });
    }

    [Fact]
    public void LocalDbInstanceApi_GetVersions_Handles_Arbitrary_Counts()
    {
        PropertyCheck.Run<NonNegativeInt>((value) =>
        {
            // Arrange
            int count = value.Get;

            // Act and Assert
            Should.NotThrow(() => fixture.Target.GetVersions(IntPtr.Zero, ref count));
            return true;
        });
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

    private bool SanitizeInstanceName(NonEmptyString instanceName, out string value)
        => SanitizeInstanceName(instanceName.Get, out value);

    private bool SanitizeInstanceName(NonNull<string> instanceName, out string value)
        => SanitizeInstanceName(instanceName.Get, out value);

    private bool SanitizeInstanceName(string instanceName, out string value)
    {
        value = string.Empty;

        bool isValid =
            !instanceName.Any(InvalidNameChars.Contains) &&
            !string.Equals(instanceName, "v11.0", StringComparison.Ordinal) &&
            !string.Equals(instanceName, "MSSQLLocalDB", StringComparison.Ordinal);

        if (isValid)
        {
            _instanceNames.Add(instanceName);
            value = instanceName;
        }

        return isValid;
    }

    private static class PropertyCheck
    {
        public static void Run<T>(Func<T, bool> property)
            => Check.QuickThrowOnFailure(Prop.ForAll(property));

        public static void Run<T1, T2>(Func<T1, T2, bool> property)
            => Check.QuickThrowOnFailure(Prop.ForAll(property));

        public static void Run<T1, T2, T3>(Func<T1, T2, T3, bool> property)
            => Check.QuickThrowOnFailure(Prop.ForAll(property));
    }
}
