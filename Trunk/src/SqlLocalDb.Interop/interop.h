// --------------------------------------------------------------------------------------------------------------------
// <copyright file="interop.h" company="http://sqllocaldb.codeplex.com">
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
