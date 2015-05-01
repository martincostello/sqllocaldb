// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporarySqlLocalDbInstance.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   TemporarySqlLocalDbInstance.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.SqlClient;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing a temporary SQL LocalDB instance.
    /// </summary>
    /// <remarks>
    /// The temporary SQL LocalDB instances that are created by instances of this class are automatically
    /// started when they are instantiated, and are then subsequently deleted when they are disposed of.
    /// </remarks>
    public class TemporarySqlLocalDbInstance : ISqlLocalDbInstance, IDisposable
    {
        /// <summary>
        /// The default <see cref="ISqlLocalDbProvider"/> instance to use to create temporary instances.
        /// </summary>
        private static readonly ISqlLocalDbProvider DefaultProvider = new SqlLocalDbProvider();

        /// <summary>
        /// Whether to delete the files associated with the instance when disposed.
        /// </summary>
        private readonly bool _deleteFiles;

        /// <summary>
        /// The temporary SQL LocalDB instance.
        /// </summary>
        private readonly ISqlLocalDbInstance _instance;

        /// <summary>
        /// Whether the instance has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlLocalDbInstance"/> class.
        /// </summary>
        /// <param name="instanceName">The name of the temporary SQL LocalDB instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        public TemporarySqlLocalDbInstance(string instanceName)
            : this(instanceName, DefaultProvider)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlLocalDbInstance"/> class.
        /// </summary>
        /// <param name="instanceName">The name of the temporary SQL LocalDB instance.</param>
        /// <param name="provider">The <see cref="ISqlLocalDbProvider"/> to use to create the temporary instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> or <paramref name="provider"/> is <see langword="null"/>.
        /// </exception>
        public TemporarySqlLocalDbInstance(string instanceName, ISqlLocalDbProvider provider)
            : this(instanceName, provider, SqlLocalDbApi.AutomaticallyDeleteInstanceFiles)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlLocalDbInstance"/> class.
        /// </summary>
        /// <param name="instanceName">The name of the temporary SQL LocalDB instance.</param>
        /// <param name="provider">The <see cref="ISqlLocalDbProvider"/> to use to create the temporary instance.</param>
        /// <param name="deleteFiles">Whether to delete the file(s) associated with the SQL LocalDB instance when deleted.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> or <paramref name="provider"/> is <see langword="null"/>.
        /// </exception>
        public TemporarySqlLocalDbInstance(string instanceName, ISqlLocalDbProvider provider, bool deleteFiles)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            if (provider == null)
            {
                throw new ArgumentNullException("provider");
            }

            _instance = provider.CreateInstance(instanceName);
            _deleteFiles = deleteFiles;

            try
            {
                _instance.Start();
            }
            catch (Exception)
            {
                SqlLocalDbInstance.Delete(_instance, throwIfNotFound: true, deleteFiles: _deleteFiles);
                throw;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlLocalDbInstance"/> class.
        /// </summary>
        /// <param name="instance">The <see cref="ISqlLocalDbInstance"/> instance to use.</param>
        /// <remarks>
        /// Used for unit testing.
        /// </remarks>
        internal TemporarySqlLocalDbInstance(ISqlLocalDbInstance instance)
        {
            _instance = instance;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TemporarySqlLocalDbInstance"/> class.
        /// </summary>
        ~TemporarySqlLocalDbInstance()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the name of the LocalDB instance.
        /// </summary>
        public string Name
        {
            get { return _instance.Name; }
        }

        /// <summary>
        /// Gets the named pipe that should be used
        /// to connect to the LocalDB instance.
        /// </summary>
        public string NamedPipe
        {
            get { return _instance.NamedPipe; }
        }

        /// <summary>
        /// Gets a value indicating whether to delete the instance file(s) when the instance is disposed of.
        /// </summary>
        internal bool DeleteFiles
        {
            get { return _deleteFiles; }
        }

        /// <summary>
        /// Gets the temporary SQL LocalDB instance associated with this instance.
        /// </summary>
        internal ISqlLocalDbInstance Instance
        {
            get { return _instance; }
        }

        /// <summary>
        /// Creates a new instance of <see cref="TemporarySqlLocalDbInstance"/> with a randomly assigned name.
        /// </summary>
        /// <returns>
        /// The created instance of <see cref="TemporarySqlLocalDbInstance"/>.
        /// </returns>
        public static TemporarySqlLocalDbInstance Create()
        {
            return Create(SqlLocalDbApi.AutomaticallyDeleteInstanceFiles);
        }

        /// <summary>
        /// Creates a new instance of <see cref="TemporarySqlLocalDbInstance"/> with a randomly assigned name.
        /// </summary>
        /// <param name="deleteFiles">Whether to delete the file(s) associated with the SQL LocalDB instance when deleted.</param>
        /// <returns>
        /// The created instance of <see cref="TemporarySqlLocalDbInstance"/>.
        /// </returns>
        public static TemporarySqlLocalDbInstance Create(bool deleteFiles)
        {
            string instanceName = Guid.NewGuid().ToString();
            return new TemporarySqlLocalDbInstance(instanceName, DefaultProvider, deleteFiles);
        }

        /// <summary>
        /// Creates a connection to the LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="SqlConnection" /> that
        /// can be used to connect to the LocalDB instance.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The instance has been disposed.
        /// </exception>
        public SqlConnection CreateConnection()
        {
            EnsureNotDisposed();
            return _instance.CreateConnection();
        }

        /// <summary>
        /// Creates an instance of <see cref="SqlConnectionStringBuilder" /> containing
        /// the default SQL connection string to connect to the LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="SqlConnectionStringBuilder" /> containing
        /// the default SQL connection string to connect to the LocalDB instance.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The instance has been disposed.
        /// </exception>
        public SqlConnectionStringBuilder CreateConnectionStringBuilder()
        {
            EnsureNotDisposed();
            return _instance.CreateConnectionStringBuilder();
        }

        /// <summary>
        /// Returns information about the LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="ISqlLocalDbInstanceInfo" /> containing
        /// information about the LocalDB instance.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// The instance has been disposed.
        /// </exception>
        public ISqlLocalDbInstanceInfo GetInstanceInfo()
        {
            EnsureNotDisposed();
            return _instance.GetInstanceInfo();
        }

        /// <summary>
        /// Shares the LocalDB instance using the specified name.
        /// </summary>
        /// <param name="sharedName">The name to use to share the instance.</param>
        /// <exception cref="ObjectDisposedException">
        /// The instance has been disposed.
        /// </exception>
        public void Share(string sharedName)
        {
            EnsureNotDisposed();
            _instance.Share(sharedName);
        }

        /// <summary>
        /// Starts the LocalDB instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The instance has been disposed.
        /// </exception>
        public void Start()
        {
            EnsureNotDisposed();
            _instance.Start();
        }

        /// <summary>
        /// Stops the LocalDB instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The instance has been disposed.
        /// </exception>
        public void Stop()
        {
            EnsureNotDisposed();
            _instance.Stop();
        }

        /// <summary>
        /// Stops sharing the LocalDB instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The instance has been disposed.
        /// </exception>
        public void Unshare()
        {
            EnsureNotDisposed();
            _instance.Unshare();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged resources;
        /// <see langword="false" /> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (_instance != null)
                {
                    try
                    {
                        _instance.Stop();
                    }
                    catch (SqlLocalDbException ex)
                    {
                        // Ignore the exception if we could not stop the instance
                        // because it does not exist, otherwise log the error.
                        if (ex.ErrorCode != SqlLocalDbErrors.UnknownInstance)
                        {
                            Logger.Error(Logger.TraceEvent.StopFailed, SR.TemporarySqlLocalDbInstance_StopFailedFormat, _instance.Name, ex.ErrorCode);
                        }
                    }

                    try
                    {
                        SqlLocalDbInstance.Delete(_instance, throwIfNotFound: false, deleteFiles: _deleteFiles);
                    }
                    catch (SqlLocalDbException ex)
                    {
                        // Ignore the exception if we could not delete the instance because it was in use
                        if (ex.ErrorCode != SqlLocalDbErrors.InstanceBusy)
                        {
                            Logger.Error(Logger.TraceEvent.DeleteFailed, SR.TemporarySqlLocalDbInstance_DeleteFailedFormat, _instance.Name, ex.ErrorCode);
                        }
                    }
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Ensures that the instance has not been disposed of.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// The instance has been disposed.
        /// </exception>
        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }
    }
}