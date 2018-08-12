// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Win32;

namespace MartinCostello.SqlLocalDb.Interop
{
    /// <summary>
    /// A class representing an implementation of <see cref="IRegistry"/> for the Windows registry. This class cannot be inherited.
    /// </summary>
    internal sealed class WindowsRegistry : IRegistry
    {
        /// <inheritdoc />
        public IRegistryKey OpenSubKey(string keyName)
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, writable: false);
            return key == null ? null : new WindowsRegistryKey(key);
        }
    }
}
