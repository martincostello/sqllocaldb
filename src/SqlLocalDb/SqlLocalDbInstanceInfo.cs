// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class representing information about an instance of SQL LocalDB. This class cannot be inherited.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    internal sealed class SqlLocalDbInstanceInfo : ISqlLocalDbInstanceInfo, ISqlLocalDbApiAdapter
    {
        //// Internally created type used to provide "pit-of-success" semantics on
        //// instances of ISqlLocalDbInstanceInfo so that the state can be updated
        //// during mutations of the API without returning marshalled types directly.

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbInstanceInfo"/> class.
        /// </summary>
        /// <param name="api">The <see cref="ISqlLocalDbApi"/> associated with the instance.</param>
        internal SqlLocalDbInstanceInfo(ISqlLocalDbApi api)
        {
            Api = api;
        }

        /// <inheritdoc />
        public bool ConfigurationCorrupt { get; internal set; }

        /// <inheritdoc />
        public bool Exists { get; internal set; }

        /// <inheritdoc />
        public bool IsAutomatic { get; internal set; }

        /// <inheritdoc />
        public bool IsRunning { get; internal set; }

        /// <inheritdoc />
        public bool IsShared { get; internal set; }

        /// <inheritdoc />
        public DateTime LastStartTimeUtc { get; internal set; }

        /// <inheritdoc />
        public Version LocalDbVersion { get; internal set; }

        /// <inheritdoc />
        public string Name { get; internal set; }

        /// <inheritdoc />
        public string NamedPipe { get; internal set; }

        /// <inheritdoc />
        public string OwnerSid { get; internal set; }

        /// <inheritdoc />
        public string SharedName { get; internal set; }

        /// <inheritdoc />
        ISqlLocalDbApi ISqlLocalDbApiAdapter.LocalDb => Api;

        /// <summary>
        /// Gets the <see cref="ISqlLocalDbApi"/> associated with this instance.
        /// </summary>
        private ISqlLocalDbApi Api { get; }

        /// <inheritdoc />
        public override string ToString() => Name;

        /// <summary>
        /// Updates the state of the instance from the specified value.
        /// </summary>
        /// <param name="other">The other value to use to update the instance's state.</param>
        internal void Update(ISqlLocalDbInstanceInfo other)
        {
            if (other == null || ReferenceEquals(other, this))
            {
                return;
            }

            ConfigurationCorrupt = other.ConfigurationCorrupt;
            Exists = other.Exists;
            IsAutomatic = other.IsAutomatic;
            IsRunning = other.IsRunning;
            IsShared = other.IsShared;
            LastStartTimeUtc = other.LastStartTimeUtc;
            LocalDbVersion = other.LocalDbVersion;
            Name = other.Name;
            NamedPipe = other.NamedPipe;
            OwnerSid = other.OwnerSid;
            SharedName = other.SharedName;
        }
    }
}
