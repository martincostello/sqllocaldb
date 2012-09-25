// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbApi.cs" company="http://sqllocaldb.codeplex.com">
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
//   SqlLocalDbApi.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
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
        #region Fields

        /// <summary>
        /// The available versions of SQL Server LocalDB installed on the local machine.
        /// </summary>
        private static string[] _versions;

        /// <summary>
        /// The timeout for stopping an instance of LocalDB.
        /// </summary>
        private static TimeSpan _stopTimeout = TimeSpan.FromMinutes(1);

        #endregion

        #region Properties

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
                IList<string> versions = SqlLocalDbApi.Versions;

                if (versions.Count < 1)
                {
                    string message = SRHelper.Format(
                        SR.SqlLocalDbApi_NoVersionsFormat,
                        Environment.MachineName);

                    throw new InvalidOperationException(message);
                }

                // TODO Verify whether the latest value is the first or last element
                return versions[0];
            }
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
        /// Gets an <see cref="Array"/> of <see cref="String"/>
        /// containing the available versions of SQL LocalDB.
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
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/>
        /// could not be stopped or the installed versions of SQL LocalDB could not be determined.
        /// </exception>
        public static void CreateInstance(string instanceName)
        {
            // Use the latest version
            CreateInstance(instanceName, SqlLocalDbApi.LatestVersion);
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
        /// The SQL Server LocalDB instance specified by <paramref name="instanceName"/>
        /// and <paramref name="version"/> could not be created.
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

            int hr = NativeMethods.CreateInstance(
                version,
                instanceName);

            if (hr != 0)
            {
                throw GetLocalDbError(hr, instanceName);
            }
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
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            int hr = NativeMethods.DeleteInstance(instanceName);

            if (hr != 0)
            {
                throw GetLocalDbError(hr, instanceName);
            }
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
        public static ISqlLocalDbInstanceInfo GetInstanceInfo(string instanceName)
        {
            if (instanceName == null)
            {
                throw new ArgumentNullException("instanceName");
            }

            int size = Marshal.SizeOf(typeof(LocalDBInstanceInfo));
            IntPtr ptrInfo = Marshal.AllocHGlobal(size);

            try
            {
                int hr = NativeMethods.GetInstanceInfo(
                    instanceName,
                    ptrInfo,
                    size);

                if (hr != 0)
                {
                    throw GetLocalDbError(hr, instanceName);
                }

                LocalDBInstanceInfo info = (LocalDBInstanceInfo)Marshal.PtrToStructure(
                    ptrInfo,
                    typeof(LocalDBInstanceInfo));

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
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Requires enumeration of native API and allocating unmanaged memory.")]
        public static IList<string> GetInstanceNames()
        {
            // Query the LocalDB API to get the number of instances
            int count = 0;
            int hr = NativeMethods.GetInstanceNames(IntPtr.Zero, ref count);

            if (hr != 0 &&
                hr != SqlLocalDbErrors.InsufficientBuffer)
            {
                throw GetLocalDbError(hr);
            }

            if (count == 0)
            {
                // Shortcut, no instances
                return new string[0];
            }

            // Allocate enough memory to receive the instance name array
            int nameLength = (NativeMethods.MAX_LOCALDB_INSTANCE_NAME_LENGTH + 1) * sizeof(char);
            IntPtr ptrNames = Marshal.AllocHGlobal(nameLength * count);

            try
            {
                hr = NativeMethods.GetInstanceNames(ptrNames, ref count);

                if (hr != 0)
                {
                    throw GetLocalDbError(hr);
                }

                // Read the instance names back from unmanaged memory
                string[] names = new string[count];

                for (int i = 0; i < names.Length; i++)
                {
                    // Determine the offset of the element, and get the string from the array
                    IntPtr offset = new IntPtr(ptrNames.ToInt64() + (nameLength * i));
                    names[i] = Marshal.PtrToStringAuto(offset);
                }

                return names;
            }
            finally
            {
                Marshal.FreeHGlobal(ptrNames);
            }
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
        public static ISqlLocalDbVersionInfo GetVersionInfo(string version)
        {
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }

            int size = Marshal.SizeOf(typeof(LocalDBVersionInfo));
            IntPtr ptrInfo = Marshal.AllocHGlobal(size);

            try
            {
                int hr = NativeMethods.GetVersionInfo(
                    version,
                    ptrInfo,
                    size);

                if (hr != 0)
                {
                    throw GetLocalDbError(hr);
                }

                LocalDBVersionInfo info = (LocalDBVersionInfo)Marshal.PtrToStructure(
                    ptrInfo,
                    typeof(LocalDBVersionInfo));

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
        /// <paramref name="instanceName"/> or <paramref name="instanceName"/> is <see langword="null"/>.
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
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                string sid = identity.User.Value;
                ShareInstance(sid, instanceName, sharedInstanceName);
            }
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
                // the causes "interesting" results when using the other
                // API functions on it.  Block this explicitly to stop
                // callers messing up their LocalDB instance.
                throw new ArgumentException(SR.SqlLocalDbApi_NoInstanceName, "instanceName");
            }

            // Get the binary version of the SID from its string
            SecurityIdentifier sid = new SecurityIdentifier(ownerSid);
            byte[] binaryForm = new byte[SecurityIdentifier.MaxBinaryLength];
            sid.GetBinaryForm(binaryForm, 0);

            IntPtr ptrSid = Marshal.AllocHGlobal(binaryForm.Length);

            try
            {
                // Copy the SID binary form to unmanaged memory
                Marshal.Copy(binaryForm, 0, ptrSid, binaryForm.Length);

                int hr = NativeMethods.ShareInstance(
                    IntPtr.Zero,
                    instanceName,
                    sharedInstanceName);

                if (hr != 0)
                {
                    throw GetLocalDbError(hr, instanceName);
                }
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

            StringBuilder buffer = new StringBuilder(NativeMethods.LOCALDB_MAX_SQLCONNECTION_BUFFER_SIZE + 1);
            int size = buffer.Capacity;

            int hr = NativeMethods.StartInstance(
                instanceName,
                buffer,
                ref size);

            if (hr != 0)
            {
                throw GetLocalDbError(hr, instanceName);
            }

            return buffer.ToString();
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
            int hr = NativeMethods.StartTracing();

            if (hr != 0)
            {
                throw GetLocalDbError(hr);
            }
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

            int hr = NativeMethods.StopInstance(
                instanceName,
                (int)timeout.TotalSeconds);

            if (hr != 0)
            {
                throw GetLocalDbError(hr, instanceName);
            }
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
            int hr = NativeMethods.StopTracing();

            if (hr != 0)
            {
                throw GetLocalDbError(hr);
            }
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
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

            int hr = NativeMethods.UnshareInstance(instanceName);

            if (hr != 0)
            {
                throw GetLocalDbError(hr, instanceName);
            }
        }

        /// <summary>
        /// Returns an <see cref="Exception"/> representing the specified LocalDB HRESULT.
        /// </summary>
        /// <param name="hr">The HRESULT returned by the LocalDB API.</param>
        /// <param name="instanceName">The name of the instance that caused the error, if any.</param>
        /// <returns>
        /// An <see cref="Exception"/> representing <paramref name="hr"/>.
        /// </returns>
        private static Exception GetLocalDbError(int hr, string instanceName = "")
        {
            if (hr == SqlLocalDbErrors.NotInstalled)
            {
                string message = SRHelper.Format(
                    SR.SqlLocalDbApi_NotInstalledFormat,
                    Environment.MachineName);

                throw new InvalidOperationException(message);
            }

            StringBuilder buffer = new StringBuilder(NativeMethods.LOCALDB_MAX_SQLCONNECTION_BUFFER_SIZE + 1);
            int size = buffer.Capacity;

            // Get the description of the error from the LocalDB API
            int hr2 = NativeMethods.GetLocalDbError(
                hr,
                SR.Culture == null ? 0 : SR.Culture.LCID,
                buffer,
                ref size);

            if (hr2 != 0)
            {
                // Use a generic message if getting the message from the API failed
                string message = SRHelper.Format(
                    SR.SqlLocalDbApi_GenericFailureFormat,
                    hr2);

                return new SqlLocalDbException(
                    message,
                    hr2,
                    instanceName);
            }

            return new SqlLocalDbException(
                buffer.ToString(),
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
                    throw GetLocalDbError(hr);
                }

                // Allocate enough memory to receive the version name array
                int versionLength = (NativeMethods.MAX_LOCALDB_VERSION_LENGTH + 1) * sizeof(char);
                IntPtr ptrVersions = Marshal.AllocHGlobal(versionLength * count);

                try
                {
                    hr = NativeMethods.GetVersions(ptrVersions, ref count);

                    if (hr != 0)
                    {
                        throw GetLocalDbError(hr);
                    }

                    // Read the version strings back from unmanaged memory
                    string[] versions = new string[count];

                    for (int i = 0; i < versions.Length; i++)
                    {
                        // Determine the offset of the element, and get the string from the array
                        IntPtr offset = new IntPtr(ptrVersions.ToInt64() + (versionLength * i));
                        versions[i] = Marshal.PtrToStringAuto(offset);
                    }

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

        #endregion
    }
}