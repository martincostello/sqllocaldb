// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MartinCostello.SqlLocalDb.Interop;

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

#if NETSTANDARD2_0
    /// <summary>
    /// An array containing the null character. This field is read-only.
    /// </summary>
    private static readonly char[] _nullArray = ['\0'];
#endif

    /// <summary>
    /// Synchronization object to protect loading the native library and its functions. This field is read-only.
    /// </summary>
    private readonly object _syncRoot = new();

    /// <summary>
    /// Whether the instance has been disposed of.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// The path of the library that was loaded.
    /// </summary>
    private string? _libraryPath;

    /// <summary>
    /// The handle to the native SQL LocalDB API.
    /// </summary>
    private volatile SafeLibraryHandle? _handle;

    /// <summary>
    /// The delegate to the <c>LocalDBCreateInstance</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBCreateInstance? _localDBCreateInstance;

    /// <summary>
    /// The delegate to the <c>LocalDBDeleteInstance</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBDeleteInstance? _localDBDeleteInstance;

    /// <summary>
    /// The delegate to the <c>LocalDBFormatMessage</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBFormatMessage? _localDBFormatMessage;

    /// <summary>
    /// The delegate to the <c>LocalDBGetInstanceInfo</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBGetInstanceInfo? _localDBGetInstanceInfo;

    /// <summary>
    /// The delegate to the <c>LocalDBGetInstances</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBGetInstances? _localDBGetInstances;

    /// <summary>
    /// The delegate to the <c>LocalDBGetVersionInfo</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBGetVersionInfo? _localDBGetVersionInfo;

    /// <summary>
    /// The delegate to the <c>LocalDBGetVersions</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBGetVersions? _localDBGetVersions;

    /// <summary>
    /// The delegate to the <c>LocalDBShareInstance</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBShareInstance? _localDBShareInstance;

    /// <summary>
    /// The delegate to the <c>LocalDBStartInstance</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBStartInstance? _localDBStartInstance;

    /// <summary>
    /// The delegate to the <c>LocalDBStartTracing</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBStartTracing? _localDBStartTracing;

    /// <summary>
    /// The delegate to the <c>LocalDBStopInstance</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBStopInstance? _localDBStopInstance;

    /// <summary>
    /// The delegate to the <c>LocalDBStopTracing</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBStopTracing? _localDBStopTracing;

    /// <summary>
    /// The delegate to the <c>LocalDBUnshareInstance</c> LocalDB API function.
    /// </summary>
    private Functions.LocalDBUnshareInstance? _localDBUnshareInstance;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalDbInstanceApi"/> class.
    /// </summary>
    /// <param name="apiVersion">The version of the SQL LocalDB Instance API to load.</param>
    /// <param name="registry">The <see cref="IRegistry"/> to use.</param>
    /// <param name="logger">The logger to use.</param>
    internal LocalDbInstanceApi(string apiVersion, IRegistry registry, ILogger<LocalDbInstanceApi> logger)
    {
        ApiVersion = apiVersion;
        Registry = registry;
        Logger = logger;
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="LocalDbInstanceApi"/> class.
    /// </summary>
    ~LocalDbInstanceApi()
    {
        DisposeInternal();
    }

    /// <summary>
    /// Gets the version of the SQL LocalDB native API loaded, if any.
    /// </summary>
    internal Version? NativeApiVersion { get; private set; }

    /// <summary>
    /// Gets the API version to use.
    /// </summary>
    private string ApiVersion { get; }

    /// <summary>
    /// Gets the <see cref="ILogger"/> to use.
    /// </summary>
    private ILogger Logger { get; }

    /// <summary>
    /// Gets the <see cref="IRegistry"/> to use.
    /// </summary>
    private IRegistry Registry { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        DisposeInternal();
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
#if NET6_0_OR_GREATER
        return Encoding.Unicode.GetString(bytes).TrimEnd('\0');
#else
        return Encoding.Unicode.GetString(bytes).TrimEnd(_nullArray);
#endif
    }

    /// <summary>
    /// Creates a new instance of SQL Server LocalDB.
    /// </summary>
    /// <param name="wszVersion">The LocalDB version, for example 11.0 or 11.0.1094.2.</param>
    /// <param name="pInstanceName">The name for the LocalDB instance to create.</param>
    /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
    /// <returns>The HRESULT returned by the LocalDB API.</returns>
    internal int CreateInstance(string wszVersion, string pInstanceName, int dwFlags)
    {
        return EnsureFunctionAndInvoke(
            "LocalDBCreateInstance",
            ref _localDBCreateInstance,
            (function) => function!(wszVersion, pInstanceName, dwFlags));
    }

    /// <summary>
    /// Deletes the specified SQL Server Express LocalDB instance.
    /// </summary>
    /// <param name="pInstanceName">The name of the LocalDB instance to delete.</param>
    /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
    /// <returns>The HRESULT returned by the LocalDB API.</returns>
    internal int DeleteInstance(string pInstanceName, int dwFlags)
    {
        return EnsureFunctionAndInvoke(
            "LocalDBDeleteInstance",
            ref _localDBDeleteInstance,
            (function) => function!(pInstanceName, dwFlags));
    }

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
    {
        return EnsureFunctionAndInvoke(
            "LocalDBGetInstanceInfo",
            ref _localDBGetInstanceInfo,
            (function) => function!(wszInstanceName, pInstanceInfo, dwInstanceInfoSize));
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
    internal int GetInstanceNames(IntPtr pInstanceNames, ref int lpdwNumberOfInstances)
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
    internal int GetLocalDbError(int hrLocalDB, int dwLanguageId, StringBuilder wszMessage, ref int lpcchMessage)
    {
        var function = EnsureFunction("LocalDBFormatMessage", ref _localDBFormatMessage);

        if (function == null)
        {
            return SqlLocalDbErrors.NotInstalled;
        }

        return function(hrLocalDB, LocalDbTruncateErrorMessage, dwLanguageId, wszMessage, ref lpcchMessage);
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
    internal int GetVersionInfo(string wszVersionName, IntPtr pVersionInfo, int dwVersionInfoSize)
    {
        return EnsureFunctionAndInvoke(
            "LocalDBGetVersionInfo",
            ref _localDBGetVersionInfo,
            (function) => function!(wszVersionName, pVersionInfo, dwVersionInfoSize));
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
    internal int GetVersions(IntPtr pVersion, ref int lpdwNumberOfVersions)
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
    /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
    /// <returns>The HRESULT returned by the LocalDB API.</returns>
    internal int ShareInstance(IntPtr pOwnerSID, string pInstancePrivateName, string pInstanceSharedName, int dwFlags)
    {
        return EnsureFunctionAndInvoke(
            "LocalDBShareInstance",
            ref _localDBShareInstance,
            (function) => function!(pOwnerSID, pInstancePrivateName, pInstanceSharedName, dwFlags));
    }

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
    {
        var function = EnsureFunction("LocalDBStartInstance", ref _localDBStartInstance);

        if (function == null)
        {
            return SqlLocalDbErrors.NotInstalled;
        }

        return function(pInstanceName, dwFlags, wszSqlConnection, ref lpcchSqlConnection);
    }

    /// <summary>
    /// Enables tracing of API calls for all the SQL Server Express
    /// LocalDB instances owned by the current Windows user.
    /// </summary>
    /// <returns>The HRESULT returned by the LocalDB API.</returns>
    internal int StartTracing()
    {
        return EnsureFunctionAndInvoke(
            "LocalDBStartTracing",
            ref _localDBStartTracing,
            (function) => function!());
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
    internal int StopInstance(string pInstanceName, StopInstanceOptions options, int ulTimeout)
    {
        return EnsureFunctionAndInvoke(
            "LocalDBStopInstance",
            ref _localDBStopInstance,
            (function) => function!(pInstanceName, (int)options, ulTimeout));
    }

    /// <summary>
    /// Disables tracing of API calls for all the SQL Server Express LocalDB
    /// instances owned by the current Windows user.
    /// </summary>
    /// <returns>The HRESULT returned by the LocalDB API.</returns>
    internal int StopTracing()
    {
        return EnsureFunctionAndInvoke(
            "LocalDBStopTracing",
            ref _localDBStopTracing,
            (function) => function!());
    }

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
    {
        return EnsureFunctionAndInvoke(
            "LocalDBUnshareInstance",
            ref _localDBUnshareInstance,
            (function) => function!(pInstanceName, dwFlags));
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
    internal bool TryGetLocalDbApiPath(out string? fileName)
    {
        fileName = null;

        string keyName = DeriveLocalDbRegistryKey();
        using var key = Registry.OpenSubKey(keyName);

        if (key == null)
        {
            Logger.RegistryKeyNotFound(keyName);
            return false;
        }

        Version? latestVersion = null;
        Version? overrideVersion = null;
        string? path = null;

        // Is there a setting overriding the version to load?
        string overrideVersionString = ApiVersion;

        foreach (string versionString in key.GetSubKeyNames())
        {
            if (!Version.TryParse(versionString, out Version? version))
            {
                Logger.InvalidRegistryKey(versionString);
                continue;
            }

            if (!string.IsNullOrEmpty(overrideVersionString) &&
                overrideVersion == null &&
                string.Equals(versionString, overrideVersionString, StringComparison.OrdinalIgnoreCase))
            {
                Logger.NativeApiVersionOverriddenByUser(version);
                overrideVersion = version;
            }

            if (latestVersion == null ||
                latestVersion < version)
            {
                latestVersion = version;
            }
        }

        if (!string.IsNullOrEmpty(overrideVersionString) && overrideVersion == null)
        {
            Logger.NativeApiVersionOverrideNotFound(overrideVersionString);
        }

        Version? versionToUse = overrideVersion ?? latestVersion;

        if (versionToUse != null)
        {
            using (IRegistryKey? subkey = key.OpenSubKey(versionToUse.ToString()))
            {
                path = subkey?.GetValue("InstanceAPIPath");
            }

            NativeApiVersion = versionToUse;
        }

        if (string.IsNullOrEmpty(path))
        {
            Logger.NativeApiNotFound();
            return false;
        }

        if (!File.Exists(path))
        {
            Logger.NativeApiLibraryNotFound(path!);
            return false;
        }

        fileName = Path.GetFullPath(path);
        return true;
    }

    /// <summary>
    /// Derives the name of the Windows registry key name to use to locate the SQL LocalDB Instance API.
    /// </summary>
    /// <returns>
    /// The registry key name to use for the current process.
    /// </returns>
    private static string DeriveLocalDbRegistryKey()
    {
        // Open the appropriate Registry key if running as a 32-bit process on a 64-bit machine
        bool isWow64Process = Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess;

        return string.Format(
            CultureInfo.InvariantCulture,
            @"SOFTWARE\{0}Microsoft\Microsoft SQL Server Local DB\Installed Versions",
            isWow64Process ? @"Wow6432Node\" : string.Empty);
    }

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
    private T? EnsureFunction<T>(string functionName, ref T? function)
        where T : class, Delegate?
    {
        Debug.Assert(functionName != null, "functionName cannot be null.");

        if (function == null)
        {
            lock (_syncRoot)
            {
#pragma warning disable CA1508
                if (function == null)
#pragma warning restore CA1508
                {
                    function = GetDelegate<T>(functionName!);
                }
            }
        }

        return function;
    }

    /// <summary>
    /// Ensures that the specified delegate to an unmanaged function is initialized and invokes the specified callback delegate if it does.
    /// </summary>
    /// <typeparam name="T">The type of the delegate representing the unmanaged function.</typeparam>
    /// <param name="functionName">The name of the unmanaged function to ensure is loaded.</param>
    /// <param name="function">A reference to a location to ensure contains a delegate for the specified function name.</param>
    /// <param name="callback">A delegate to a callback method to invoke with the function if initialized.</param>
    /// <returns>
    /// The <see cref="int"/> result of invoking <paramref name="callback"/>, if the function was
    /// initialized; otherwise the value of <see cref="SqlLocalDbErrors.NotInstalled"/> is returned.
    /// </returns>
    private int EnsureFunctionAndInvoke<T>(string functionName, ref T? function, Func<T, int> callback)
        where T : class, Delegate?
    {
        Debug.Assert(callback != null, "callback cannot be null.");

        function = EnsureFunction(functionName, ref function);

        return function == null ? SqlLocalDbErrors.NotInstalled : callback!(function);
    }

    /// <summary>
    /// Ensures that the LocalDB native API has been loaded.
    /// </summary>
    /// <returns>
    /// A <see cref="SafeLibraryHandle"/> pointing to the loaded
    /// SQL LocalDB API, if successful; otherwise <see langword="null"/>.
    /// </returns>
    private SafeLibraryHandle? EnsureLocalDBLoaded()
    {
        if (_handle == null)
        {
            lock (_syncRoot)
            {
                if (_handle == null)
                {
                    if (!TryGetLocalDbApiPath(out string? fileName) || fileName == null)
                    {
                        return null;
                    }

#if NET6_0_OR_GREATER
                    if (!NativeLibrary.TryLoad(
                            fileName,
                            typeof(LocalDbInstanceApi).Assembly,
                            DllImportSearchPath.UserDirectories,
                            out IntPtr handle))
                    {
                        int error = Marshal.GetLastWin32Error();
                        Logger.NativeApiLoadFailed(fileName, error);
                        _handle = null;
                    }
                    else
                    {
                        _handle = new SafeLibraryHandle(handle);
                        _libraryPath = fileName;
                        Logger.NativeApiLoaded(fileName);
                    }
#else
                    // Check if the local machine has KB2533623 installed in order
                    // to use the more secure flags when calling LoadLibraryEx
                    bool hasKB2533623;

                    using (var hModule = NativeMethods.LoadLibraryEx(NativeMethods.KernelLibName, IntPtr.Zero, 0))
                    {
                        // If the AddDllDirectory function is found then the flags are supported
                        hasKB2533623 = NativeMethods.GetProcAddress(hModule, "AddDllDirectory") != IntPtr.Zero;
                    }

                    int dwFlags = 0;

                    if (hasKB2533623)
                    {
                        // If KB2533623 is installed then specify the more secure LOAD_LIBRARY_SEARCH_DEFAULT_DIRS in dwFlags
                        dwFlags = NativeMethods.LOAD_LIBRARY_SEARCH_DEFAULT_DIRS;
                    }

                    _handle = NativeMethods.LoadLibraryEx(fileName, IntPtr.Zero, dwFlags);

                    if (_handle.IsInvalid)
                    {
                        int error = Marshal.GetLastWin32Error();
                        Logger.NativeApiLoadFailed(fileName, error);
                        _handle = null;
                    }
                    else
                    {
                        Logger.NativeApiLoaded(fileName);
                        _libraryPath = fileName;
                    }
#endif
                }
            }
        }

        return _handle;
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
    private T? GetDelegate<T>(string functionName)
        where T : class, Delegate?
    {
        Debug.Assert(functionName != null, "functionName cannot be null.");

        SafeLibraryHandle? handle = EnsureLocalDBLoaded();

        if (handle == null)
        {
            Logger.NativeApiNotLoaded();
            return null;
        }

#if NET6_0_OR_GREATER
        if (!NativeLibrary.TryGetExport(handle.DangerousGetHandle(), functionName, out IntPtr address))
        {
            Logger.NativeApiFunctionNotFound(functionName);
            return null;
        }
#else

        IntPtr address = NativeMethods.GetProcAddress(handle, functionName!);

        if (address == IntPtr.Zero)
        {
            Logger.NativeApiFunctionNotFound(functionName!);
            return null;
        }
#endif

        return Marshal.GetDelegateForFunctionPointer<T>(address);
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    private void DisposeInternal()
    {
        if (!_disposed)
        {
            if (_handle != null)
            {
                _handle.Dispose();

                Logger.NativeApiUnloaded(_libraryPath!);

                _libraryPath = null;
            }

            _disposed = true;
        }
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
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbcreateinstance-function</c>.
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDBCreateInstance(
            [MarshalAs(UnmanagedType.LPWStr)] string wszVersion,
            [MarshalAs(UnmanagedType.LPWStr)] string pInstanceName,
            int dwFlags);

        /// <summary>
        /// Deletes the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to delete.</param>
        /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        /// <remarks>
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbdeleteinstance-function</c>.
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDBDeleteInstance(
            [MarshalAs(UnmanagedType.LPWStr)] string pInstanceName,
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
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbformatmessage-function</c>.
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDBFormatMessage(
            int hrLocalDB,
            int dwFlags,
            int dwLanguageId,
            [MarshalAs(UnmanagedType.LPWStr)][Out] StringBuilder wszMessage,
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
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbgetinstanceinfo-function</c>.
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDBGetInstanceInfo(
            [MarshalAs(UnmanagedType.LPWStr)] string wszInstanceName,
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
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbgetinstances-function</c>.
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
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbgetversioninfo-function</c>.
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDBGetVersionInfo(
            [MarshalAs(UnmanagedType.LPWStr)] string wszVersionName,
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
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbgetversions-function</c>.
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
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbshareinstance-functionN</c>.
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDBShareInstance(
            IntPtr pOwnerSID,
            [MarshalAs(UnmanagedType.LPWStr)] string pInstancePrivateName,
            [MarshalAs(UnmanagedType.LPWStr)] string pInstanceSharedName,
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
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbstartinstance-function</c>.
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDBStartInstance(
            [MarshalAs(UnmanagedType.LPWStr)] string pInstanceName,
            int dwFlags,
            [MarshalAs(UnmanagedType.LPWStr)][Out] StringBuilder wszSqlConnection,
            ref int lpcchSqlConnection);

        /// <summary>
        /// Enables tracing of API calls for all the SQL Server Express
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        /// <remarks>
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbstarttracing-function</c>.
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
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbstopinstance-function</c>.
        /// </remarks>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate int LocalDBStopInstance(
            [MarshalAs(UnmanagedType.LPWStr)] string pInstanceName,
            int dwFlags,
            int ulTimeout);

        /// <summary>
        /// Disables tracing of API calls for all the SQL Server Express LocalDB
        /// instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        /// <remarks>
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbstoptracing-function</c>.
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
        /// See <c>https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/localdbunshareinstance-function</c>.
        /// </remarks>
        internal delegate int LocalDBUnshareInstance(
            [MarshalAs(UnmanagedType.LPWStr)] string pInstanceName,
            int dwFlags);
    }
}
