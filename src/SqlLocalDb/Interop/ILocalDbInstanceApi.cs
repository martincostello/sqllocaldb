// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Runtime.InteropServices;
using System.Text;
using AdvancedDLSupport;

namespace MartinCostello.SqlLocalDb.Interop
{
    /// <summary>
    /// Defines the native SQL Server LocalDB Instance API.
    /// </summary>
    [NativeSymbols(Prefix = "LocalDB")]
    internal interface ILocalDbInstanceApi : IDisposable
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
        int CreateInstance(
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
        /// See <c>http://technet.microsoft.com/en-us/library/hh214724.aspx</c>.
        /// </remarks>
        int DeleteInstance(
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
        /// See <c>http://technet.microsoft.com/en-us/library/hh214483.aspx</c>.
        /// </remarks>
        int FormatMessage(
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
        /// See <c>http://technet.microsoft.com/en-us/library/hh245734.aspx</c>.
        /// </remarks>
        int GetInstanceInfo(
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
        /// See <c>http://technet.microsoft.com/en-us/library/hh234622.aspx</c>.
        /// </remarks>
        int GetInstances(IntPtr pInstanceNames, ref int lpdwNumberOfInstances);

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
        int GetVersionInfo(
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
        /// See <c>http://technet.microsoft.com/en-us/library/hh231031.aspx</c>.
        /// </remarks>
        int GetVersions(IntPtr pVersion, ref int lpdwNumberOfVersions);

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
        int ShareInstance(
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
        /// See <c>http://technet.microsoft.com/en-us/library/hh217143.aspx</c>.
        /// </remarks>
        int StartInstance(
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
        /// See <c>http://technet.microsoft.com/en-us/library/hh247594.aspx</c>.
        /// </remarks>
        int StartTracing();

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
        int StopInstance(
            [MarshalAs(UnmanagedType.LPWStr)] string pInstanceName,
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
        int StopTracing();

        /// <summary>
        /// Stops the sharing of the specified SQL Server Express LocalDB instance.
        /// </summary>
        /// <param name="pInstanceName">The private name for the LocalDB instance to share.</param>
        /// <param name="dwFlags">Reserved for future use. Currently should be set to 0.</param>
        /// <returns>The HRESULT returned by the LocalDB API.</returns>
        /// <remarks>
        /// See <c>http://technet.microsoft.com/en-us/library/hh215383.aspx</c>.
        /// </remarks>
        int UnshareInstance(
            [MarshalAs(UnmanagedType.LPWStr)] string pInstanceName,
            int dwFlags);
    }
}
