// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using AdvancedDLSupport;
using Microsoft.Extensions.Logging;

namespace MartinCostello.SqlLocalDb.Interop
{
    /// <summary>
    /// A class containing methods for interop with the SQL LocalDB Instance API.  This class cannot be inherited.
    /// </summary>
    internal sealed class LocalDbInstanceApi : IDisposable
    {
        /// <summary>
        /// The maximum size of SQL Server LocalDB connection string.
        /// </summary>
        internal const int MaximumSqlConnectionStringBufferLength = 260;

        /// <summary>
        /// The maximum size of SQL Server LocalDB instance names.
        /// </summary>
        internal const int MaximumInstanceNameLength = 128;

        /// <summary>
        /// The maximum size of a SQL Server LocalDB version string.
        /// </summary>
        internal const int MaximumInstanceVersionLength = 43;

        /// <summary>
        /// The maximum length of a SID string.
        /// </summary>
        internal const int MaximumSidStringLength = 186;

        /// <summary>
        /// Specifies that error messages that are too long should be truncated.
        /// </summary>
        private const int LocalDbTruncateErrorMessage = 1;

        /// <summary>
        /// An array containing the null character. This field is read-only.
        /// </summary>
        private static readonly char[] _nullArray = new char[] { '\0' };

        /// <summary>
        /// The native API. This field is read-only.
        /// </summary>
        private readonly ILocalDbInstanceApi _api;

        /// <summary>
        /// The native API path resolved. This field is read-only.
        /// </summary>
        private readonly LocalDbInstanceApiPathResolver _pathResolver;

