// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbInstanceTests.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbInstanceTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.SqlClient;
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
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbInstanceTests"/> class.
        /// </summary>
        public SqlLocalDbInstanceTests()
        {
        }

        #endregion

        #region Methods

        [TestMethod]
        [Description("Tests .ctor() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsIfInstanceNameIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => new SqlLocalDbInstance(null),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests .ctor() if instanceName does not exist.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Constructor_ThrowsIfInstanceNameDoesNotExist()
        {
            throw ErrorAssert.Throws<InvalidOperationException>(
                () => new SqlLocalDbInstance(Guid.NewGuid().ToString()));
        }

        [TestMethod]
        [Description("Tests .ctor() if the instance is not started.")]
        public void Constructor_ThrowsIfInstanceNotStarted()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

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
        public void Constructor_InstanceIsStarted()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            try
            {
                SqlLocalDbApi.CreateInstance(instanceName);

                try
                {
                    string namedPipe = SqlLocalDbApi.StartInstance(instanceName);

                    SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

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
        public void CreateConnection_InstanceIsStarted()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
                target.Start();

                try
                {
                    SqlConnection result = target.CreateConnection();

                    try
                    {
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
        public void CreateConnection_InstanceIsStartedAndConnectionStringBuilderIsNull()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                Mock<SqlLocalDbInstance> mock = new Mock<SqlLocalDbInstance>(instanceName);
                mock.CallBase = true;
                mock.Setup((i) => i.CreateConnectionStringBuilder()).Returns(null as SqlConnectionStringBuilder);

                SqlLocalDbInstance target = mock.Object;
                target.Start();

                try
                {
                    SqlConnection result = target.CreateConnection();

                    try
                    {
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
        public void CreateConnectionStringBuilder_InstanceStarted()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
                target.Start();

                try
                {
                    SqlConnectionStringBuilder result = target.CreateConnectionStringBuilder();

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
        public void CreateConnectionStringBuilder_ThrowsIfInstanceNotStarted()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
                
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
        public void Delete_ThrowsIfInstanceIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbInstance.Delete(null),
                "instance");
        }

        [TestMethod]
        [Description("Tests Delete() if instance is invalid.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void Delete_ThrowsIfInstanceIsInvalid()
        {
            Helpers.EnsureLocalDBInstalled();

            const string InstanceName = "\0";

            Mock<ISqlLocalDbInstance> instance = new Mock<ISqlLocalDbInstance>();
            instance.Setup((i) => i.Name).Returns(InstanceName);

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbInstance.Delete(instance.Object));

            Assert.AreEqual(SqlLocalDbErrors.InvalidParameter, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(InstanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests Delete().")]
        public void Delete()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbApi.CreateInstance(instanceName);

            Mock<ISqlLocalDbInstance> instance = new Mock<ISqlLocalDbInstance>();
            instance.Setup((i) => i.Name).Returns(instanceName);

            ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);
            Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
            Assert.IsTrue(info.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");

            SqlLocalDbInstance.Delete(instance.Object);

            info = SqlLocalDbApi.GetInstanceInfo(instanceName);
            Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
            Assert.IsFalse(info.Exists, "The SQL LocalDB instance was not deleted.");
        }

        [TestMethod]
        [Description("Tests GetInstanceInfo().")]
        public void GetInstanceInfo()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            ISqlLocalDbInstanceInfo expected = SqlLocalDbApi.GetInstanceInfo(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

                ISqlLocalDbInstanceInfo result = target.GetInstanceInfo();

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
        public void Share_InstanceIsStarted()
        {
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
        public void Share_ThrowsIfSharedNameIsNull()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

                throw ErrorAssert.Throws<ArgumentNullException>(
                    () => target.Share(null),
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
        public void Share_ThrowsIfSharedNameIsInvalid()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);

                SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                    () => target.Share("\0"));

                Assert.AreEqual(SqlLocalDbErrors.InvalidParameter, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
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
        public void Start()
        {
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

                target.Start();

                try
                {
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
        public void Start_ThrowsIfAnErrorOccurs()
        {
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

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => target.Start());

            Assert.AreEqual(SqlLocalDbErrors.UnknownInstance, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests Stop().")]
        public void Stop()
        {
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

                target.Stop();

                try
                {
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
        public void Stop_ThrowsIfAnErrorOccurs()
        {
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

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => target.Stop());

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
        public void Unshare_InstanceIsStarted()
        {
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

                    target.Unshare();

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
        public void Unshare_ThrowsIfAnErrorOccurs()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbInstance target = new SqlLocalDbInstance(instanceName);
                target.Start();

                try
                {
                    SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                        () => target.Unshare());

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

        #endregion
    }
}