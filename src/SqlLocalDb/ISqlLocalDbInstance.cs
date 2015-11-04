// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISqlLocalDbInstance.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   ISqlLocalDbInstance.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.SqlClient;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// Defines an SQL Server LocalDB instance.
    /// </summary>
    public interface ISqlLocalDbInstance
    {
        /// <summary>
        /// Gets the name of the LocalDB instance.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// Gets the named pipe that should be used
        /// to connect to the LocalDB instance.
        /// </summary>
        string NamedPipe
        {
            get;
        }

        /// <summary>
        /// Creates a connection to the LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="SqlConnection"/> that
        /// can be used to connect to the LocalDB instance.
        /// </returns>
        SqlConnection CreateConnection();

        /// <summary>
        /// Creates an instance of <see cref="SqlConnectionStringBuilder"/> containing
        /// the default SQL connection string to connect to the LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="SqlConnectionStringBuilder"/> containing
        /// the default SQL connection string to connect to the LocalDB instance.
        /// </returns>
        SqlConnectionStringBuilder CreateConnectionStringBuilder();

        /// <summary>
        /// Returns information about the LocalDB instance.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="ISqlLocalDbInstanceInfo"/> containing
        /// information about the LocalDB instance.
        /// </returns>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Requires querying the LocalDB native API.")]
        ISqlLocalDbInstanceInfo GetInstanceInfo();

        /// <summary>
        /// Shares the LocalDB instance using the specified name.
        /// </summary>
        /// <param name="sharedName">The name to use to share the instance.</param>
        void Share(string sharedName);

        /// <summary>
        /// Starts the LocalDB instance.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the LocalDB instance.
        /// </summary>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1716:IdentifiersShouldNotMatchKeywords",
            MessageId = "Stop",
            Justification = "Matches the name of the LocalDB native API function.")]
        void Stop();

        /// <summary>
        /// Stops sharing the LocalDB instance.
        /// </summary>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the LocalDB native API function.")]
        void Unshare();
    }
}
