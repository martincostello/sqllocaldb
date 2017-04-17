// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbInstance.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbInstance.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.SqlClient;
using System.Diagnostics;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing an instance of SQL Server LocalDB.
    /// </summary>
    [DebuggerDisplay("{Name}")]
    [Serializable]
    public class SqlLocalDbInstance : ISqlLocalDbInstance
    {
        /// <summary>
        /// The SQL Server LocalDB instance name.
        /// </summary>
        private readonly string _instanceName;

        /// <summary>
        /// The named pipe to the SQL Server LocalDB instance.
        /// </summary>
        private string _namedPipe;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbInstance"/> class.
        /// </summary>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> does not exist.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> could not be obtained.
        /// </exception>
        public SqlLocalDbInstance(string instanceName)
            : this(instanceName, SqlLocalDbApiWrapper.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbInstance"/> class.
        /// </summary>
        /// <param name="instanceName">The name of the SQL Server LocalDB instance.</param>
        /// <param name="localDB">The <see cref="ISqlLocalDbApi"/> instance to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> does not exist.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance specified by <paramref name="instanceName"/> could not be obtained.
        /// </exception>
        internal SqlLocalDbInstance(string instanceName, ISqlLocalDbApi localDB)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException(nameof(instanceName));
            }

            Debug.Assert(localDB != null, "localDB cannot be  null.");

            ISqlLocalDbInstanceInfo info = localDB.GetInstanceInfo(instanceName);

            if (info == null || !info.Exists)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_InstanceNotFoundFormat,
                    instanceName);

                Logger.Error(Logger.TraceEvent.General, message);

                throw new InvalidOperationException(message);
            }

            _instanceName = instanceName;
            _namedPipe = info.NamedPipe;
        }

        /// <summary>
        /// Gets a value indicating whether the LocalDB instance is running.
        /// </summary>
        public bool IsRunning => !string.IsNullOrEmpty(_namedPipe);

        /// <summary>
        /// Gets the name of the LocalDB instance.
        /// </summary>
        public string Name => _instanceName;

        /// <summary>
        /// Gets the named pipe that should be used
        /// to connect to the LocalDB instance.
        /// </summary>
        public string NamedPipe => _namedPipe;

        /// <summary>
        /// Deletes the specified <see cref="ISqlLocalDbInstance"/> instance.
        /// </summary>
        /// <param name="instance">The LocalDB instance to delete.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instance"/> could not be deleted.
        /// </exception>
        public static void Delete(ISqlLocalDbInstance instance)
        {
            Delete(instance, throwIfNotFound: true);
        }

        /// <summary>
        /// Creates a <see cref="SqlConnection"/> instance to communicate
        /// with the SQL Server LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="SqlConnection"/> that can be used
        /// to communicate with the SQL Server Local DB instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The value of <see cref="SqlLocalDbInstance.IsRunning"/> is <see langword="false"/>.
        /// </exception>
        public virtual SqlConnection CreateConnection()
        {
            SqlConnectionStringBuilder builder = CreateConnectionStringBuilder();

            if (builder == null)
            {
                return new SqlConnection();
            }

            return new SqlConnection(builder.ConnectionString);
        }

        /// <summary>
        /// Creates an instance of <see cref="SqlConnectionStringBuilder"/> containing
        /// the default SQL connection string to connect to the LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="SqlConnectionStringBuilder"/> containing
        /// the default SQL connection string to connect to the LocalDB instance.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// The value of <see cref="SqlLocalDbInstance.IsRunning"/> is <see langword="false"/>.
        /// </exception>
        public virtual SqlConnectionStringBuilder CreateConnectionStringBuilder()
        {
            if (!this.IsRunning)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_NotRunningFormat,
                    _instanceName);

                throw new InvalidOperationException(message);
            }

            return new SqlConnectionStringBuilder()
            {
                DataSource = _namedPipe
            };
        }

        /// <summary>
        /// Returns information about the LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="ISqlLocalDbInstanceInfo"/> containing information about the LocalDB instance.
        /// </returns>
        public virtual ISqlLocalDbInstanceInfo GetInstanceInfo() => SqlLocalDbApi.GetInstanceInfo(_instanceName);

        /// <summary>
        /// Shares the LocalDB instance using the specified name.
        /// </summary>
        /// <param name="sharedName">The name to use to share the instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="sharedName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance could not be shared.
        /// </exception>
        public virtual void Share(string sharedName)
        {
            if (sharedName == null)
            {
                throw new ArgumentNullException(nameof(sharedName));
            }

            try
            {
                SqlLocalDbApi.ShareInstance(_instanceName, sharedName);
            }
            catch (SqlLocalDbException e)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_ShareFailedFormat,
                    _instanceName);

                Logger.Error(Logger.TraceEvent.ShareInstance, message);

                throw new SqlLocalDbException(
                    message,
                    e.ErrorCode,
                    e.InstanceName,
                    e);
            }
        }

        /// <summary>
        /// Starts the SQL Server LocalDB instance.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance could not be started.
        /// </exception>
        public void Start()
        {
            try
            {
                // The pipe name changes per instance lifetime
                _namedPipe = SqlLocalDbApi.StartInstance(_instanceName);
            }
            catch (SqlLocalDbException e)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_StartFailedFormat,
                    _instanceName);

                Logger.Error(Logger.TraceEvent.StartInstance, message);

                throw new SqlLocalDbException(
                    message,
                    e.ErrorCode,
                    e.InstanceName,
                    e);
            }
        }

        /// <summary>
        /// Stops the SQL Server LocalDB instance.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance could not be stopped.
        /// </exception>
        public void Stop()
        {
            try
            {
                SqlLocalDbApi.StopInstance(_instanceName);
                _namedPipe = string.Empty;
            }
            catch (SqlLocalDbException e)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_StopFailedFormat,
                    _instanceName);

                Logger.Error(Logger.TraceEvent.StopInstance, message);

                throw new SqlLocalDbException(
                    message,
                    e.ErrorCode,
                    e.InstanceName,
                    e);
            }
        }

        /// <summary>
        /// Stops sharing the LocalDB instance.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// The LocalDB instance could not be unshared.
        /// </exception>
        public virtual void Unshare()
        {
            try
            {
                SqlLocalDbApi.UnshareInstance(_instanceName);
            }
            catch (SqlLocalDbException e)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_UnshareFailedFormat,
                    _instanceName);

                Logger.Error(Logger.TraceEvent.UnshareInstance, message);

                throw new SqlLocalDbException(
                    message,
                    e.ErrorCode,
                    e.InstanceName,
                    e);
            }
        }

        /// <summary>
        /// Deletes the specified <see cref="ISqlLocalDbInstance"/> instance.
        /// </summary>
        /// <param name="instance">The LocalDB instance to delete.</param>
        /// <param name="throwIfNotFound">
        /// Whether to throw an exception if the SQL LocalDB instance
        /// associated with <paramref name="instance"/> cannot be found.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instance"/> could not be deleted.
        /// </exception>
        internal static void Delete(ISqlLocalDbInstance instance, bool throwIfNotFound)
        {
            Delete(instance, throwIfNotFound, SqlLocalDbApi.AutomaticallyDeleteInstanceFiles);
        }

        /// <summary>
        /// Deletes the specified <see cref="ISqlLocalDbInstance"/> instance.
        /// </summary>
        /// <param name="instance">The LocalDB instance to delete.</param>
        /// <param name="throwIfNotFound">
        /// Whether to throw an exception if the SQL LocalDB instance
        /// associated with <paramref name="instance"/> cannot be found.
        /// </param>
        /// <param name="deleteFiles">
        /// Whether to delete the file(s) associated with the SQL LocalDB instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instance"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instance"/> could not be deleted.
        /// </exception>
        internal static void Delete(ISqlLocalDbInstance instance, bool throwIfNotFound, bool deleteFiles)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            try
            {
                SqlLocalDbApi.DeleteInstanceInternal(instance.Name, throwIfNotFound, deleteFiles);
            }
            catch (SqlLocalDbException ex)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbInstance_DeleteFailedFormat,
                    instance.Name);

                Logger.Error(Logger.TraceEvent.DeleteFailed, message);

                throw new SqlLocalDbException(
                    message,
                    ex.ErrorCode,
                    ex.InstanceName,
                    ex);
            }
        }
    }
}
