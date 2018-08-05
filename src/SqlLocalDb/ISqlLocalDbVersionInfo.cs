// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// Defines information about a version of SQL Server LocalDB.
    /// </summary>
    public interface ISqlLocalDbVersionInfo
    {
        /// <summary>
        /// Gets a value indicating whether the instance files exist on disk.
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Gets the version name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the version.
        /// </summary>
        Version Version { get; }
    }
}
