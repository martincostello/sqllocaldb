// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbApiWrapper.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbApiWrapper.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing an implementation of <see cref="ISqlLocalDbApi"/> that wraps access to <see cref="SqlLocalDbApi"/>.
    /// </summary>
    [Serializable]
    public class SqlLocalDbApiWrapper : ISqlLocalDbApi
    {
        /// <summary>
        /// The shared singleton instance of <see cref="SqlLocalDbApiWrapper"/>.  This field is read-only.
        /// </summary>
        internal static readonly SqlLocalDbApiWrapper Instance = new SqlLocalDbApiWrapper();

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbApiWrapper"/> class.
        /// </summary>
        public SqlLocalDbApiWrapper()
        {
        }

        /// <summary>
        /// Gets the version string for the latest installed version of SQL Server LocalDB.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// No versions of SQL Server LocalDB are installed on the local machine.
        /// </exception>
        public virtual string LatestVersion
        {
            get { return SqlLocalDbApi.LatestVersion; }
        }

        /// <summary>
        /// Gets an <see cref="IList{T}"/> of <see cref="string"/> containing the available version(s) of SQL LocalDB.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The installed versions of SQL LocalDB could not be determined.
        /// </exception>
        public virtual IList<string> Versions
        {
            get { return SqlLocalDbApi.Versions; }
        }

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance.</param>
        /// <param name="version">The version of SQL Server LocalDB to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> or <paramref name="version"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> and <paramref name="version"/> could not be created.
        /// </exception>
        public virtual void CreateInstance(string instanceName, string version)
        {
            SqlLocalDbApi.CreateInstance(instanceName, version);
        }

        /// <summary>
        /// Deletes the specified SQL Server LocalDB instance.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to delete.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be deleted.
        /// </exception>
        public virtual void DeleteInstance(string instanceName)
        {
            SqlLocalDbApi.DeleteInstance(instanceName);
        }

        /// <summary>
        /// Returns information about the specified LocalDB instance.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance to get the information for.</param>
        /// <returns>
        /// An instance of <see cref="ISqlLocalDbInstanceInfo"/> containing information
        /// about the LocalDB instance specified by <paramref name="instanceName"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The information for the SQL Server LocalDB instance specified by
        /// <paramref name="instanceName"/> could not obtained.
        /// </exception>
        public virtual ISqlLocalDbInstanceInfo GetInstanceInfo(string instanceName)
        {
            return SqlLocalDbApi.GetInstanceInfo(instanceName);
        }

        /// <summary>
        /// Returns the names of all the SQL Server LocalDB instances for the current user.
        /// </summary>
        /// <returns>
        /// The names of the the SQL Server LocalDB instances for the current user.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance names could not be determined.
        /// </exception>
        public virtual IList<string> GetInstanceNames()
        {
            return SqlLocalDbApi.GetInstanceNames();
        }

        /// <summary>
        /// Returns information about the specified LocalDB version.
        /// </summary>
        /// <param name="version">The name of the LocalDB version to get the information for.</param>
        /// <returns>
        /// An instance of <see cref="ISqlLocalDbVersionInfo"/> containing information
        /// about the LocalDB version specified by <paramref name="version"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="version"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The information for the SQL Server LocalDB version specified by
        /// <paramref name="version"/> could not be obtained.
        /// </exception>
        public virtual ISqlLocalDbVersionInfo GetVersionInfo(string version)
        {
            return SqlLocalDbApi.GetVersionInfo(version);
        }

        /// <summary>
        /// Returns whether SQL LocalDB is installed on the current machine.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if SQL Server LocalDB is installed on the
        /// current machine; otherwise <see langword="false"/>.
        /// </returns>
        public virtual bool IsLocalDBInstalled()
        {
            return SqlLocalDbApi.IsLocalDBInstalled();
        }

        /// <summary>
        /// Shares the specified SQL Server LocalDB instance with other
        /// users of the computer, using the specified shared name.
        /// </summary>
        /// <param name="ownerSid">The SID of the instance owner.</param>
        /// <param name="instanceName">The private name for the LocalDB instance to share.</param>
        /// <param name="sharedInstanceName">The shared name for the LocalDB instance to share.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ownerSid"/>, <paramref name="instanceName"/> or
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be shared.
        /// </exception>
        public virtual void ShareInstance(string ownerSid, string instanceName, string sharedInstanceName)
        {
            SqlLocalDbApi.ShareInstance(ownerSid, instanceName, sharedInstanceName);
        }

        /// <summary>
        /// Starts the specified instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance to start.</param>
        /// <returns>
        /// The named pipe to use to connect to the LocalDB instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be started.
        /// </exception>
        public virtual string StartInstance(string instanceName)
        {
            return SqlLocalDbApi.StartInstance(instanceName);
        }

        /// <summary>
        /// Enables tracing of native API calls for all the SQL Server
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// Tracing could not be initialized.
        /// </exception>
        public virtual void StartTracing()
        {
            SqlLocalDbApi.StartTracing();
        }

        /// <summary>
        /// Stops the specified instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to stop.
        /// </param>
        /// <param name="timeout">
        /// The amount of time to give the LocalDB instance to stop.
        /// If the value is <see cref="TimeSpan.Zero"/>, the method will
        /// return immediately and not wait for the instance to stop.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="timeout"/> is less than <see cref="TimeSpan.Zero"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be stopped.
        /// </exception>
        /// <remarks>
        /// The <paramref name="timeout"/> parameter is rounded to the nearest second.
        /// </remarks>
        public virtual void StopInstance(string instanceName, TimeSpan timeout)
        {
            SqlLocalDbApi.StopInstance(instanceName, timeout);
        }

        /// <summary>
        /// Disables tracing of native API calls for all the SQL Server
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// Tracing could not be disabled.
        /// </exception>
        public virtual void StopTracing()
        {
            SqlLocalDbApi.StopTracing();
        }

        /// <summary>
        /// Stops the sharing of the specified SQL Server LocalDB instance.
        /// </summary>
        /// <param name="instanceName">
        /// The private name for the LocalDB instance to share.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by
        /// <paramref name="instanceName"/> could not be unshared.
        /// </exception>
        public virtual void UnshareInstance(string instanceName)
        {
            SqlLocalDbApi.UnshareInstance(instanceName);
        }
    }
}
