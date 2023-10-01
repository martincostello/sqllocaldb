// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Win32;

namespace MartinCostello.SqlLocalDb.Interop;

/// <summary>
/// A class representing an implementation of <see cref="IRegistryKey"/> for a Windows registry key. This class cannot be inherited.
/// </summary>
internal sealed class WindowsRegistryKey(RegistryKey key) : IRegistryKey
{
    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    void IDisposable.Dispose() => key.Dispose();

    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public string[] GetSubKeyNames() => key.GetSubKeyNames();

    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public string? GetValue(string name) => key.GetValue(name, null, RegistryValueOptions.None) as string;

    /// <inheritdoc />
#if NET5_0_OR_GREATER
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public IRegistryKey? OpenSubKey(string keyName)
    {
        RegistryKey? subkey = key.OpenSubKey(keyName);
        return subkey is null ? null : new WindowsRegistryKey(subkey);
    }
}
