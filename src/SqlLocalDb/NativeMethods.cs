// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   NativeMethods.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing native P/Invoke methods.  This class cannot be inherited.
    /// </summary>
    [SecurityCritical]
    internal static class NativeMethods
    {
        /// <summary>
        /// The maximum size of SQL Server LocalDB connection string.
        /// </summary>
        internal const int LOCALDB_MAX_SQLCONNECTION_BUFFER_SIZE = 260;

        /// <summary>
        /// The maximum size of SQL Server LocalDB instance names.
        /// </summary>
        internal const int MAX_LOCALDB_INSTANCE_NAME_LENGTH = 128;

        /// <summary>
        /// The maximum size of an SQL Server LocalDB version string.
        /// </summary>
        internal const int MAX_LOCALDB_VERSION_LENGTH = 43;

        /// <summary>
        /// The maximum length of a SID string.
        /// </summary>
        internal const int MAX_STRING_SID_LENGTH = 186;

        /// <summary>
        /// Specifies that error messages that are too long should be truncated.
        /// </summary>
        private const int LOCALDB_TRUNCATE_ERR_MESSAGE = 1;

        /// <summary>
        /// The name of the Windows Kernel library.
        /// </summary>
        private const string KernelLibName = "kernel32.dll";

        /// <summary>
        /// Synchronization object to protect loading the native library and its functions.
        /// </summary>
        private static readonly object _syncRoot = new object();

        /// <summary>
        /// The handle to the native SQL LocalDB API.
        /// </summary>
        private static SafeLibraryHandle _localDB;

        /// <summary>
        /// The delegate to the <c>LocalDBCreateInstance</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBCreateInstance _localDBCreateInstance;

        /// <summary>
        /// The delegate to the <c>LocalDBDeleteInstance</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBDeleteInstance _localDBDeleteInstance;

        /// <summary>
        /// The delegate to the <c>LocalDBFormatMessage</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBFormatMessage _localDBFormatMessage;

        /// <summary>
        /// The delegate to the <c>LocalDBGetInstanceInfo</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBGetInstanceInfo _localDBGetInstanceInfo;

        /// <summary>
        /// The delegate to the <c>LocalDBGetInstances</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBGetInstances _localDBGetInstances;

        /// <summary>
        /// The delegate to the <c>LocalDBGetVersionInfo</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBGetVersionInfo _localDBGetVersionInfo;

        /// <summary>
        /// The delegate to the <c>LocalDBGetVersions</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBGetVersions _localDBGetVersions;

        /// <summary>
        /// The delegate to the <c>LocalDBShareInstance</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBShareInstance _localDBShareInstance;

        /// <summary>
        /// The delegate to the <c>LocalDBStartInstance</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBStartInstance _localDBStartInstance;

        /// <summary>
        /// The delegate to the <c>LocalDBStartTracing</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBStartTracing _localDBStartTracing;

        /// <summary>
        /// The delegate to the <c>LocalDBStopInstance</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBStopInstance _localDBStopInstance;

        /// <summary>
        /// The delegate to the <c>LocalDBStopTracing</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBStopTracing _localDBStopTracing;

        /// <summary>
        /// The delegate to the <c>LocalDBUnshareInstance</c> LocalDB API function.
        /// </summary>
        private static Functions.LocalDBUnshareInstance _localDBUnshareInstance;

        /// <summary>
        /// Gets the version of the SQL LocalDB native API loaded, if any.
        /// </summary>
        internal static Version NativeApiVersion
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="wszVersion">The LocalDB version, for example 11.0 or 11.0.1094.2.</param>
        /// <param name="pInstanceName">The name for the LocalDB instance to create.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal static int CreateInstance(string wszVersion, string pInstanceName)
        {
            var function = EnsureFunction("LocalDBCreateInstance", ref _localDBCreateInstance);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(wszVersion, pInstanceName, 0);
        }

        /// <summary>
        /// Deletes the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to delete.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal static int DeleteInstance(string pInstanceName)
        {
            var function = EnsureFunction("LocalDBDeleteInstance", ref _localDBDeleteInstance);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(pInstanceName, 0);
        }

        /// <summary>
        /// Frees a specified library.
        /// </summary>
        /// <param name="handle">The handle to the module to free.</param>
        /// <returns>Whether the library was successfully unloaded.</returns>
        [DllImport(KernelLibName)]
        [return: MarshalAs(UnmanagedType.Bool)]
        [SecurityCritical]
        internal static extern bool FreeLibrary(IntPtr handle);

        /// <summary>
        /// Returns information for the specified SQL Server Express LocalDB instance,
        /// such as whether it exists, the LocalDB version it uses, whether it is running,
        /// and so on.
        /// </summary>
        /// <param name="wszInstanceName">The instance name.</param>
        /// <param name="pInstanceInfo">The buffer to store the information about the LocalDB instance.</param>
        /// <param name="dwInstanceInfoSize">Holds the size of the InstanceInfo buffer.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal static int GetInstanceInfo(string wszInstanceName, IntPtr pInstanceInfo, int dwInstanceInfoSize)
        {
            var function = EnsureFunction("LocalDBGetInstanceInfo", ref _localDBGetInstanceInfo);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(wszInstanceName, pInstanceInfo, dwInstanceInfoSize);
        }

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
        internal static int GetInstanceNames(IntPtr pInstanceNames, ref int lpdwNumberOfInstances)
        {
            var function = EnsureFunction("LocalDBGetInstances", ref _localDBGetInstances);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(pInstanceNames, ref lpdwNumberOfInstances);
        }

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
        internal static int GetLocalDbError(int hrLocalDB, int dwLanguageId, StringBuilder wszMessage, ref int lpcchMessage)
        {
            var function = EnsureFunction("LocalDBFormatMessage", ref _localDBFormatMessage);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(hrLocalDB, LOCALDB_TRUNCATE_ERR_MESSAGE, dwLanguageId, wszMessage, ref lpcchMessage);
        }

        /// <summary>
        /// Returns information for the specified SQL Server Express LocalDB version,
        /// such as whether it exists and the full LocalDB version number (including
        /// build and release numbers).
        /// </summary>
        /// <param name="wszVersionName">The LocalDB version name.</param>
        /// <param name="pVersionInfo">The buffer to store the information about the LocalDB version.</param>
        /// <param name="dwVersionInfoSize">Holds the size of the VersionInfo buffer.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal static int GetVersionInfo(string wszVersionName, IntPtr pVersionInfo, int dwVersionInfoSize)
        {
            var function = EnsureFunction("LocalDBGetVersionInfo", ref _localDBGetVersionInfo);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(wszVersionName, pVersionInfo, dwVersionInfoSize);
        }

        /// <summary>
        /// Returns all SQL Server Express LocalDB versions available on the computer.
        /// </summary>
        /// <param name="pVersion">Contains names of the LocalDB versions that are available on the user’s workstation.</param>
        /// <param name="lpdwNumberOfVersions">
        /// On input holds the number of slots for versions in the <paramref name="pVersion"/>
        /// buffer. On output, holds the number of existing LocalDB versions.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal static int GetVersions(IntPtr pVersion, ref int lpdwNumberOfVersions)
        {
            var function = EnsureFunction("LocalDBGetVersions", ref _localDBGetVersions);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(pVersion, ref lpdwNumberOfVersions);
        }

        /// <summary>
        /// Shares the specified SQL Server Express LocalDB instance with other
        /// users of the computer, using the specified shared name.
        /// </summary>
        /// <param name="pOwnerSID">The SID of the instance owner.</param>
        /// <param name="pInstancePrivateName">The private name for the LocalDB instance to share.</param>
        /// <param name="pInstanceSharedName">The shared name for the LocalDB instance to share.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal static int ShareInstance(IntPtr pOwnerSID, string pInstancePrivateName, string pInstanceSharedName)
        {
            var function = EnsureFunction("LocalDBShareInstance", ref _localDBShareInstance);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(pOwnerSID, pInstancePrivateName, pInstanceSharedName, 0);
        }

        /// <summary>
        /// Starts the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to start.</param>
        /// <param name="wszSqlConnection">The buffer to store the connection string to the LocalDB instance.</param>
        /// <param name="lpcchSqlConnection">
        /// On input contains the size of the <paramref name="wszSqlConnection"/> buffer in
        /// characters, including any trailing nulls. On output, if the given buffer size is
        /// too small, contains the required buffer size in characters, including any trailing nulls.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal static int StartInstance(string pInstanceName, StringBuilder wszSqlConnection, ref int lpcchSqlConnection)
        {
            var function = EnsureFunction("LocalDBStartInstance", ref _localDBStartInstance);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(pInstanceName, 0, wszSqlConnection, ref lpcchSqlConnection);
        }

        /// <summary>
        /// Enables tracing of API calls for all the SQL Server Express
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal static int StartTracing()
        {
            var function = EnsureFunction("LocalDBStartTracing", ref _localDBStartTracing);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function();
        }

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
        internal static int StopInstance(string pInstanceName, StopInstanceOptions options, int ulTimeout)
        {
            var function = EnsureFunction("LocalDBStopInstance", ref _localDBStopInstance);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(pInstanceName, (int)options, ulTimeout);
        }

        /// <summary>
        /// Disables tracing of API calls for all the SQL Server Express LocalDB
        /// instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal static int StopTracing()
        {
            var function = EnsureFunction("LocalDBStopTracing", ref _localDBStopTracing);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function();
        }

        /// <summary>
        /// Stops the sharing of the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">
        /// The private name for the LocalDB instance to share.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        internal static int UnshareInstance(string pInstanceName)
        {
            var function = EnsureFunction("LocalDBUnshareInstance", ref _localDBUnshareInstance);

            if (function == null)
            {
                return SqlLocalDbErrors.NotInstalled;
            }

            return function(pInstanceName, 0);
        }

        /// <summary>
        /// Retrieves the address of an exported function or variable from the specified dynamic-link library (DLL).
        /// </summary>
        /// <param name="hModule">A handle to the DLL module that contains the function or variable. </param>
        /// <param name="lpProcName">The function or variable name, or the function's ordinal value.</param>
        /// <returns>
        /// If the function succeeds, the return value is the address of the exported function or variable.
        /// If the function fails, the return value is <see cref="IntPtr.Zero"/>.
        /// </returns>
        /// <remarks>
        /// See <c>http://msdn.microsoft.com/en-us/library/windows/desktop/ms683212%28v=vs.85%29.aspx</c>.
        /// </remarks>
        [DllImport(KernelLibName, BestFitMapping = false, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true)]
        private static extern IntPtr GetProcAddress(
            SafeLibraryHandle hModule,
            [MarshalAs(UnmanagedType.LPStr)]
            string lpProcName);

        /// <summary>
        /// Loads the specified module into the address space of the calling process.
        /// The specified module may cause other modules to be loaded.
        /// </summary>
        /// <param name="lpFileName">The name of the module.</param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the module.
        /// If the function fails, the return value is <see langword="null"/>.
        /// </returns>
        /// <remarks>
        /// See <c>http://msdn.microsoft.com/en-us/library/windows/desktop/ms684175%28v=vs.85%29.aspx</c>.
        /// </remarks>
        [DllImport(KernelLibName, BestFitMapping = false, CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true)]
        private static extern SafeLibraryHandle LoadLibrary(
            [MarshalAs(UnmanagedType.LPStr)]
            string lpFileName);

        /// <summary>
        /// Determines whether the specified process is running under WOW64.
        /// </summary>
        /// <param name="hProcess">A handle to the process.</param>
        /// <param name="Wow64Process">
        /// A pointer to a value that is set to <see langword="true"/> if the
        /// process is running under WOW64. If the process is running under 32-bit
        /// Windows, the value is set to <see langword="false"/>. If the process
        /// is a 64-bit application running under 64-bit Windows, the value is also
        /// set to <see langword="false"/>.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a non-zero value.
        /// If the function fails, the return value is zero.
        /// </returns>
        /// <remarks>
        /// See <c>http://msdn.microsoft.com/en-us/library/windows/desktop/ms684139%28v=vs.85%29.aspx</c>.
        /// </remarks>
        [DllImport(KernelLibName, CallingConvention = CallingConvention.Winapi, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsWow64Process(
            IntPtr hProcess,
            [MarshalAs(UnmanagedType.Bool)]
            out bool Wow64Process);

        /// <summary>
        /// Ensures that the specified delegate to an unmanaged function is initialized.
        /// </summary>
        /// <typeparam name="T">The type of the delegate representing the unmanaged function.</typeparam>
        /// <param name="functionName">The name of the unmanaged function to ensure is loaded.</param>
        /// <param name="function">A reference to a location to ensure contains a delegate for the specified function name.</param>
        /// <returns>
        /// An instance of <typeparamref name="T"/> that points to the specified unmanaged
        /// function, if found; otherwise <see langword="null"/>.
        /// </returns>
        private static T EnsureFunction<T>(string functionName, ref T function)
            where T : class
        {
            if (function == null)
            {
                lock (_syncRoot)
                {
                    if (function == null)
                    {
                        function = GetDelegate<T>(functionName);
                    }
                }
            }

            return function;
        }

        /// <summary>
        /// Ensures that the LocalDB native API has been loaded.
        /// </summary>
        /// <returns>
        /// A <see cref="SafeLibraryHandle"/> pointing to the loaded
        /// SQL LocalDB API, if successful; otherwise <see langword="null"/>.
        /// </returns>
        private static SafeLibraryHandle EnsureLocalDBLoaded()
        {
            if (_localDB == null)
            {
                lock (_syncRoot)
                {
                    if (_localDB == null)
                    {
                        string fileName;

                        if (!TryGetLocalDbApiPath(out fileName))
                        {
                            return null;
                        }

                        _localDB = LoadLibrary(fileName);

                        if (_localDB == null ||
                            _localDB.IsInvalid)
                        {
                            int error = Marshal.GetLastWin32Error();
                            Logger.Error(Logger.TraceEvent.NativeApiLoadFailed, SR.NativeMethods_NativeApiLoadFailedFormat, fileName, error);
                            _localDB = null;
                        }
                        else
                        {
                            Logger.Verbose(Logger.TraceEvent.NativeApiLoaded, SR.NativeMethods_NativeApiLoadedFormat, fileName);
                        }
                    }
                }
            }

            return _localDB;
        }

        /// <summary>
        /// Returns a delegate of the specified type to the specified unmanaged function.
        /// </summary>
        /// <typeparam name="T">The type of the delegate to return.</typeparam>
        /// <param name="functionName">The name of the unmanaged function.</param>
        /// <returns>
        /// An instance of <typeparamref name="T"/> that points to the specified unmanaged
        /// function, if found; otherwise <see langword="null"/>.
        /// </returns>
        private static T GetDelegate<T>(string functionName)
            where T : class
        {
            SafeLibraryHandle handle = EnsureLocalDBLoaded();

            if (handle == null)
            {
                Logger.Warning(Logger.TraceEvent.NativeApiNotLoaded, SR.NativeMethods_NativeApiNotLoaded);
                return null;
            }

            IntPtr ptr = GetProcAddress(handle, functionName);

            if (ptr == IntPtr.Zero)
            {
                Logger.Error(Logger.TraceEvent.FunctionNotFound, SR.NativeMethods_FunctionNotFoundFormat, functionName);
                return null;
            }

            return Marshal.GetDelegateForFunctionPointer(ptr, typeof(T)) as T;
        }

        /// <summary>
        /// Returns whether the current process is a WOW64 process.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if the current process is a WOW64
        /// process; otherwise <see langword="false"/>.
        /// </returns>
        private static bool IsWow64Process()
        {
            if (IntPtr.Size == 8)
            {
                return false;
            }

            // This function is not supported before Windows XP
            if ((Environment.OSVersion.Version.Major == 5 &&
                 Environment.OSVersion.Version.Minor >= 1) ||
                Environment.OSVersion.Version.Major >= 6)
            {
                using (Process process = Process.GetCurrentProcess())
                {
                    bool wow64Process;

                    if (!IsWow64Process(process.Handle, out wow64Process))
                    {
                        return false;
                    }

                    return wow64Process;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Tries to obtaining the path to the latest version of the SQL LocalDB
        /// native API DLL for the currently executing process.
        /// </summary>
        /// <param name="fileName">
        /// When the method returns, contains the path to the SQL Local DB API
        /// to use, if found; otherwise <see langword="null"/>.
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the native API path was successfully found;
        /// otherwise <see langword="false"/>.
        /// </returns>
        private static bool TryGetLocalDbApiPath(out string fileName)
        {
            fileName = null;

            // Open the appropriate Registry key if running as a 32-bit process on a 64-bit machine
            string keyName = string.Format(
                CultureInfo.InvariantCulture,
                @"SOFTWARE\{0}Microsoft\Microsoft SQL Server Local DB\Installed Versions",
                IsWow64Process() ? @"Wow6432Node\" : string.Empty);

            RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName, false);

            if (key == null)
            {
                Logger.Warning(Logger.TraceEvent.RegistryKeyNotFound, SR.NativeMethods_RegistryKeyNotFoundFormat, keyName);
                return false;
            }

            Version latestVersion = null;
            Version overrideVersion = null;

            // Is there a setting overriding the version to load?
            string overrideVersionString = SqlLocalDbConfig.NativeApiOverrideVersionString;

            string path = null;

            try
            {
                foreach (string versionString in key.GetSubKeyNames())
                {
                    Version version;

                    try
                    {
                        version = new Version(versionString);
                    }
                    catch (ArgumentException)
                    {
                        Logger.Warning(Logger.TraceEvent.InvalidRegistryKey, SR.NativeMethods_InvalidRegistryKeyNameFormat, versionString);
                        continue;
                    }
                    catch (FormatException)
                    {
                        Logger.Warning(Logger.TraceEvent.InvalidRegistryKey, SR.NativeMethods_InvalidRegistryKeyNameFormat, versionString);
                        continue;
                    }
                    catch (OverflowException)
                    {
                        Logger.Warning(Logger.TraceEvent.InvalidRegistryKey, SR.NativeMethods_InvalidRegistryKeyNameFormat, versionString);
                        continue;
                    }

                    if (!string.IsNullOrEmpty(overrideVersionString) &&
                        overrideVersion == null &&
                        string.Equals(versionString, overrideVersionString, StringComparison.OrdinalIgnoreCase))
                    {
                        Logger.Verbose(Logger.TraceEvent.NativeApiVersionOverriddenByUser, SR.NativeMethods_ApiVersionOverriddenByUserFormat, version);
                        overrideVersion = version;
                    }

                    if (latestVersion == null ||
                        latestVersion < version)
                    {
                        latestVersion = version;
                    }
                }

                Version versionToUse = overrideVersion ?? latestVersion;

                if (versionToUse != null)
                {
                    using (var subkey = key.OpenSubKey(versionToUse.ToString()))
                    {
                        path = subkey.GetValue("InstanceAPIPath", null, RegistryValueOptions.None) as string;
                    }

                    NativeApiVersion = versionToUse;
                }
            }
            finally
            {
                key.Close();
            }

            if (string.IsNullOrEmpty(path))
            {
                Logger.Warning(Logger.TraceEvent.NoNativeApiFound, SR.NativeMethods_NoNativeApiFound);
                return false;
            }

            if (!File.Exists(path))
            {
                Logger.Error(Logger.TraceEvent.NativeApiPathNotFound, SR.NativeMethods_NativeApiNotFoundFormat, path);
                return false;
            }

            fileName = Path.GetFullPath(path);
            return true;
        }

        /// <summary>
        /// A class containing delegates to functions in the SQL LocalDB native API.
        /// </summary>
        private static class Functions
        {
            /// <summary>
            /// Creates a new instance of SQL Server LocalDB.
            /// </summary>
            /// <param name="wszVersion">The LocalDB version, for example 11.0 or 11.0.1094.2.</param>
            /// <param name="pInstanceName">The name for the LocalDB instance to create.</param>
            /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh214784.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBCreateInstance(
                [MarshalAs(UnmanagedType.LPWStr)]
                string wszVersion,
                [MarshalAs(UnmanagedType.LPWStr)]
                string pInstanceName,
                int dwFlags);

            /// <summary>
            /// Deletes the specified SQL Server Express LocalDB instance.
            /// </summary>
            /// <param name="pInstanceName">The name of the LocalDB instance to delete.</param>
            /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh214724.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBDeleteInstance(
                [MarshalAs(UnmanagedType.LPWStr)]
                string pInstanceName,
                int dwFlags);

            /// <summary>
            /// Returns the localized textual description for the specified SQL Server Express LocalDB error.
            /// </summary>
            /// <param name="hrLocalDB">The LocalDB error code.</param>
            /// <param name="dwFlags">The flags specifying the behavior of this function.</param>
            /// <param name="dwLanguageId">The language desired (LANGID) or 0, in which case the Win32 FormatMessage language order is used.</param>
            /// <param name="wszMessage">The buffer to store the LocalDB error message.</param>
            /// <param name="lpcchMessage">
            /// On input contains the size of the <paramref name="wszMessage"/> buffer in characters. On output,
            /// if the given buffer size is too small, contains the buffer size required in characters, including
            /// any trailing nulls.  If the function succeeds, contains the number of characters in the message,
            /// excluding any trailing nulls.
            /// </param>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh214483.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBFormatMessage(
                int hrLocalDB,
                int dwFlags,
                int dwLanguageId,
                [MarshalAs(UnmanagedType.LPWStr)][Out]
                StringBuilder wszMessage,
                ref int lpcchMessage);

            /// <summary>
            /// Returns information for the specified SQL Server Express LocalDB instance,
            /// such as whether it exists, the LocalDB version it uses, whether it is running,
            /// and so on.
            /// </summary>
            /// <param name="wszInstanceName">The instance name.</param>
            /// <param name="pInstanceInfo">The buffer to store the information about the LocalDB instance.</param>
            /// <param name="dwInstanceInfoSize">Holds the size of the InstanceInfo buffer.</param>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh245734.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBGetInstanceInfo(
                [MarshalAs(UnmanagedType.LPWStr)]
                string wszInstanceName,
                IntPtr pInstanceInfo,
                int dwInstanceInfoSize);

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
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh234622.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBGetInstances(IntPtr pInstanceNames, ref int lpdwNumberOfInstances);

            /// <summary>
            /// Returns information for the specified SQL Server Express LocalDB version,
            /// such as whether it exists and the full LocalDB version number (including
            /// build and release numbers).
            /// </summary>
            /// <param name="wszVersionName">The LocalDB version name.</param>
            /// <param name="pVersionInfo">The buffer to store the information about the LocalDB version.</param>
            /// <param name="dwVersionInfoSize">Holds the size of the VersionInfo buffer.</param>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh234365.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBGetVersionInfo(
                [MarshalAs(UnmanagedType.LPWStr)]
                string wszVersionName,
                IntPtr pVersionInfo,
                int dwVersionInfoSize);

            /// <summary>
            /// Returns all SQL Server Express LocalDB versions available on the computer.
            /// </summary>
            /// <param name="pVersion">Contains names of the LocalDB versions that are available on the user’s workstation.</param>
            /// <param name="lpdwNumberOfVersions">
            /// On input holds the number of slots for versions in the <paramref name="pVersion"/>
            /// buffer. On output, holds the number of existing LocalDB versions.
            /// </param>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh231031.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBGetVersions(IntPtr pVersion, ref int lpdwNumberOfVersions);

            /// <summary>
            /// Shares the specified SQL Server Express LocalDB instance with other
            /// users of the computer, using the specified shared name.
            /// </summary>
            /// <param name="pOwnerSID">The SID of the instance owner.</param>
            /// <param name="pInstancePrivateName">The private name for the LocalDB instance to share.</param>
            /// <param name="pInstanceSharedName">The shared name for the LocalDB instance to share.</param>
            /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh245693.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBShareInstance(
                IntPtr pOwnerSID,
                [MarshalAs(UnmanagedType.LPWStr)]
                string pInstancePrivateName,
                [MarshalAs(UnmanagedType.LPWStr)]
                string pInstanceSharedName,
                int dwFlags);

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
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh217143.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBStartInstance(
                [MarshalAs(UnmanagedType.LPWStr)]
                string pInstanceName,
                int dwFlags,
                [MarshalAs(UnmanagedType.LPWStr)][Out]
                StringBuilder wszSqlConnection,
                ref int lpcchSqlConnection);

            /// <summary>
            /// Enables tracing of API calls for all the SQL Server Express
            /// LocalDB instances owned by the current Windows user.
            /// </summary>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh247594.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBStartTracing();

            /// <summary>
            /// Stops the specified SQL Server Express LocalDB instance from running.
            /// </summary>
            /// <param name="pInstanceName">The name of the LocalDB instance to stop.</param>
            /// <param name="dwFlags">One or a combination of the flag values specifying the way to stop the instance.</param>
            /// <param name="ulTimeout">
            /// The time in seconds to wait for this operation to complete. If this
            /// value is 0, this function will return immediately without waiting for the LocalDB instance to stop.
            /// </param>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh215035.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBStopInstance(
                [MarshalAs(UnmanagedType.LPWStr)]
                string pInstanceName,
                int dwFlags,
                int ulTimeout);

            /// <summary>
            /// Disables tracing of API calls for all the SQL Server Express LocalDB
            /// instances owned by the current Windows user.
            /// </summary>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh214120.aspx</c>.
            /// </remarks>
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            internal delegate int LocalDBStopTracing();

            /// <summary>
            /// Stops the sharing of the specified SQL Server Express LocalDB instance.
            /// </summary>
            /// <param name="pInstanceName">The private name for the LocalDB instance to share.</param>
            /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
            /// <returns>The HRESULT returned by the LocalDB API.</returns>
            /// <remarks>
            /// See <c>http://technet.microsoft.com/en-us/library/hh215383.aspx</c>.
            /// </remarks>
            internal delegate int LocalDBUnshareInstance(
                [MarshalAs(UnmanagedType.LPWStr)]
                string pInstanceName,
                int dwFlags);
        }
    }
}