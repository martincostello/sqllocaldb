// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbApiTests.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbApiTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
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
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbApiTests"/> class.
        /// </summary>
        public SqlLocalDbApiTests()
        {
        }

        #endregion

        #region Methods

        [TestMethod]
        [Description("Tests CreateInstance(string) if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstance_ThrowsIfInstanceNameIsNull()
        {
            Helpers.EnsureLocalDBInstalled();

            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.CreateInstance(null),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests CreateInstance(string, string) if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstance_OverloadThrowsIfInstanceNameIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.CreateInstance(null, string.Empty),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests CreateInstance(string, string) if version is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstance_ThrowsIfVersionNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.CreateInstance(string.Empty, null),
                "version");
        }

        [TestMethod]
        [Description("Tests CreateInstance(string) if the instance does not already exist.")]
        public void CreateInstance_InstanceDoesNotExist()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            IList<string> instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.DoesNotContain(instanceNames.ToArray(), instanceName, "The specified instance name already exists.");

            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
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
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests CreateInstance(string) if the instance cannot be created.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void CreateInstance_InstanceCreationFails()
        {
            Helpers.EnsureLocalDBInstalled();

            // Use an invalid instance name
            string instanceName = string.Empty;

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.CreateInstance(instanceName));

            Assert.AreEqual(SqlLocalDbErrors.InvalidParameter, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests DeleteInstance() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteInstance_ThrowsIfInstanceNameIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.DeleteInstance(null),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests DeleteInstance() if instanceName does not exist.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void DeleteInstance_ThrowsIfInstanceNameDoesNotExist()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.DeleteInstance(instanceName));

            Assert.AreEqual(SqlLocalDbErrors.UnknownInstance, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");
            
            throw error;
        }

        [TestMethod]
        [Description("Tests DeleteInstance(string).")]
        public void DeleteInstance()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            IList<string> instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.DoesNotContain(instanceNames.ToArray(), instanceName, "The specified instance name already exists.");

            SqlLocalDbApi.CreateInstance(instanceName);

            instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.Contains(instanceNames.ToArray(), instanceName, "The specified instance was not created.");

            SqlLocalDbApi.DeleteInstance(instanceName);

            ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

            Assert.IsNotNull(info, "GetInstanceInfo() returned null.");
            Assert.AreEqual(instanceName, info.Name, "ISqlLocalDbInstanceInfo.Name is incorrect..");
            Assert.IsFalse(info.Exists, "The LocalDB instance has not been deleted.");
        }

        [TestMethod]
        [Description("Tests GetInstanceInfo() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetInstanceInfo_ThrowsIfInstanceNameIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.GetInstanceInfo(null),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests GetInstanceInfo() if instanceName is invalid.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void GetInstanceInfo_ThrowsIfInstanceNameIsInvalid()
        {
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.GetInstanceInfo(string.Empty));

            Assert.AreEqual(SqlLocalDbErrors.InvalidParameter, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(string.Empty, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests GetInstanceInfo() if instanceName does not exist.")]
        public void GetInstanceInfo_InstanceNameDoesNotExist()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            ISqlLocalDbInstanceInfo result = SqlLocalDbApi.GetInstanceInfo(instanceName);

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
        public void GetInstanceNames()
        {
            Helpers.EnsureLocalDBInstalled();

            IList<string> result = SqlLocalDbApi.GetInstanceNames();

            Assert.IsNotNull(result, "GetInstanceNames() returned null.");
            Assert.IsTrue(result.Count > 0, "IList<string>.Count is less than one.");
            CollectionAssert.AllItemsAreNotNull(result.ToArray(), "An SQL LocalDB instance name is null.");
            CollectionAssert.AllItemsAreUnique(result.ToArray(), "A duplicate SQL LocalDb instance name was returned.");
        }

        [TestMethod]
        [Description("Tests GetVersionInfo() if version is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetVersionInfo_ThrowsIfVersionIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.GetVersionInfo(null),
                "version");
        }

        [TestMethod]
        [Description("Tests GetVersionInfo() if instanceName is invalid.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void GetVersionInfo_ThrowsIfInstanceNameIsInvalid()
        {
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.GetVersionInfo(string.Empty));

            Assert.AreEqual(SqlLocalDbErrors.InvalidParameter, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(string.Empty, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests GetVersionInfo().")]
        public void GetVersionInfo()
        {
            Helpers.EnsureLocalDBInstalled();

            string version = SqlLocalDbApi.LatestVersion;

            ISqlLocalDbVersionInfo result = SqlLocalDbApi.GetVersionInfo(version);

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
        [Description("Tests ShareInstance() if sharedInstanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShareInstance_ThrowsIfSharedInstanceNameIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.ShareInstance(string.Empty, string.Empty, null),
                "sharedInstanceName");
        }

        [TestMethod]
        [Description("Tests ShareInstance() if ownerSid is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShareInstance_ThrowsIfOwnerSidIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.ShareInstance(null, string.Empty, string.Empty),
                "ownerSid");
        }

        [TestMethod]
        [Description("Tests ShareInstance() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShareInstance_ThrowsIfInstanceNameIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.ShareInstance(string.Empty, null, string.Empty),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests ShareInstance().")]
        public void ShareInstance()
        {
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

                    SqlLocalDbApi.ShareInstance(
                        instanceName,
                        sharedName);

                    try
                    {
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
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests ShareInstance() if instanceName is invalid.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void ShareInstance_ThrowsIfInstanceNameIsInvalid()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.ShareInstance(instanceName, string.Empty));

            Assert.AreEqual(SqlLocalDbErrors.InvalidParameter, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests ShareInstance() if instanceName is the empty string.")]
        [ExpectedException(typeof(ArgumentException))]
        public void ShareInstance_ThrowsIfInstanceNameIsEmptyString()
        {
            throw ErrorAssert.Throws<ArgumentException>(
                () => SqlLocalDbApi.ShareInstance(string.Empty, Guid.NewGuid().ToString()),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests StartInstance() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StartInstance_ThrowsIfInstanceNameIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.StartInstance(null),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests StartInstance() if instanceName does not exist.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void StartInstance_ThrowsIfInstanceNameDoesNotExist()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.StartInstance(instanceName));

            Assert.AreEqual(SqlLocalDbErrors.UnknownInstance, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests StartInstance(string).")]
        public void StartInstance()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            IList<string> instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.DoesNotContain(instanceNames.ToArray(), instanceName, "The specified instance name already exists.");

            SqlLocalDbApi.CreateInstance(instanceName);

            instanceNames = SqlLocalDbApi.GetInstanceNames();
            CollectionAssert.Contains(instanceNames.ToArray(), instanceName, "The specified instance was not created.");

            try
            {
                DateTime beforeStart = DateTime.UtcNow;

                string namedPipe = SqlLocalDbApi.StartInstance(instanceName);

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
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests the StartTracing() method.")]
        public void StartTracing()
        {
            // No easy way to ensure it's initialized, so just test method doesn't throw
            Helpers.EnsureLocalDBInstalled();
            SqlLocalDbApi.StartTracing();
            SqlLocalDbApi.StartTracing();
        }

        [TestMethod]
        [Description("Tests StopInstance(string) if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StopInstance_ThrowsIfInstanceNameIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.StopInstance(null),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests StopInstance(string, TimeSpan) if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StopInstance_ThrowsIfInstanceNameIsNull2()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.StopInstance(null, TimeSpan.Zero),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests StopInstance(string, TimeSpan) if timeout is less than zero.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StopInstance_ThrowsIfTimeoutIsLessThanZero()
        {
            TimeSpan value = TimeSpan.Zero - TimeSpan.FromTicks(1);

            ArgumentOutOfRangeException error = ErrorAssert.Throws<ArgumentOutOfRangeException>(
               () => SqlLocalDbApi.StopInstance(string.Empty, value),
               "timeout");

            Assert.AreEqual(
                value,
                error.ActualValue,
                "ArgumentOutOfRangeException.ActualValue is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests StopInstance() if instanceName does not exist.")]
        [ExpectedException(typeof(SqlLocalDbException))]
        public void StopInstance_ThrowsIfInstanceNameDoesNotExist()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.StopInstance(instanceName));

            Assert.AreEqual(SqlLocalDbErrors.UnknownInstance, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests StopInstance(string).")]
        public void StopInstance()
        {
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
                SqlLocalDbApi.StopInstance(instanceName);

                ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                Assert.IsNotNull(info, "GetInstanceInfo() returned null.");

                Assert.AreEqual(string.Empty, info.NamedPipe, "ISqlLocalDbInstanceInfo.NamedPipe is incorrect.");
                Assert.IsFalse(info.IsRunning, "The LocalDB instance has not been started");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests StopInstance(string, StopInstanceOptions, TimeSpan).")]
        public void StopInstance_When_All_Parameters_Specified()
        {
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
                SqlLocalDbApi.StopInstance(instanceName, options, timeout);

                ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);

                Assert.IsNotNull(info, "GetInstanceInfo() returned null.");

                Assert.AreEqual(string.Empty, info.NamedPipe, "ISqlLocalDbInstanceInfo.NamedPipe is incorrect.");
                Assert.IsFalse(info.IsRunning, "The LocalDB instance has not been started");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests the default value of the StopTimeout property.")]
        public void StopTimeout_DefaultValue()
        {
            TimeSpan result = SqlLocalDbApi.StopTimeout;

            Assert.AreEqual(
                TimeSpan.FromMinutes(1),
                result,
                "SqlLocalDbApi.StopTimeout is incorrect.");
        }

        [TestMethod]
        [Description("Tests the the StopTimeout property if value is invalid.")]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void StopTimeout_ThrowsIfValueIsInvalid()
        {
            TimeSpan value = TimeSpan.FromMinutes(-6);

            ArgumentOutOfRangeException error = ErrorAssert.Throws<ArgumentOutOfRangeException>(
                () => SqlLocalDbApi.StopTimeout = value,
                "value");

            Assert.AreEqual(
                value,
                error.ActualValue,
                "ArgumentOutOfRangeException.ActualValue is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests the StopTimeout property.")]
        public void StopTimeout()
        {
            TimeSpan oldValue = SqlLocalDbApi.StopTimeout;

            try
            {
                TimeSpan value = TimeSpan.FromMilliseconds(500);
                SqlLocalDbApi.StopTimeout = value;
                Assert.AreEqual(value, SqlLocalDbApi.StopTimeout, "SqlLocalDbApi.StopTimeout is incorrect.");
            }
            finally
            {
                SqlLocalDbApi.StopTimeout = oldValue;
            }
        }

        [TestMethod]
        [Description("Tests the StopTracing() method.")]
        public void StopTracing()
        {
            // No easy way to ensure it's disabled, so just test method doesn't throw
            Helpers.EnsureLocalDBInstalled();
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
        public void UnshareInstance_ThrowsIfInstanceNameIsNull()
        {
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => SqlLocalDbApi.UnshareInstance(null),
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
        public void UnshareInstance_ThrowsIfInstanceNameDoesNotExist()
        {
            Helpers.EnsureLocalDBInstalled();

            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbException error = ErrorAssert.Throws<SqlLocalDbException>(
                () => SqlLocalDbApi.UnshareInstance(instanceName));

            Assert.AreEqual(SqlLocalDbErrors.InstanceNotShared, error.ErrorCode, "SqlLocalDbException.ErrorCode is incorrect.");
            Assert.AreEqual(instanceName, error.InstanceName, "SqlLocalDbException.InstanceName is incorrect.");

            throw error;
        }

        [TestMethod]
        [Description("Tests UnshareInstance().")]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the method under test.")]
        public void UnshareInstance()
        {
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

                    SqlLocalDbApi.UnshareInstance(instanceName);

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
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        #endregion
    }
}