// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlLocalDbVersionInfo.cs" company="http://sqllocaldb.codeplex.com">
//   New BSD License (BSD)
// </copyright>
// <license>
//   This source code is subject to terms and conditions of the New BSD Licence (BSD). 
//   A copy of the license can be found in the License.txt file at the root of this
//   distribution.  By using this source code in any fashion, you are agreeing to be
//   bound by the terms of the New BSD Licence. You must not remove this notice, or
//   any other, from this software.
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