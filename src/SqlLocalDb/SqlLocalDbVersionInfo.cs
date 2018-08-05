// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class representing information about a version of SQL Server LocalDB. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    internal sealed class SqlLocalDbVersionInfo : ISqlLocalDbVersionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbVersionInfo"/> class.
        /// </summary>
        internal SqlLocalDbVersionInfo()
        {
        }

        /// <inheritdoc />
        public bool Exists { get; internal set; }

        /// <inheritdoc />
        public string Name { get; internal set; }

        /// <inheritdoc />
        public Version Version { get; internal set; }

        /// <inheritdoc />
        public override string ToString() => Name;

        /// <summary>
        /// Updates the state of the instance from the specified value.
        /// </summary>
        /// <param name="other">The other value to use to update the instance's state.</param>
        internal void Update(ISqlLocalDbVersionInfo other)
        {
            if (other == null || ReferenceEquals(other, this))
            {
                return;
            }

            Exists = other.Exists;
            Name = other.Name;
            Version = other.Version;
        }
    }
}
