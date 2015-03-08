// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbApiTests.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbApiTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="SqlLocalDbApi"/> class.
    /// </summary>
    [TestClass]
    public class SqlLocalDbApiTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbApiTests"/> class.
        /// </summary>
        public SqlLocalDbApiTests()
        {
        }

        [TestMethod]
        [Description("Tests the default value of the AutomaticallyDeleteInstanceFiles property.")]
        public void SqlLocalDbApi_AutomaticallyDeleteInstanceFiles_Is_False_By_Default()
        {
            // Act
            bool value = SqlLocalDbApi.AutomaticallyDeleteInstanceFiles;

            // Assert
            Assert.IsFalse(value, "The default value of SqlLocalDbApi.AutomaticallyDeleteInstanceFiles is incorrect.");
        }

        [TestMethod]
        [Description("Tests the default value of the AutomaticallyDeleteInstanceFiles property if overridden in the configuration file.")]
        public void SqlLocalDbApi_AutomaticallyDeleteInstanceFiles_Can_Be_Overridden_From_Configuration_File()
        {
            // Act
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    bool value = SqlLocalDbApi.AutomaticallyDeleteInstanceFiles;

                    // Assert
                    Assert.IsTrue(value, "The default value of SqlLocalDbApi.AutomaticallyDeleteInstanceFiles is incorrect.");
                },
                configurationFile: "SqlLocalDbApiTests.AutomaticallyDeleteInstanceFiles.config");
        }

        [TestMethod]
        [Description("Tests that the AutomaticallyDeleteInstanceFiles property can be set.")]
        public void SqlLocalDbApi_AutomaticallyDeleteInstanceFiles_Can_Be_Set()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Act
                    SqlLocalDbApi.AutomaticallyDeleteInstanceFiles = true;
                    bool value = SqlLocalDbApi.AutomaticallyDeleteInstanceFiles;

                    // Assert
                    Assert.IsTrue(value, "The value of SqlLocalDbApi.AutomaticallyDeleteInstanceFiles is incorrect.");
                });
        }

        [TestMethod]
        [Description("Tests CreateInstance(string) if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_CreateInstance_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = null;

            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.CreateInstance(instanceName),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests CreateInstance(string, string) if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_CreateInstance_With_Version_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            string instanceName = null;
            string version = string.Empty;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.CreateInstance(instanceName, version),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests CreateInstance(string, string) if version is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_CreateInstance_Throws_If_Version_Is_Null()
        {
            // Arrange
            string instanceName = string.Empty;
            string version = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.CreateInstance(instanceName, version),
                "version");
        }

        [TestMethod]
        [Description("Tests CreateInstance(string) if the instance does not already exist.")]
        public void SqlLocalDbApi_CreateInstance_If_Instance_Does_Not_Exist()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            IList<string> instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.DoesNotContain(instanceNames.ToArray(), instanceName, "The specified instance name already exists.");

            // Act
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                // Assert
                instanceNames = SqlLocalDbApi.GetInstanceNames();
                CollectionAssert.Contains(instanceNames.ToArray(), instanceName, "The specified instance was not created.");

                ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
                Assert.AreEqual(instanceName, info.Name, "ISqlLocalDbInstanceInfo.Name is incorrect.");
                Assert.IsTrue(info.Exists, "The LocalDB instance has not been created.");
                Assert.IsFalse(info.IsAutomatic, "ISqlLocalDbInstanceInfo.IsAutomatic is incorrect.");
                Assert.IsFalse(info.IsRunning, "ISqlLocalDbInstanceInfo.IsRunning is incorrect.");
                Assert.IsFalse(info.IsShared, "ISqlLocalDbInstanceInfo.IsShared is incorrect.");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstanceInternal(instanceName, throwIfNotFound: false);
            }
        }

        [TestMethod]
        [Description("Tests CreateInstance(string) if the instance cannot be created.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbApi_CreateInstance_If_Instance_Creation_Fails()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            // Use an invalid instance name
            string instanceName = string.Empty;

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.CreateInstance(instanceName));

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.InvalidParameter, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests DeleteInstance() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_DeleteInstance_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            string instanceName = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.DeleteInstance(instanceName),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests DeleteInstance() if instanceName does not exist.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbApi_DeleteInstance_Throws_If_InstanceName_Does_Not_Exist()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.DeleteInstance(instanceName));

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.UnknownInstance, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");
            
            throw error;
        }

        [TestMethod]
        [Description("Tests DeleteInstance(string).")]
        public void SqlLocalDbApi_DeleteInstance_Deletes_Instance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            IList<string> instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.DoesNotContain(instanceNames.ToArray(), instanceName, "The specified instance name already exists.");

            SqlLocalDbApi.CreateInstance(instanceName);

            instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.Contains(instanceNames.ToArray(), instanceName, "The specified instance was not created.");

            // Act
            SqlLocalDbApi.DeleteInstance(instanceName);

            // Assert
            ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

            Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
            Assert.AreEqual(instanceName, info.Name, "ISqlLocalDbInstanceInfo.Name is incorrect..");
            Assert.IsFalse(info.Exists, "The LocalDB instance has not been deleted.");

            string instancePath = GetInstanceFolderPath(instanceName);
            Assert.IsTrue(Directory.Exists(instancePath), "The instance folder was deleted.");
            Assert.AreNotEqual(0, Directory.GetFiles(instancePath).Length, "The instance files were deleted.");
        }

        [TestMethod]
        [Description("Tests DeleteInstance(string) deletes the instance folder if AutomaticallyDeleteInstanceFiles is true.")]
        public void SqlLocalDbApi_DeleteInstance_Deletes_Folder_If_AutomaticallyDeleteInstanceFiles_Is_True()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    string instanceName = Guid.NewGuid().ToString();

                    IList<string> instanceNames = SqlLocalDbApi.GetInstanceNames();
                    CollectionAssert.DoesNotContain(instanceNames.ToArray(), instanceName, "The specified instance name already exists.");

                    SqlLocalDbApi.CreateInstance(instanceName);

                    instanceNames = SqlLocalDbApi.GetInstanceNames();
                    CollectionAssert.Contains(instanceNames.ToArray(), instanceName, "The specified instance was not created.");

                    // Act
                    SqlLocalDbApi.AutomaticallyDeleteInstanceFiles = true;
                    SqlLocalDbApi.DeleteInstance(instanceName);

                    // Assert
                    ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                    Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
                    Assert.AreEqual(instanceName, info.Name, "ISqlLocalDbInstanceInfo.Name is incorrect..");
                    Assert.IsFalse(info.Exists, "The LocalDB instance has not been deleted.");

                    string instancePath = GetInstanceFolderPath(instanceName);
                    Assert.IsFalse(Directory.Exists(instancePath), "The instance folder was not deleted.");
                });
        }

        [TestMethod]
        [Description("Tests DeleteInstance(string, bool).")]
        public void SqlLocalDbApi_DeleteInstance_Deletes_Instance_Folder()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            IList<string> instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.DoesNotContain(instanceNames.ToArray(), instanceName, "The specified instance name already exists.");

            SqlLocalDbApi.CreateInstance(instanceName);

            instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.Contains(instanceNames.ToArray(), instanceName, "The specified instance was not created.");

            // Act
            SqlLocalDbApi.DeleteInstance(instanceName, deleteFiles: true);

            // Assert
            ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

            Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
            Assert.AreEqual(instanceName, info.Name, "ISqlLocalDbInstanceInfo.Name is incorrect..");
            Assert.IsFalse(info.Exists, "The LocalDB instance has not been deleted.");

            string instancePath = GetInstanceFolderPath(instanceName);
            Assert.IsFalse(Directory.Exists(instancePath), "The instance folder was not deleted.");
        }

        [TestMethod]
        [Description("Tests DeleteUserInstances() deletes only user instances and does not delete the instance folders.")]
        public void SqlLocalDbApi_DeleteUserInstances_Deletes_Only_User_Instances()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                IList<string> namesBefore = SqlLocalDbApi.GetInstanceNames();
                IList<string> versionsBefore = SqlLocalDbApi.Versions;

                // Act
                int result = SqlLocalDbApi.DeleteUserInstances();

                // Assert
                IList<string> namesAfter = SqlLocalDbApi.GetInstanceNames();
                IList<string> versionsAfter = SqlLocalDbApi.Versions;

                // The default instances have a name which is the version prefixed with 'v' when
                // using the SQL LocalDB 2012 native API. With the SQL LocalDB 2014 native API
                // (and presumably later versions), the default instance is named 'MSSQLLocalDB'.
                string[] expectedNames = namesAfter
                    .Where((p) => p.StartsWith("v", StringComparison.Ordinal) ||
                                  string.Equals(p, "MSSQLLocalDB", StringComparison.Ordinal) ||
                                  string.Equals(p, @".\MSSQLLocalDB", StringComparison.Ordinal))
                    .ToArray();

                CollectionAssert.AreEquivalent(expectedNames, namesAfter.ToArray(), "One or more instance was not deleted.");
                CollectionAssert.AreEquivalent(versionsBefore.ToArray(), versionsAfter.ToArray(), "One or more default instances was deleted.");
                Assert.AreEqual(namesBefore.Count - namesAfter.Count, result, "DeleteUserInstances() returned incorrect result.");

                string instancePath = GetInstanceFolderPath(instanceName);
                Assert.IsTrue(Directory.Exists(instancePath), "The instance folder was deleted.");
                Assert.AreNotEqual(0, Directory.GetFiles(instancePath).Length, "The instance files were deleted.");
            }
            finally
            {
                // Try to delete the instance we created if the test fails
                SqlLocalDbApi.DeleteInstanceInternal(instanceName, throwIfNotFound: false, deleteFiles: true);
            }
        }

        [TestMethod]
        [Description("Tests DeleteUserInstances() deletes only user instances and deletes the instance folders if AutomaticallyDeleteInstanceFiles is true.")]
        public void SqlLocalDbApi_DeleteUserInstances_Deletes_Only_User_Instances_If_AutomaticallyDeleteInstanceFiles_Is_True()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    string instanceName = Guid.NewGuid().ToString();
                    SqlLocalDbApi.CreateInstance(instanceName);

                    try
                    {
                        IList<string> namesBefore = SqlLocalDbApi.GetInstanceNames();
                        IList<string> versionsBefore = SqlLocalDbApi.Versions;

                        // Act
                        SqlLocalDbApi.AutomaticallyDeleteInstanceFiles = true;
                        int result = SqlLocalDbApi.DeleteUserInstances();

                        // Assert
                        IList<string> namesAfter = SqlLocalDbApi.GetInstanceNames();
                        IList<string> versionsAfter = SqlLocalDbApi.Versions;

                        // The default instances have a name which is the version prefixed with 'v' when
                        // using the SQL LocalDB 2012 native API. With the SQL LocalDB 2014 native API
                        // (and presumably later versions), the default instance is named 'MSSQLLocalDB'.
                        string[] expectedNames = namesAfter
                            .Where((p) => p.StartsWith("v", StringComparison.Ordinal) ||
                                          string.Equals(p, "MSSQLLocalDB", StringComparison.Ordinal) ||
                                          string.Equals(p, @".\MSSQLLocalDB", StringComparison.Ordinal))
                            .ToArray();

                        CollectionAssert.AreEquivalent(expectedNames, namesAfter.ToArray(), "One or more instance was not deleted.");
                        CollectionAssert.AreEquivalent(versionsBefore.ToArray(), versionsAfter.ToArray(), "One or more default instances was deleted.");
                        Assert.AreEqual(namesBefore.Count - namesAfter.Count, result, "DeleteUserInstances() returned incorrect result.");

                        string instancePath = GetInstanceFolderPath(instanceName);
                        Assert.IsFalse(Directory.Exists(instancePath), "The instance folder was not deleted.");
                    }
                    finally
                    {
                        // Try to delete the instance we created if the test fails
                        SqlLocalDbApi.DeleteInstanceInternal(instanceName, throwIfNotFound: false, deleteFiles: true);
                    }
                });
        }

        [TestMethod]
        [Description("Tests DeleteUserInstances() deletes only user instances and deletes the instance folders.")]
        public void SqlLocalDbApi_DeleteUserInstances_Deletes_Only_User_Instances_And_Deletes_Instance_Folder()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                IList<string> namesBefore = SqlLocalDbApi.GetInstanceNames();
                IList<string> versionsBefore = SqlLocalDbApi.Versions;

                // Act
                int result = SqlLocalDbApi.DeleteUserInstances(deleteFiles: true);

                // Assert
                IList<string> namesAfter = SqlLocalDbApi.GetInstanceNames();
                IList<string> versionsAfter = SqlLocalDbApi.Versions;

                // The default instances have a name which is the version prefixed with 'v' when
                // using the SQL LocalDB 2012 native API. With the SQL LocalDB 2014 native API
                // (and presumably later versions), the default instance is named 'MSSQLLocalDB'.
                string[] expectedNames = namesAfter
                    .Where((p) => p.StartsWith("v", StringComparison.Ordinal) ||
                                  string.Equals(p, "MSSQLLocalDB", StringComparison.Ordinal) ||
                                  string.Equals(p, @".\MSSQLLocalDB", StringComparison.Ordinal))
                    .ToArray();

                CollectionAssert.AreEquivalent(expectedNames, namesAfter.ToArray(), "One or more instance was not deleted.");
                CollectionAssert.AreEquivalent(versionsBefore.ToArray(), versionsAfter.ToArray(), "One or more default instances was deleted.");
                Assert.AreEqual(namesBefore.Count - namesAfter.Count, result, "DeleteUserInstances() returned incorrect result.");

                string instancePath = GetInstanceFolderPath(instanceName);
                Assert.IsFalse(Directory.Exists(instancePath), "The instance folder was not deleted.");
            }
            finally
            {
                // Try to delete the instance we created if the test fails
                SqlLocalDbApi.DeleteInstanceInternal(instanceName, throwIfNotFound: false, deleteFiles: true);
            }
        }

        [TestMethod]
        [Description("Tests GetInstanceInfo() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_GetInstanceInfo_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            string instanceName = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.GetInstanceInfo(instanceName),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests GetInstanceInfo() if instanceName is invalid.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbApi_GetInstanceInfo_Throws_If_InstanceName_Is_Invalid()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = "\\\\";

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.GetInstanceInfo(instanceName));

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.InvalidInstanceName, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests GetInstanceInfo() if instanceName does not exist.")]
        public void SqlLocalDbApi_GetInstanceInfo_If_InstanceName_Does_Not_Exist()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            // Act
            ISqlLocalDbInstanceInfo result = SqlLocalDbApi.GetInstanceInfo(instanceName);

            // Assert
            Assert.IsNotNull(result, "GetInstanceInfo() returned null.");
            Assert.IsInstanceOfType(result, typeof(LocalDbInstanceInfo), "GetInstanceInfo() returned incorrect type.");
            Assert.AreEqual(instanceName, result.Name, "ISqlLocalDbInstanceInfo.Name is incorrect.");

            Assert.IsFalse(result.ConfigurationCorrupt, "ISqlLocalDbInstanceInfo.ConfigurationCorrupt is incorrect.");
            Assert.IsFalse(result.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");
            Assert.IsFalse(result.IsAutomatic, "ISqlLocalDbInstanceInfo.IsAutomatic is incorrect.");
            Assert.IsFalse(result.IsRunning, "ISqlLocalDbInstanceInfo.IsRunning is incorrect.");
            Assert.IsFalse(result.IsShared, "ISqlLocalDbInstanceInfo.IsShared is incorrect.");
            Assert.AreEqual(DateTime.MinValue, result.LastStartTimeUtc, "ISqlLocalDbInstanceInfo.LastStartTimeUtc is incorrect.");
            Assert.AreEqual(new Version(0, 0, 0, 0), result.LocalDbVersion, "ISqlLocalDbInstanceInfo.Version is incorrect.");
            Assert.AreEqual(string.Empty, result.NamedPipe, "ISqlLocalDbInstanceInfo.NamedPipe is incorrect.");
            Assert.AreEqual(string.Empty, result.OwnerSid, "ISqlLocalDbInstanceInfo.OwnerSid is incorrect.");
            Assert.AreEqual(string.Empty, result.SharedName, "ISqlLocalDbInstanceInfo.SharedName is incorrect.");
        }

        [TestMethod]
        [Description("Tests GetInstanceNames().")]
        public void SqlLocalDbApi_GetInstanceNames_Returns_Instance_Names()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            // Act
            IList<string> result = SqlLocalDbApi.GetInstanceNames();

            // Assert
            Assert.IsNotNull(result, "GetInstanceNames() returned null.");
            Assert.IsTrue(result.Count > 0, "IList<string>.Count is less than one.");
            CollectionAssert.AllItemsAreNotNull(result.ToArray(), "An SQL LocalDB instance name is null.");
            CollectionAssert.AllItemsAreUnique(result.ToArray(), "A duplicate SQL LocalDb instance name was returned.");
        }

        [TestMethod]
        [Description("Tests that GetInstancesFolderPath() returns the correct value.")]
        public void SqlLocalDbApi_GetInstancesFolderPath_Returns_Correct_Path()
        {
            // Arrange
            string expected = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Microsoft",
                "Microsoft SQL Server Local DB",
                "Instances");

            // Act
            string result = SqlLocalDbApi.GetInstancesFolderPath();

            // Assert
            Assert.AreEqual(expected, result, "GetInstancesFolderPath() returned incorrect value.");
        }

        [TestMethod]
        [Description("Tests GetVersionInfo() if version is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_GetVersionInfo_Throws_If_Version_Is_Null()
        {
            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.GetVersionInfo(null),
                "version");
        }

        [TestMethod]
        [Description("Tests GetVersionInfo() if instanceName is invalid.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbApi_GetVersionInfo_Throws_If_InstanceName_Is_Invalid()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string version = "\\\\";

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.GetVersionInfo(version));

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.InvalidParameter, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(string.Empty, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests GetVersionInfo().")]
        public void SqlLocalDbApi_GetVersionInfo_Returns_Version_Information()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string version = SqlLocalDbApi.LatestVersion;

            // Act
            ISqlLocalDbVersionInfo result = SqlLocalDbApi.GetVersionInfo(version);

            // Assert
            Assert.IsNotNull(result, "GetVersionInfo() returned null.");
            Assert.IsInstanceOfType(result, typeof(LocalDbVersionInfo), "GetVersionInfo() returned incorrect type.");
            Assert.IsTrue(result.Exists, "ISqlLocalDbVersionInfo.Exists is incorrect.");
            Assert.IsNotNull(result.Name, "ISqlLocalDbVersionInfo.Name is null.");
            Assert.IsNotNull(result.Version, "ISqlLocalDbVersionInfo.Version is null.");

            StringAssert.StartsWith(result.Name, version, "ISqlLocalDbVersionInfo.Name is incorrect.");

            Version versionFromString = new Version(version);

            Assert.AreEqual(versionFromString.Major, result.Version.Major, "ISqlLocalDbVersionInfo.Version.Major is incorrect.");
            Assert.AreEqual(versionFromString.Minor, result.Version.Minor, "ISqlLocalDbVersionInfo.Version.Minor is incorrect.");
        }

        [TestMethod]
        [Description("Tests the LatestVersion property returns the correct value.")]
        public void SqlLocalDbApi_LatestVersion_Returns_Latest_Version()
        {
            // Arrange
            IList<string> installedVersions = SqlLocalDbApi.Versions;

            if (installedVersions.Count < 2)
            {
                Assert.Inconclusive("Only one version of SQL LocalDB is installed on '{0}'.", Environment.MachineName);
            }

            List<Version> versions = installedVersions
                .Select((p) => new Version(p))
                .ToList();

            versions.Sort();

            string expected = versions
                .Last()
                .ToString();

            // Act
            string result = SqlLocalDbApi.LatestVersion;

            // Assert
            Assert.AreEqual(expected, result, "SqlLocalDbApi.LatestVersion returned incorrect result.");
        }

        [TestMethod]
        [Description("Tests ShareInstance() if sharedInstanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_ShareInstance_Throws_If_SharedInstanceName_Is_Null()
        {
            // Arrange
            string ownerSid = string.Empty;
            string instanceName = string.Empty;
            string sharedInstanceName = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.ShareInstance(ownerSid, instanceName, sharedInstanceName),
                "sharedInstanceName");
        }

        [TestMethod]
        [Description("Tests ShareInstance() if ownerSid is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_ShareInstance_Throws_If_OwnerSid_Is_Null()
        {
            // Arrange
            string ownerSid = null;
            string instanceName = string.Empty;
            string sharedInstanceName = string.Empty;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.ShareInstance(ownerSid, instanceName, sharedInstanceName),
                "ownerSid");
        }

        [TestMethod]
        [Description("Tests ShareInstance() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_ShareInstance_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            string ownerSid = string.Empty;
            string instanceName = null;
            string sharedInstanceName = string.Empty;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.ShareInstance(ownerSid, instanceName, sharedInstanceName),
                "instanceName");
        }

        [TestMethod]
        [TestCategory(TestCategories.RequiresAdministrativePermissions)]
        [Description("Tests ShareInstance().")]
        public void SqlLocalDbApi_ShareInstance_Shares_Instance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();
            Helpers.EnsureUserIsAdmin();

            string instanceName = Guid.NewGuid().ToString();
            string sharedName = Guid.NewGuid().ToString();

            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbApi.StartInstance(instanceName);

                try
                {
                    ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                    Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
                    Assert.IsFalse(info.IsShared, "ISqlLocalDbInstanceInfo.IsShared is incorrect.");
                    Assert.AreEqual(string.Empty, info.SharedName, "ISqlLocalDbInstanceInfo.SharedName is incorrect.");

                    // Act
                    SqlLocalDbApi.ShareInstance(
                        instanceName,
                        sharedName);

                    try
                    {
                        // Assert
                        info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                        Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
                        Assert.IsTrue(info.IsShared, "ISqlLocalDbInstanceInfo.IsShared is incorrect.");
                        Assert.AreEqual(sharedName, info.SharedName, "ISqlLocalDbInstanceInfo.SharedName is incorrect.");
                    }
                    finally
                    {
                        SqlLocalDbApi.UnshareInstance(instanceName);
                    }
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstanceInternal(instanceName, throwIfNotFound: false);
            }
        }

        [TestMethod]
        [Description("Tests ShareInstance() if instanceName is invalid.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbApi_ShareInstance_Throws_If_InstanceName_Is_Invalid()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            string sharedInstanceName = "\\\\";

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.ShareInstance(instanceName, sharedInstanceName));

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.InvalidInstanceName, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests ShareInstance() if instanceName is the empty string.")]
        [ExpectedException(typeof(ArgumentException))]
        public void SqlLocalDbApi_ShareInstance_Throws_If_InstanceName_Is_Empty_String()
        {
            // Act and Assert
            throw ErrorAssert.Throws<ArgumentException>(
                () => SqlLocalDbApi.ShareInstance(string.Empty, Guid.NewGuid().ToString()),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests StartInstance() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_StartInstance_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            string instanceName = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.StartInstance(instanceName),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests StartInstance() if instanceName does not exist.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbApi_StartInstance_Throws_If_InstanceName_Does_Not_Exist()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.StartInstance(instanceName));

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.UnknownInstance, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests StartInstance(string).")]
        public void SqlLocalDbApi_StartInstance_Starts_Instance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            IList<string> instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.DoesNotContain(instanceNames.ToArray(), instanceName, "The specified instance name already exists.");

            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                instanceNames = SqlLocalDbApi.GetInstanceNames();
                CollectionAssert.Contains(instanceNames.ToArray(), instanceName, "The specified instance was not created.");

                try
                {
                    DateTime beforeStart = DateTime.UtcNow;

                    // Act
                    string namedPipe = SqlLocalDbApi.StartInstance(instanceName);

                    // Assert
                    DateTime afterStart = DateTime.UtcNow;

                    Assert.IsNotNull(namedPipe, "StartInstance() returned null.");

                    ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                    Assert.IsNotNull(info, "GetInstanceInfo() returned null.");

                    Assert.AreEqual(namedPipe, info.NamedPipe, "The returned named pipe is incorrect.");
                    Assert.IsTrue(info.IsRunning, "The LocalDB instance has not been started");
                    Assert.AreEqual(DateTimeKind.Utc, info.LastStartTimeUtc.Kind, "ISqlLocalDbInstanceInfo.LastStartTimeUtc.Kind is incorrect.");
                    Assert.IsTrue(info.LastStartTimeUtc >= beforeStart, "ISqlLocalDbInstanceInfo.LastStartTimeUtc is too early.");
                    Assert.IsTrue(info.LastStartTimeUtc <= afterStart, "ISqlLocalDbInstanceInfo.LastStartTimeUtc is too late.");
                }
                finally
                {
                    SqlLocalDbApi.StopInstance(instanceName);
                }
            }
            finally
            {
                SqlLocalDbApi.DeleteInstanceInternal(instanceName, throwIfNotFound: false);
            }
        }

        [TestMethod]
        [Description("Tests the StartTracing() method.")]
        public void SqlLocalDbApi_StartTracing_Does_Not_Throw()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            // Act (no Assert)
            // No easy way to ensure it's initialized, so just test method doesn't throw
            SqlLocalDbApi.StartTracing();

            try
            {
                SqlLocalDbApi.StartTracing();
            }
            finally
            {
                SqlLocalDbApi.StopTracing();
            }
        }

        [TestMethod]
        [Description("Tests StopInstance(string) if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_StopInstance_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            string instanceName = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.StopInstance(instanceName),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests StopInstance(string, TimeSpan) if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbApi_StopInstance_With_Timeout_Throws_If_InstanceName_Is_Null()
        {
            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.StopInstance(null, TimeSpan.Zero),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests StopInstance(string, TimeSpan) if timeout is less than zero.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SqlLocalDbApi_StopInstance_Throws_If_Timeout_Is_Less_Than_Zero()
        {
            // Arrange
            TimeSpan value = TimeSpan.Zero - TimeSpan.FromTicks(1);

            // Act
            ArgumentOutOfRangeException error = ErrorAssert.Throws<ArgumentOutOfRangeException>(
               () => SqlLocalDbApi.StopInstance(string.Empty, value),
               "timeout");

            // Assert
            Assert.AreEqual(
                value,
                error.ActualValue,
                "ArgumentOutOfRangeException.ActualValue is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests StopInstance() if instanceName does not exist.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void SqlLocalDbApi_StopInstance_Throws_If_InstanceName_Does_Not_Exist()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.StopInstance(instanceName));

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.UnknownInstance, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests StopInstance(string).")]
        public void SqlLocalDbApi_StopInstance_Stops_Instance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            IList<string> instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.DoesNotContain(instanceNames.ToArray(), instanceName, "The specified instance name already exists.");

            SqlLocalDbApi.CreateInstance(instanceName);

            instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.Contains(instanceNames.ToArray(), instanceName, "The specified instance was not created.");

            try
            {
                SqlLocalDbApi.StartInstance(instanceName);

                // Act
                SqlLocalDbApi.StopInstance(instanceName);

                // Assert
                ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                Assert.IsNotNull(info, "GetInstanceInfo() returned null.");

                Assert.AreEqual(string.Empty, info.NamedPipe, "ISqlLocalDbInstanceInfo.NamedPipe is incorrect.");
                Assert.IsFalse(info.IsRunning, "The LocalDB instance has not been started");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstanceInternal(instanceName, throwIfNotFound: false);
            }
        }

        [TestMethod]
        [Description("Tests StopInstance(string, StopInstanceOptions, TimeSpan).")]
        public void SqlLocalDbApi_StopInstance_When_All_Parameters_Specified()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();
            StopInstanceOptions options = StopInstanceOptions.KillProcess;
            TimeSpan timeout = TimeSpan.FromSeconds(30);

            IList<string> instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.DoesNotContain(instanceNames.ToArray(), instanceName, "The specified instance name already exists.");

            SqlLocalDbApi.CreateInstance(instanceName);

            instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.Contains(instanceNames.ToArray(), instanceName, "The specified instance was not created.");

            try
            {
                SqlLocalDbApi.StartInstance(instanceName);

                // Act
                SqlLocalDbApi.StopInstance(instanceName, options, timeout);

                // Assert
                ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                Assert.IsNotNull(info, "GetInstanceInfo() returned null.");

                Assert.AreEqual(string.Empty, info.NamedPipe, "ISqlLocalDbInstanceInfo.NamedPipe is incorrect.");
                Assert.IsFalse(info.IsRunning, "The LocalDB instance has not been started");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstanceInternal(instanceName, throwIfNotFound: false);
            }
        }

        [TestMethod]
        [Description("Tests the default value of the StopOptions property.")]
        public void SqlLocalDbApi_StopOptions_Has_Correct_Default_Value()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Act
                    StopInstanceOptions result = SqlLocalDbApi.StopOptions;

                    // Assert
                    Assert.AreEqual(
                        StopInstanceOptions.None,
                        result,
                        "SqlLocalDbApi.StopOptions is incorrect.");
                });
        }

        [TestMethod]
        [Description("Tests the StopOptions property.")]
        public void SqlLocalDbApi_StopOptions_Can_Be_Set()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    StopInstanceOptions value = StopInstanceOptions.NoWait;

                    // Act
                    SqlLocalDbApi.StopOptions = value;

                    // Assert
                    Assert.AreEqual(value, StopInstanceOptions.NoWait, "SqlLocalDbApi.StopOptions is incorrect.");
                });
        }

        [TestMethod]
        [Description("Tests the default value of the StopTimeout property.")]
        public void SqlLocalDbApi_StopTimeout_DefaultValue()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Act
                    TimeSpan result = SqlLocalDbApi.StopTimeout;

                    // Assert
                    Assert.AreEqual(
                        TimeSpan.FromMinutes(1),
                        result,
                        "SqlLocalDbApi.StopTimeout is incorrect.");
                });
        }

        [TestMethod]
        [Description("Tests the the StopTimeout property if value is invalid.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SqlLocalDbApi_StopTimeout_Throws_If_Value_Is_Invalid()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    TimeSpan value = TimeSpan.FromMinutes(-6);

                    // Act
                    ArgumentOutOfRangeException error = ErrorAssert.Throws<ArgumentOutOfRangeException>(
                        () => SqlLocalDbApi.StopTimeout = value,
                        "value");

                    // Assert
                    Assert.AreEqual(
                        value,
                        error.ActualValue,
                        "ArgumentOutOfRangeException.ActualValue is incorrect.");

                    throw error;
                });
        }

        [TestMethod]
        [Description("Tests the StopTimeout property.")]
        public void SqlLocalDbApi_StopTimeout_Can_Be_Set()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    TimeSpan value = TimeSpan.FromMilliseconds(500);

                    // Act
                    SqlLocalDbApi.StopTimeout = value;

                    // Assert
                    Assert.AreEqual(value, SqlLocalDbApi.StopTimeout, "SqlLocalDbApi.StopTimeout is incorrect.");
                });
        }

        [TestMethod]
        [Description("Tests the StopTracing() method.")]
        public void SqlLocalDbApi_StopTracing_Does_Not_Throw()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            // Act (no Assert)
            // No easy way to ensure it's disabled, so just test method doesn't throw
            SqlLocalDbApi.StopTracing();
            SqlLocalDbApi.StopTracing();
        }

        [TestMethod]
        [Description("Tests UnshareInstance() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the method under test.")]
        public void SqlLocalDbApi_UnshareInstance_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            string instanceName = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.UnshareInstance(instanceName),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests UnshareInstance() if instanceName does not exist.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the method under test.")]
        public void SqlLocalDbApi_UnshareInstance_Throws_If_InstanceName_Does_Not_Exist()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.UnshareInstance(instanceName));

            // Assert
            Assert.AreEqual(SqlLocalDbErrors.InstanceNotShared, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [TestCategory(TestCategories.RequiresAdministrativePermissions)]
        [Description("Tests UnshareInstance().")]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the method under test.")]
        public void SqlLocalDbApi_UnshareInstance_Stops_Sharing_Instance()
        {
            // Arrange
            Helpers.EnsureLocalDBInstalled();
            Helpers.EnsureUserIsAdmin();

            string instanceName = Guid.NewGuid().ToString();
            string sharedName = Guid.NewGuid().ToString();

            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                SqlLocalDbApi.StartInstance(instanceName);

                try
                {
                    SqlLocalDbApi.ShareInstance(
                        instanceName,
                        sharedName);

                    ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                    Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
                    Assert.IsTrue(info.IsShared, "ISqlLocalDbInstanceInfo.IsShared is incorrect.");
                    Assert.AreEqual(sharedName, info.SharedName, "ISqlLocalDbInstanceInfo.SharedName is incorrect.");

                    // Act
                    SqlLocalDbApi.UnshareInstance(instanceName);

                    // Assert
                    info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                    Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
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
                SqlLocalDbApi.DeleteInstanceInternal(instanceName, throwIfNotFound: false);
            }
        }

        [TestMethod]
        [Description("Tests that DeleteInstanceFiles() escapes the instanceName parameter to prevent path traversal when deleting files.")]
        public void SqlLocalDbApi_DeleteInstanceFiles_Escapes_InstanceName()
        {
            // Arrange - Create a path to a directory somewhere on disk that is separate to LocalDB
            string directoryName = Guid.NewGuid().ToString();
            string path = Path.Combine(Path.GetTempPath(), directoryName);

            // Get the drive letters associated with the SQL LocalDB instances and the temporary directory
            string instancesFolderPathRoot = Directory.GetDirectoryRoot(SqlLocalDbApi.GetInstancesFolderPath());
            string tempPathRoot = Directory.GetDirectoryRoot(path);

            // Assert - We can't run this test if %TEMP% isn't on the same drive as %LOCALAPPDATA%.
            if (!string.Equals(instancesFolderPathRoot, tempPathRoot, StringComparison.OrdinalIgnoreCase))
            {
                Assert.Inconclusive(
                    "The SQL LocalDB instances folder is stored on a different drive ({0}) than the temporary directory ({1}).",
                    instancesFolderPathRoot,
                    tempPathRoot);
            }

            // Get the path of the temporary directory without the drive and use it to build an instance
            // name that would cause path traversal from the SQL LocalDB instances folder to the temporary folder.
            string pathWithoutDrive = path.Replace(tempPathRoot, string.Empty);

            int pathSegments = path.Split(Path.DirectorySeparatorChar).Length;

            string instanceName = Path.Combine(
                string.Join(Path.DirectorySeparatorChar.ToString(), Enumerable.Repeat("..", pathSegments)),
                pathWithoutDrive);

            Directory.CreateDirectory(path);

            try
            {
                // Act
                SqlLocalDbApi.DeleteInstanceFiles(instanceName);

                // Assert
                Assert.IsTrue(Directory.Exists(path), "The external directory was deleted due to path traversal in the instance name.");
            }
            finally
            {
                if (Directory.Exists(path))
                {
                    Directory.Delete(path, true);
                }
            }
        }

        [TestMethod]
        [Description("Tests that SqlLocalDbApi uses the correct default values if the configuration section is defined and the attributes are set.")]
        public void SqlLocalDbApi_Uses_User_Values_If_Configuration_Section_Defined_And_Attributes_Specified()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Assert
                    Assert.AreEqual(true, SqlLocalDbApi.AutomaticallyDeleteInstanceFiles, "SqlLocalDbApi.AutomaticallyDeleteInstanceFiles is incorrect.");
                    Assert.AreEqual(StopInstanceOptions.KillProcess | StopInstanceOptions.NoWait, SqlLocalDbApi.StopOptions, "SqlLocalDbApi.StopOptions is incorrect.");
                    Assert.AreEqual(TimeSpan.FromSeconds(30), SqlLocalDbApi.StopTimeout, "SqlLocalDbApi.StopTimeout is incorrect.");
                },
                configurationFile: "SqlLocalDbApiTests.PropertiesOverridden.config");
        }

        [TestMethod]
        [Description("Tests that the DefaultInstanceName property returns the correct value.")]
        public void SqlLocalDbApi_DefaultInstanceName_Returns_Correct_Value()
        {
            // Act
            string result = SqlLocalDbApi.DefaultInstanceName;

            // Assert
            if (NativeMethods.NativeApiVersion.Major == 11)
            {
                Assert.AreEqual("v11.0", result, "SqlLocalDbApi.DefaultInstanceName returned incorrect value.");
            }
            else
            {
                Assert.AreEqual("MSSQLLocalDB", result, "SqlLocalDbApi.DefaultInstanceName returned incorrect value.");
            }
        }

        [TestMethod]
        [Description("Tests that the DefaultInstanceName property returns the correct value for SQL LocalDB 2012.")]
        public void SqlLocalDbApi_DefaultInstanceName_Returns_Correct_Value_2012()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Act
                    string result = SqlLocalDbApi.DefaultInstanceName;

                    // Assert
                    Assert.AreEqual("v11.0", result, "SqlLocalDbApi.DefaultInstanceName returned incorrect value.");
                },
                configurationFile: "SqlLocalDbApiTests.DefaultInstanceName.2012.config");
        }

        [TestMethod]
        [Description("Tests that the DefaultInstanceName property returns the correct value for SQL LocalDB 2014.")]
        public void SqlLocalDbApi_DefaultInstanceName_Returns_Correct_Value_2014()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Act
                    string result = SqlLocalDbApi.DefaultInstanceName;

                    // Assert
                    Assert.AreEqual("MSSQLLocalDB", result, "SqlLocalDbApi.DefaultInstanceName returned incorrect value.");
                },
                configurationFile: "SqlLocalDbApiTests.DefaultInstanceName.2014.config");
        }

        [TestMethod]
        [Description("Tests that the DefaultLanguageId property returns the correct default value.")]
        public void SqlLocalDbApi_DefaultLanguageId_Returns_Correct_Value()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Act
                    int result = SqlLocalDbApi.DefaultLanguageId;

                    // Assert
                    Assert.AreEqual(0, result, "SqlLocalDbApi.DefaultLanguageId returned incorrect value.");
                });
        }

        [TestMethod]
        [Description("Tests that the DefaultLanguageId property is used to format error messages.")]
        public void SqlLocalDbApi_DefaultLanguageId_Used_By_GetLocalDbError_To_Format_Error_Messages()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Arrange
                    int hr = SqlLocalDbErrors.AdminRightsRequired;
                    int traceEventId = 0;

                    int lcid = GetLcidForCulture("en-US");
                    SqlLocalDbApi.DefaultLanguageId = lcid;

                    // Act
                    Exception result = SqlLocalDbApi.GetLocalDbError(hr, traceEventId);

                    // Assert
                    Assert.AreEqual("Administrator privileges are required in order to execute this operation.\r\n", result.Message, "The exception message was not correct for LCID {0}.", lcid);
                    Assert.AreEqual(hr, result.HResult, "The HResult returned in the exception is incorrect.");

                    // Arrange
                    lcid = GetLcidForCulture("es-ES");
                    SqlLocalDbApi.DefaultLanguageId = lcid;

                    // Act
                    result = SqlLocalDbApi.GetLocalDbError(hr, traceEventId);

                    // Assert
                    Assert.AreEqual("Se requieren privilegios de administrador para ejecutar esta operación.\r\n", result.Message, "The exception message was not correct for LCID {0}.", lcid);
                    Assert.AreEqual(hr, result.HResult, "The HResult returned in the exception is incorrect.");

                    // Arrange
                    lcid = GetLcidForCulture("fr-FR");
                    SqlLocalDbApi.DefaultLanguageId = lcid;

                    // Act
                    result = SqlLocalDbApi.GetLocalDbError(hr, traceEventId);

                    // Assert
                    Assert.AreEqual("Des privilèges d'administrateur sont requis pour exécuter cette opération.\r\n", result.Message, "The exception message was not correct for LCID {0}.", lcid);
                    Assert.AreEqual(hr, result.HResult, "The HResult returned in the exception is incorrect.");
                });
        }

        [TestMethod]
        [Description("Tests that GetLocalDbError() does not mask the original error if the DefaultLanguageId property is not a recognized LCID.")]
        public void SqlLocalDbApi_GetLocalDbError_Does_Not_Mask_Original_Error_If_DefaultLanguageId_Is_Not_Recognized()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Arrange
                    int hr = SqlLocalDbErrors.AdminRightsRequired;
                    int traceEventId = 0;

                    int lcid = GetLcidForCulture("en-GB");  // N.B. A new culture must be used if a future version supports en-GB.
                    SqlLocalDbApi.DefaultLanguageId = lcid;

                    // Act
                    Exception result = SqlLocalDbApi.GetLocalDbError(hr, traceEventId);

                    // Assert
                    Assert.AreEqual("An error occurred with SQL Server LocalDB. HRESULT = -1983577826", result.Message, "The exception message was not correct for LCID {0}.", lcid);
                    Assert.AreEqual(hr, result.HResult, "The HResult returned in the exception is incorrect.");
                });
        }

        /// <summary>
        /// Returns the full path of the folder for the specified SQL LocalDB instance.
        /// </summary>
        /// <param name="instanceName">The name of the SQL LocalDB instance.</param>
        /// <returns>
        /// The full path of the folder for the specified SQL LocalDB instance.
        /// </returns>
        private static string GetInstanceFolderPath(string instanceName)
        {
            return Path.Combine(SqlLocalDbApi.GetInstancesFolderPath(), instanceName);
        }

        /// <summary>
        /// Returns the LCID to use for the specified culture.
        /// </summary>
        /// <param name="name">The name of the culture to get the LCID for.</param>
        /// <returns>
        /// The LCID for the culture specified by <paramref name="name"/>.
        /// </returns>
        private static int GetLcidForCulture(string name)
        {
            return CultureInfo.GetCultureInfo(name).LCID;
        }
    }
}