// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporarySqlLocalDbInstanceTests.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   TemporarySqlLocalDbInstanceTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="TemporarySqlLocalDbInstance"/> class.
    /// </summary>
    [TestClass]
    public class TemporarySqlLocalDbInstanceTests
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlLocalDbInstanceTests"/> class.
        /// </summary>
        public TemporarySqlLocalDbInstanceTests()
        {
        }

        #endregion

        #region Methods

        [TestMethod]
        [Description("Tests .ctor() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            string instanceName = null;
            ISqlLocalDbProvider provider = Mock.Of<ISqlLocalDbProvider>();

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () =>
                {
                    using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instanceName, provider))
                    {
                    }
                },
                "instanceName");
        }

        [TestMethod]
        [Description("Tests .ctor() if provider is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_Throws_If_Provider_Is_Null()
        {
            // Arrange
            string instanceName = Guid.NewGuid().ToString();
            ISqlLocalDbProvider provider = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () =>
                {
                    using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instanceName, provider))
                    {
                    }
                },
                "provider");
        }

        [TestMethod]
        [Description("Tests .ctor(string).")]
        public void Constructor_Creates_Temporary_Instance()
        {
            // Arrange
            string instanceName = "MyTempInstance" + Guid.NewGuid().ToString();

            // Act
            using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instanceName))
            {
                // Assert
                Assert.IsNotNull(target.Instance, "TemporarySqlLocalDbInstance.Instance is null.");

                // Check the instance was created
                AssertExistence(instanceName, exists: true);

                // The instance is not running if there is no pipe open
                Assert.IsFalse(string.IsNullOrEmpty(target.Instance.NamedPipe), "The temporary SQL LocalDB instance has not been started.");
            }

            // The instance should have been deleted
            AssertExistence(instanceName, exists: false);
        }

        [TestMethod]
        [Description("Tests .ctor(string, ISqlLocalDbProvider).")]
        public void Constructor_Creates_Temporary_Instance_With_Specified_Provider()
        {
            // Arrange
            var mock = new Mock<SqlLocalDbProvider>()
            {
                CallBase = true,
            };

            string instanceName = "MyTempInstance" + Guid.NewGuid().ToString();
            SqlLocalDbProvider provider = mock.Object;

            // Act
            using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instanceName, provider))
            {
                // Assert
                Mock.Get(provider).Verify((p) => p.CreateInstance(instanceName), Times.Once());

                Assert.IsNotNull(target.Instance, "TemporarySqlLocalDbInstance.Instance is null.");

                // Check the instance was created
                AssertExistence(instanceName, exists: true);

                // The instance is not running if there is no pipe open
                Assert.IsFalse(string.IsNullOrEmpty(target.Instance.NamedPipe), "The temporary SQL LocalDB instance has not been started.");
            }

            // The instance should have been deleted
            AssertExistence(instanceName, exists: false);
        }

        [TestMethod]
        [Description("Tests Create().")]
        public void Create_Creates_Temporary_Instance_With_Random_Name()
        {
            string instanceName;

            // Act
            using (TemporarySqlLocalDbInstance target = TemporarySqlLocalDbInstance.Create())
            {
                // Assert
                Assert.IsNotNull(target.Instance, "TemporarySqlLocalDbInstance.Instance is null.");

                instanceName = target.Instance.Name;

                Guid notUsed;
                Assert.IsTrue(Guid.TryParse(instanceName, out notUsed), "The random instance name is not a valid GUID.");

                // Check the instance was created
                AssertExistence(instanceName, exists: true);

                // The instance is not running if there is no pipe open
                Assert.IsFalse(string.IsNullOrEmpty(target.Instance.NamedPipe), "The temporary SQL LocalDB instance has not been started.");
            }

            // The instance should have been deleted
            AssertExistence(instanceName, exists: false);

            // Verify that the same random name isn't used again
            using (TemporarySqlLocalDbInstance target = TemporarySqlLocalDbInstance.Create())
            {
                Assert.AreNotEqual(instanceName, target.Instance.Name, "The same random name was used to generate a temporary instance.");
            }
        }

        /// <summary>
        /// Assets that the specified SQL LocalDB instance name is in the specified state of existence.
        /// </summary>
        /// <param name="instanceName">The instance name to assert for.</param>
        /// <param name="exists">Whether the specified instance name is expected to exist.</param>
        private static void AssertExistence(string instanceName, bool exists)
        {
            ISqlLocalDbInstanceInfo info = SqlLocalDbApi.GetInstanceInfo(instanceName);
            Assert.IsNotNull(info, "SqlLocalDbApi.GetInstanceInfo() returned null.");
            Assert.AreEqual(exists, info.Exists, "ISqlLocalDbInstanceInfo.Exists is incorrect.");
        }

        #endregion
    }
}