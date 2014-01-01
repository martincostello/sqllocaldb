// --------------------------------------------------------------------------------------------------------------------
// <copyright file="interop.h" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   interop.h
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#define LOCALDB_DEFINE_PROXY_FUNCTIONS

extern HRESULT APIENTRY CreateInstance(
    PCWSTR wszVersion,
    PCWSTR pInstanceName);

extern HRESULT APIENTRY DeleteInstance(
    PCWSTR pInstanceName);

extern HRESULT APIENTRY GetInstanceInfo(
    PCWSTR wszInstanceName,
    void* pInstanceInfo,
    DWORD dwInstanceInfoSize);

extern HRESULT APIENTRY GetInstanceNames(
    void* pInstanceNames,
    LPDWORD lpdwNumberOfInstances);

extern HRESULT APIENTRY GetLocalDbError(
    HRESULT hrLocalDB,
    DWORD dwLanguageId, 
    LPWSTR wszMessage, 
    LPDWORD lpcchMessage);

extern HRESULT APIENTRY GetVersionInfo(
    PCWSTR wszVersionName,
    void* pVersionInfo,
    DWORD dwVersionInfoSize);

extern HRESULT APIENTRY GetVersions(
    void* pVersion,
    LPDWORD lpdwNumberOfVersions);

extern HRESULT APIENTRY ShareInstance(
    void* pOwnerSID,
    PCWSTR pInstancePrivateName,
    PCWSTR pInstanceSharedName);

extern HRESULT APIENTRY StartInstance(
    PCWSTR pInstanceName,
    LPWSTR wszSqlConnection, 
    LPDWORD lpcchSqlConnection);

extern HRESULT APIENTRY StartTracing(void);

extern HRESULT APIENTRY StopInstance(
    PCWSTR pInstanceName,
    ULONG ulTimeout);

extern HRESULT APIENTRY StopTracing(void);

extern HRESULT APIENTRY UnshareInstance(
    PCWSTR pInstanceName);
