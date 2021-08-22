// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MartinCostello.SqlLocalDb.Interop
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
            SizeConst = (LocalDbInstanceApi.MaximumInstanceNameLength + 1) * sizeof(char))]
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
        internal System.Runtime.InteropServices.ComTypes.FILETIME LastStartUtc;

        /// <summary>
        /// The named pipe that should be used to communicate with the instance.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>wszConnection</c> member.
        /// </remarks>
        [MarshalAs(
            UnmanagedType.ByValArray,
            SizeConst = LocalDbInstanceApi.MaximumSqlConnectionStringBufferLength * sizeof(char))]
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
            SizeConst = (LocalDbInstanceApi.MaximumInstanceNameLength + 1) * sizeof(char))]
        internal byte[] SharedInstanceName;

        /// <summary>
        /// The SID of the LocalDB instance owner if the instance is shared.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>wszOwnerSID</c> member.
        /// </remarks>
        [MarshalAs(
            UnmanagedType.ByValArray,
            SizeConst = (LocalDbInstanceApi.MaximumSidStringLength + 1) * sizeof(char))]
        internal byte[] OwnerSID;

        /// <summary>
        /// Whether the instance is automatic.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>bIsAutomatic</c> member.
        /// </remarks>
        internal bool IsAutomatic;

        /// <summary>
        /// Gets a value indicating whether the Registry configuration is corrupt.
        /// </summary>
        bool ISqlLocalDbInstanceInfo.ConfigurationCorrupt => ConfigurationCorrupted;

        /// <summary>
        /// Gets a value indicating whether this <see cref="ISqlLocalDbInstanceInfo"/> is exists.
        /// </summary>
        bool ISqlLocalDbInstanceInfo.Exists => Exists;

        /// <summary>
        /// Gets a value indicating whether the instance is automatic.
        /// </summary>
        bool ISqlLocalDbInstanceInfo.IsAutomatic => IsAutomatic;

        /// <summary>
        /// Gets a value indicating whether the instance is currently running.
        /// </summary>
        bool ISqlLocalDbInstanceInfo.IsRunning => IsRunning;

        /// <summary>
        /// Gets a value indicating whether the instance is shared.
        /// </summary>
        bool ISqlLocalDbInstanceInfo.IsShared => IsShared;

        /// <summary>
        /// Gets the UTC date and time the instance was last started.
        /// </summary>
        DateTime ISqlLocalDbInstanceInfo.LastStartTimeUtc
        {
            get
            {
                // Return DateTime.MinValue equivalent, rather than 01/01/1600
                if (LastStartUtc.dwHighDateTime == 0 &&
                    LastStartUtc.dwLowDateTime == 0)
                {
                    return new DateTime(0, DateTimeKind.Utc);
                }

                return DateTime.FromFileTimeUtc(((long)LastStartUtc.dwHighDateTime << 32) | (uint)LastStartUtc.dwLowDateTime);
            }
        }

        /// <summary>
        /// Gets the LocalDB version for the instance.
        /// </summary>
        Version ISqlLocalDbInstanceInfo.LocalDbVersion => new Version((int)Major, (int)Minor, (int)Build, (int)Revision);

        /// <summary>
        /// Gets the name of the instance.
        /// </summary>
        string ISqlLocalDbInstanceInfo.Name => LocalDbInstanceApi.MarshalString(InstanceName);

        /// <summary>
        /// Gets the named pipe that should be used to communicate with the instance.
        /// </summary>
        string ISqlLocalDbInstanceInfo.NamedPipe => LocalDbInstanceApi.MarshalString(Connection);

        /// <summary>
        /// Gets the SID of the LocalDB instance owner if the instance is shared.
        /// </summary>
        string ISqlLocalDbInstanceInfo.OwnerSid => LocalDbInstanceApi.MarshalString(OwnerSID);

        /// <summary>
        /// Gets the shared name of the LocalDB instance if the instance is shared.
        /// </summary>
        string ISqlLocalDbInstanceInfo.SharedName => LocalDbInstanceApi.MarshalString(SharedInstanceName);

        /// <summary>
        /// Gets the name to display in the debugger.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
        private string DebuggerDisplayName => ((ISqlLocalDbInstanceInfo)this).Name;
    }
}
