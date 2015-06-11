using System;
using System.Data.Common;
using System.Data.SqlLocalDb;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlogSample
{
    /// <summary>
    /// A class that sets up the test assembly.
    /// </summary>
    [TestClass]
    public class TestSetup
    {
        /// <summary>
        /// The lazily-initialized shared SQL Local DB instance. This field is read-only.
        /// </summary>
        /// <remarks>
        /// This instance is lazily-initialized so that tests in this assembly do not
        /// pay a start-up performance penalty of creating the instance if no tests
        /// that actually use it are run in a particular test run. For example a trait
        /// filter may be used that excludes tests that use the database from running.
        /// </remarks>
        private static readonly Lazy<TemporarySqlLocalDbInstance> SharedInstance = new Lazy<TemporarySqlLocalDbInstance>(TemporarySqlLocalDbInstance.Create);

        /// <summary>
        /// The name of the shared blog database. This field is read-only.
        /// </summary>
        /// <remarks>
        /// This name is fixed on start-up so that the code-first database
        /// is only set up by EntityFramework once per test run for tests to
        /// improve performance. Tests that are not sensitive to data from
        /// other tests in the same test run being present in the database
        /// can use this database instead of creating their own.
        /// </remarks>
        private static readonly string SharedDatabaseName = GenerateDatabaseName();

        /// <summary>
        /// Gets the connection string to the shared test blog database.
        /// </summary>
        internal static string ConnectionString
        {
            get { return GetConnectionStringForDatabase(SharedDatabaseName); }
        }

        /// <summary>
        /// Initializes the test assembly.
        /// </summary>
        /// <param name="context">The test context.</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext context)
        {
            // Add any custom test initialization code here.
        }

        /// <summary>
        /// Cleans up the test assembly.
        /// </summary>
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            // Dispose of the temporary SQL LocalDB instance (if it was created by a test)
            if (SharedInstance.IsValueCreated)
            {
                SharedInstance.Value.Dispose();
            }
        }

        /// <summary>
        /// Generates a random database name.
        /// </summary>
        /// <returns>
        /// The randomly generated database name.
        /// </returns>
        internal static string GenerateDatabaseName()
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "Blog_{0:yyyy-MM-dd-HH-MM-ss}_{1}",
                DateTime.Now,
                Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Returns a SQL connection string that points to a non-existent database
        /// in the shared SQL LocalDB instance in use by the test assembly.
        /// </summary>
        /// <returns>
        /// The SQL connection string that can be used to create a new database.
        /// </returns>
        internal static string GetConnectionStringForNewDatabase()
        {
            string initialCatalog = GenerateDatabaseName();
            return GetConnectionStringForDatabase(initialCatalog);
        }

        /// <summary>
        /// Returns a SQL connection string that points at the specified database
        /// in the shared SQL LocalDB instance in use by the test assembly.
        /// </summary>
        /// <param name="initialCatalog">The name of the database.</param>
        /// <returns>
        /// The SQL connection string to use to connect to the specified database.
        /// </returns>
        internal static string GetConnectionStringForDatabase(string initialCatalog)
        {
            // Get the base SQL connection string to the SQL LocalDB instance.
            // If it has not already been created it will be created now.
            DbConnectionStringBuilder builder = SharedInstance.Value.CreateConnectionStringBuilder();

            // Update the catalog name to the specified database name
            builder.SetInitialCatalogName(initialCatalog);

            // Return the modified SQL connection string which points at the
            // shared SQL LocalDB instance and the specified database.
            return builder.ConnectionString;
        }
    }
}
