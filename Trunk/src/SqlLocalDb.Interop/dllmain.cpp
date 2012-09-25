// --------------------------------------------------------------------------------------------------------------------
// <copyright file="dllmain.cpp" company="http://sqllocaldb.codeplex.com">
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
//   dllmain.cpp : Defines the entry point for the DLL application.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#include "stdafx.h"

BOOL APIENTRY DllMain(
    HMODULE hModule,
    DWORD ul_reason_for_call,
    LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }

    return TRUE;
}
