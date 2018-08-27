// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class representing a temporary SQL LocalDB instance. This class cannot be inherited.
    /// </summary>
    /// <remarks>
    /// The temporary SQL LocalDB instances that are created by instances of this class are automatically
    /// started when they are instantiated, and are then subsequently deleted when they are disposed of.
    /// </remarks>
    public sealed class TemporarySqlLocalDbInstance : IDisposable, ISqlLocalDbApiAdapter
    {
        /// <summary>
        /// The lazily initialized name of the temporary SQL LocalDB instance. This field is read-only.
        /// </summary>
        private readonly Lazy<string> _instanceName;

        /// <summary>
        /// Whether the instance has been disposed.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The <see cref="ILogger"/> to use, if any.
        /// </summary>
        private ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlLocalDbInstance"/> class.
        /// </summary>
        /// <param name="api">The <see cref="ISqlLocalDbApi"/> to use to create the temporary instance.</param>
        /// <param name="deleteFiles">Whether to delete the file(s) associated with the SQL LocalDB instance when deleted.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="api"/> is <see langword="null"/>.
        /// </exception>
        internal TemporarySqlLocalDbInstance(ISqlLocalDbApi api, bool deleteFiles)
        {
            Api = api ?? throw new ArgumentNullException(nameof(api));
            DeleteFiles = deleteFiles;
            _instanceName = new Lazy<string>(EnsureInitialized);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="TemporarySqlLocalDbInstance"/> class.
        /// </summary>
        ~TemporarySqlLocalDbInstance()
        {
            DisposeInternal();
        }

        /// <summary>
        /// Gets the connection string for the temporary SQL LocalDB instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Thrown if the instance has been disposed of.
        /// </exception>
        public string ConnectionString => GetInstanceInfo().GetConnectionString();

        /// <summary>
        /// Gets the name of the temporary SQL LocalDB instance.
        /// </summary>
        /// <exception cref="ObjectDisposedException">
        /// Thrown if the instance has been disposed of.
        /// </exception>
        public string Name
        {
            get
            {
                EnsureNotDisposed();
                return _instanceName.Value;
            }
        }

        /// <inheritdoc />
        ISqlLocalDbApi ISqlLocalDbApiAdapter.LocalDb => Api;

        /// <summary>
        /// Gets the <see cref="ISqlLocalDbApi"/> to use.
        /// </summary>
        private ISqlLocalDbApi Api { get; }

        /// <summary>
        /// Gets a value indicating whether to delete the instance file(s) when the instance is disposed of.
        /// </summary>
        private bool DeleteFiles { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            DisposeInternal();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the temporary SQL LocalDB instance.
        /// </summary>
        /// <returns>
        /// An <see cref="ISqlLocalDbInstanceInfo"/> representing the temporary SQL LocalDB instance.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// Thrown if the instance has been disposed of.
        /// </exception>
        public ISqlLocalDbInstanceInfo GetInstanceInfo() => Api.GetInstanceInfo(Name);

        /// <summary>
        /// Returns an <see cref="ISqlLocalDbInstanceManager"/> that can be used to manage the instance.
        /// </summary>
        /// <returns>
        /// An <see cref="ISqlLocalDbInstanceManager"/> that can be used to manage the temporary SQL LocalDB instance.
        /// </returns>
        /// <exception cref="ObjectDisposedException">
        /// Thrown if the instance has been disposed of.
        /// </exception>
        public ISqlLocalDbInstanceManager Manage() => GetInstanceInfo().Manage();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        private void DisposeInternal()
        {
            if (!_disposed)
            {
                if (_instanceName != null && _instanceName.IsValueCreated)
                {
                    string instanceName = _instanceName.Value;

                    try
                    {
                        Api.StopInstance(instanceName, timeout: null);
                    }
                    catch (SqlLocalDbException ex)
                    {
                        // Ignore the exception if we could not stop the instance
                        // because it does not exist, otherwise log the error.
                        if (ex.ErrorCode != SqlLocalDbErrors.UnknownInstance)
                        {
                            _logger?.StoppingTemporaryInstanceFailed(instanceName, ex.ErrorCode);
                        }
                    }

                    try
                    {
                        DeleteInstance(instanceName);
                    }
                    catch (SqlLocalDbException ex)
                    {
                        // Ignore the exception if we could not delete the instance because it was still in use
                        if (ex.ErrorCode != SqlLocalDbErrors.InstanceBusy)
                        {
                            _logger?.DeletingInstanceFailed(instanceName, ex.ErrorCode);
                        }
                    }
                }

                _disposed = true;
            }
        }

        /// <summary>
        /// Ensures that the instance has been initialized.
        /// </summary>
        /// <returns>
        /// The name of the SQL LocalDB instance that was created.
        /// </returns>
        private string EnsureInitialized()
        {
            ILoggerFactory loggerFactory = null;

            if (Api is SqlLocalDbApi localDB)
            {
                loggerFactory = localDB.LoggerFactory;
            }

            _logger = loggerFactory?.CreateLogger<TemporarySqlLocalDbInstance>();

            string instanceName = Guid.NewGuid().ToString();
            Api.CreateInstance(instanceName, Api.LatestVersion);

            try
            {
                Api.StartInstance(instanceName);
                return instanceName;
            }
            catch (Exception)
            {
                DeleteInstance(instanceName);
                throw;
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
                throw new ObjectDisposedException(GetType().FullName);
            }
        }

        /// <summary>
        /// Deletes the specified SQL LocalDB instance.
        /// </summary>
        /// <param name="instanceName">The SQL LocalDB instance to delete.</param>
        private void DeleteInstance(string instanceName)
        {
            if (Api is SqlLocalDbApi localDB)
            {
                localDB.DeleteInstanceInternal(instanceName, throwIfNotFound: false, deleteFiles: DeleteFiles);
            }
            else
            {
                Api.DeleteInstance(instanceName);
            }
        }
    }
}
