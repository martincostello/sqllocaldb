// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace MartinCostello.SqlLocalDb.Interop
{
    /// <summary>
    /// A class representing an implementation of <see cref="IRegistryKey"/> for a Windows registry key. This class cannot be inherited.
    /// </summary>
    internal sealed class WindowsRegistryKey : IRegistryKey
    {
        /// <summary>
        /// The <see cref="RegistryKey"/> wrapped by the instance. This field is read-only.
        /// </summary>
        private readonly RegistryKey _key;

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowsRegistryKey"/> class.
        /// </summary>
        /// <param name="key">The <see cref="RegistryKey"/> to wrap.</param>
        internal WindowsRegistryKey(RegistryKey key)
        {
            Debug.Assert(key != null, "key cannot be null.");
            _key = key;
        }

        /// <inheritdoc />
        void IDisposable.Dispose() => _key.Dispose();

        /// <inheritdoc />
        public string[] GetSubKeyNames() => _key.GetSubKeyNames();

        /// <inheritdoc />
        public string GetValue(string name) => _key.GetValue(name, null, RegistryValueOptions.None) as string;

        /// <inheritdoc />
        public IRegistryKey OpenSubKey(string keyName)
        {
            RegistryKey key = _key.OpenSubKey(keyName);
            return key == null ? null : new WindowsRegistryKey(key);
        }
    }
}
