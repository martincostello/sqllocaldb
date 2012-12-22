// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalDbVersionInfo.cs" company="http://sqllocaldb.codeplex.com">
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
        #region Fields

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

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the instance files exist on disk.
        /// </summary>
        bool ISqlLocalDbVersionInfo.Exists
        {
            get { return this.Exists; }
        }

        /// <summary>
        /// Gets the version name.
        /// </summary>
        string ISqlLocalDbVersionInfo.Name
        {
            get { return Text.Encoding.Unicode.GetString(this.Name).TrimEnd(new char[] { '\0' }); }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        Version ISqlLocalDbVersionInfo.Version
        {
            get { return new Version((int)this.Major, (int)this.Minor, (int)this.Build, (int)this.Revision); }
        }

        /// <summary>
        /// Gets the name to display in the debugger
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DebuggerNonUserCode]    // Use to hide from Code Coverage
        private string DebuggerDisplayName
        {
            get { return ((ISqlLocalDbVersionInfo)this).Name; }
        }

        #endregion
    }
}