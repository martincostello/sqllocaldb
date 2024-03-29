// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

#if !NET6_0_OR_GREATER

using System.Runtime.InteropServices;

namespace MartinCostello.SqlLocalDb.Interop;

/// <summary>
/// A class containing native P/Invoke methods.  This class cannot be inherited.
/// </summary>
internal static class NativeMethods
{
    /// <summary>
    /// This value represents the recommended maximum number of directories an application should include in its DLL search path.
    /// </summary>
    /// <remarks>
    /// Only supported on Windows Vista, 7, Server 2008 and Server 2008 R2 with KB2533623.
    /// See <c>https://learn.microsoft.com/windows/win32/api/libloaderapi/nf-libloaderapi-loadlibraryexa</c>.
    /// </remarks>
    internal const int LOAD_LIBRARY_SEARCH_DEFAULT_DIRS = 0x00001000;

    /// <summary>
    /// The name of the Windows Kernel library.
    /// </summary>
    internal const string KernelLibName = "kernel32.dll";

    /// <summary>
    /// Frees a specified library.
    /// </summary>
    /// <param name="hModule">The handle to the module to free.</param>
    /// <returns>Whether the library was successfully unloaded.</returns>
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [DllImport(KernelLibName)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool FreeLibrary(IntPtr hModule);

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
    /// See <c>https://learn.microsoft.com/windows/win32/api/libloaderapi/nf-libloaderapi-getprocaddress</c>.
    /// </remarks>
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [DllImport(KernelLibName, BestFitMapping = false, CharSet = CharSet.Ansi, ThrowOnUnmappableChar = true)]
    internal static extern IntPtr GetProcAddress(
        SafeLibraryHandle hModule,
        [MarshalAs(UnmanagedType.LPStr)] string lpProcName);

    /// <summary>
    /// Loads the specified module into the address space of the calling process.
    /// The specified module may cause other modules to be loaded.
    /// </summary>
    /// <param name="lpFileName">The name of the module.</param>
    /// <param name="hFile">This parameter is reserved for future use. It must be <see cref="IntPtr.Zero"/>.</param>
    /// <param name="dwFlags">The action to be taken when loading the module.</param>
    /// <returns>
    /// If the function succeeds, the return value is a handle to the module.
    /// If the function fails, the return value is <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// See <c>https://docs.microsoft.com/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibraryexa</c>.
    /// </remarks>
    [DefaultDllImportSearchPaths(DllImportSearchPath.UserDirectories)]
    [DllImport(KernelLibName, BestFitMapping = false, CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true)]
    internal static extern SafeLibraryHandle LoadLibraryEx(
        [MarshalAs(UnmanagedType.LPStr)] string lpFileName,
        IntPtr hFile,
        int dwFlags);
}
#endif
