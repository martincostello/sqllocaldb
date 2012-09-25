// --------------------------------------------------------------------------------------------------------------------
// <copyright file="interop.cpp" company="http://sqllocaldb.codeplex.com">
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
//   interop.cpp : Defines the exported functions for the DLL application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#include "stdafx.h"
#include "interop.h"
#include "sqlncli.h"

//
// Creates a new instance of SQL Server LocalDB.
//
// Parameters:
// wszVersion:    The LocalDB version, for example 11.0 or 11.0.1094.2.
// pInstanceName: The name for the LocalDB instance to create.
//
// Returns:       The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY CreateInstance(
    PCWSTR wszVersion,
    PCWSTR pInstanceName)
{
    return LocalDBCreateInstance(
        wszVersion,
        pInstanceName,
        0);							// Reserved
}

//
// Deletes the specified SQL Server Express LocalDB instance.
//
// Parameters:
// pInstanceName: The name of the LocalDB instance to delete.
//
// Returns:       The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY DeleteInstance(
    PCWSTR pInstanceName)
{
    return LocalDBDeleteInstance(
        pInstanceName,
        0);							// Reserved
}

//
// Returns information for the specified SQL Server Express LocalDB instance,
// such as whether it exists, the LocalDB version it uses, whether it is running, and so on.
//
// Parameters:
// wszInstanceName:    The instance name.
// pInstanceInfo       The buffer to store the information about the LocalDB instance.
// dwInstanceInfoSize: Holds the size of the InstanceInfo buffer.
//
// Returns:            The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY GetInstanceInfo(
    PCWSTR wszInstanceName,
    void* pInstanceInfo,
    DWORD dwInstanceInfoSize)
{
    size_t s = sizeof(LocalDBInstanceInfo);

    if (s != NULL)
    {
    }

    return LocalDBGetInstanceInfo(
        wszInstanceName,
        (PLocalDBInstanceInfo)pInstanceInfo,
        dwInstanceInfoSize);
}

//
// Returns all SQL Server Express LocalDB instances with the given version.
//
// Parameters:
// pInstanceNames:        When this function returns, contains the names of both named and
//                        default LocalDB instances on the user’s workstation.
// lpdwNumberOfInstances: On input, contains the number of slots for instance
//                        names in the pInstanceNames buffer. On output, contains
//                        the number of LocalDB instances found on the user’s workstation.
//
// Returns:               The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY GetInstanceNames(
    void* pInstanceNames,
    LPDWORD lpdwNumberOfInstances)
{
    return LocalDBGetInstances(
           (PTLocalDBInstanceName)pInstanceNames,
           lpdwNumberOfInstances);
}

//
// Returns the localized textual description for the specified SQL Server Express LocalDB error.
//
// Parameters:
// hrLocalDB:    The LocalDB error code.
// dwLanguageId: The language desired (LANGID) or 0, in which case the Win32 FormatMessage language order is used.
// wszMessage:   The buffer to store the LocalDB error message.
// lpcchMessage: On input contains the size of the wszMessage buffer in characters. On output, if the given buffer
//               size is too small, contains the buffer size required in characters, including any trailing nulls.
//               If the function succeeds, contains the number of characters in the message, excluding any trailing nulls.
//
// Returns:      The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY GetLocalDbError(
    HRESULT hrLocalDB,
    DWORD dwLanguageId, 
    LPWSTR wszMessage, 
    LPDWORD lpcchMessage)
{
    return LocalDBFormatMessage(
        hrLocalDB,
        LOCALDB_TRUNCATE_ERR_MESSAGE, 
        dwLanguageId, 
        wszMessage, 
        lpcchMessage);
}

//
// Returns information for the specified SQL Server Express LocalDB version,
// such as whether it exists and the full LocalDB version number (including
// build and release numbers).
//
// Parameters:
// wszVersionName:    The LocalDB version name.
// pVersionInfo:      The buffer to store the information about the LocalDB version.
// dwVersionInfoSize: Holds the size of the VersionInfo buffer.
//
// Returns:           The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY GetVersionInfo(
    PCWSTR wszVersionName,
    void* pVersionInfo,
    DWORD dwVersionInfoSize)
{
    return LocalDBGetVersionInfo(
        wszVersionName,
        (PLocalDBVersionInfo)pVersionInfo,
        dwVersionInfoSize);
}

//
// Returns all SQL Server Express LocalDB versions available on the computer.
// 
// Parameters:
// pVersion:             Contains names of the LocalDB versions that are available on the user’s workstation.
// lpdwNumberOfVersions: On input holds the number of slots for versions in the pVersionNames buffer. On output,
//                       holds the number of existing LocalDB versions.
//
// Returns:              The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY GetVersions(
    void* pVersion,
    LPDWORD lpdwNumberOfVersions)
{
    return LocalDBGetVersions(
        (PTLocalDBVersion)pVersion,
        lpdwNumberOfVersions);
}

//
// Shares the specified SQL Server Express LocalDB instance with other users
// of the computer, using the specified shared name.
//
// Parameters:
// pOwnerSID:            The SID of the instance owner.
// pInstancePrivateName: The private name for the LocalDB instance to share.
// pInstanceSharedName:  The shared name for the LocalDB instance to share.
//
// Returns:              The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY ShareInstance(
    void* pOwnerSID,
    PCWSTR pInstancePrivateName,
    PCWSTR pInstanceSharedName)
{
    return LocalDBShareInstance(
        (PSID)pOwnerSID,
        pInstancePrivateName,
        pInstanceSharedName,
        0);	// Reserved
}

// Starts the specified SQL Server Express LocalDB instance.
//
// Parameters:
// pInstanceName:      The name of the LocalDB instance to start.
// wszSqlConnection:   The buffer to store the connection string to the LocalDB instance.
// lpcchSqlConnection: On input contains the size of the wszSqlConnection buffer in characters,
//                     including any trailing nulls. On output, if the given buffer size is too
//                     small, contains the required buffer size in characters, including any
//                     trailing nulls.
//
// Returns:            The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY StartInstance(
    PCWSTR pInstanceName,
    LPWSTR wszSqlConnection, 
    LPDWORD lpcchSqlConnection)
{
    return LocalDBStartInstance(
        pInstanceName,
        0,							// Reserved
        wszSqlConnection,
        lpcchSqlConnection);
}

//
// Enables tracing of API calls for all the SQL Server Express
// LocalDB instances owned by the current Windows user.
//
// Returns: The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY StartTracing(void)
{
    return LocalDBStartTracing();
}

//
// Stops the specified SQL Server Express LocalDB instance from running.
//
// Parameters:
// pInstanceName: The name of the LocalDB instance to stop.
// ulTimeout :    The time in seconds to wait for this operation to complete.
//                If this value is 0, this function will return immediately
//                without waiting for the LocalDB instance to stop.
//
// Returns:       The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY StopInstance(
    PCWSTR pInstanceName,
    ULONG ulTimeout)
{
    return LocalDBStopInstance(
        pInstanceName,
        0,						// Reserved
        ulTimeout);
}

//
// Disables tracing of API calls for all the SQL Server
// Express LocalDB instances owned by the current Windows user.
//
// Returns: The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY StopTracing(void)
{
    return LocalDBStopTracing();
}

//
// Stops the sharing of the specified SQL Server Express LocalDB instance.
//
// Parameters:
// pInstancedName: The private name for the LocalDB instance to share.
//
// Returns:        The HRESULT returned by the LocalDB API.
//
extern HRESULT APIENTRY UnshareInstance(
    PCWSTR pInstanceName)
{
    return LocalDBUnshareInstance(
        pInstanceName,
        0);	// Reserved
}
