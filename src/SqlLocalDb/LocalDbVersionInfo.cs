// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalDbVersionInfo.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   LocalDBVersionInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A structure representing version about an SQL Server LocalDB version.
    /// </summary>
    /// <remarks>
    /// See <c>http://msdn.microsoft.com/en-us/library/hh234365.aspx</c>.
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplayName}")]
    [Serializable]
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
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (NativeMethods.MAX_LOCALDB_VERSION_LENGTH + 1) * sizeof(char))]
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
        string ISqlLocalDbVersionInfo.Name => NativeMethods.MarshalString(Name);

        /// <summary>
        /// Gets the version.
        /// </summary>
        Version ISqlLocalDbVersionInfo.Version => new Version((int)Major, (int)Minor, (int)Build, (int)Revision);

        /// <summary>
        /// Gets the name to display in the debugger
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private string DebuggerDisplayName => ((ISqlLocalDbVersionInfo)this).Name;
    }
}
