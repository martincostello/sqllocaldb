// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlLocalDbProvider.cs" company="http://sqllocaldb.codeplex.com">
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
//   ISqlLocalDbProvider.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// Defines methods for obtaining instances of <see cref="ISqlLocalDbInstance"/>.
    /// </summary>
    public interface ISqlLocalDbProvider
    {
        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="ISqlLocalDbInstance"/>.
        /// </summary>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance to create.</param>
        /// <returns>
        /// The created instance of <see cref="ISqlLocalDbInstance"/>.
        /// </returns>
        ISqlLocalDbInstance CreateInstance(string instanceName);

        /// <summary>
        /// Returns an existing instance of <see cref="ISqlLocalDbInstance"/>.
        /// </summary>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance to return.</param>
        /// <returns>
        /// The existing instance of <see cref="ISqlLocalDbInstance"/>.
        /// </returns>
        ISqlLocalDbInstance GetInstance(string instanceName);

        /// <summary>
        /// Returns information about the available SQL Server LocalDB instances.
        /// </summary>
        /// <returns>
        /// An <see cref="IList&lt;ISqlLocalDbInstanceInfo&gt;"/> containing information
        /// about the available SQL Server LocalDB instances on the current machine.
        /// </returns>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Requires querying the native LocalDB API.")]
        IList<ISqlLocalDbInstanceInfo> GetInstances();

        /// <summary>
        /// Returns information about the installed SQL Server LocalDB versions.
        /// </summary>
        /// <returns>
        /// An <see cref="IList&lt;ISqlLocalDbVersionInfo&gt;"/> containing information
        /// about the SQL Server LocalDB versions installed on the current machine.
        /// </returns>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Requires querying the native LocalDB API.")]
        IList<ISqlLocalDbVersionInfo> GetVersions();

        #endregion
    }
}