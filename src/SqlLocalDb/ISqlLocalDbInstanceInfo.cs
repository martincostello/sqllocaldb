// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// Defines information about a SQL Server LocalDB instance.
/// </summary>
public interface ISqlLocalDbInstanceInfo
{
    /// <summary>
    /// Gets a value indicating whether the Registry configuration is corrupt.
    /// </summary>
    bool ConfigurationCorrupt { get; }

    /// <summary>
    /// Gets a value indicating whether the instance's files exist on disk.
    /// </summary>
    bool Exists { get; }

    /// <summary>
    /// Gets a value indicating whether the instance is automatic.
    /// </summary>
    bool IsAutomatic { get; }

    /// <summary>
    /// Gets a value indicating whether the instance is currently running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    /// Gets a value indicating whether the instance is shared.
    /// </summary>
    bool IsShared { get; }

    /// <summary>
    /// Gets the UTC date and time the instance was last started.
    /// </summary>
    DateTime LastStartTimeUtc { get; }

    /// <summary>
    /// Gets the LocalDB version for the instance.
    /// </summary>
    Version LocalDbVersion { get; }

    /// <summary>
    /// Gets the name of the instance.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the named pipe that should be used to communicate with the instance.
    /// </summary>
    string NamedPipe { get; }

    /// <summary>
    /// Gets the SID of the LocalDB instance owner if the instance is shared.
    /// </summary>
    string OwnerSid { get; }

    /// <summary>
    /// Gets the shared name of the LocalDB instance if the instance is shared.
    /// </summary>
    string SharedName { get; }
}
