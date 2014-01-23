// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   NativeMethods.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing native P/Invoke methods.  This class cannot be inherited.
    /// </summary>
    [SecurityCritical]
    internal static class NativeMethods
    {
        #region Constants

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
        /// The name of the x86 SQL Server LocalDB API wrapper DLL.
        /// </summary>
        private const string InteropLibName32 = "System.Data.SqlLocalDb.Interop.x86.dll";

        /// <summary>
        /// The name of the x64 SQL Server LocalDB API wrapper DLL.
        /// </summary>
        private const string InteropLibName64 = "System.Data.SqlLocalDb.Interop.x64.dll";

        #endregion

        #region Fields

        /// <summary>
        /// Whether the currently executing process is 64-bit.
        /// </summary>
        private static readonly bool _is64Bit = IntPtr.Size == 8;

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="wszVersion">The LocalDB version, for example 11.0 or 11.0.1094.2.</param>
        /// <param name="pInstanceName">The name for the LocalDB instance to create.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int CreateInstance(string wszVersion, string pInstanceName)
        {
            return _is64Bit ?
                CreateInstance64(wszVersion, pInstanceName) :
                CreateInstance32(wszVersion, pInstanceName);
        }

        /// <summary>
        /// Deletes the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to delete.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int DeleteInstance(string pInstanceName)
        {
            return _is64Bit ?
                DeleteInstance64(pInstanceName) :
                DeleteInstance32(pInstanceName);
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
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int GetInstanceInfo(string wszInstanceName, IntPtr pInstanceInfo, int dwInstanceInfoSize)
        {
            return _is64Bit ?
                GetInstanceInfo64(wszInstanceName, pInstanceInfo, dwInstanceInfoSize) :
                GetInstanceInfo32(wszInstanceName, pInstanceInfo, dwInstanceInfoSize);
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
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int GetInstanceNames(IntPtr pInstanceNames, ref int lpdwNumberOfInstances)
        {
            return _is64Bit ?
                GetInstanceNames64(pInstanceNames, ref lpdwNumberOfInstances) :
                GetInstanceNames32(pInstanceNames, ref lpdwNumberOfInstances);
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
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int GetLocalDbError(int hrLocalDB, int dwLanguageId, StringBuilder wszMessage, ref int lpcchMessage)
        {
            return _is64Bit ?
                GetLocalDbError64(hrLocalDB, dwLanguageId, wszMessage, ref lpcchMessage) :
                GetLocalDbError32(hrLocalDB, dwLanguageId, wszMessage, ref lpcchMessage);
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
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int GetVersionInfo(string wszVersionName, IntPtr pVersionInfo, int dwVersionInfoSize)
        {
            return _is64Bit ?
                GetVersionInfo64(wszVersionName, pVersionInfo, dwVersionInfoSize) :
                GetVersionInfo32(wszVersionName, pVersionInfo, dwVersionInfoSize);
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
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int GetVersions(IntPtr pVersion, ref int lpdwNumberOfVersions)
        {
            return _is64Bit ?
                GetVersions64(pVersion, ref lpdwNumberOfVersions) :
                GetVersions32(pVersion, ref lpdwNumberOfVersions);
        }

        /// <summary>
        /// Shares the specified SQL Server Express LocalDB instance with other
        /// users of the computer, using the specified shared name.
        /// </summary>
        /// <param name="pOwnerSID">The SID of the instance owner.</param>
        /// <param name="pInstancePrivateName">The private name for the LocalDB instance to share.</param>
        /// <param name="pInstanceSharedName">The shared name for the LocalDB instance to share.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int ShareInstance(IntPtr pOwnerSID, string pInstancePrivateName, string pInstanceSharedName)
        {
            return _is64Bit ?
                ShareInstance64(pOwnerSID, pInstancePrivateName, pInstanceSharedName) :
                ShareInstance32(pOwnerSID, pInstancePrivateName, pInstanceSharedName);
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
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int StartInstance(string pInstanceName, StringBuilder wszSqlConnection, ref int lpcchSqlConnection)
        {
            return _is64Bit ?
                StartInstance64(pInstanceName, wszSqlConnection, ref lpcchSqlConnection) :
                StartInstance32(pInstanceName, wszSqlConnection, ref lpcchSqlConnection);
        }

        /// <summary>
        /// Enables tracing of API calls for all the SQL Server Express
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int StartTracing()
        {
            return _is64Bit ?
                StartTracing64() :
                StartTracing32();
        }

        /// <summary>
        /// Stops the specified SQL Server Express LocalDB instance from running.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to stop.</param>
        /// <param name="ulTimeout">
        /// The time in seconds to wait for this operation to complete. If this
        /// value is 0, this function will return immediately without waiting for the LocalDB instance to stop.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int StopInstance(string pInstanceName, int ulTimeout)
        {
            return _is64Bit ?
                StopInstance64(pInstanceName, ulTimeout) :
                StopInstance32(pInstanceName, ulTimeout);
        }

        /// <summary>
        /// Disables tracing of API calls for all the SQL Server Express LocalDB
        /// instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int StopTracing()
        {
            return _is64Bit ?
                StopTracing64() :
                StopTracing32();
        }

        /// <summary>
        /// Stops the sharing of the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">
        /// The private name for the LocalDB instance to share.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [SecuritySafeCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Portability",
            "CA1903:UseOnlyApiFromTargetedFramework",
            MessageId = "System.Security.SecuritySafeCriticalAttribute",
            Justification = ".NET 3.5 SP1 is required if using Entity Framework.")]
        internal static int UnshareInstance(string pInstanceName)
        {
            return _is64Bit ?
                UnshareInstance64(pInstanceName) :
                UnshareInstance32(pInstanceName);
        }

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="wszVersion">The LocalDB version, for example 11.0 or 11.0.1094.2.</param>
        /// <param name="pInstanceName">The name for the LocalDB instance to create.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "CreateInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int CreateInstance32(
            [MarshalAs(UnmanagedType.LPWStr)]
            string wszVersion,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName);

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="wszVersion">The LocalDB version, for example 11.0 or 11.0.1094.2.</param>
        /// <param name="pInstanceName">The name for the LocalDB instance to create.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "CreateInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int CreateInstance64(
            [MarshalAs(UnmanagedType.LPWStr)]
            string wszVersion,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName);

        /// <summary>
        /// Deletes the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to delete.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "DeleteInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int DeleteInstance32(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName);

        /// <summary>
        /// Deletes the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to delete.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "DeleteInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int DeleteInstance64(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName);

        /// <summary>
        /// Returns information for the specified SQL Server Express LocalDB instance,
        /// such as whether it exists, the LocalDB version it uses, whether it is running,
        /// and so on.
        /// </summary>
        /// <param name="wszInstanceName">The instance name.</param>
        /// <param name="pInstanceInfo">The buffer to store the information about the LocalDB instance.</param>
        /// <param name="dwInstanceInfoSize">Holds the size of the InstanceInfo buffer.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "GetInstanceInfo")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int GetInstanceInfo32(
            [MarshalAs(UnmanagedType.LPWStr)]
            string wszInstanceName,
            IntPtr pInstanceInfo,
            int dwInstanceInfoSize);

        /// <summary>
        /// Returns information for the specified SQL Server Express LocalDB instance,
        /// such as whether it exists, the LocalDB version it uses, whether it is running,
        /// and so on.
        /// </summary>
        /// <param name="wszInstanceName">The instance name.</param>
        /// <param name="pInstanceInfo">The buffer to store the information about the LocalDB instance.</param>
        /// <param name="dwInstanceInfoSize">Holds the size of the InstanceInfo buffer.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "GetInstanceInfo")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int GetInstanceInfo64(
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
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "GetInstanceNames")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int GetInstanceNames32(
            IntPtr pInstanceNames,
            ref int lpdwNumberOfInstances);

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
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "GetInstanceNames")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int GetInstanceNames64(
            IntPtr pInstanceNames,
            ref int lpdwNumberOfInstances);

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
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "GetLocalDbError")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int GetLocalDbError32(
            int hrLocalDB,
            int dwLanguageId,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder wszMessage,
            ref int lpcchMessage);

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
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "GetLocalDbError")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int GetLocalDbError64(
            int hrLocalDB,
            int dwLanguageId,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder wszMessage,
            ref int lpcchMessage);

        /// <summary>
        /// Returns information for the specified SQL Server Express LocalDB version,
        /// such as whether it exists and the full LocalDB version number (including
        /// build and release numbers).
        /// </summary>
        /// <param name="wszVersionName">The LocalDB version name.</param>
        /// <param name="pVersionInfo">The buffer to store the information about the LocalDB version.</param>
        /// <param name="dwVersionInfoSize">Holds the size of the VersionInfo buffer.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "GetVersionInfo")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int GetVersionInfo32(
            [MarshalAs(UnmanagedType.LPWStr)]
            string wszVersionName,
            IntPtr pVersionInfo,
            int dwVersionInfoSize);

        /// <summary>
        /// Returns information for the specified SQL Server Express LocalDB version,
        /// such as whether it exists and the full LocalDB version number (including
        /// build and release numbers).
        /// </summary>
        /// <param name="wszVersionName">The LocalDB version name.</param>
        /// <param name="pVersionInfo">The buffer to store the information about the LocalDB version.</param>
        /// <param name="dwVersionInfoSize">Holds the size of the VersionInfo buffer.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "GetVersionInfo")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int GetVersionInfo64(
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
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "GetVersions")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int GetVersions32(
            IntPtr pVersion,
            ref int lpdwNumberOfVersions);

        /// <summary>
        /// Returns all SQL Server Express LocalDB versions available on the computer.
        /// </summary>
        /// <param name="pVersion">Contains names of the LocalDB versions that are available on the user’s workstation.</param>
        /// <param name="lpdwNumberOfVersions">
        /// On input holds the number of slots for versions in the <paramref name="pVersion"/>
        /// buffer. On output, holds the number of existing LocalDB versions.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "GetVersions")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int GetVersions64(
            IntPtr pVersion,
            ref int lpdwNumberOfVersions);

        /// <summary>
        /// Shares the specified SQL Server Express LocalDB instance with other
        /// users of the computer, using the specified shared name.
        /// </summary>
        /// <param name="pOwnerSID">The SID of the instance owner.</param>
        /// <param name="pInstancePrivateName">The private name for the LocalDB instance to share.</param>
        /// <param name="pInstanceSharedName">The shared name for the LocalDB instance to share.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "ShareInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int ShareInstance32(
            IntPtr pOwnerSID,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstancePrivateName,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceSharedName);

        /// <summary>
        /// Shares the specified SQL Server Express LocalDB instance with other
        /// users of the computer, using the specified shared name.
        /// </summary>
        /// <param name="pOwnerSID">The SID of the instance owner.</param>
        /// <param name="pInstancePrivateName">The private name for the LocalDB instance to share.</param>
        /// <param name="pInstanceSharedName">The shared name for the LocalDB instance to share.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "ShareInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int ShareInstance64(
            IntPtr pOwnerSID,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstancePrivateName,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceSharedName);

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
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "StartInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int StartInstance32(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder wszSqlConnection,
            ref int lpcchSqlConnection);

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
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "StartInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int StartInstance64(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName,
            [MarshalAs(UnmanagedType.LPWStr)]
            StringBuilder wszSqlConnection,
            ref int lpcchSqlConnection);

        /// <summary>
        /// Enables tracing of API calls for all the SQL Server Express
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "StartTracing")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int StartTracing32();

        /// <summary>
        /// Enables tracing of API calls for all the SQL Server Express
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "StartTracing")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int StartTracing64();

        /// <summary>
        /// Stops the specified SQL Server Express LocalDB instance from running.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to stop.</param>
        /// <param name="ulTimeout">
        /// The time in seconds to wait for this operation to complete. If this
        /// value is 0, this function will return immediately without waiting for the LocalDB instance to stop.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "StopInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int StopInstance32(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName,
            int ulTimeout);

        /// <summary>
        /// Stops the specified SQL Server Express LocalDB instance from running.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to stop.</param>
        /// <param name="ulTimeout">
        /// The time in seconds to wait for this operation to complete. If this
        /// value is 0, this function will return immediately without waiting for the LocalDB instance to stop.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "StopInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int StopInstance64(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName,
            int ulTimeout);

        /// <summary>
        /// Disables tracing of API calls for all the SQL Server Express LocalDB
        /// instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "StopTracing")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int StopTracing32();

        /// <summary>
        /// Disables tracing of API calls for all the SQL Server Express LocalDB
        /// instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "StopTracing")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int StopTracing64();

        /// <summary>
        /// Stops the sharing of the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">
        /// The private name for the LocalDB instance to share.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName32, CharSet = CharSet.Auto, EntryPoint = "UnshareInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int UnshareInstance32(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName);

        /// <summary>
        /// Stops the sharing of the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">
        /// The private name for the LocalDB instance to share.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName64, CharSet = CharSet.Auto, EntryPoint = "UnshareInstance")]
        [SecurityCritical]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        private static extern int UnshareInstance64(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName);

        #endregion
    }
}