// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LocalDbInstanceInfo.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   LocalDBInstanceInfo.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A structure representing information about a SQL Server LocalDB instance.
    /// </summary>
    /// <remarks>
    /// See <c>http://msdn.microsoft.com/en-us/library/hh245734.aspx</c>.
    /// </remarks>
    [DebuggerDisplay("{DebuggerDisplayName}")]
    [StructLayout(LayoutKind.Sequential)]
    internal struct LocalDbInstanceInfo : ISqlLocalDbInstanceInfo
    {
        #region Fields

        /// <summary>
        /// The size of an unmanaged type in bytes.  This field is read-only.
        /// </summary>
        internal static readonly int MarshalSize = Marshal.SizeOf(typeof(LocalDbInstanceInfo));

        /// <summary>
        /// Contains the size of the <see cref="LocalDbInstanceInfo"/> struct.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>cbLocalDBInstanceInfoSize</c> member.
        /// </remarks>
        internal uint Size;

        /// <summary>
        /// The name of the LocalDB instance.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>wszInstanceName</c> member.
        /// </remarks>
        [MarshalAs(
            UnmanagedType.ByValArray,
            SizeConst = (NativeMethods.MAX_LOCALDB_INSTANCE_NAME_LENGTH + 1) * sizeof(char))]
        internal byte[] InstanceName;

        /// <summary>
        /// Whether the instance files exist on disk.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>bExists</c> member.
        /// </remarks>
        internal bool Exists;

        /// <summary>
        /// Whether the Registry configuration is corrupt.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>bConfigurationCorrupted</c> member.
        /// </remarks>
        internal bool ConfigurationCorrupted;

        /// <summary>
        /// Whether the instance is currently running.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>bIsRunning</c> member.
        /// </remarks>
        internal bool IsRunning;

        /// <summary>
        /// The LocalDB major version number.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>dwMajor</c> member.
        /// </remarks>
        internal uint Major;

        /// <summary>
        /// The LocalDB minor version number.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>dwMinor</c> member.
        /// </remarks>
        internal uint Minor;

        /// <summary>
        /// The LocalDB build version number.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>dwBuild</c> member.
        /// </remarks>
        internal uint Build;

        /// <summary>
        /// The LocalDB revision version number.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>dwRevision</c> member.
        /// </remarks>
        internal uint Revision;

        /// <summary>
        /// The UTC date and time the instance was last started.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>ftLastStartUTC</c> member.
        /// </remarks>
        internal Runtime.InteropServices.ComTypes.FILETIME LastStartUtc;

        /// <summary>
        /// The named pipe that should be used to communicate with the instance.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>wszConnection</c> member.
        /// </remarks>
        [MarshalAs(
            UnmanagedType.ByValArray,
            SizeConst = NativeMethods.LOCALDB_MAX_SQLCONNECTION_BUFFER_SIZE * sizeof(char))]
        internal byte[] Connection;

        /// <summary>
        /// Whether the instance is shared.
        /// </summary>
        /// <remarks>
        /// Maps to the bIsShared member.
        /// </remarks>
        internal bool IsShared;

        /// <summary>
        /// The shared name of the LocalDB instance if the instance is shared.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>wszSharedInstanceName</c> member.
        /// </remarks>
        [MarshalAs(
            UnmanagedType.ByValArray,
            SizeConst = (NativeMethods.MAX_LOCALDB_INSTANCE_NAME_LENGTH + 1) * sizeof(char))]
        internal byte[] SharedInstanceName;

        /// <summary>
        /// The SID of the LocalDB instance owner if the instance is shared.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>wszOwnerSID</c> member.
        /// </remarks>
        [MarshalAs(
            UnmanagedType.ByValArray,
            SizeConst = (NativeMethods.MAX_STRING_SID_LENGTH + 1) * sizeof(char))]
        internal byte[] OwnerSID;

        /// <summary>
        /// Whether the instance is automatic.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>bIsAutomatic</c> member.
        /// </remarks>
        internal bool IsAutomatic;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the Registry configuration is corrupt.
        /// </summary>
        bool ISqlLocalDbInstanceInfo.ConfigurationCorrupt
        {
            get { return this.ConfigurationCorrupted; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="ISqlLocalDbInstanceInfo"/> is exists.
        /// </summary>
        bool ISqlLocalDbInstanceInfo.Exists
        {
            get { return this.Exists; }
        }

        /// <summary>
        /// Gets a value indicating whether the instance is automatic.
        /// </summary>
        bool ISqlLocalDbInstanceInfo.IsAutomatic
        {
            get { return this.IsAutomatic; }
        }

        /// <summary>
        /// Gets a value indicating whether the instance is currently running.
        /// </summary>
        bool ISqlLocalDbInstanceInfo.IsRunning
        {
            get { return this.IsRunning; }
        }

        /// <summary>
        /// Gets a value indicating whether the instance is shared.
        /// </summary>
        bool ISqlLocalDbInstanceInfo.IsShared
        {
            get { return this.IsShared; }
        }

        /// <summary>
        /// Gets the UTC date and time the instance was last started.
        /// </summary>
        DateTime ISqlLocalDbInstanceInfo.LastStartTimeUtc
        {
            get
            {
                // Return DateTime.MinValue equivalent, rather than 01/01/1600
                if (this.LastStartUtc.dwHighDateTime == 0 &&
                    this.LastStartUtc.dwLowDateTime == 0)
                {
                    return new DateTime(0, DateTimeKind.Utc);
                }

                return DateTime.FromFileTimeUtc(((long)this.LastStartUtc.dwHighDateTime << 32) | (uint)this.LastStartUtc.dwLowDateTime);
            }
        }

        /// <summary>
        /// Gets the LocalDB version for the instance.
        /// </summary>
        Version ISqlLocalDbInstanceInfo.LocalDbVersion
        {
            get { return new Version((int)this.Major, (int)this.Minor, (int)this.Build, (int)this.Revision); }
        }

        /// <summary>
        /// Gets the name of the instance.
        /// </summary>
        string ISqlLocalDbInstanceInfo.Name
        {
            get { return Encoding.Unicode.GetString(this.InstanceName).TrimEnd(new char[] { '\0' }); }
        }

        /// <summary>
        /// Gets the named pipe that should be used to communicate with the instance.
        /// </summary>
        string ISqlLocalDbInstanceInfo.NamedPipe
        {
            get { return Encoding.Unicode.GetString(this.Connection).TrimEnd(new char[] { '\0' }); }
        }

        /// <summary>
        /// Gets the SID of the LocalDB instance owner if the instance is shared.
        /// </summary>
        string ISqlLocalDbInstanceInfo.OwnerSid
        {
            get { return Encoding.Unicode.GetString(this.OwnerSID).TrimEnd(new char[] { '\0' }); }
        }

        /// <summary>
        /// Gets the shared name of the LocalDB instance if the instance is shared.
        /// </summary>
        string ISqlLocalDbInstanceInfo.SharedName
        {
            get { return Encoding.Unicode.GetString(this.SharedInstanceName).TrimEnd(new char[] { '\0' }); }
        }

        /// <summary>
        /// Gets the name to display in the debugger
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [DebuggerNonUserCode]    // Use to hide from Code Coverage
        private string DebuggerDisplayName
        {
            get { return ((ISqlLocalDbInstanceInfo)this).Name; }
        }

        #endregion
    }
}