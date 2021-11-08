// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Win32.SafeHandles;

namespace MartinCostello.SqlLocalDb.Interop;

/// <summary>
/// A class that represents a handle to a library.  This class cannot be inherited.
/// </summary>
internal sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SafeLibraryHandle"/> class.
    /// </summary>
    public SafeLibraryHandle()
        : base(true)
    {
    }

    /// <summary>
    /// Executes the code required to free the handle.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the handle is released successfully;
    /// otherwise, in the event of a catastrophic failure,
    /// <see langword="false"/>. In this case, it generates a ReleaseHandleFailed
    /// Managed Debugging Assistant.
    /// </returns>
    protected override bool ReleaseHandle() => NativeMethods.FreeLibrary(handle);
}
