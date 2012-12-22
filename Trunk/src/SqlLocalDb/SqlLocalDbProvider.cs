// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbProvider.cs" company="http://sqllocaldb.codeplex.com">
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
//   SqlLocalDbProvider.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing a provider for obtaining instances of <see cref="SqlLocalDbInstance"/>
    /// </summary>
    [Serializable]
    public class SqlLocalDbProvider : ISqlLocalDbProvider
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbProvider"/> class.
        /// </summary>
        public SqlLocalDbProvider()
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="ISqlLocalDbInstance"/> with a unique random name.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="ISqlLocalDbInstance"/>.
        /// </returns>
        /// <exception cref="SqlLocalDbException">
        /// A new LocalDB instance could not be created.
        /// </exception>
        public virtual SqlLocalDbInstance CreateInstance()
        {
            return CreateInstance(Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Creates a new instance of <see cref="ISqlLocalDbInstance"/>.
        /// </summary>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance to create.</param>
        /// <returns>
        /// The created instance of <see cref="ISqlLocalDbInstance"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> already exists.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> could not be created.
        /// </exception>
        public virtual SqlLocalDbInstance CreateInstance(string instanceName)
        {
            ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

            if (info.Exists)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbFactory_InstanceExistsFormat,
                    instanceName);

                Logger.Error(Logger.TraceEvent.CreateInstance, message);

                throw new InvalidOperationException(message);
            }

            SqlLocalDbApi.CreateInstance(instanceName);
            return GetInstance(instanceName);
        }

        /// <summary>
        /// Returns an existing instance of <see cref="ISqlLocalDbInstance"/>.
        /// </summary>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance to return.</param>
        /// <returns>
        /// The existing instance of <see cref="ISqlLocalDbInstance"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> does not exist.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> could not be obtained.
        /// </exception>
        public virtual SqlLocalDbInstance GetInstance(string instanceName)
        {
            return new SqlLocalDbInstance(instanceName);
        }

        /// <summary>
        /// Returns information about the available SQL Server LocalDB instances.
        /// </summary>
        /// <returns>
        /// An <see cref="IList&lt;ISqlLocalDbInstanceInfo&gt;"/> containing information
        /// about the available SQL Server LocalDB instances on the current machine.
        /// </returns>
        public virtual IList<ISqlLocalDbInstanceInfo> GetInstances()
        {
            IList<string> instanceNames = SqlLocalDbApi.Versions;

            List<ISqlLocalDbInstanceInfo> instances = new List<ISqlLocalDbInstanceInfo>();

            foreach (string name in instanceNames)
            {
                ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(name);
                instances.Add(info);
            }

            return instances;
        }

        /// <summary>
        /// Returns information about the installed SQL Server LocalDB versions.
        /// </summary>
        /// <returns>
        /// An <see cref="IList&lt;ISqlLocalDbVersionInfo&gt;"/> containing information
        /// about the SQL Server LocalDB versions installed on the current machine.
        /// </returns>
        public virtual IList<ISqlLocalDbVersionInfo> GetVersions()
        {
            IList<string> versionNames = SqlLocalDbApi.Versions;

            List<ISqlLocalDbVersionInfo> versions = new List<ISqlLocalDbVersionInfo>();

            foreach (string version in versionNames)
            {
                ISqlLocalDbVersionInfo info = SqlLocalDbApi.GetVersionInfo(version);
                versions.Add(info);
            }

            return versions;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ISqlLocalDbInstance"/>.
        /// </summary>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance to create.</param>
        /// <returns>
        /// The created instance of <see cref="ISqlLocalDbInstance"/>.
        /// </returns>
        ISqlLocalDbInstance ISqlLocalDbProvider.CreateInstance(string instanceName)
        {
            return CreateInstance(instanceName);
        }

        /// <summary>
        /// Returns an existing instance of <see cref="ISqlLocalDbInstance"/>.
        /// </summary>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance to return.</param>
        /// <returns>
        /// The existing instance of <see cref="ISqlLocalDbInstance"/>.
        /// </returns>
        ISqlLocalDbInstance ISqlLocalDbProvider.GetInstance(string instanceName)
        {
            return GetInstance(instanceName);
        }

        #endregion
    }
}