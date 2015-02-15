// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbInstanceTests.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbInstanceTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.SqlClient;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="SqlLocalDbInstance"/> class.
    /// </summary>
    [TestClass]
    public class SqlLocalDbInstanceTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbInstanceTests"/> class.
        /// </summary>
        public SqlLocalDbInstanceTests()
        {
        }

        [TestMethod]
        [Description("Tests .ctor() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbInstance_Constructor_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            string instanceName = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => new SqlLocalDbInstance(instanceName),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests .ctor() if instanceName does not exist.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SqlLocalDbInstance_Constructor_Throws_If_Instance_Name_Does_Not_Exist()
        {
            // Arrange
            string instanceName = Guid.NewGuid().ToString();

            // Act and Assert
            throw ErrorAssert.Throws<InvalidOperationException>(
                () => new SqlLocalDbInstance(instanceName));
        }

        [TestMethod]
        [Description("Tests .ctor() if the instance is not started.")]
        public void SqlLocalDbInstance_Constructor_Throws_If_Instance_Not_Started()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                // Act
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

                // Assert
                Assert.IsFalse(target.IsRunning, "SqlLocalDbInstance.IsRunning is incorrect.");
                Assert.AreEqual(instanceName, target.Name, "SqlLocalDbInstance.Name is incorrect.");
                Assert.AreEqual(string.Empty, target.NamedPipe, "SqlLocalDbInstance.NamedPipe is incorrect.");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests .ctor() if the instance is started.")]
        public void SqlLocalDbInstance_Constructor_If_Instance_Is_Started()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            try
            {
                SqlLocalDbApi.CreateInstance(instanceName);

                try
                {
                    string namedPipe = SqlLocalDbApi.StartInstance(instanceName);

                    // Act
                    SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

                    // Assert
                    Assert.IsTrue(target.IsRunning, "SqlLocalDbInstance.IsRunning is incorrect.");
                    Assert.AreEqual(instanceName, target.Name, "SqlLocalDbInstance.Name is incorrect.");
                    Assert.AreEqual(namedPipe, target.NamedPipe, "SqlLocalDbInstance.NamedPipe is incorrect.");
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests CreateConnection() if the instance is started.")]
        public void SqlLocalDbInstance_CreateConnection_If_Instance_Is_Started()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
                target.Start();

                try
                {
                    // Act
                    SqlConnection result = target.CreateConnection();

                    try
                    {
                        // Assert
                        Assert.IsNotNull(result, "CreateConnection() returned null.");
                        Assert.AreEqual(ConnectionState.Closed, result.State, "SqlConnection.State is incorrect.");
                        StringAssert.Contains(result.ConnectionString, target.NamedPipe, "SqlConnection.ConnectionString is incorrect.");
                    }
                    finally
                    {
                        if (result != null)
                        {
                            result.Dispose();
                        }
                    }
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests CreateConnection() if the instance is started and the CreateConnectionStringBuilder() returns null.")]
        public void SqlLocalDbInstance_CreateConnection_If_Instance_Is_Started_And_Returned_ConnectionStringBuilder_Is_Null()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                Mock<SqlLocalDbInstance> mock = new Mock<SqlLocalDbInstance>(instanceName);
                mock.CallBase = true;

                mock.Setup((p) => p.CreateConnectionStringBuilder())
                    .Returns(null as SqlConnectionStringBuilder);

                SqlLocalDbInstance target = mock.Object;
                target.Start();

                try
                {
                    // Act
                    SqlConnection result = target.CreateConnection();

                    try
                    {
                        // Assert
                        Assert.IsNotNull(result, "CreateConnection() returned null.");
                        Assert.AreEqual(ConnectionState.Closed, result.State, "SqlConnection.State is incorrect.");
                        Assert.AreEqual(string.Empty, result.ConnectionString, "SqlConnection.ConnectionString is incorrect.");
                    }
                    finally
                    {
                        if (result != null)
                        {
                            result.Dispose();
                        }
                    }
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests CreateConnectionStringBuilder() if the instance is started.")]
        public void SqlLocalDbInstance_CreateConnectionStringBuilder_If_Instance_Started()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
                target.Start();

                try
                {
                    // Act
                    SqlConnectionStringBuilder result = target.CreateConnectionStringBuilder();

                    // Assert
                    Assert.IsNotNull(result, "CreateConnection() returned null.");
                    StringAssert.Contains(result.ConnectionString, target.NamedPipe, "SqlConnection.ConnectionString is incorrect.");
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests CreateConnection() if the instance is not started.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void SqlLocalDbInstance_CreateConnectionStringBuilder_Throws_If_Instance_Not_Started()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
             
                // Act and Assert
                throw ErrorAssert.Throws<InvalidOperationException>(
                    () => target.CreateConnectionStringBuilder());
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests Delete() if instance is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbInstance_Delete_Throws_If_Instance_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstance instance = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbInstance.Delete(instance),
                "instance");
        }

        [TestMethod]
        [Description("Tests Delete() if instance is invalid.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbInstance_Delete_Throws_If_Instance_Is_Invalid()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            const string InstanceName = "\\\\";

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.Name).Returns(InstanceName);

            ISqlLocalDbInstance instance = mock.Object;

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbInstance.Delete(instance));

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.InvalidInstanceName, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(InstanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests Delete().")]
        public void SqlLocalDbInstance_Delete_Deletes_Instance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbApi.CreateInstance(instanceName);

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.Name).Returns(instanceName);

            ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);
            Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
            Assert.IsTrue(info.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");

            ISqlLocalDbInstance instance = mock.Object;

            // Act
            SqlLocalDbInstance.Delete(instance);

            // Assert
            info = SqlLocalDbApi.GetInstanceInfo(instanceName);
            Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
            Assert.IsFalse(info.Exists, "The SQL LocalDB instance was not deleted.");

            string path = Path.Combine(SqlLocalDbApi.GetInstancesFolderPath(), instanceName);
            Assert.IsTrue(Directory.Exists(path), "The instance folder was deleted.");
            Assert.AreNotEqual(0, Directory.GetFiles(path).Length, "The instance files were deleted.");
        }

        [TestMethod]
        [Description("Tests Delete() if SqlLocalDbApi.AutomaticallyDeleteInstanceFiles is true.")]
        public void SqlLocalDbInstance_Delete_If_SqlLocalDbApi_AutomaticallyDeleteInstanceFiles_Is_True()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    SqlLocalDbApi.AutomaticallyDeleteInstanceFiles = true;

                    string instanceName = Guid.NewGuid().ToString();

                    SqlLocalDbApi.CreateInstance(instanceName);

                    Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
                    mock.Setup((p) => p.Name).Returns(instanceName);

                    ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);
                    Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
                    Assert.IsTrue(info.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");

                    ISqlLocalDbInstance instance = mock.Object;

                    // Act
                    SqlLocalDbInstance.Delete(instance);

                    // Assert
                    info = SqlLocalDbApi.GetInstanceInfo(instanceName);
                    Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
                    Assert.IsFalse(info.Exists, "The SQL LocalDB instance was not deleted.");

                    string path = Path.Combine(SqlLocalDbApi.GetInstancesFolderPath(), instanceName);
                    Assert.IsFalse(Directory.Exists(path), "The instance folder was not deleted.");
                });
        }

        [TestMethod]
        [Description("Tests GetInstanceInfo().")]
        public void SqlLocalDbInstance_GetInstanceInfo_Returns_Information_For_The_Specified_Instance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            ISqlLocalDbInstanceInfo expected = SqlLocalDbApi.GetInstanceInfo(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

                // Act
                ISqlLocalDbInstanceInfo result = target.GetInstanceInfo();

                // Assert
                Assert.IsNotNull(result, "GetInstanceInfo() returned null.");
                Assert.AreEqual(expected.ConfigurationCorrupt, result.ConfigurationCorrupt, "ISqlLocalDbInstanceInfo. is incorrect.");
                Assert.AreEqual(expected.Exists, result.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");
                Assert.AreEqual(expected.IsAutomatic, result.IsAutomatic, "ISqlLocalDbInstanceInfo.IsAutomatic is incorrect.");
                Assert.AreEqual(expected.IsRunning, result.IsRunning, "ISqlLocalDbInstanceInfo.IsRunning is incorrect.");
                Assert.AreEqual(expected.IsShared, result.IsShared, "ISqlLocalDbInstanceInfo.IsShared is incorrect.");
                Assert.AreEqual(expected.LastStartTimeUtc, result.LastStartTimeUtc, "ISqlLocalDbInstanceInfo.LastStartTimeUtc is incorrect.");
                Assert.AreEqual(expected.LocalDbVersion, result.LocalDbVersion, "ISqlLocalDbInstanceInfo.LocalDbVersion is incorrect.");
                Assert.AreEqual(expected.Name, result.Name, "ISqlLocalDbInstanceInfo.Name is incorrect.");
                Assert.AreEqual(expected.NamedPipe, result.NamedPipe, "ISqlLocalDbInstanceInfo.NamedPipe is incorrect.");
                Assert.AreEqual(expected.OwnerSid, result.OwnerSid, "ISqlLocalDbInstanceInfo.OwnerSid is incorrect.");
                Assert.AreEqual(expected.SharedName, result.SharedName, "ISqlLocalDbInstanceInfo.SharedName is incorrect.");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.RequiresAdministrativePermissions)]
        [Description("Tests Share() if the instance is started.")]
        public void SqlLocalDbInstance_Share_If_Instance_Is_Started()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();
            Helpers.EnsureUserIsAdmin();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
                target.Start();

                try
                {
                    string sharedName = Guid.NewGuid().ToString();

                    // Act
                    target.Share(sharedName);

                    // Assert
                    ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                    Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
                    Assert.IsTrue(info.IsShared, "ISqlLocalDbInstanceInfo.IsShared is incorrect.");
                    Assert.AreEqual(sharedName, info.SharedName, "ISqlLocalDbInstanceInfo.SharedName is incorrect.");
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests Share() if sharedName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbInstance_Share_Throws_If_SharedName_Is_Null()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

                string sharedName = null;

                // Act and Assert
                throw ErrorAssert.Throws<ArgumentNullException>(
                    () => target.Share(sharedName),
                    "sharedName");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests Share() if sharedName is invalid.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbInstance_Share_Throws_If_SharedName_Is_Invalid()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            string sharedName = "\\\\";

            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

                // Act
                SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                    () => target.Share(sharedName));

                // Assert
                Assert.AreEqual(SqlLocalDbErrors.InvalidInstanceName, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
                Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

                throw error;
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests Start().")]
        public void SqlLocalDbInstance_Start_Starts_Instance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

                Assert.IsFalse(target.IsRunning, "SqlLocalDbInstance.IsRunning is incorrect.");
                Assert.AreEqual(string.Empty, target.NamedPipe, "SqlLocalDbInstance.NamedPipe is incorrect.");

                ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);
                Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
                Assert.IsFalse(info.IsRunning, "ISqlLocalDbInstanceInfo.IsRunning is incorrect.");

                // Act
                target.Start();

                try
                {
                    // Assert
                    info = SqlLocalDbApi.GetInstanceInfo(instanceName);
                    Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
                    Assert.IsTrue(info.IsRunning, "The SQL LocalDB instance was not started.");

                    Assert.IsTrue(target.IsRunning, "SqlLocalDbInstance.IsRunning is incorrect.");
                    Assert.AreNotEqual(string.Empty, target.NamedPipe, "SqlLocalDbInstance.NamedPipe is incorrect.");
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests Start() if an error occurs.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbInstance_Start_Throws_If_An_Error_Occurs()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            SqlLocalDbInstance target;

            try
            {
                target = new SqlLocalDbInstance(instanceName);
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => target.Start());

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.UnknownInstance, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests Stop().")]
        public void SqlLocalDbInstance_Stop_Stops_Instance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
                target.Start();

                ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);
                Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
                Assert.IsTrue(info.IsRunning, "The SQL LocalDB instance was not started.");

                Assert.IsTrue(target.IsRunning, "SqlLocalDbInstance.IsRunning is incorrect.");
                Assert.AreNotEqual(string.Empty, target.NamedPipe, "SqlLocalDbInstance.NamedPipe is incorrect.");

                // Act
                target.Stop();

                try
                {
                    // Assert
                    info = SqlLocalDbApi.GetInstanceInfo(instanceName);
                    Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
                    Assert.IsFalse(info.IsRunning, "ISqlLocalDbInstanceInfo.IsRunning is incorrect.");

                    Assert.IsFalse(target.IsRunning, "SqlLocalDbInstance.IsRunning is incorrect.");
                    Assert.AreEqual(string.Empty, target.NamedPipe, "SqlLocalDbInstance.NamedPipe is incorrect.");
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests Stop() if an error occurs.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbInstance_Stop_Throws_If_An_Error_Occurs()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            SqlLocalDbInstance target;

            try
            {
                target = new SqlLocalDbInstance(instanceName);
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => target.Stop());

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.UnknownInstance, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [TestCategory(TestCategories.RequiresAdministrativePermissions)]
        [Description("Tests Unshare() if the instance is started.")]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the method under test.")]
        public void SqlLocalDbInstance_Unshare_If_Instance_Is_Started()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();
            Helpers.EnsureUserIsAdmin();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
                target.Start();

                try
                {
                    string sharedName = Guid.NewGuid().ToString();
                    target.Share(sharedName);

                    ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                    Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
                    Assert.IsTrue(info.IsShared, "ISqlLocalDbInstanceInfo.IsShared is incorrect.");
                    Assert.AreEqual(sharedName, info.SharedName, "ISqlLocalDbInstanceInfo.SharedName is incorrect.");

                    // Act
                    target.Unshare();

                    // Assert
                    info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                    Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
                    Assert.IsFalse(info.IsShared, "ISqlLocalDbInstanceInfo.IsShared is incorrect.");
                    Assert.AreEqual(string.Empty, info.SharedName, "ISqlLocalDbInstanceInfo.SharedName is incorrect.");
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests Unshare() if an error occurs.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the method under test.")]
        public void SqlLocalDbInstance_Unshare_Throws_If_An_Error_Occurs()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
                target.Start();

                try
                {
                    // Act
                    SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                        () => target.Unshare());

                    // Assert
                    Assert.AreEqual(SqlLocalDbErrors.InstanceNotShared, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
                    Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

                    throw error;
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }
    }
}