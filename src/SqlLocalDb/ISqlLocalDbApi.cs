// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// Defines the interface to the SQL LocalDB Instance API.
    /// </summary>
    public interface ISqlLocalDbApi
    {
        /// <summary>
        /// Gets the name of the default SQL LocalDB instance.
        /// </summary>
        string DefaultInstanceName { get; }

        /// <summary>
        /// Gets the version string for the latest installed version of SQL Server LocalDB.
        /// </summary>
        string LatestVersion { get; }

        /// <summary>
        /// Gets an <see cref="IReadOnlyList{T}"/> of <see cref="string"/> containing the available version(s) of SQL LocalDB.
        /// </summary>
        IReadOnlyList<string> Versions { get; }

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance.</param>
        /// <param name="version">The version of SQL Server LocalDB to use.</param>
        /// <returns>
        /// An <see cref="ISqlLocalDbInstanceInfo"/> containing information about the instance that was created.
        /// </returns>
        ISqlLocalDbInstanceInfo CreateInstance(string instanceName, string version);

        /// <summary>
        /// Deletes the specified SQL Server LocalDB instance.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to delete.
        /// </param>
        void DeleteInstance(string instanceName);

        /// <summary>
        /// Returns information about the specified LocalDB instance.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance to get the information for.</param>
        /// <returns>
        /// An instance of <see cref="ISqlLocalDbInstanceInfo"/> containing information
        /// about the LocalDB instance specified by <paramref name="instanceName"/>.
        /// </returns>
        ISqlLocalDbInstanceInfo GetInstanceInfo(string instanceName);

        /// <summary>
        /// Returns the names of all the SQL Server LocalDB instances for the current user.
        /// </summary>
        /// <returns>
        /// The names of the the SQL Server LocalDB instances for the current user.
        /// </returns>
        IReadOnlyList<string> GetInstanceNames();

        /// <summary>
        /// Returns information about the specified LocalDB version.
        /// </summary>
        /// <param name="version">The name of the LocalDB version to get the information for.</param>
        /// <returns>
        /// An instance of <see cref="ISqlLocalDbVersionInfo"/> containing information
        /// about the LocalDB version specified by <paramref name="version"/>.
        /// </returns>
        ISqlLocalDbVersionInfo GetVersionInfo(string version);

        /// <summary>
        /// Returns whether SQL LocalDB is installed on the current machine.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if SQL Server LocalDB is installed on the
        /// current machine; otherwise <see langword="false"/>.
        /// </returns>
        bool IsLocalDBInstalled();

        /// <summary>
        /// Shares the specified SQL Server LocalDB instance with other
        /// users of the computer, using the specified shared name.
        /// </summary>
        /// <param name="ownerSid">The SID of the instance owner.</param>
        /// <param name="instanceName">The private name for the LocalDB instance to share.</param>
        /// <param name="sharedInstanceName">The shared name for the LocalDB instance to share.</param>
        void ShareInstance(string ownerSid, string instanceName, string sharedInstanceName);

        /// <summary>
        /// Starts the specified instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance to start.</param>
        /// <returns>
        /// The named pipe to use to connect to the LocalDB instance.
        /// </returns>
        string StartInstance(string instanceName);

        /// <summary>
        /// Enables tracing of native API calls for all the SQL Server
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        void StartTracing();

        /// <summary>
        /// Stops the specified instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to stop.
        /// </param>
        /// <param name="timeout">
        /// The optional amount of time to give the LocalDB instance to stop.
        /// </param>
        void StopInstance(string instanceName, TimeSpan? timeout);

        /// <summary>
        /// Disables tracing of native API calls for all the SQL Server
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        void StopTracing();

        /// <summary>
        /// Stops the sharing of the specified SQL Server LocalDB instance.
        /// </summary>
        /// <param name="instanceName">
        /// The private name for the LocalDB instance to stop sharing.
        /// </param>
        void UnshareInstance(string instanceName);
    }
}