        /// <summary>
        /// Whether the instance has been disposed of.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="LocalDbInstanceApi"/> class.
        /// </summary>
        /// <param name="apiVersion">The version of the SQL LocalDB Instance API to load.</param>
        /// <param name="registry">The <see cref="IRegistry"/> to use.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use.</param>
        internal LocalDbInstanceApi(string apiVersion, IRegistry registry, ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory.CreateLogger<LocalDbInstanceApi>() ?? throw new InvalidOperationException(SRHelper.Format(SR.SqlLocalDbApi_NoLoggerFormat, nameof(LocalDbInstanceApi)));
            var loaderLogger = loggerFactory.CreateLogger<LocalDbInstanceApiPathResolver>() ?? throw new InvalidOperationException(SRHelper.Format(SR.SqlLocalDbApi_NoLoggerFormat, nameof(LocalDbInstanceApiPathResolver)));

            var options =
                ImplementationOptions.EnableOptimizations |
                ImplementationOptions.GenerateDisposalChecks |
                ImplementationOptions.UseLazyBinding;

            _pathResolver = new LocalDbInstanceApiPathResolver(apiVersion, registry, loaderLogger);

            try
            {
                _api = new NativeLibraryBuilder(options, _pathResolver).ActivateInterface<ILocalDbInstanceApi>("SqlLocalDb");
            }
            catch (FileNotFoundException)
            {
                _api = null; // Not installed
            }
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="LocalDbInstanceApi"/> class.
        /// </summary>
        ~LocalDbInstanceApi()
        {
            Dispose(false);
        }

        /// <summary>
        /// Gets the version of the SQL LocalDB native API loaded, if any.
        /// </summary>
        internal Version NativeApiVersion => _pathResolver.NativeApiVersion;

        /// <summary>
        /// Gets the <see cref="ILogger"/> to use.
        /// </summary>
        private ILogger Logger { get; }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Marshals the specified <see cref="Array"/> of <see cref="byte"/> to a <see cref="string"/>.
        /// </summary>
        /// <param name="bytes">The array to marshal as a <see cref="string"/>.</param>
        /// <returns>
        /// A <see cref="string"/> representation of <paramref name="bytes"/>.
        /// </returns>
        internal static string MarshalString(byte[] bytes)
        {
            Debug.Assert(bytes != null, "bytes cannot be null.");
            return Encoding.Unicode.GetString(bytes).TrimEnd(_nullArray);
        }

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="wszVersion">The LocalDB version, for example 11.0 or 11.0.1094.2.</param>
        /// <param name="pInstanceName">The name for the LocalDB instance to create.</param>
        /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int CreateInstance(string wszVersion, string pInstanceName, int dwFlags)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.CreateInstance(wszVersion, pInstanceName, dwFlags);

        /// <summary>
        /// Deletes the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to delete.</param>
        /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int DeleteInstance(string pInstanceName, int dwFlags)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.DeleteInstance(pInstanceName, dwFlags);

        /// <summary>
        /// Returns information for the specified SQL Server Express LocalDB instance,
        /// such as whether it exists, the LocalDB version it uses, whether it is running,
        /// and so on.
        /// </summary>
        /// <param name="wszInstanceName">The instance name.</param>
        /// <param name="pInstanceInfo">The buffer to store the information about the LocalDB instance.</param>
        /// <param name="dwInstanceInfoSize">Holds the size of the InstanceInfo buffer.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int GetInstanceInfo(string wszInstanceName, IntPtr pInstanceInfo, int dwInstanceInfoSize)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.GetInstanceInfo(wszInstanceName, pInstanceInfo, dwInstanceInfoSize);

        /// <summary>
        /// Returns all SQL Server Express LocalDB instances with the given version.
        /// </summary>
        /// <param name="pInstanceNames">
        /// When this function returns, contains the names of both named and default
        /// LocalDB instances on the user’s workstation.
        /// </param>
        /// <param name="lpdwNumberOfInstances">
        /// On input, contains the number of slots for instance names in the
        /// <paramref name="pInstanceNames"/> buffer. On output, contains the number
        /// of LocalDB instances found on the user’s workstation.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int GetInstanceNames(IntPtr pInstanceNames, ref int lpdwNumberOfInstances)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.GetInstances(pInstanceNames, ref lpdwNumberOfInstances);

        /// <summary>
        /// Returns the localized textual description for the specified SQL Server Express LocalDB error.
        /// </summary>
        /// <param name="hrLocalDB">The LocalDB error code.</param>
        /// <param name="dwLanguageId">The language desired (LANGID) or 0, in which case the Win32 FormatMessage language order is used.</param>
        /// <param name="wszMessage">The buffer to store the LocalDB error message.</param>
        /// <param name="lpcchMessage">
        /// On input contains the size of the <paramref name="wszMessage"/> buffer in characters. On output,
        /// if the given buffer size is too small, contains the buffer size required in characters, including
        /// any trailing nulls.  If the function succeeds, contains the number of characters in the message,
        /// excluding any trailing nulls.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int GetLocalDbError(int hrLocalDB, int dwLanguageId, StringBuilder wszMessage, ref int lpcchMessage)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.FormatMessage(hrLocalDB, LocalDbTruncateErrorMessage, dwLanguageId, wszMessage, ref lpcchMessage);

        /// <summary>
        /// Returns information for the specified SQL Server Express LocalDB version,
        /// such as whether it exists and the full LocalDB version number (including
        /// build and release numbers).
        /// </summary>
        /// <param name="wszVersionName">The LocalDB version name.</param>
        /// <param name="pVersionInfo">The buffer to store the information about the LocalDB version.</param>
        /// <param name="dwVersionInfoSize">Holds the size of the VersionInfo buffer.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int GetVersionInfo(string wszVersionName, IntPtr pVersionInfo, int dwVersionInfoSize)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.GetVersionInfo(wszVersionName, pVersionInfo, dwVersionInfoSize);

        /// <summary>
        /// Returns all SQL Server Express LocalDB versions available on the computer.
        /// </summary>
        /// <param name="pVersion">Contains names of the LocalDB versions that are available on the user’s workstation.</param>
        /// <param name="lpdwNumberOfVersions">
        /// On input holds the number of slots for versions in the <paramref name="pVersion"/>
        /// buffer. On output, holds the number of existing LocalDB versions.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int GetVersions(IntPtr pVersion, ref int lpdwNumberOfVersions)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.GetVersions(pVersion, ref lpdwNumberOfVersions);

        /// <summary>
        /// Shares the specified SQL Server Express LocalDB instance with other
        /// users of the computer, using the specified shared name.
        /// </summary>
        /// <param name="pOwnerSID">The SID of the instance owner.</param>
        /// <param name="pInstancePrivateName">The private name for the LocalDB instance to share.</param>
        /// <param name="pInstanceSharedName">The shared name for the LocalDB instance to share.</param>
        /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int ShareInstance(IntPtr pOwnerSID, string pInstancePrivateName, string pInstanceSharedName, int dwFlags)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.ShareInstance(pOwnerSID, pInstancePrivateName, pInstanceSharedName, dwFlags);

        /// <summary>
        /// Starts the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to start.</param>
        /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
        /// <param name="wszSqlConnection">The buffer to store the connection string to the LocalDB instance.</param>
        /// <param name="lpcchSqlConnection">
        /// On input contains the size of the <paramref name="wszSqlConnection"/> buffer in
        /// characters, including any trailing nulls. On output, if the given buffer size is
        /// too small, contains the required buffer size in characters, including any trailing nulls.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int StartInstance(string pInstanceName, int dwFlags, StringBuilder wszSqlConnection, ref int lpcchSqlConnection)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.StartInstance(pInstanceName, dwFlags, wszSqlConnection, ref lpcchSqlConnection);

        /// <summary>
        /// Enables tracing of API calls for all the SQL Server Express
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int StartTracing()
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.StartTracing();

        /// <summary>
        /// Stops the specified SQL Server Express LocalDB instance from running.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to stop.</param>
        /// <param name="options">One or a combination of the flag values specifying the way to stop the instance.</param>
        /// <param name="ulTimeout">
        /// The time in seconds to wait for this operation to complete. If this
        /// value is 0, this function will return immediately without waiting for the LocalDB instance to stop.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int StopInstance(string pInstanceName, StopInstanceOptions options, int ulTimeout)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.StopInstance(pInstanceName, (int)options, ulTimeout);

        /// <summary>
        /// Disables tracing of API calls for all the SQL Server Express LocalDB
        /// instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int StopTracing()
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.StopTracing();

        /// <summary>
        /// Stops the sharing of the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">
        /// The private name for the LocalDB instance to share.
        /// </param>
        /// <param name="dwFlags">
        /// Reserved for future use. Currently should be set to 0.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal int UnshareInstance(string pInstanceName, int dwFlags)
            => _api == null ? SqlLocalDbErrors.NotInstalled : _api.UnshareInstance(pInstanceName, dwFlags);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged resources;
        /// <see langword="false" /> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose of managed resources
                }

                // Dispose of unmanaged resources
                if (_api != null)
                {
                    _api.Dispose();
                    Logger.NativeApiUnloaded(_pathResolver.LibraryPath);
                }

                _disposed = true;
            }
        }
    }
}
