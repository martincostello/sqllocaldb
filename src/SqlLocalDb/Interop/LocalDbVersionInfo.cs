// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MartinCostello.SqlLocalDb.Interop;

/// <summary>
/// A structure representing version about a SQL Server LocalDB version.
/// </summary>
/// <remarks>
/// See <c>http://msdn.microsoft.com/en-us/library/hh234365.aspx</c>.
/// </remarks>
[DebuggerDisplay("{DebuggerDisplayName}")]
[StructLayout(LayoutKind.Sequential)]
internal struct LocalDbVersionInfo : ISqlLocalDbVersionInfo
{
    /// <summary>
    /// The size of an unmanaged type in bytes.  This field is read-only.
    /// </summary>
    internal static readonly int MarshalSize = Marshal.SizeOf(typeof(LocalDbVersionInfo));

    /// <summary>
    /// The size of the <see cref="LocalDbVersionInfo"/> structure.
    /// </summary>
    /// <remarks>
    /// Maps to the <c>cbLocalDBVersionInfoSize</c> member.
    /// </remarks>
    internal uint Size;

    /// <summary>
    /// The version name.
    /// </summary>
    /// <remarks>
    /// Maps to the <c>wszVersion</c> member.
    /// </remarks>
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (LocalDbInstanceApi.MaximumInstanceVersionLength + 1) * sizeof(char))]
    internal byte[] Name;

    /// <summary>
    /// Whether the instance files exist on disk.
    /// </summary>
    /// <remarks>
    /// Maps to the <c>bExists</c> member.
    /// </remarks>
    internal bool Exists;

    /// <summary>
    /// The major version number.
    /// </summary>
    /// <remarks>
    /// Maps to the <c>dwMajor</c> member.
    /// </remarks>
    internal uint Major;

    /// <summary>
    /// The minor version number.
    /// </summary>
    /// <remarks>
    /// Maps to the <c>dwMinor</c> member.
    /// </remarks>
    internal uint Minor;

    /// <summary>
    /// The build version number.
    /// </summary>
    /// <remarks>
    /// Maps to the <c>dwBuild</c> member.
    /// </remarks>
    internal uint Build;

    /// <summary>
    /// The revision version number.
    /// </summary>
    /// <remarks>
    /// Maps to the <c>dwRevision</c> member.
    /// </remarks>
    internal uint Revision;

    /// <summary>
    /// Gets a value indicating whether the instance files exist on disk.
    /// </summary>
    bool ISqlLocalDbVersionInfo.Exists => Exists;

    /// <summary>
    /// Gets the version name.
    /// </summary>
    string ISqlLocalDbVersionInfo.Name => LocalDbInstanceApi.MarshalString(Name);

    /// <summary>
    /// Gets the version.
    /// </summary>
    Version ISqlLocalDbVersionInfo.Version => new Version((int)Major, (int)Minor, (int)Build, (int)Revision);

    /// <summary>
    /// Gets the name to display in the debugger.
    /// </summary>
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    private string DebuggerDisplayName => ((ISqlLocalDbVersionInfo)this).Name;
}
