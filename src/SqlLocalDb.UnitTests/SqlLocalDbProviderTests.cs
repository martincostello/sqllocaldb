// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbProviderTests.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbProviderTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="SqlLocalDbProvider"/> class.
    /// </summary>
    [TestClass]
    public class SqlLocalDbProviderTests
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbProviderTests"/> class.
        /// </summary>
        public SqlLocalDbProviderTests()
        {
        }

        #endregion

        #region Methods

        [TestMethod]
        [Description("Tests .ctor().")]
        public void Default_Constructor_Uses_Correct_LocalDB_Instance()
        {
            // Act
            SqlLocalDbProvider target = new SqlLocalDbProvider();

            // Assert
            Assert.IsNotNull(target.LocalDB, "SqlLocalDbProvider.LocalDB is null.");
            Assert.IsInstanceOfType(target.LocalDB, typeof(SqlLocalDbApiWrapper), "SqlLocalDbProvider.LocalDB is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(ISqlLocalDbApi).")]
        public void Constructor_Uses_Specified_LocalDB_Instance()
        {
            // Arrange
            ISqlLocalDbApi localDB = Mock.Of<ISqlLocalDbApi>();

            // Act
            SqlLocalDbProvider target = new SqlLocalDbProvider(localDB);

            // Assert
            Assert.AreSame(localDB, target.LocalDB, "SqlLocalDbProvider.LocalDB is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(ISqlLocalDbApi) throws if localDB is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_If_LocalDB_Is_Null()
        {
            // Arrange
            ISqlLocalDbApi localDB = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => new SqlLocalDbProvider(localDB),
                "localDB");
        }

        [TestMethod]
        [Description("Tests CreateInstance().")]
        public void CreateInstance_Void()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();
            
            // Act
            SqlLocalDbInstance result = target.CreateInstance();

            // Assert
            Assert.IsNotNull(result, "CreateInstance() returned null.");
            Assert.IsNotNull(result.Name, "SqlLocalDbInstance.Name is null.");

            try
            {
                ISqlLocalDbInstanceInfo info = result.GetInstanceInfo();

                Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
                Assert.IsTrue(info.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");
                Assert.IsFalse(info.IsRunning, "ISqlLocalDbInstanceInfo.IsRunning is incorrect.");

                Assert.AreEqual(
                    new Version(target.Version),
                    new Version(info.LocalDbVersion.Major, info.LocalDbVersion.Minor),
                    "ISqlLocalDbInstanceInfo.LocalDbVersion is incorrect.");

                Guid guid;
                Assert.IsTrue(Guid.TryParse(result.Name, out guid), "SqlLocalDbInstance.Name is not a valid GUID.");
            }
            finally
            {
                SqlLocalDbInstance.Delete(result);
            }
        }

        [TestMethod]
        [Description("Tests CreateInstance(string).")]
        public void CreateInstance_String()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();
            string instanceName = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbInstance result = target.CreateInstance(instanceName);

            // Assert
            Assert.IsNotNull(result, "CreateInstance() returned null.");
            Assert.AreEqual(instanceName, result.Name, "SqlLocalDbInstance.Name is incorrect.");

            try
            {
                ISqlLocalDbInstanceInfo info = result.GetInstanceInfo();

                Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
                Assert.IsTrue(info.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");
                Assert.IsFalse(info.IsRunning, "ISqlLocalDbInstanceInfo.IsRunning is incorrect.");

                Assert.AreEqual(
                    new Version(target.Version),
                    new Version(info.LocalDbVersion.Major, info.LocalDbVersion.Minor),
                    "ISqlLocalDbInstanceInfo.LocalDbVersion is incorrect.");
            }
            finally
            {
                SqlLocalDbInstance.Delete(result);
            }
        }

        [TestMethod]
        [Description("Tests CreateInstance(string) if the underlying API returned no instance information.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateInstance_Throws_If_No_Instance_Info_From_Api()
        {
            // Arrange
            ISqlLocalDbApi localDB = Mock.Of<ISqlLocalDbApi>();

            SqlLocalDbProvider target = new SqlLocalDbProvider(localDB);
            string instanceName = Guid.NewGuid().ToString();

            // Act and Assert
            throw ErrorAssert.Throws<InvalidOperationException>(
                () => target.CreateInstance(instanceName));
        }

        [TestMethod]
        [Description("Tests CreateInstance(string) if the instance already exists.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateInstance_ThrowsIfInstanceAlreadyExists()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();
            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                // Act and Assert
                throw ErrorAssert.Throws<InvalidOperationException>(
                    () => target.CreateInstance(instanceName));
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests CreateInstance(string) uses the latest version of SQL LocalDB if not overridden.")]
        public void CreateInstance_Uses_Latest_Version()
        {
            // Arrange
            string instanceName = Guid.NewGuid().ToString();
            string latestVersion = "1.2.3.4";

            Mock<ISqlLocalDbInstanceInfo> mockInfo = new Mock<ISqlLocalDbInstanceInfo>();

            mockInfo
                .SetupSequence((p) => p.Exists)
                .Returns(false)
                .Returns(true);

            Mock<ISqlLocalDbApi> mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.LatestVersion)
                .Returns(latestVersion)
                .Verifiable();

            mock.Setup((p) => p.CreateInstance(instanceName, latestVersion))
                .Verifiable();

            mock.Setup((p) => p.GetInstanceInfo(instanceName))
                .Returns(mockInfo.Object)
                .Verifiable();

            ISqlLocalDbApi localDB = mock.Object;

            SqlLocalDbProvider target = new SqlLocalDbProvider(localDB);

            // Act
            target.CreateInstance(instanceName);

            // Assert
            mock.Verify();
        }

        [TestMethod]
        [Description("Tests CreateInstance(string) uses the specfied version of SQL LocalDB if overridden.")]
        public void CreateInstance_Uses_Specified_Version()
        {
            // Arrange
            string instanceName = Guid.NewGuid().ToString();
            string latestVersion = "2.3.4.5";
            string version = "1.2.3.4";

            Mock<ISqlLocalDbInstanceInfo> mockInfo = new Mock<ISqlLocalDbInstanceInfo>();

            mockInfo
                .SetupSequence((p) => p.Exists)
                .Returns(false)
                .Returns(true);

            Mock<ISqlLocalDbApi> mock = new Mock<ISqlLocalDbApi>();

            mock.Setup((p) => p.LatestVersion)
                .Returns(latestVersion);

            mock.Setup((p) => p.CreateInstance(instanceName, version))
                .Verifiable();

            mock.Setup((p) => p.GetInstanceInfo(instanceName))
                .Returns(mockInfo.Object)
                .Verifiable();

            ISqlLocalDbApi localDB = mock.Object;

            SqlLocalDbProvider target = new SqlLocalDbProvider(localDB);
            target.Version = version;

            // Act
            target.CreateInstance(instanceName);

            // Assert
            mock.Verify();
        }

        [TestMethod]
        [Description("Tests GetInstance(string).")]
        public void GetInstance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();
            string instanceName = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                // Assert
                SqlLocalDbInstance result = target.GetInstance(instanceName);

                Assert.IsNotNull(result, "CreateInstance() returned null.");
                Assert.AreEqual(instanceName, result.Name, "SqlLocalDbInstance.Name is incorrect.");

                ISqlLocalDbInstanceInfo info = result.GetInstanceInfo();

                Assert.IsTrue(info.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");
                Assert.IsFalse(info.IsRunning, "ISqlLocalDbInstanceInfo.IsRunning is incorrect.");

                Guid guid;
                Assert.IsTrue(Guid.TryParse(result.Name, out guid), "SqlLocalDbInstance.Name is not a valid GUID.");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests GetInstances()")]
        public void GetInstances()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();
            
            // Act
            IList<ISqlLocalDbInstanceInfo> result = target.GetInstances();

            int initialCount = result.Count;

            // Assert
            Assert.IsNotNull(result, "GetInstances() returned null.");
            Assert.IsTrue(result.Count > 0, "No instances were returned by GetInstances().");
            CollectionAssert.AllItemsAreNotNull(result.ToList(), "GetInstances() returned a null instance.");

            string instanceName = Guid.NewGuid().ToString();
            target.CreateInstance(instanceName);

            try
            {
                result = target.GetInstances();

                // Assert
                Assert.IsNotNull(result, "GetInstances() returned null.");
                Assert.AreEqual(initialCount + 1, result.Count, "An incorrect number instances were returned by GetInstances().");
                CollectionAssert.AllItemsAreNotNull(result.ToList(), "GetInstances() returned a null instance.");
                Assert.IsTrue(result.Where((p) => p.Name == instanceName).Any(), "The new instance was not returned in the created set of instances.");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }

            result = target.GetInstances();

            // Assert
            Assert.IsNotNull(result, "GetInstances() returned null.");
            Assert.AreEqual(initialCount, result.Count, "An incorrect number instances were returned by GetInstances().");
            CollectionAssert.AllItemsAreNotNull(result.ToList(), "GetInstances() returned a null instance.");
        }

        [TestMethod]
        [Description("Tests GetVersions()")]
        public void GetVersions()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();

            // Act
            IList<ISqlLocalDbVersionInfo> result = target.GetVersions();

            // Assert
            Assert.IsNotNull(result, "GetVersions() returned null.");
            Assert.IsTrue(result.Count > 0, "No versions were returned by GetVersions().");
            CollectionAssert.AllItemsAreNotNull(result.ToList(), "GetVersions() returned a null instance.");
        }

        [TestMethod]
        [Description("Tests ISqlLocalDbFactory.CreateInstance(string).")]
        public void ISqlLocalDbFactory_CreateInstance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            ISqlLocalDbProvider target = new SqlLocalDbProvider();
            string instanceName = Guid.NewGuid().ToString();

            // Act
            ISqlLocalDbInstance result = target.CreateInstance(instanceName);

            // Assert
            Assert.IsNotNull(result, "CreateInstance() returned null.");
            Assert.AreEqual(instanceName, result.Name, "SqlLocalDbInstance.Name is incorrect.");

            try
            {
                ISqlLocalDbInstanceInfo info = result.GetInstanceInfo();

                Assert.IsTrue(info.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");
                Assert.IsFalse(info.IsRunning, "ISqlLocalDbInstanceInfo.IsRunning is incorrect.");

                Guid guid;
                Assert.IsTrue(Guid.TryParse(result.Name, out guid), "SqlLocalDbInstance.Name is not a valid GUID.");
            }
            finally
            {
                SqlLocalDbInstance.Delete(result);
            }
        }

        [TestMethod]
        [Description("Tests ISqlLocalDbFactory.GetInstance(string).")]
        public void ISqlLocalDbFactory_GetInstance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            ISqlLocalDbProvider target = new SqlLocalDbProvider();
            string instanceName = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                // Assert
                ISqlLocalDbInstance result = target.GetInstance(instanceName);

                Assert.IsNotNull(result, "CreateInstance() returned null.");
                Assert.AreEqual(instanceName, result.Name, "SqlLocalDbInstance.Name is incorrect.");

                ISqlLocalDbInstanceInfo info = result.GetInstanceInfo();

                Assert.IsTrue(info.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");
                Assert.IsFalse(info.IsRunning, "ISqlLocalDbInstanceInfo.IsRunning is incorrect.");

                Guid guid;
                Assert.IsTrue(Guid.TryParse(result.Name, out guid), "SqlLocalDbInstance.Name is not a valid GUID.");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests the Version property getter and setter work correctly.")]
        public void Version_Property_Can_Be_Set_Correctly()
        {
            // Arrange
            string value = "3.4.5.6";
            string latestVersion = SqlLocalDbApi.LatestVersion;

            SqlLocalDbProvider target = new SqlLocalDbProvider();

            // Act
            string result = target.Version;

            // Assert
            Assert.AreEqual(latestVersion, result, "SqlLocalDbProvider.Version returned incorrect value.");

            // Act
            target.Version = value;
            result = target.Version;

            // Assert
            Assert.AreEqual(value, result, "SqlLocalDbProvider.Version returned incorrect value.");

            // Act
            target.Version = null;
            result = target.Version;

            // Assert
            Assert.AreEqual(latestVersion, result, "SqlLocalDbProvider.Version returned incorrect value.");
        }

        #endregion
    }
}