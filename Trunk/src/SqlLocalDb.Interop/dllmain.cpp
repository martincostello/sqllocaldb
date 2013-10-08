// --------------------------------------------------------------------------------------------------------------------
// <copyright file="dllmain.cpp" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2013
// </copyright>
// <license>
//   See license.txt in the project root for license information.
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
