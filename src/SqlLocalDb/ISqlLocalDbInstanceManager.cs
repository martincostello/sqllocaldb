// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// Defines an interface for managing instances of SQL LocalDB.
/// </summary>
public interface ISqlLocalDbInstanceManager
{
    /// <summary>
    /// Gets the name of the LocalDB instance.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the named pipe that should be used to connect to the LocalDB instance.
    /// </summary>
    string NamedPipe { get; }

    /// <summary>
    /// Gets the current state of the instance.
    /// </summary>
    /// <returns>
    /// An <see cref="ISqlLocalDbInstanceInfo"/> representing the current state of the instance being managed.
    /// </returns>
    ISqlLocalDbInstanceInfo GetInstanceInfo();

    /// <summary>
    /// Shares the LocalDB instance using the specified name.
    /// </summary>
    /// <param name="sharedName">The name to use to share the instance.</param>
    void Share(string sharedName);

    /// <summary>
    /// Starts the LocalDB instance.
    /// </summary>
    void Start();

#pragma warning disable CA1716 // Identifiers should not match keywords
    /// <summary>
    /// Stops the LocalDB instance.
    /// </summary>
    void Stop();
#pragma warning restore CA1716 // Identifiers should not match keywords

    /// <summary>
    /// Stops sharing the LocalDB instance.
    /// </summary>
    void Unshare();
}
