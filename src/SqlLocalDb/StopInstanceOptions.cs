// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StopInstanceOptions.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   StopInstanceOptions.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Data.SqlLocalDb
{
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
}