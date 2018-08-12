// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;

namespace MartinCostello.SqlLocalDb.Interop
{
    /// <summary>
    /// Defines a registry sub-key.
    /// </summary>
    internal interface IRegistryKey : IRegistry, IDisposable
    {
        /// <summary>
        /// Retrieves an array of strings that contains all the sub-key names.
        /// </summary>
        /// <returns>
        /// An array of strings that contains the names of the sub-keys for the current key.
        /// </returns>
        string[] GetSubKeyNames();

        /// <summary>
        /// Retrieves the value associated with the specified name.
        /// </summary>
        /// <param name="name">The name of the value to retrieve. This string is not case-sensitive.</param>
        /// <returns>
        /// The value associated with <paramref name="name"/>, or <see langword="null"/> if <paramref name="name"/> is not found.
        /// </returns>
        string GetValue(string name);
    }
}
