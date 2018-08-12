// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using MartinCostello.SqlLocalDb.Interop;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class representing a wrapper to the SQL Server LocalDB Instance API. This class cannot be inherited.
    /// </summary>
    public sealed class SqlLocalDbApi : ISqlLocalDbApi, ISqlLocalDbApiAdapter, IDisposable
    {
        /// <summary>
        /// The name of the default instance in SQL LocalDB 2012.
        /// </summary>
        private const string DefaultInstanceName2012 = "v11.0";

        /// <summary>
        /// The name of the default instance in SQL LocalDB 2014 and later.
        /// </summary>
        private const string DefaultInstanceName2014AndLater = "MSSQLLocalDB";

        /// <summary>
        /// The maximum length of a SQL LocalDB instance name, in bytes.
        /// </summary>
        private const int MaxInstanceNameLength = (LocalDbInstanceApi.MaximumInstanceNameLength + 1) * sizeof(char);

        /// <summary>
        /// The maximum length of a SQL LocalDB version string, in bytes.
        /// </summary>
        private const int MaxVersionLength = (LocalDbInstanceApi.MaximumInstanceVersionLength + 1) * sizeof(char);

        /// <summary>
        /// The value to pass to functions which have a reserved parameter for future use.
        /// </summary>
        private const int ReservedValue = 0;

        /// <summary>
        /// The native API. This field is read-only.
        /// </summary>
        private readonly LocalDbInstanceApi _api;

        /// <summary>
        /// Whether the instance has been disposed of.
        /// </summary>
        private bool _disposed;

        /// <summary>
        /// The available versions of SQL Server LocalDB installed on the local machine.
        /// </summary>
        private string[] _versions;

        /// <summary>
        /// The timeout for stopping an instance of LocalDB.
        /// </summary>
        private TimeSpan _stopTimeout;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbApi"/> class.
        /// </summary>
        public SqlLocalDbApi()
            : this(new SqlLocalDbOptions(), NullLoggerFactory.Instance)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbApi"/> class.
        /// </summary>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="loggerFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="loggerFactory"/> did create the required loggers.
        /// </exception>
        public SqlLocalDbApi(ILoggerFactory loggerFactory)
            : this(new SqlLocalDbOptions(), loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbApi"/> class.
        /// </summary>
        /// <param name="options">The <see cref="SqlLocalDbOptions"/> to use.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="options"/> or <paramref name="loggerFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="loggerFactory"/> did create the required loggers.
        /// </exception>
        public SqlLocalDbApi(SqlLocalDbOptions options, ILoggerFactory loggerFactory)
            : this(options, new WindowsRegistry(), loggerFactory)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbApi"/> class.
        /// </summary>
        /// <param name="options">The <see cref="SqlLocalDbOptions"/> to use.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="registry">The <see cref="IRegistry"/> to use.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="options"/>, <paramref name="registry"/> or <paramref name="loggerFactory"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <paramref name="loggerFactory"/> did not create the required loggers.
        /// </exception>
        internal SqlLocalDbApi(SqlLocalDbOptions options, IRegistry registry, ILoggerFactory loggerFactory)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            Logger = loggerFactory.CreateLogger<SqlLocalDbApi>() ?? throw new InvalidOperationException(SRHelper.Format(SR.SqlLocalDbApi_NoLoggerFormat, nameof(SqlLocalDbApi)));
            var apiLogger = loggerFactory.CreateLogger<LocalDbInstanceApi>() ?? throw new InvalidOperationException(SRHelper.Format(SR.SqlLocalDbApi_NoLoggerFormat, nameof(LocalDbInstanceApi)));

            AutomaticallyDeleteInstanceFiles = options.AutomaticallyDeleteInstanceFiles;
            LanguageId = options.LanguageId;
            StopOptions = options.StopOptions;
            StopTimeout = options.StopTimeout;

            _api = new LocalDbInstanceApi(options.NativeApiOverrideVersion, registry, apiLogger);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to automatically delete the
        /// files associated with SQL LocalDB instances when they are deleted.
        /// </summary>
        /// <remarks>
        /// Setting the value of this property affects the behavior of all delete
        /// operations in the current <see cref="AppDomain"/> unless the overloads
        /// of <see cref="DeleteInstance(string, bool)"/> and <see cref="DeleteUserInstances(bool)"/> are
        /// used. The default value is <see langword="false"/>, unless overridden
        /// by the <c>SQLLocalDB:AutomaticallyDeleteInstanceFiles</c> application configuration setting.
        /// </remarks>
        public bool AutomaticallyDeleteInstanceFiles { get; set; }

        /// <summary>
        /// Gets the name of the default SQL LocalDB instance.
        /// </summary>
        public string DefaultInstanceName
        {
            get
            {
                // Force the native API to be loaded so we can determine its version
                if (!IsLocalDBInstalled())
                {
                    // It isn't installed, so don't assume anything
                    return string.Empty;
                }

                if (_api.NativeApiVersion.Major == 11)
                {
                    return DefaultInstanceName2012;
                }

                // Provided Microsoft do not change the default name again this should work for all versions 12.0+
                return DefaultInstanceName2014AndLater;
            }
        }

        /// <summary>
        /// Gets or sets the locale ID (LCID) to use for formatting error messages.
        /// </summary>
        /// <remarks>
        /// The default value for this property is zero, in which case the <c>Win32</c> <c>FormatMessage</c> language
        /// order is used. This property is provided for integrators to specifically override the language used from
        /// the defaults used by the local installed operating system.
        /// </remarks>
        public int LanguageId { get; set; }

        /// <summary>
        /// Gets the version string for the latest installed version of SQL Server LocalDB.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// No versions of SQL Server LocalDB are installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        public string LatestVersion
        {
            get
            {
                EnsurePlatformSupported();

                // Access through property to ensure initialized
                IReadOnlyList<string> versions = Versions;

                if (versions.Count < 1)
                {
                    string message = SRHelper.Format(SR.SqlLocalDbApi_NoVersionsFormat, Environment.MachineName);
                    throw new InvalidOperationException(message);
                }

                // Return the version with the highest number
                return versions
                    .Select((p) => new Version(p))
                    .OrderBy((p) => p)
                    .Last()
                    .ToString();
            }
        }

        /// <summary>
        /// Gets or sets the options to use when stopping instances of SQL LocalDB.
        /// </summary>
        public StopInstanceOptions StopOptions { get; set; }

        /// <summary>
        /// Gets or sets the default timeout to use when
        /// stopping instances of SQL LocalDB.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public TimeSpan StopTimeout
        {
            get
            {
                return _stopTimeout;
            }

            set
            {
                if (value < TimeSpan.Zero)
                {
                    string message = SRHelper.Format(SR.SqlLocalDbApi_TimeoutTooSmallFormat, nameof(value));
                    throw new ArgumentOutOfRangeException(nameof(value), value, message);
                }

                _stopTimeout = value;
            }
        }

        /// <summary>
        /// Gets an <see cref="IList{T}"/> of <see cref="string"/> containing
        /// the available versions of SQL LocalDB.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The installed versions of SQL LocalDB could not be determined.
        /// </exception>
        public IReadOnlyList<string> Versions
        {
            get
            {
                if (!IsWindows)
                {
                    return Array.Empty<string>();
                }

                if (_versions == null)
                {
                    // Use lazy initialization to allow some functionality to be
                    // accessed regardless of whether LocalDB is installed
                    _versions = GetLocalDbVersions();
                }

                return (string[])_versions.Clone();
            }
        }

        /// <inheritdoc />
        ISqlLocalDbApi ISqlLocalDbApiAdapter.LocalDb => this;

        /// <summary>
        /// Gets a value indicating whether the executing platform is Microsoft Windows.
        /// </summary>
        internal static bool IsWindows { get; } = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        /// <summary>
        /// Gets the <see cref="ILoggerFactory"/> to use.
        /// </summary>
        internal ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Gets the <see cref="ILogger"/> to use.
        /// </summary>
        private ILogger Logger { get; }

        /// <summary>
        /// Gets the full path of the directory containing the SQL LocalDB instance files for the current user.
        /// </summary>
        /// <returns>
        /// The full path of the directory containing the SQL LocalDB instance files for the current user.
        /// </returns>
        /// <remarks>
        /// The folder usually used to store SQL LocalDB instance files is <c>&#37;LOCALAPPDATA&#37;\Microsoft\Microsoft SQL Server Local DB\Instances</c>.
        /// </remarks>
        public static string GetInstancesFolderPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft",
                "Microsoft SQL Server Local DB",
                "Instances");
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance.</param>
        /// <returns>
        /// An <see cref="ISqlLocalDbInstanceInfo"/> containing information about the instance that was created.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// No versions of SQL Server LocalDB are installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could
        /// not be stopped or the installed versions of SQL LocalDB could not be determined.
        /// </exception>
        public ISqlLocalDbInstanceInfo CreateInstance(string instanceName) => CreateInstance(instanceName, LatestVersion);

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance.</param>
        /// <param name="version">The version of SQL Server LocalDB to use.</param>
        /// <returns>
        /// An <see cref="ISqlLocalDbInstanceInfo"/> containing information about the instance that was created.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> or <paramref name="version"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> and <paramref name="version"/> could not be created.
        /// </exception>
        public ISqlLocalDbInstanceInfo CreateInstance(string instanceName, string version)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException(nameof(instanceName));
            }

            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            Logger.CreatingInstance(instanceName, version);

            InvokeThrowOnError(
                () => _api.CreateInstance(version, instanceName, ReservedValue),
                EventIds.CreatingInstanceFailed,
                instanceName);

            Logger.CreatedInstance(instanceName, version);

            return GetInstanceInfo(instanceName);
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
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be deleted.
        /// </exception>
        public void DeleteInstance(string instanceName) => DeleteInstance(instanceName, deleteFiles: AutomaticallyDeleteInstanceFiles);

        /// <summary>
        /// Deletes the specified SQL Server LocalDB instance.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to delete.
        /// </param>
        /// <param name="deleteFiles">
        /// Whether to delete the file(s) associated with the SQL LocalDB instance.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be deleted.
        /// </exception>
        public void DeleteInstance(string instanceName, bool deleteFiles)
        {
            DeleteInstanceInternal(instanceName, throwIfNotFound: true, deleteFiles: deleteFiles);
        }

        /// <summary>
        /// Deletes all user instances of SQL LocalDB on the current machine.
        /// </summary>
        /// <returns>
        /// The number of user instances of SQL LocalDB that were deleted.
        /// </returns>
        /// <remarks>
        /// The default instance(s) of any version(s) of SQL LocalDB that are
        /// installed on the local machine are not deleted.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        public int DeleteUserInstances() => DeleteUserInstances(deleteFiles: AutomaticallyDeleteInstanceFiles);

        /// <summary>
        /// Deletes all user instances of SQL LocalDB on the current machine,
        /// optionally also deleting all of the file(s) associated with the
        /// SQL LocalDB user instance(s) from disk.
        /// </summary>
        /// <param name="deleteFiles">
        /// Whether to delete the file(s) associated with the SQL LocalDB user instance(s).
        /// </param>
        /// <returns>
        /// The number of user instances of SQL LocalDB that were deleted.
        /// </returns>
        /// <remarks>
        /// The default instance(s) of any version(s) of SQL LocalDB that are
        /// installed on the local machine are not deleted.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        public int DeleteUserInstances(bool deleteFiles)
        {
            int instancesDeleted = 0;

            IReadOnlyList<string> instanceNames = GetInstanceNames();

            foreach (string instanceName in instanceNames)
            {
                ISqlLocalDbInstanceInfo info = GetInstanceInfo(instanceName);

                // Do not try to delete automatic instances.
                // These are the default instances created for each version.
                // As of SQL LocalDB 2014, the default instance is named 'MSSQLLocalDB'.
                if (!info.Exists ||
                    info.IsAutomatic ||
                    string.Equals(info.Name, DefaultInstanceName2014AndLater, StringComparison.Ordinal))
                {
                    continue;
                }

                // In some cases, SQL LocalDB may report instance names in calls
                // to enumerate the instances that do not actually exist. Presumably
                // this can occur if the installation/instances become corrupted.
                // Such failures to delete an instance should be ignored
                try
                {
                    if (DeleteInstanceInternal(instanceName, throwIfNotFound: false, deleteFiles: deleteFiles))
                    {
                        instancesDeleted++;
                    }
                }
                catch (SqlLocalDbException ex)
                {
                    if (ex.ErrorCode == SqlLocalDbErrors.InstanceBusy)
                    {
                        Logger.DeletingInstanceFailedAsInUse(ex, ex.InstanceName);
                        continue;
                    }

                    throw;
                }
            }

            return instancesDeleted;
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
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The information for the SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not obtained.
        /// </exception>
        public ISqlLocalDbInstanceInfo GetInstanceInfo(string instanceName)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException(nameof(instanceName));
            }

            Logger.GettingInstanceInfo(instanceName);

            LocalDbInstanceInfo info;

            int size = LocalDbInstanceInfo.MarshalSize;
            IntPtr ptrInfo = Marshal.AllocHGlobal(size);

            try
            {
                InvokeThrowOnError(
                    () => _api.GetInstanceInfo(instanceName, ptrInfo, size),
                    EventIds.GettingInstanceInfoFailed,
                    instanceName);

                info = MarshalStruct<LocalDbInstanceInfo>(ptrInfo);
            }
            finally
            {
                Marshal.FreeHGlobal(ptrInfo);
            }

            Logger.GotInstanceInfo(instanceName);

            var result = new SqlLocalDbInstanceInfo(this);

            result.Update(info);

            return result;
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
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance names could not be determined.
        /// </exception>
        public IReadOnlyList<string> GetInstanceNames()
        {
            EnsurePlatformSupported();

            Logger.GettingInstanceNames();

            // Query the LocalDB API to get the number of instances
            int count = 0;
            int hr = _api.GetInstanceNames(IntPtr.Zero, ref count);

            if (hr != 0 &&
                hr != SqlLocalDbErrors.InsufficientBuffer)
            {
                throw GetLocalDbError(hr, EventIds.GettingInstanceNamesFailed);
            }

            string[] names;

            // Allocate enough memory to receive the instance name array
            int nameLength = MaxInstanceNameLength;
            IntPtr ptrNames = Marshal.AllocHGlobal(nameLength * count);

            try
            {
                InvokeThrowOnError(
                    () => _api.GetInstanceNames(ptrNames, ref count),
                    EventIds.GettingInstanceNamesFailed);

                // Read the instance names back from unmanaged memory
                names = MarshalStringArray(ptrNames, nameLength, count);
            }
            finally
            {
                Marshal.FreeHGlobal(ptrNames);
            }

            Logger.GotInstanceNames(names.Length);

            return names;
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
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The information for the SQL Server LocalDB version specified by <paramref name="version"/> could not be obtained.
        /// </exception>
        public ISqlLocalDbVersionInfo GetVersionInfo(string version)
        {
            if (version == null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            Logger.GettingVersionInfo(version);

            LocalDbVersionInfo info;

            int size = LocalDbVersionInfo.MarshalSize;
            IntPtr ptrInfo = Marshal.AllocHGlobal(size);

            try
            {
                InvokeThrowOnError(
                    () => _api.GetVersionInfo(version, ptrInfo, size),
                    EventIds.GettingVersionInfoFailed);

                info = MarshalStruct<LocalDbVersionInfo>(ptrInfo);
            }
            finally
            {
                Marshal.FreeHGlobal(ptrInfo);
            }

            Logger.GotVersionInfo(version);

            var result = new SqlLocalDbVersionInfo();

            result.Update(info);

            return result;
        }

        /// <summary>
        /// Returns whether the specified LocalDB instance exists.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance to check for existence.</param>
        /// <returns>
        /// <see langword="true"/> if the LocalDB instance specified by
        /// <paramref name="instanceName"/> exists; otherwise <see langword="false"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// Whether the SQL Server LocalDB instance specified by <paramref name="instanceName"/> exists could not be determined.
        /// </exception>
        public bool InstanceExists(string instanceName)
        {
            try
            {
                return GetInstanceInfo(instanceName).Exists;
            }
            catch (SqlLocalDbException ex) when (ex.ErrorCode == SqlLocalDbErrors.UnknownInstance)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns whether SQL LocalDB is installed on the current machine.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if SQL Server LocalDB is installed on the
        /// current machine; otherwise <see langword="false"/>.
        /// </returns>
        public bool IsLocalDBInstalled()
        {
            // SQL LocalDB is only supported on Windows, so shortcut
            if (!IsWindows)
            {
                return false;
            }

            // Call one of the "get info" functions with a zero buffer.
            // If LocalDB is installed, it will return a "buffer too small" HRESULT,
            // otherwise it will return the "not installed" HRESULT.
            int notUsed = 0;
            int hr = _api.GetVersions(IntPtr.Zero, ref notUsed);
            return hr != SqlLocalDbErrors.NotInstalled;
        }

        /// <summary>
        /// Shares the specified SQL Server LocalDB instance with other
        /// users of the computer, using the specified shared name.
        /// </summary>
        /// <param name="ownerSid">The SID of the instance owner.</param>
        /// <param name="instanceName">The private name for the LocalDB instance to share.</param>
        /// <param name="sharedInstanceName">The shared name for the LocalDB instance to share as.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ownerSid"/>, <paramref name="instanceName"/> or <paramref name="sharedInstanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be shared.
        /// </exception>
        public void ShareInstance(
            string ownerSid,
            string instanceName,
            string sharedInstanceName)
        {
            if (ownerSid == null)
            {
                throw new ArgumentNullException(nameof(ownerSid));
            }

            if (instanceName == null)
            {
                throw new ArgumentNullException(nameof(instanceName));
            }

            if (sharedInstanceName == null)
            {
                throw new ArgumentNullException(nameof(sharedInstanceName));
            }

            if (string.IsNullOrEmpty(instanceName))
            {
                // There is a bug in the SQL LocalDB native API that
                // lets you share out an instance with no name, which
                // then causes "interesting" results when using the other
                // API functions on it.  Block this explicitly to stop
                // callers messing up their LocalDB instance.
                throw new ArgumentException(SR.SqlLocalDbApi_NoInstanceName, nameof(instanceName));
            }

            Logger.SharingInstance(instanceName, ownerSid, sharedInstanceName);

            // Get the binary version of the SID from its string
            byte[] binaryForm = GetOwnerSidAsByteArray(ownerSid);

            IntPtr ptrSid = Marshal.AllocHGlobal(binaryForm.Length);

            try
            {
                // Copy the SID binary form to unmanaged memory
                Marshal.Copy(binaryForm, 0, ptrSid, binaryForm.Length);

                InvokeThrowOnError(
                    () => _api.ShareInstance(ptrSid, instanceName, sharedInstanceName, ReservedValue),
                    EventIds.SharingInstanceFailed,
                    instanceName);

                Logger.SharedInstance(instanceName, ownerSid, sharedInstanceName);
            }
            finally
            {
                Marshal.FreeHGlobal(ptrSid);
            }
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
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be started.
        /// </exception>
        public string StartInstance(string instanceName)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException(nameof(instanceName));
            }

            Logger.StartingInstance(instanceName);

            int size = LocalDbInstanceApi.MaximumSqlConnectionStringBufferLength + 1;
            var buffer = new StringBuilder(size);

            InvokeThrowOnError(
                () => _api.StartInstance(instanceName, ReservedValue, buffer, ref size),
                EventIds.StartingInstanceFailed,
                instanceName);

            string namedPipe = buffer.ToString();

            Logger.StartedInstance(instanceName, namedPipe);

            return namedPipe;
        }

        /// <summary>
        /// Enables tracing of native API calls for all the SQL Server
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// Tracing could not be initialized.
        /// </exception>
        public void StartTracing()
        {
            Logger.StartingTracing();

            InvokeThrowOnError(_api.StartTracing, EventIds.StartingTracingFailed);

            Logger.StartedTracing();
        }

        /// <summary>
        /// Stops the specified instance of SQL Server LocalDB using
        /// the timeout specified by <see cref="StopTimeout"/>.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to stop.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be stopped.
        /// </exception>
        public void StopInstance(string instanceName)
        {
            StopInstance(instanceName, _stopTimeout);
        }

        /// <summary>
        /// Stops the specified instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to stop.
        /// </param>
        /// <param name="timeout">
        /// The optional amount of time to give the LocalDB instance to stop.
        /// If no value is specified, the value of <see cref="StopTimeout"/> will
        /// be used. If the value is <see cref="TimeSpan.Zero"/>, the method will
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
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be stopped.
        /// </exception>
        /// <remarks>
        /// The <paramref name="timeout"/> parameter is rounded to the nearest second.
        /// </remarks>
        public void StopInstance(string instanceName, TimeSpan? timeout)
        {
            StopInstance(instanceName, StopOptions, timeout);
        }

        /// <summary>
        /// Stops the specified instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to stop.
        /// </param>
        /// <param name="options">
        /// The options specifying the way to stop the instance.
        /// </param>
        /// <param name="timeout">
        /// The amount of time to give the LocalDB instance to stop.
        /// If no value is specified, the value of <see cref="StopTimeout"/> will
        /// be used. If the value is <see cref="TimeSpan.Zero"/>, the method will
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
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be stopped.
        /// </exception>
        /// <remarks>
        /// The <paramref name="timeout"/> parameter is rounded to the nearest second.
        /// </remarks>
        public void StopInstance(string instanceName, StopInstanceOptions options, TimeSpan? timeout)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException(nameof(instanceName));
            }

            TimeSpan theTimeout = timeout ?? StopTimeout;

            if (theTimeout < TimeSpan.Zero)
            {
                string message = SRHelper.Format(SR.SqlLocalDbApi_TimeoutTooSmallFormat, nameof(timeout));
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, message);
            }

            Logger.StoppingInstance(instanceName, theTimeout, options);
            Stopwatch stopwatch = Stopwatch.StartNew();

            InvokeThrowOnError(
                () => _api.StopInstance(instanceName, options, (int)theTimeout.TotalSeconds),
                EventIds.StoppingInstanceFailed,
                instanceName);

            stopwatch.Stop();

            Logger.StoppedInstance(instanceName, stopwatch.Elapsed);
        }

        /// <summary>
        /// Disables tracing of native API calls for all the SQL Server
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// Tracing could not be disabled.
        /// </exception>
        public void StopTracing()
        {
            Logger.StoppingTracing();

            InvokeThrowOnError(_api.StopTracing, EventIds.StoppingTracingFailed);

            Logger.StoppedTracing();
        }

        /// <summary>
        /// Stops the sharing of the specified SQL Server LocalDB instance.
        /// </summary>
        /// <param name="instanceName">
        /// The private name for the LocalDB instance to stop sharing.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be unshared.
        /// </exception>
        public void UnshareInstance(string instanceName)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException(nameof(instanceName));
            }

            Logger.UnsharingInstance(instanceName);

            InvokeThrowOnError(
                () => _api.UnshareInstance(instanceName, ReservedValue),
                EventIds.UnsharingInstanceFailed,
                instanceName);

            Logger.UnsharedInstance(instanceName);
        }

        /// <summary>
        /// Throws an exception if the SQL LocalDB Instance API is not supported by the current platform.
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        internal static void EnsurePlatformSupported()
        {
            if (!IsWindows)
            {
                throw new PlatformNotSupportedException(SR.SqlLocalDbApi_PlatformNotSupported);
            }
        }

        /// <summary>
        /// Returns whether the specified SQL LocalDB instance name is one of the default instance names.
        /// </summary>
        /// <param name="instanceName">The instance name to test.</param>
        /// <returns>
        /// <see langword="true"/> if <paramref name="instanceName"/> is one of the default SQL LocalDB
        /// instance names; otherwise <see langword="false"/>.
        /// </returns>
        internal static bool IsDefaultInstanceName(string instanceName)
        {
            return
                string.Equals(instanceName, DefaultInstanceName2014AndLater, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(instanceName, DefaultInstanceName2012, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Deletes the specified SQL Server LocalDB instance.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to delete.
        /// </param>
        /// <param name="throwIfNotFound">
        /// Whether to throw an exception if the SQL LocalDB instance
        /// specified by <paramref name="instanceName"/> cannot be found.
        /// </param>
        /// <param name="deleteFiles">
        /// Whether to delete the file(s) associated with the SQL LocalDB instance.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the instance specified by <paramref name="instanceName"/>
        /// was successfully deleted; <see langword="false"/> if <paramref name="throwIfNotFound"/>
        /// is <see langword="false"/> and the instance did not exist.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be deleted.
        /// </exception>
        internal bool DeleteInstanceInternal(string instanceName, bool throwIfNotFound, bool deleteFiles = false)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException(nameof(instanceName));
            }

            EnsurePlatformSupported();

            Logger.DeletingInstance(instanceName);

            int hr = _api.DeleteInstance(instanceName, ReservedValue);

            if (hr != 0)
            {
                if (!throwIfNotFound && hr == SqlLocalDbErrors.UnknownInstance)
                {
                    Logger.DeletingInstanceFailedAsNotFound(instanceName);
                    return false;
                }

                throw GetLocalDbError(hr, EventIds.DeletingInstanceFailed, instanceName);
            }

            if (deleteFiles)
            {
                DeleteInstanceFiles(instanceName);
            }

            Logger.DeletedInstance(instanceName);
            return true;
        }

        /// <summary>
        /// Deletes the file(s) from disk that are associated with the specified SQL LocalDB instance.
        /// </summary>
        /// <param name="instanceName">The name of the SQL LocalDB instance to delete the file(s) for.</param>
        internal void DeleteInstanceFiles(string instanceName)
        {
            Debug.Assert(instanceName != null, "instanceName cannot be null.");

            // Sanitize the instance name's path to prevent directory traversal
            string instanceNameSafe = Path.GetFullPath(instanceName);
            instanceNameSafe = Path.GetFileName(instanceNameSafe);

            string instancePath = Path.Combine(GetInstancesFolderPath(), instanceNameSafe);

            try
            {
                if (Directory.Exists(instancePath))
                {
                    Logger.DeletingInstanceFiles(instanceName, instancePath);

                    Directory.Delete(instancePath, recursive: true);

                    Logger.DeletedInstanceFiles(instanceName, instancePath);
                }
            }
            catch (Exception ex) when (ex is ArgumentException || ex is IOException || ex is UnauthorizedAccessException)
            {
                Logger.DeletingInstanceFilesFailed(instanceName, instancePath);
            }
        }

        /// <summary>
        /// Returns an <see cref="Exception"/> representing the specified LocalDB HRESULT.
        /// </summary>
        /// <param name="hr">The HRESULT returned by the LocalDB API.</param>
        /// <param name="eventId">The trace event Id associated with the error.</param>
        /// <param name="instanceName">The name of the instance that caused the error, if any.</param>
        /// <returns>
        /// An <see cref="Exception"/> representing <paramref name="hr"/>.
        /// </returns>
        internal Exception GetLocalDbError(int hr, EventId eventId, string instanceName = "")
        {
            string message;

            Logger.LogError(eventId, SR.SqlLocalDbApi_NativeResultFormat, hr.ToString("X", CultureInfo.InvariantCulture));

            if (hr == SqlLocalDbErrors.NotInstalled)
            {
                Logger.NotInstalled();
                throw new InvalidOperationException(SRHelper.Format(SR.SqlLocalDbApi_NotInstalledFormat, Environment.MachineName));
            }

            int size = LocalDbInstanceApi.MaximumSqlConnectionStringBufferLength + 1;
            var buffer = new StringBuilder(size);

            // Get the description of the error from the LocalDB API.
            int hr2 = _api.GetLocalDbError(
                hr,
                LanguageId,
                buffer,
                ref size);

            if (hr2 == 0)
            {
                message = buffer.ToString();
                Logger.LogError(eventId, message);
            }
            else if (hr2 == SqlLocalDbErrors.UnknownLanguageId)
            {
                Logger.LogError(
                    eventId,
                    SR.SqlLocalDbApi_LogGenericFailureFormat,
                    hr2.ToString("X", CultureInfo.InvariantCulture));

                // If the value of DefaultLanguageId was not understood by the API,
                // then log an error informing the user. Do not throw an exception in
                // this case as otherwise we will mask the original exception from the user.
                Logger.InvalidLanguageId(LanguageId);

                // Use a generic message if getting the message from the API failed
                message = SRHelper.Format(SR.SqlLocalDbApi_GenericFailureFormat, hr);
            }
            else
            {
                Logger.LogError(
                    eventId,
                    SR.SqlLocalDbApi_LogGenericFailureFormat,
                    hr2.ToString("X", CultureInfo.InvariantCulture));

                // Use a generic message if getting the message from the API failed.
                // N.B. That if this occurs, then the original error is masked (although it is logged).
                message = SRHelper.Format(SR.SqlLocalDbApi_GenericFailureFormat, hr2);

                return new SqlLocalDbException(message, hr2, instanceName);
            }

            return new SqlLocalDbException(message, hr, instanceName);
        }

        /// <summary>
        /// Converts a <see cref="string"/> representation of a SID to an array of bytes.
        /// </summary>
        /// <param name="ownerSid">The SID to convert to a byte array.</param>
        /// <returns>
        /// An <see cref="Array"/> of <see cref="byte"/> containing a representation of <paramref name="ownerSid"/>.
        /// </returns>
        private static byte[] GetOwnerSidAsByteArray(string ownerSid)
        {
            // Get the binary version of the SID from its string
            SecurityIdentifier sid = new SecurityIdentifier(ownerSid);
            byte[] binaryForm = new byte[SecurityIdentifier.MaxBinaryLength];
            sid.GetBinaryForm(binaryForm, 0);
            return binaryForm;
        }

        /// <summary>
        /// Convenience method to marshal string arrays from unmanaged memory.
        /// </summary>
        /// <param name="ptr">A pointer to an unmanaged block of memory.</param>
        /// <param name="length">The length of each string in the array.</param>
        /// <param name="count">The number of elements in the array.</param>
        /// <returns>
        /// An <see cref="Array"/> of strings read from <paramref name="ptr"/>.
        /// </returns>
        private static string[] MarshalStringArray(IntPtr ptr, int length, int count)
        {
            Debug.Assert(ptr != IntPtr.Zero, "The unmanaged memory pointer is invalid.");
            Debug.Assert(length > 0, "The length of the elements cannot be less than one.");
            Debug.Assert(count > -1, "The number of elements in the array cannot be negative.");

            string[] result = new string[count];

            for (int i = 0; i < result.Length; i++)
            {
                // Determine the offset of the element, and get the string from the array
                IntPtr offset = new IntPtr(ptr.ToInt64() + (length * i));
                result[i] = Marshal.PtrToStringAuto(offset);
            }

            return result;
        }

        /// <summary>
        /// Convenience method to marshal structures from unmanaged memory.
        /// </summary>
        /// <typeparam name="T">The type of structure to marshal from unmanaged memory.</typeparam>
        /// <param name="ptr">A pointer to an unmanaged block of memory.</param>
        /// <returns>
        /// The instance of <typeparamref name="T"/> read from <paramref name="ptr"/>.
        /// </returns>
        private static T MarshalStruct<T>(IntPtr ptr)
            where T : struct
        {
            return (T)Marshal.PtrToStructure(ptr, typeof(T));
        }

        /// <summary>
        /// Gets the available versions of SQL LocalDB installed on the local machine.
        /// </summary>
        /// <returns>
        /// An <see cref="Array"/> of <see cref="string"/> containing the available versions.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The installed LocalDB versions could not be enumerated.
        /// </exception>
        private string[] GetLocalDbVersions()
        {
            EnsurePlatformSupported();

            string[] versions;

            try
            {
                Logger.GettingVersions();

                // Query the LocalDB API to get the number of instances
                int count = 0;
                int hr = _api.GetVersions(IntPtr.Zero, ref count);

                if (hr != 0 &&
                    hr != SqlLocalDbErrors.InsufficientBuffer)
                {
                    throw GetLocalDbError(hr, EventIds.GettingVersionsFailed);
                }

                // Allocate enough memory to receive the version name array
                int versionLength = MaxVersionLength;
                IntPtr ptrVersions = Marshal.AllocHGlobal(versionLength * count);

                try
                {
                    hr = _api.GetVersions(ptrVersions, ref count);

                    if (hr != 0)
                    {
                        throw GetLocalDbError(hr, EventIds.GettingVersionsFailed);
                    }

                    // Read the version strings back from unmanaged memory
                    versions = MarshalStringArray(ptrVersions, versionLength, count);
                }
                finally
                {
                    Marshal.FreeHGlobal(ptrVersions);
                }
            }
            catch (SqlLocalDbException ex)
            {
                throw new SqlLocalDbException(
                    SR.SqlLocalDbApi_VersionEnumerationFailed,
                    ex.ErrorCode,
                    ex.InstanceName,
                    ex);
            }

            Logger.GotVersions(versions.Length);

            return versions;
        }

        /// <summary>
        /// Invokes the specified delegate, throwing an exception if the delegate does not return zero.
        /// </summary>
        /// <param name="func">A delegate to a method to invoke.</param>
        /// <param name="eventId">The trace event Id if a non-zero value is returned.</param>
        /// <param name="instanceName">The name of the instance that caused the error, if any.</param>
        /// <exception cref="PlatformNotSupportedException">
        /// The method is called from a non-Windows operating system.
        /// </exception>
        private void InvokeThrowOnError(Func<int> func, EventId eventId, string instanceName = "")
        {
            EnsurePlatformSupported();

            int hr = func();

            if (hr != 0)
            {
                throw GetLocalDbError(hr, eventId, instanceName);
            }
        }

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
                    _api.Dispose();
                }

                _disposed = true;
            }
        }
    }
}
