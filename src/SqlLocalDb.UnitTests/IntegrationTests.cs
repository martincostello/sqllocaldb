// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IntegrationTests.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   IntegrationTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing integration tests for the <c>System.Data.SqlLocalDb</c> assembly.
    /// </summary>
    [TestClass]
    public class IntegrationTests
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegrationTests"/> class.
        /// </summary>
        public IntegrationTests()
        {
        }

        #endregion

        #region Methods

        [TestMethod]
        [TestCategory(TestCategories.Integration)]
        [Description("An end-to-end test for the System.Data.SqlLocalDb API.")]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA2202:Do not dispose objects multiple times",
            Justification = "It is not disposed multiple times.")]
        public void End_To_End()
        {
            ISqlLocalDbApi localDB = new SqlLocalDbApiWrapper();

            if (!localDB.IsLocalDBInstalled())
            {
                Assert.Fail("SQL LocalDB is not installed.");
            }

            ISqlLocalDbProvider provider = new SqlLocalDbProvider();

            IList<ISqlLocalDbVersionInfo> versions = provider.GetVersions();

            Assert.IsNotNull(versions, "GetVersions() returned null.");
            CollectionAssert.AllItemsAreNotNull(versions.ToArray(), "GetVersions() returned a null item.");

            foreach (ISqlLocalDbVersionInfo version in versions)
            {
                Assert.IsNotNull(version.Name, "ISqlLocalDbVersionInfo.Name is null.");
                Assert.AreNotEqual(string.Empty, version.Name, "ISqlLocalDbVersionInfo.Name is incorrect.");
                Assert.IsTrue(version.Exists, "ISqlLocalDbVersionInfo.Exists is incorrect for version '{0}'.", version.Name);
                Assert.IsNotNull(version.Version, "ISqlLocalDbVersionInfo.Version is null for version '{0}'.", version.Name);
                Assert.AreNotEqual(string.Empty, version.Version, "ISqlLocalDbVersionInfo.Version is incorrect for version '{0}'.", version.Name);
            }

            IList<ISqlLocalDbInstanceInfo> instances = provider.GetInstances();

            Assert.IsNotNull(instances, "GetInstances() returned null.");
            CollectionAssert.AllItemsAreNotNull(instances.ToArray(), "GetInstances() returned a null item.");

            foreach (ISqlLocalDbInstanceInfo instanceInfo in instances)
            {
                Assert.IsNotNull(instanceInfo.Name, "ISqlLocalDbInstanceInfo.Name is null.", instanceInfo.Name);

                if (string.Equals(instanceInfo.Name, "v12.0", StringComparison.Ordinal) &&
                    NativeMethods.NativeApiVersion == new Version(11, 0))
                {
                    // The SQL LocalDB 2012 native library reports the name as v12.0 instead of MSSQLLocalDB,
                    // which then if queried states that it does not exist (because it doesn't actually exist)
                    // under that name.  In this case, skip this test. We could fudge this in the wrapper itself,
                    // but that's probably not the best idea as the default instance name may change again in SQL v13.0.
                    continue;
                }

                Assert.AreNotEqual(string.Empty, instanceInfo.Name, "ISqlLocalDbInstanceInfo.Name is incorrect.", instanceInfo.Name);
                Assert.IsFalse(instanceInfo.ConfigurationCorrupt, "ISqlLocalDbInstanceInfo.ConfigurationCorrupt is incorrect for instance '{0}'.", instanceInfo.Name);
                Assert.IsTrue(instanceInfo.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect for instance '{0}'.", instanceInfo.Name);
                Assert.IsNotNull(instanceInfo.LocalDbVersion, "ISqlLocalDbInstanceInfo.LocalDbVersion is null for instance '{0}'.", instanceInfo.Name);
                Assert.AreNotEqual(string.Empty, instanceInfo.LocalDbVersion, "ISqlLocalDbInstanceInfo.LocalDbVersion is incorrect for instance '{0}'.", instanceInfo.Name);
                Assert.IsNotNull(instanceInfo.OwnerSid, "ISqlLocalDbInstanceInfo.OwnerSid is null for instance '{0}'.", instanceInfo.Name);
                Assert.AreNotEqual(string.Empty, instanceInfo.OwnerSid, "ISqlLocalDbInstanceInfo.OwnerSid is incorrect for instance '{0}'.", instanceInfo.Name);
            }

            string instanceName = Guid.NewGuid().ToString();
            string sharedInstanceName = string.Empty;

            ISqlLocalDbInstance instance = provider.CreateInstance(instanceName);

            instance.Start();

            try
            {
                bool currentUserIsAdmin = Helpers.IsCurrentUserAdmin();

                if (currentUserIsAdmin)
                {
                    sharedInstanceName = Guid.NewGuid().ToString();
                    instance.Share(sharedInstanceName);

                    // Restart the instance so it listens on the new shared name's pipe
                    instance.Stop();
                    instance.Start();
                }

                try
                {
                    ISqlLocalDbInstanceInfo info = provider.GetInstances()
                        .Where((p) => string.Equals(p.Name, instanceName, StringComparison.Ordinal))
                        .FirstOrDefault();

                    Assert.IsNotNull(info, "GetInstances() did not return the created instance.");
                    Assert.AreEqual(sharedInstanceName, info.SharedName, "ISqlLocalDbInstanceInfo.SharedName is incorrect.");

                    using (SqlConnection connection = instance.CreateConnection())
                    {
                        Assert.IsNotNull(connection, "CreateConnection() returned null.");
                        TestConnection(connection);
                    }

                    if (currentUserIsAdmin)
                    {
                        SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                        builder.DataSource = string.Format(CultureInfo.InvariantCulture, "(localdb)\\.\\{0}", sharedInstanceName);
                        builder.IntegratedSecurity = true;

                        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                        {
                            TestConnection(connection);
                        }
                    }
                }
                finally
                {
                    if (currentUserIsAdmin)
                    {
                        instance.Unshare();
                    }
                }
            }
            finally
            {
                instance.Stop();
                localDB.DeleteInstance(instance.Name);
            }
        }

        /// <summary>
        /// Tests that the specified <see cref="SqlConnection"/> can be used to create a test database.
        /// </summary>
        /// <param name="connection">The <see cref="SqlConnection"/> to use to create the test database.</param>
        private static void TestConnection(SqlConnection connection)
        {
            connection.Open();

            try
            {
                using (SqlCommand command = new SqlCommand("create database [MyDatabase]", connection))
                {
                    command.ExecuteNonQuery();
                }

                try
                {
                    using (SqlCommand command = new SqlCommand("create table [MyDatabase].[dbo].[TestTable] ([Id] int not null primary key clustered, [Value] int not null);", connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    Random random = new Random();
                    int id = random.Next();
                    int value = random.Next();

                    using (SqlCommand command = new SqlCommand("insert into [MyDatabase].[dbo].[TestTable] ([Id], [Value]) values (@id, @value);", connection))
                    {
                        command.Parameters.Add(new SqlParameter("id", id));
                        command.Parameters.Add(new SqlParameter("value", value));

                        command.ExecuteNonQuery();
                    }

                    using (SqlCommand command = new SqlCommand("select top 1 [Value] from [MyDatabase].[dbo].[TestTable] where [Id] = @id;", connection))
                    {
                        command.Parameters.Add(new SqlParameter("id", id));

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            Assert.IsTrue(reader.Read(), "SqlDataReader.Read() returned incorrect result.");
                            Assert.AreEqual(1, reader.FieldCount, "SqlDataReader.FieldCount is incorrect.");

                            int actual = reader.GetInt32(0);

                            Assert.AreEqual(value, actual, "The read query result value is incorrect.");
                            Assert.IsFalse(reader.Read(), "SqlDataReader.Read() returned incorrect result.");
                        }
                    }
                }
                finally
                {
                    using (SqlCommand command = new SqlCommand("drop database [MyDatabase]", connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            finally
            {
                connection.Close();
            }
        }

        #endregion
    }
}