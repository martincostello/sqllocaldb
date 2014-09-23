// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlLocalDbApi.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   ISqlLocalDbApi.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
#if ASYNC
using System.Threading.Tasks;
#endif

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// Defines the interface to the SQL LocalDB API.
    /// </summary>
    public interface ISqlLocalDbApi
    {
        #region Properties

        /// <summary>
        /// Gets the version string for the latest installed version of SQL Server LocalDB.
        /// </summary>
        string LatestVersion
        {
            get;
        }

        /// <summary>
        /// Gets an <see cref="Array"/> of <see cref="String"/> containing the available version(s) of SQL LocalDB.
        /// </summary>
        IList<string> Versions
        {
            get;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance.</param>
        /// <param name="version">The version of SQL Server LocalDB to use.</param>
        void CreateInstance(string instanceName, string version);

        /// <summary>
        /// Deletes the specified SQL Server LocalDB instance.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to delete.
        /// </param>
        void DeleteInstance(string instanceName);

        /// <summary>
        /// Returns information about the specified LocalDB instance.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance to get the information for.</param>
        /// <returns>
        /// An instance of <see cref="ISqlLocalDbInstanceInfo"/> containing information
        /// about the LocalDB instance specified by <paramref name="instanceName"/>.
        /// </returns>
        ISqlLocalDbInstanceInfo GetInstanceInfo(string instanceName);

        /// <summary>
        /// Returns the names of all the SQL Server LocalDB instances for the current user.
        /// </summary>
        /// <returns>
        /// The names of the the SQL Server LocalDB instances for the current user.
        /// </returns>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Requires enumerating the API which is non-trivial.")]
        IList<string> GetInstanceNames();

        /// <summary>
        /// Returns information about the specified LocalDB version.
        /// </summary>
        /// <param name="version">The name of the LocalDB version to get the information for.</param>
        /// <returns>
        /// An instance of <see cref="ISqlLocalDbVersionInfo"/> containing information
        /// about the LocalDB version specified by <paramref name="version"/>.
        /// </returns>
        ISqlLocalDbVersionInfo GetVersionInfo(string version);

        /// <summary>
        /// Returns whether SQL LocalDB is installed on the current machine.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if SQL Server LocalDB is installed on the
        /// current machine; otherwise <see langword="false"/>.
        /// </returns>
        bool IsLocalDBInstalled();

        /// <summary>
        /// Shares the specified SQL Server LocalDB instance with other
        /// users of the computer, using the specified shared name.
        /// </summary>
        /// <param name="ownerSid">The SID of the instance owner.</param>
        /// <param name="instanceName">The private name for the LocalDB instance to share.</param>
        /// <param name="sharedInstanceName">The shared name for the LocalDB instance to share.</param>
        void ShareInstance(string ownerSid, string instanceName, string sharedInstanceName);

        /// <summary>
        /// Starts the specified instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance to start.</param>
        /// <returns>
        /// The named pipe to use to connect to the LocalDB instance.
        /// </returns>
        string StartInstance(string instanceName);

        /// <summary>
        /// Enables tracing of native API calls for all the SQL Server
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        void StartTracing();

        /// <summary>
        /// Stops the specified instance of SQL Server LocalDB.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to stop.
        /// </param>
        /// <param name="timeout">
        /// The amount of time to give the LocalDB instance to stop.
        /// If the value is <see cref="TimeSpan.Zero"/>, the method will
        /// return immediately and not wait for the instance to stop.
        /// </param>
        void StopInstance(string instanceName, TimeSpan timeout);

        /// <summary>
        /// Disables tracing of native API calls for all the SQL Server
        /// LocalDB instances owned by the current Windows user.
        /// </summary>
        /// <exception cref="SqlLocalDbException">
        /// Tracing could not be disabled.
        /// </exception>
        void StopTracing();

        /// <summary>
        /// Stops the sharing of the specified SQL Server LocalDB instance.
        /// </summary>
        /// <param name="instanceName">
        /// The private name for the LocalDB instance to stop sharing.
        /// </param>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the native LocalDB API function.")]
        void UnshareInstance(string instanceName);

#if ASYNC

        /// <summary>
        /// Creates a new instance of SQL Server LocalDB as an asynchronous operation.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance.</param>
        /// <param name="version">The version of SQL Server LocalDB to use.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task CreateInstanceAsync(string instanceName, string version);

        /// <summary>
        /// Deletes the specified SQL Server LocalDB instance as an asynchronous operation.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to delete.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task DeleteInstanceAsync(string instanceName);

        /// <summary>
        /// Shares the specified SQL Server LocalDB instance with other users of the
        /// computer, using the specified shared name, as an asynchronous operation.
        /// </summary>
        /// <param name="ownerSid">The SID of the instance owner.</param>
        /// <param name="instanceName">The private name for the LocalDB instance to share.</param>
        /// <param name="sharedInstanceName">The shared name for the LocalDB instance to share.</param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task ShareInstanceAsync(string ownerSid, string instanceName, string sharedInstanceName);

        /// <summary>
        /// Starts the specified instance of SQL Server LocalDB as an asynchronous operation.
        /// </summary>
        /// <param name="instanceName">The name of the LocalDB instance to start.</param>
        /// <returns>
        /// A <see cref="Task{T}"/> representing the asynchronous operation which returns the
        /// named pipe to use to connect to the LocalDB instance.
        /// </returns>
        Task<string> StartInstanceAsync(string instanceName);

        /// <summary>
        /// Stops the specified instance of SQL Server LocalDB as an asynchronous operation.
        /// </summary>
        /// <param name="instanceName">
        /// The name of the LocalDB instance to stop.
        /// </param>
        /// <param name="timeout">
        /// The amount of time to give the LocalDB instance to stop.
        /// If the value is <see cref="TimeSpan.Zero"/>, the method will
        /// return immediately and not wait for the instance to stop.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        Task StopInstanceAsync(string instanceName, TimeSpan timeout);

        /// <summary>
        /// Stops the sharing of the specified SQL Server LocalDB instance as an asynchronous operation.
        /// </summary>
        /// <param name="instanceName">
        /// The private name for the LocalDB instance to share.
        /// </param>
        /// <returns>
        /// A <see cref="Task"/> representing the asynchronous operation.
        /// </returns>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the native LocalDB API function.")]
        Task UnshareInstanceAsync(string instanceName);

#endif

        #endregion
    }
}