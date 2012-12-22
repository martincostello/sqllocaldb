// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbProviderTests.cs" company="http://sqllocaldb.codeplex.com">
//   New BSD License (BSD)
// </copyright>
// <license>
//   This source code is subject to terms and conditions of the New BSD Licence (BSD). 
//   A copy of the license can be found in the License.txt file at the root of this
//   distribution.  By using this source code in any fashion, you are agreeing to be
//   bound by the terms of the New BSD Licence. You must not remove this notice, or
//   any other, from this software.
// </license>
// <summary>
//   SqlLocalDbProviderTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        [Description("Tests CreateInstance().")]
        public void CreateInstance_Void()
        {
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();
            
            SqlLocalDbInstance result = target.CreateInstance();

            Assert.IsNotNull(result, "CreateInstance() returned null.");
            Assert.IsNotNull(result.Name, "SqlLocalDbInstance.Name is null.");

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
        [Description("Tests CreateInstance(string).")]
        public void CreateInstance_String()
        {
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbInstance result = target.CreateInstance(instanceName);

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
        [Description("Tests CreateInstance(string) if the instance already exists.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void CreateInstance_ThrowsIfInstanceAlreadyExists()
        {
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();

            string instanceName = Guid.NewGuid().ToString();

            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
                throw ErrorAssert.Throws<InvalidOperationException>(
                    () => target.CreateInstance(instanceName));
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests GetInstance(string).")]
        public void GetInstance()
        {
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
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
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();
            
            IList<ISqlLocalDbInstanceInfo> result = target.GetInstances();

            Assert.IsNotNull(result, "GetInstances() returned null.");
            Assert.IsTrue(result.Count > 0, "No instances were returned by GetInstances().");
            CollectionAssert.AllItemsAreNotNull(result.ToList(), "GetInstances() returned a null instance.");
        }

        [TestMethod]
        [Description("Tests GetVersions()")]
        public void GetVersions()
        {
            Helpers.EnsureLocalDBInstalled();

            SqlLocalDbProvider target = new SqlLocalDbProvider();

            IList<ISqlLocalDbVersionInfo> result = target.GetVersions();

            Assert.IsNotNull(result, "GetVersions() returned null.");
            Assert.IsTrue(result.Count > 0, "No versions were returned by GetVersions().");
            CollectionAssert.AllItemsAreNotNull(result.ToList(), "GetVersions() returned a null instance.");
        }

        [TestMethod]
        [Description("Tests ISqlLocalDbFactory.CreateInstance(string).")]
        public void ISqlLocalDbFactory_CreateInstance()
        {
            Helpers.EnsureLocalDBInstalled();

            ISqlLocalDbProvider target = new SqlLocalDbProvider();

            string instanceName = Guid.NewGuid().ToString();
            ISqlLocalDbInstance result = target.CreateInstance(instanceName);

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
            Helpers.EnsureLocalDBInstalled();

            ISqlLocalDbProvider target = new SqlLocalDbProvider();

            string instanceName = Guid.NewGuid().ToString();
            SqlLocalDbApi.CreateInstance(instanceName);

            try
            {
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

        #endregion
    }
}