// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbProvider.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
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
        /// <summary>
        /// The instance of <see cref="ISqlLocalDbApi"/> in use by the provider.
        /// </summary>
        private readonly ISqlLocalDbApi _localDB;

        /// <summary>
        /// The version of SQL LocalDB to use to create instances, if specified.
        /// </summary>
        /// <remarks>
        /// If the value is <see langword="null"/>, the latest version reported by <see cref="_localDB"/> is used.
        /// </remarks>
        private string _version;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbProvider"/> class.
        /// </summary>
        public SqlLocalDbProvider()
        {
            _localDB = SqlLocalDbApiWrapper.Instance;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbProvider"/> class.
        /// </summary>
        /// <param name="localDB">The instance of <see cref="ISqlLocalDbApi"/> to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="localDB"/> is <see langword="null"/>.
        /// </exception>
        public SqlLocalDbProvider(ISqlLocalDbApi localDB)
        {
            _localDB = localDB ?? throw new ArgumentNullException(nameof(localDB));
        }

        /// <summary>
        /// Gets or sets the version of SQL LocalDB to use to create instances.
        /// </summary>
        /// <remarks>
        /// If no value is set, the latest version of SQL LocalDB installed on the local machine is used.
        /// </remarks>
        public virtual string Version
        {
            get { return _version ?? _localDB.LatestVersion;  }
            set { _version = value; }
        }

        /// <summary>
        /// Gets the <see cref="ISqlLocalDbApi"/> in use by the instance.
        /// </summary>
        internal ISqlLocalDbApi LocalDB => _localDB;

        /// <summary>
        /// Creates a new instance of <see cref="ISqlLocalDbInstance"/> with a unique random name.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="ISqlLocalDbInstance"/>.
        /// </returns>
        /// <exception cref="SqlLocalDbException">
        /// A new LocalDB instance could not be created.
        /// </exception>
        public virtual SqlLocalDbInstance CreateInstance() => CreateInstance(Guid.NewGuid().ToString());

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
        /// The LocalDB instance specified by <paramref name="instanceName"/> already exists or
        /// no LocalDB instance information was returned by the underlying SQL LocalDB API.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> could not be created.
        /// </exception>
        public virtual SqlLocalDbInstance CreateInstance(string instanceName)
        {
            ISqlLocalDbInstanceInfo info = _localDB.GetInstanceInfo(instanceName);

            if (info == null)
            {
                throw new InvalidOperationException(SR.SqlLocalDbProvider_NoInstance);
            }

            if (info.Exists)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbFactory_InstanceExistsFormat,
                    instanceName);

                Logger.Error(Logger.TraceEvent.CreateInstance, message);

                throw new InvalidOperationException(message);
            }

            string version;

            // If creating the default instance, the version number must not be specified
            if (SqlLocalDbApi.IsDefaultInstanceName(instanceName))
            {
                version = string.Empty;
            }
            else
            {
                version = _version ?? _localDB.LatestVersion;
            }

            _localDB.CreateInstance(instanceName, version);

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
        public virtual SqlLocalDbInstance GetInstance(string instanceName) => new SqlLocalDbInstance(instanceName, _localDB);

        /// <summary>
        /// Returns information about the available SQL Server LocalDB instances.
        /// </summary>
        /// <returns>
        /// An <see cref="IList&lt;ISqlLocalDbInstanceInfo&gt;"/> containing information
        /// about the available SQL Server LocalDB instances on the current machine.
        /// </returns>
        public virtual IList<ISqlLocalDbInstanceInfo> GetInstances()
        {
            IList<string> instanceNames = _localDB.GetInstanceNames();

            List<ISqlLocalDbInstanceInfo> instances = new List<ISqlLocalDbInstanceInfo>();

            if (instanceNames != null)
            {
                foreach (string name in instanceNames)
                {
                    ISqlLocalDbInstanceInfo info = _localDB.GetInstanceInfo(name);
                    instances.Add(info);
                }
            }

            return instances;
        }

        /// <summary>
        /// Returns information about the installed SQL Server LocalDB version(s).
        /// </summary>
        /// <returns>
        /// An <see cref="IList&lt;ISqlLocalDbVersionInfo&gt;"/> containing information
        /// about the SQL Server LocalDB version(s) installed on the current machine.
        /// </returns>
        public virtual IList<ISqlLocalDbVersionInfo> GetVersions()
        {
            IList<string> versionNames = _localDB.Versions;

            List<ISqlLocalDbVersionInfo> versions = new List<ISqlLocalDbVersionInfo>();

            if (versionNames != null)
            {
                foreach (string version in versionNames)
                {
                    ISqlLocalDbVersionInfo info = _localDB.GetVersionInfo(version);
                    versions.Add(info);
                }
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
        ISqlLocalDbInstance ISqlLocalDbProvider.CreateInstance(string instanceName) => CreateInstance(instanceName);

        /// <summary>
        /// Returns an existing instance of <see cref="ISqlLocalDbInstance"/>.
        /// </summary>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance to return.</param>
        /// <returns>
        /// The existing instance of <see cref="ISqlLocalDbInstance"/>.
        /// </returns>
        ISqlLocalDbInstance ISqlLocalDbProvider.GetInstance(string instanceName) => GetInstance(instanceName);
    }
}
