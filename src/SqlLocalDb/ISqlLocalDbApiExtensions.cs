// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class containing extension methods for the <see cref="ISqlLocalDbApi"/> interface.  This class cannot be inherited.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ISqlLocalDbApiExtensions
    {
        /// <summary>
        /// Creates a new instance of <see cref="TemporarySqlLocalDbInstance"/> with a randomly assigned name.
        /// </summary>
        /// <param name="api">The <see cref="ISqlLocalDbApi"/> to use to create the temporary instance.</param>
        /// <param name="deleteFiles">An optional value indicating whether to delete the file(s) associated with the SQL LocalDB instance when it is deleted.</param>
        /// <returns>
        /// The created instance of <see cref="TemporarySqlLocalDbInstance"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="api"/> is <see langword="null"/>.
        /// </exception>
        public static TemporarySqlLocalDbInstance CreateTemporaryInstance(this ISqlLocalDbApi api, bool deleteFiles = false)
            => new TemporarySqlLocalDbInstance(api, deleteFiles);

        /// <summary>
        /// Gets the default SQL Local DB instance.
        /// </summary>
        /// <param name="api">The <see cref="ISqlLocalDbApi"/> to use to get the default instance.</param>
        /// <returns>
        /// The default SQL Local DB instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="api"/> is <see langword="null"/>.
        /// </exception>
        public static ISqlLocalDbInstanceInfo GetDefaultInstance(this ISqlLocalDbApi api)
        {
            if (api == null)
            {
                throw new ArgumentNullException(nameof(api));
            }

            return api.GetOrCreateInstance(api.DefaultInstanceName);
        }

        /// <summary>
        /// Returns information about the available SQL Server LocalDB instances.
        /// </summary>
        /// <param name="api">The <see cref="ISqlLocalDbApi"/> to use to enumerate the instances.</param>
        /// <returns>
        /// An <see cref="IReadOnlyList{ISqlLocalDbInstanceInfo}"/> containing information
        /// about the available SQL Server LocalDB instances on the current machine.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="api"/> is <see langword="null"/>.
        /// </exception>
        public static IReadOnlyList<ISqlLocalDbInstanceInfo> GetInstances(this ISqlLocalDbApi api)
        {
            if (api == null)
            {
                throw new ArgumentNullException(nameof(api));
            }

            IReadOnlyList<string> instanceNames = api.GetInstanceNames();

            var instances = new List<ISqlLocalDbInstanceInfo>(instanceNames?.Count ?? 0);

            if (instanceNames != null)
            {
                foreach (string name in instanceNames)
                {
                    ISqlLocalDbInstanceInfo info = api.GetInstanceInfo(name);
                    instances.Add(info);
                }
            }

            return instances;
        }

        /// <summary>
        /// Returns information about the installed SQL Server LocalDB version(s).
        /// </summary>
        /// <param name="api">The <see cref="ISqlLocalDbApi"/> to use to enumerate the installed versions.</param>
        /// <returns>
        /// An <see cref="IReadOnlyList{ISqlLocalDbVersionInfo}"/> containing information
        /// about the SQL Server LocalDB version(s) installed on the current machine.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="api"/> is <see langword="null"/>.
        /// </exception>
        public static IReadOnlyList<ISqlLocalDbVersionInfo> GetVersions(this ISqlLocalDbApi api)
        {
            if (api == null)
            {
                throw new ArgumentNullException(nameof(api));
            }

            IReadOnlyList<string> versionNames = api.Versions;
            var versions = new List<ISqlLocalDbVersionInfo>(versionNames?.Count ?? 0);

            if (versionNames != null)
            {
                foreach (string version in versionNames)
                {
                    ISqlLocalDbVersionInfo info = api.GetVersionInfo(version);
                    versions.Add(info);
                }
            }

            return versions;
        }

        /// <summary>
        /// Gets a SQL Local DB instance with the specified name if it exists, otherwise a new instance with the specified name is created.
        /// </summary>
        /// <param name="api">The <see cref="ISqlLocalDbApi"/> to use to get or create the instance.</param>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance to get or create.</param>
        /// <returns>
        /// A SQL Local DB instance with the name specified by <paramref name="instanceName"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="api"/> or <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        public static ISqlLocalDbInstanceInfo GetOrCreateInstance(this ISqlLocalDbApi api, string instanceName)
        {
            if (api == null)
            {
                throw new ArgumentNullException(nameof(api));
            }

            if (instanceName == null)
            {
                throw new ArgumentNullException(nameof(instanceName));
            }

            // Instance names in SQL Local DB are case-insensitive
            if (string.Equals(api.DefaultInstanceName, instanceName, StringComparison.OrdinalIgnoreCase) ||
                SqlLocalDbApi.IsDefaultInstanceName(instanceName))
            {
                // The default instance is always listed, even if it does not exist,
                // so need to query that separately to verify whether to get or create.
                ISqlLocalDbInstanceInfo info = api.GetInstanceInfo(instanceName);

                if (info != null)
                {
                    // If it exists (or it's a default instance), we can't create
                    // it so just return the information about it immediately.
                    if (info.Exists || info.IsAutomatic)
                    {
                        return info;
                    }
                }
            }

            if (api.InstanceExists(instanceName))
            {
                return api.GetInstanceInfo(instanceName);
            }
            else
            {
                return api.CreateInstance(instanceName, api.LatestVersion);
            }
        }

        /// <summary>
        /// Shares the specified SQL Server LocalDB instance with other users of the computer,
        /// using the specified shared name for the current Windows user.
        /// </summary>
        /// <param name="api">The <see cref="ISqlLocalDbApi"/> to use to share the instance.</param>
        /// <param name="instanceName">The private name for the LocalDB instance to share.</param>
        /// <param name="sharedInstanceName">The shared name for the LocalDB instance to share.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="api"/>, <paramref name="instanceName"/> or <paramref name="sharedInstanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        public static void ShareInstance(this ISqlLocalDbApi api, string instanceName, string sharedInstanceName)
        {
            if (api == null)
            {
                throw new ArgumentNullException(nameof(api));
            }

            SqlLocalDbApi.EnsurePlatformSupported();

            string ownerSid;

#pragma warning disable CA1416
            using (var identity = WindowsIdentity.GetCurrent())
            {
                ownerSid = identity.User!.Value;
            }
#pragma warning restore CA1416

            api.ShareInstance(ownerSid, instanceName, sharedInstanceName);
        }
    }
}
