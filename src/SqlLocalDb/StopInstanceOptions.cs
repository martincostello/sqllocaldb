// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// An enumeration of options to control the behavior when stopping a SQL LocalDB instance.
/// </summary>
[Flags]
public enum StopInstanceOptions
{
    /// <summary>
    /// Shut down using the <c>SHUTDOWN</c> Transact-SQL command.
    /// </summary>
    None = 0,

    /// <summary>
    /// Shut down immediately using the kill process operating system command.
    /// </summary>
    KillProcess = 1,

    /// <summary>
    /// Shut down using the <c>WITH NOWAIT</c> option Transact-SQL command.
    /// </summary>
    NoWait = 2,
}
