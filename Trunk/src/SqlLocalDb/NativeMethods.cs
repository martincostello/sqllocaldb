// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="http://sqllocaldb.codeplex.com">
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
//   NativeMethods.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;
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
        /// The name of the SQL Server LocalDB API wrapper DLL.
        /// </summary>
        private const string InteropLibName = "System.Data.SqlLocalDb.Interop.dll";

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="wszVersion">The LocalDB version, for example 11.0 or 11.0.1094.2.</param>
        /// <param name="pInstanceName">The name for the LocalDB instance to create.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int CreateInstance(
            [MarshalAs(UnmanagedType.LPWStr)]
            string wszVersion,
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName);

        /// <summary>
        /// Deletes the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to delete.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int DeleteInstance(
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
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int GetInstanceInfo(
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
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int GetInstanceNames(
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
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int GetLocalDbError(
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
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int GetVersionInfo(
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
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int GetVersions(
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
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int ShareInstance(
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
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int StartInstance(
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
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int StartTracing();

        /// <summary>
        /// Stops the specified SQL Server Express LocalDB instance from running.
        /// </summary>
        /// <param name="pInstanceName">The name of the LocalDB instance to stop.</param>
        /// <param name="ulTimeout">
        /// The time in seconds to wait for this operation to complete. If this
        /// value is 0, this function will return immediately without waiting for the LocalDB instance to stop.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int StopInstance(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName,
            int ulTimeout);

        /// <summary>
        /// Disables tracing of API calls for all the SQL Server Express LocalDB
        /// instances owned by the current Windows user.
        /// </summary>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int StopTracing();

        /// <summary>
        /// Stops the sharing of the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">
        /// The private name for the LocalDB instance to share.
        /// </param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        [DllImport(InteropLibName, CharSet = CharSet.Auto)]
        [SecurityCritical]
        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Security",
            "CA5122:PInvokesShouldNotBeSafeCriticalFxCopRule",
            Justification = "Doesn't apply to .NET 3.5 assemblies. See http://connect.microsoft.com/VisualStudio/feedback/details/729254/bogus-ca5122-warning-about-p-invoke-declarations-should-not-be-safe-critical.")]
        internal static extern int UnshareInstance(
            [MarshalAs(UnmanagedType.LPWStr)]
            string pInstanceName);

        #endregion
    }
}