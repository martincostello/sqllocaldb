// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbApi.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbApi.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class representing a wrapper to the SQL Server LocalDB native API.
    /// This class cannot be inherited.
    /// </summary>
    public static class SqlLocalDbApi
    {
        #region Constants and Fields

        /// <summary>
        /// The maximum length of an SQL LocalDB instance name, in bytes.
        /// </summary>
        private const int MaxInstanceNameLength = (NativeMethods.MAX_LOCALDB_INSTANCE_NAME_LENGTH + 1) * sizeof(char);

        /// <summary>
        /// The maximum length of an SQL LocalDB version string, in bytes.
        /// </summary>
        private const int MaxVersionLength = (NativeMethods.MAX_LOCALDB_VERSION_LENGTH + 1) * sizeof(char);

        /// <summary>
        /// The available versions of SQL Server LocalDB installed on the local machine.
        /// </summary>
        private static string[] _versions;

        /// <summary>
        /// Whether to automatically delete the files associated with SQL LocalDB instances when they are deleted.
        /// </summary>
        private static bool _automaticallyDeleteInstanceFiles = SqlLocalDbConfig.AutomaticallyDeleteInstanceFiles;

        /// <summary>
        /// The timeout for stopping an instance of LocalDB.
        /// </summary>
        private static TimeSpan _stopTimeout = TimeSpan.FromMinutes(1);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether to automatically delete the
        /// files associated with SQL LocalDB instances when they are deleted.
        /// </summary>
        /// <remarks>
        /// Setting the value of this property affects the behavior of all delete
        /// operations in the current <see cref="AppDomain"/> unless the overloads
        /// of <see cref="DeleteInstance(String, Boolean)"/> and <see cref="DeleteUserInstances(Boolean)"/> are
        /// used. The default value is <see langword="false"/>, unless overridden
        /// by the <c>SQLLocalDB:AutomaticallyDeleteInstanceFiles</c> application configuration setting.
        /// </remarks>
        public static bool AutomaticallyDeleteInstanceFiles
        {
            get { return _automaticallyDeleteInstanceFiles; }
            set { _automaticallyDeleteInstanceFiles = value; }
        }

        /// <summary>
        /// Gets the version string for the latest installed version of SQL Server LocalDB.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// No versions of SQL Server LocalDB are installed on the local machine.
        /// </exception>
        public static string LatestVersion
        {
            get
            {
                // Access through property to ensure initialized
                IList<string> versions = Versions;

                if (versions.Count < 1)
                {
                    string message = SRHelper.Format(
                        SR.SqlLocalDbApi_NoVersionsFormat,
                        Environment.MachineName);

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
        public static StopInstanceOptions StopOptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the default timeout to use when
        /// stopping instances of SQL LocalDB.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="value"/> is less than <see cref="TimeSpan.Zero"/>.
        /// </exception>
        public static TimeSpan StopTimeout
        {
            get
            {
                return _stopTimeout;
            }

            set
            {
                if (value < TimeSpan.Zero)
                {
                    string message = SRHelper.Format(
                        SR.SqlLocalDbApi_TimeoutTooSmallFormat,
                        "value");

                    throw new ArgumentOutOfRangeException("value", value, message);
                }

                _stopTimeout = value;
            }
        }

        /// <summary>
        /// Gets an <see cref="Array"/> of <see cref="String"/> containing
        /// the available versions of SQL LocalDB.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The installed versions of SQL LocalDB could not be determined.
        /// </exception>
        public static IList<string> Versions
        {
            get
            {
                if (_versions == null)
                {
                    // Use lazy initialization to allow some functionality to be
                    // accessed regardless of whether LocalDB is installed
                    _versions = GetLocalDbVersions();
                }

                return (string[])_versions.Clone();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// No versions of SQL Server LocalDB are installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could
        /// not be stopped or the installed versions of SQL LocalDB could not be determined.
        /// </exception>
        public static void CreateInstance(string instanceName)
        {
            // Use the latest version
            CreateInstance(instanceName, LatestVersion);
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
        public static void CreateInstance(string instanceName, string version)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            Logger.Verbose(Logger.TraceEvent.CreateInstance, SR.SqlLocalDbApi_LogCreatingFormat, instanceName, version);

            InvokeThrowOnError(
                () => NativeMethods.CreateInstance(version, instanceName),
                Logger.TraceEvent.CreateInstance,
                instanceName);

            Logger.Verbose(Logger.TraceEvent.CreateInstance, SR.SqlLocalDbApi_LogCreatedFormat, instanceName, version);
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
        public static void DeleteInstance(string instanceName)
        {
            DeleteInstance(instanceName, deleteFiles: AutomaticallyDeleteInstanceFiles);
        }

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
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be deleted.
        /// </exception>
        public static void DeleteInstance(string instanceName, bool deleteFiles)
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
        public static int DeleteUserInstances()
        {
            return DeleteUserInstances(deleteFiles: AutomaticallyDeleteInstanceFiles);
        }

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
        public static int DeleteUserInstances(bool deleteFiles)
        {
            int instancesDeleted = 0;

            IList<string> instanceNames = GetInstanceNames();

            if (instanceNames != null)
            {
                foreach (string instanceName in instanceNames)
                {
                    ISqlLocalDbInstanceInfo info = GetInstanceInfo(instanceName);

                    // Do not try to delete automatic instances.
                    // These are the default instances created for each version.
                    // As of SQL LocalDB 2014, the default instance is named 'MSSQLLocalDB'.
                    if (!info.Exists ||
                        info.IsAutomatic ||
                        string.Equals(info.Name, "MSSQLLocalDB", StringComparison.Ordinal))
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
                            Logger.Warning(
                                Logger.TraceEvent.DeleteFailedAsInstanceInUse,
                                SR.SqlLocalDbApi_LogDeleteFailedAsInUseFormat,
                                ex.InstanceName);

                            continue;
                        }

                        throw;
                    }
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
        /// <exception cref="SqlLocalDbException">
        /// The information for the SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not obtained.
        /// </exception>
        public static ISqlLocalDbInstanceInfo GetInstanceInfo(string instanceName)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            Logger.Verbose(Logger.TraceEvent.GetInstanceInfo, SR.SqlLocalDbApi_LogGettingInfoFormat, instanceName);

            int size = LocalDbInstanceInfo.MarshalSize;
            IntPtr ptrInfo = Marshal.AllocHGlobal(size);

            try
            {
                InvokeThrowOnError(
                    () => NativeMethods.GetInstanceInfo(instanceName, ptrInfo, size),
                    Logger.TraceEvent.GetInstanceInfo,
                    instanceName);

                LocalDbInstanceInfo info = MarshalStruct<LocalDbInstanceInfo>(ptrInfo);

                Logger.Verbose(Logger.TraceEvent.GetInstanceInfo, SR.SqlLocalDbApi_LogGotInfoFormat, instanceName);

                return info;
            }
            finally
            {
                Marshal.FreeHGlobal(ptrInfo);
            }
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
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Requires enumeration of native API and allocating unmanaged memory.")]
        public static IList<string> GetInstanceNames()
        {
            Logger.Verbose(Logger.TraceEvent.GetInstanceNames, SR.SqlLocalDbApi_LogGetInstances);

            // Query the LocalDB API to get the number of instances
            int count = 0;
            int hr = NativeMethods.GetInstanceNames(IntPtr.Zero, ref count);

            if (hr != 0 &&
                hr != SqlLocalDbErrors.InsufficientBuffer)
            {
                throw GetLocalDbError(hr, Logger.TraceEvent.GetInstanceNames);
            }

            if (count == 0)
            {
                // Shortcut, no instances
                return new string[0];
            }

            // Allocate enough memory to receive the instance name array
            int nameLength = MaxInstanceNameLength;
            IntPtr ptrNames = Marshal.AllocHGlobal(nameLength * count);

            try
            {
                InvokeThrowOnError(
                    () => NativeMethods.GetInstanceNames(ptrNames, ref count),
                    Logger.TraceEvent.GetInstanceNames);

                // Read the instance names back from unmanaged memory
                string[] names = MarshalStringArray(ptrNames, nameLength, count);

                Logger.Verbose(Logger.TraceEvent.GetInstanceNames, SR.SqlLocalDbApi_LogGotInstancesFormat, names.Length);

                return names;
            }
            finally
            {
                Marshal.FreeHGlobal(ptrNames);
            }
        }

        /// <summary>
        /// Gets the full path of the directory containing the SQL LocalDB instance files for the current user.
        /// </summary>
        /// <returns>
        /// The full path of the directory containing the SQL LocalDB instance files for the current user.
        /// </returns>
        /// <remarks>
        /// The folder usually used to store SQL LocalDB instance files is <c>&#37;LOCALAPPDATA&#37;\Microsoft\Microsoft SQL Server Local DB\Instances</c>.
        /// </remarks>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Calls a number of methods so is more appropriate as a method.")]
        public static string GetInstancesFolderPath()
        {
            return PathCombine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft",
                "Microsoft SQL Server Local DB",
                "Instances");
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
        /// The information for the SQL Server LocalDB version specified by <paramref name="version"/> could not be obtained.
        /// </exception>
        public static ISqlLocalDbVersionInfo GetVersionInfo(string version)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            Logger.Verbose(Logger.TraceEvent.GetVersionInfo, SR.SqlLocalDbApi_LogGetVersionInfoFormat, version);

            int size = LocalDbVersionInfo.MarshalSize;
            IntPtr ptrInfo = Marshal.AllocHGlobal(size);

            try
            {
                InvokeThrowOnError(
                    () => NativeMethods.GetVersionInfo(version, ptrInfo, size),
                    Logger.TraceEvent.GetVersionInfo);

                LocalDbVersionInfo info = MarshalStruct<LocalDbVersionInfo>(ptrInfo);

                Logger.Verbose(Logger.TraceEvent.GetVersionInfo, SR.SqlLocalDbApi_LogGotVersionInfoFormat, version);

                return info;
            }
            finally
            {
                Marshal.FreeHGlobal(ptrInfo);
            }
        }

        /// <summary>
        /// Returns whether SQL LocalDB is installed on the current machine.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if SQL Server LocalDB is installed on the
        /// current machine; otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsLocalDBInstalled()
        {
            // Call one of the "get info" functions with a zero buffer.
            // If LocalDB is installed, it will return a "buffer too small" HRESULT,
            // otherwise it will return the "not installed" HRESULT.
            int dummy = 0;
            int hr = NativeMethods.GetVersions(IntPtr.Zero, ref dummy);
            return hr != SqlLocalDbErrors.NotInstalled;
        }

        /// <summary>
        /// Shares the specified SQL Server LocalDB instance with other users of the computer,
        /// using the specified shared name for the current Windows user.
        /// </summary>
        /// <param name="instanceName">The private name for the LocalDB instance to share.</param>
        /// <param name="sharedInstanceName">The shared name for the LocalDB instance to share.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="instanceName"/> or <paramref name="sharedInstanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be shared.
        /// </exception>
        public static void ShareInstance(
            string instanceName,
            string sharedInstanceName)
        {
            string ownerSid;

            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                ownerSid = identity.User.Value;                
            }

            ShareInstance(ownerSid, instanceName, sharedInstanceName);
        }

        /// <summary>
        /// Shares the specified SQL Server LocalDB instance with other
        /// users of the computer, using the specified shared name.
        /// </summary>
        /// <param name="ownerSid">The SID of the instance owner.</param>
        /// <param name="instanceName">The private name for the LocalDB instance to share.</param>
        /// <param name="sharedInstanceName">The shared name for the LocalDB instance to share.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="ownerSid"/>, <paramref name="instanceName"/> or <paramref name="sharedInstanceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be shared.
        /// </exception>
        public static void ShareInstance(
            string ownerSid,
            string instanceName,
            string sharedInstanceName)
        {
            if (ownerSid == null)
            {
                throw new ArgumentNullException("ownerSid");
            }

            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            if (sharedInstanceName == null)
            {
                throw new ArgumentNullException("sharedInstanceName");
            }

            if (string.IsNullOrEmpty(instanceName))
            {
                // There is a bug in the SQL LocalDB native API that
                // lets you share out an instance with no name, which
                // then causes "interesting" results when using the other
                // API functions on it.  Block this explicitly to stop
                // callers messing up their LocalDB instance.
                throw new ArgumentException(SR.SqlLocalDbApi_NoInstanceName, "instanceName");
            }

            Logger.Verbose(Logger.TraceEvent.ShareInstance, SR.SqlLocalDbApi_LogSharingInstanceFormat, instanceName, ownerSid, sharedInstanceName);

            // Get the binary version of the SID from its string
            SecurityIdentifier sid = new SecurityIdentifier(ownerSid);
            byte[] binaryForm = new byte[SecurityIdentifier.MaxBinaryLength];
            sid.GetBinaryForm(binaryForm, 0);

            IntPtr ptrSid = Marshal.AllocHGlobal(binaryForm.Length);

            try
            {
                // Copy the SID binary form to unmanaged memory
                Marshal.Copy(binaryForm, 0, ptrSid, binaryForm.Length);

                InvokeThrowOnError(
                    () => NativeMethods.ShareInstance(ptrSid, instanceName, sharedInstanceName),
                    Logger.TraceEvent.ShareInstance,
                    instanceName);

                Logger.Verbose(Logger.TraceEvent.ShareInstance, SR.SqlLocalDbApi_LogSharedInstanceFormat, instanceName, ownerSid, sharedInstanceName);
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
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be started.
        /// </exception>
        public static string StartInstance(string instanceName)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            Logger.Verbose(Logger.TraceEvent.StartInstance, SR.SqlLocalDbApi_LogStartingFormat, instanceName);

            StringBuilder buffer = new StringBuilder(NativeMethods.LOCALDB_MAX_SQLCONNECTION_BUFFER_SIZE + 1);
            int size = buffer.Capacity;

            InvokeThrowOnError(
                () => NativeMethods.StartInstance(instanceName, buffer, ref size),
                Logger.TraceEvent.StartInstance,
                instanceName);

            string namedPipe = buffer.ToString();

            Logger.Verbose(Logger.TraceEvent.StartInstance, SR.SqlLocalDbApi_LogStartedFormat, instanceName, namedPipe);

            return namedPipe;
        }

        /// <summary>
        /// Enables tracing of native API calls for all the SQL Server
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// Tracing could not be initialized.
        /// </exception>
        public static void StartTracing()
        {
            Logger.Verbose(Logger.TraceEvent.StartTracing, SR.SqlLocalDbApi_LogStartTracing);

            InvokeThrowOnError(NativeMethods.StartTracing, Logger.TraceEvent.StartTracing);

            Logger.Verbose(Logger.TraceEvent.StartTracing, SR.SqlLocalDbApi_LogStartedTracing);
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
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be stopped.
        /// </exception>
        public static void StopInstance(string instanceName)
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
        public static void StopInstance(string instanceName, TimeSpan timeout)
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
        public static void StopInstance(string instanceName, StopInstanceOptions options, TimeSpan timeout)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            if (timeout < TimeSpan.Zero)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbApi_TimeoutTooSmallFormat,
                    "timeout");

                throw new ArgumentOutOfRangeException("timeout", timeout, message);
            }

            Logger.Verbose(Logger.TraceEvent.StopInstance, SR.SqlLocalDbApi_LogStoppingFormat, instanceName, timeout, options);
            Stopwatch stopwatch = Stopwatch.StartNew();

            InvokeThrowOnError(
                () => NativeMethods.StopInstance(instanceName, options, (int)timeout.TotalSeconds),
                Logger.TraceEvent.StopInstance,
                instanceName);

            stopwatch.Stop();

            Logger.Verbose(Logger.TraceEvent.StopInstance, SR.SqlLocalDbApi_LogStoppedFormat, instanceName, stopwatch.Elapsed);
        }

        /// <summary>
        /// Disables tracing of native API calls for all the SQL Server
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// Tracing could not be disabled.
        /// </exception>
        public static void StopTracing()
        {
            Logger.Verbose(Logger.TraceEvent.StopTracing, SR.SqlLocalDbApi_LogStoppingTracing);

            InvokeThrowOnError(NativeMethods.StopTracing, Logger.TraceEvent.StopTracing);

            Logger.Verbose(Logger.TraceEvent.StartTracing, SR.SqlLocalDbApi_LogStoppedTracing);
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
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be unshared.
        /// </exception>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the native LocalDB API function.")]
        public static void UnshareInstance(string instanceName)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            Logger.Verbose(Logger.TraceEvent.UnshareInstance, SR.SqlLocalDbApi_LogStoppingSharingFormat, instanceName);

            InvokeThrowOnError(
                () => NativeMethods.UnshareInstance(instanceName),
                Logger.TraceEvent.UnshareInstance,
                instanceName);

            Logger.Verbose(Logger.TraceEvent.UnshareInstance, SR.SqlLocalDbApi_LogStoppedSharingFormat, instanceName);
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
        /// <exception cref="SqlLocalDbException">
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/> could not be deleted.
        /// </exception>
        internal static bool DeleteInstanceInternal(string instanceName, bool throwIfNotFound, bool deleteFiles = false)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            Logger.Verbose(Logger.TraceEvent.DeleteInstance, SR.SqlLocalDbApi_LogDeletingFormat, instanceName);

            int hr = NativeMethods.DeleteInstance(instanceName);

            if (hr != 0)
            {
                if (!throwIfNotFound && hr == SqlLocalDbErrors.UnknownInstance)
                {
                    Logger.Verbose(Logger.TraceEvent.DeleteInstance, SR.SqlLocalDbApi_InstanceDoesNotExistFormat, instanceName);
                    return false;
                }

                throw GetLocalDbError(hr, Logger.TraceEvent.DeleteInstance, instanceName);
            }

            if (deleteFiles)
            {
                DeleteInstanceFiles(instanceName);
            }

            Logger.Verbose(Logger.TraceEvent.DeleteInstance, SR.SqlLocalDbApi_LogDeletedFormat, instanceName);
            return true;
        }

        /// <summary>
        /// Deletes the file(s) from disk that are associated with the specified SQL LocalDB instance.
        /// </summary>
        /// <param name="instanceName">The name of the SQL LocalDB instance to delete the file(s) for.</param>
        private static void DeleteInstanceFiles(string instanceName)
        {
            string instancePath = Path.Combine(GetInstancesFolderPath(), instanceName);

            try
            {
                if (Directory.Exists(instancePath))
                {
                    Logger.Verbose(
                        Logger.TraceEvent.DeletingInstanceFiles,
                        SR.SqlLocalDbApi_LogDeletingInstanceFilesFormat,
                        instanceName,
                        instancePath);

                    Directory.Delete(instancePath, recursive: true);

                    Logger.Verbose(
                        Logger.TraceEvent.DeletedInstanceFiles,
                        SR.SqlLocalDbApi_LogDeletedInstanceFilesFormat,
                        instanceName,
                        instancePath);
                }
            }
            catch (ArgumentException ex)
            {
                Logger.Error(
                    Logger.TraceEvent.DeletingInstanceFilesFailed,
                    SR.SqlLocalDbApi_LogDeletingInstanceFilesFailedFormat,
                    instanceName,
                    instancePath,
                    ex.Message);
            }
            catch (IOException ex)
            {
                Logger.Error(
                    Logger.TraceEvent.DeletingInstanceFilesFailed,
                    SR.SqlLocalDbApi_LogDeletingInstanceFilesFailedFormat,
                    instanceName,
                    instancePath,
                    ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error(
                    Logger.TraceEvent.DeletingInstanceFilesFailed,
                    SR.SqlLocalDbApi_LogDeletingInstanceFilesFailedFormat,
                    instanceName,
                    instancePath,
                    ex.Message);
            }
        }

        /// <summary>
        /// Combines the specified strings into a full file system path.
        /// </summary>
        /// <param name="paths">An array containing at least two paths to combine.</param>
        /// <returns>
        /// The full path created from combining the values in <paramref name="paths"/>.
        /// </returns>
        private static string PathCombine(params string[] paths)
        {
            Debug.Assert(paths != null, "paths cannot be null.");
            Debug.Assert(paths.Length > 1, "At least two paths must be specified.");

            string path = Path.Combine(paths[0], paths[1]);

            for (int i = 2; i < paths.Length; i++)
            {
                path = Path.Combine(path, paths[i]);
            }

            return path;
        }

        /// <summary>
        /// Returns an <see cref="Exception"/> representing the specified LocalDB HRESULT.
        /// </summary>
        /// <param name="hr">The HRESULT returned by the LocalDB API.</param>
        /// <param name="traceEventId">The trace event Id associated with the error.</param>
        /// <param name="instanceName">The name of the instance that caused the error, if any.</param>
        /// <returns>
        /// An <see cref="Exception"/> representing <paramref name="hr"/>.
        /// </returns>
        private static Exception GetLocalDbError(int hr, int traceEventId, string instanceName = "")
        {
            string message;

            Logger.Error(traceEventId, SR.SqlLocalDbApi_LogNativeResultFormat, hr);

            if (hr == SqlLocalDbErrors.NotInstalled)
            {
                message = SRHelper.Format(
                    SR.SqlLocalDbApi_NotInstalledFormat,
                    Environment.MachineName);

                Logger.Error(traceEventId, message);
                throw new InvalidOperationException(message);
            }

            StringBuilder buffer = new StringBuilder(NativeMethods.LOCALDB_MAX_SQLCONNECTION_BUFFER_SIZE + 1);
            int size = buffer.Capacity;

            // Get the description of the error from the LocalDB API.
            // N.B. This doesn't currently support specifically overriding the language Id.
            const int DefaultLanaguageId = 0;

            int hr2 = NativeMethods.GetLocalDbError(
                hr,
                DefaultLanaguageId,
                buffer,
                ref size);

            if (hr2 != 0)
            {
                // Use a generic message if getting the message from the API failed
                message = SRHelper.Format(
                    SR.SqlLocalDbApi_GenericFailureFormat,
                    hr2);

                Logger.Error(traceEventId, message);

                return new SqlLocalDbException(
                    message,
                    hr2,
                    instanceName);
            }

            message = buffer.ToString();

            Logger.Error(traceEventId, message);

            return new SqlLocalDbException(
                message,
                hr,
                instanceName);
        }

        /// <summary>
        /// Gets the available versions of SQL LocalDB installed on the local machine.
        /// </summary>
        /// <returns>   
        /// An <see cref="Array"/> of <see cref="String"/> containing the available versions.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// SQL Server LocalDB is not installed on the local machine.
        /// </exception>
        /// <exception cref="SqlLocalDbException">
        /// The installed LocalDB versions could not be enumerated.
        /// </exception>
        private static string[] GetLocalDbVersions()
        {
            try
            {
                // Query the LocalDB API to get the number of instances
                int count = 0;
                int hr = NativeMethods.GetVersions(IntPtr.Zero, ref count);

                if (hr != 0 &&
                    hr != SqlLocalDbErrors.InsufficientBuffer)
                {
                    throw GetLocalDbError(hr, Logger.TraceEvent.GetVersions);
                }

                // Allocate enough memory to receive the version name array
                int versionLength = MaxVersionLength;
                IntPtr ptrVersions = Marshal.AllocHGlobal(versionLength * count);

                try
                {
                    hr = NativeMethods.GetVersions(ptrVersions, ref count);

                    if (hr != 0)
                    {
                        throw GetLocalDbError(hr, Logger.TraceEvent.GetVersions);
                    }

                    // Read the version strings back from unmanaged memory
                    string[] versions = MarshalStringArray(ptrVersions, versionLength, count);

                    return versions;
                }
                finally
                {
                    Marshal.FreeHGlobal(ptrVersions);
                }
            }
            catch (SqlLocalDbException e)
            {
                throw new SqlLocalDbException(
                    SR.SqlLocalDbException_VersionEnumerationFailed,
                    e.ErrorCode,
                    e.InstanceName,
                    e);
            }
        }

        /// <summary>
        /// Invokes the specified delegate, throwing an exception if the delegate does not return zero.
        /// </summary>
        /// <param name="func">A delegate to a method to invoke.</param>
        /// <param name="traceEventId">The trace event Id if a non-zero value is returned.</param>
        /// <param name="instanceName">The name of the instance that caused the error, if any.</param>
        private static void InvokeThrowOnError(Func<int> func, int traceEventId, string instanceName = "")
        {
            int hr = func();

            if (hr != 0)
            {
                throw GetLocalDbError(hr, traceEventId, instanceName);
            }
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

        #endregion
    }
}