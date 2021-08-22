// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.SqlLocalDb.Interop;

/// <summary>
/// Defines a method for opening a registry sub-key.
/// </summary>
internal interface IRegistry
{
    /// <summary>
    /// Retrieves a sub-key as read-only.
    /// </summary>
    /// <param name="keyName">The name or path of the sub-key to open as read-only.</param>
    /// <returns>
    /// The sub-key requested, or <see langword="null"/> if the operation failed.
    /// </returns>
    IRegistryKey? OpenSubKey(string keyName);
}
