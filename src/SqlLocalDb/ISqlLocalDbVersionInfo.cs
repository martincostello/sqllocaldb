// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlLocalDbVersionInfo.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   ISqlLocalDbVersionInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// Defines information about a version of SQL Server LocalDB.
    /// </summary>
    public interface ISqlLocalDbVersionInfo
    {
        #region Properties

        /// <summary>
        /// Gets a value indicating whether the instance files exist on disk.
        /// </summary>
        bool Exists
        {
            get;
        }

        /// <summary>
        /// Gets the version name.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        Version Version
        {
            get;
        }

        #endregion
    }
}