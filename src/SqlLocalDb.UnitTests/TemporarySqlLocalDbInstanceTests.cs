// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TemporarySqlLocalDbInstanceTests.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   TemporarySqlLocalDbInstanceTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.SqlClient;
using System.IO;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="TemporarySqlLocalDbInstanceTests"/> class.
        /// </summary>
        public TemporarySqlLocalDbInstanceTests()
        {
        }

        [TestMethod]
        [Description("Tests .ctor() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TemporarySqlLocalDbInstance_Constructor_Throws_If_InstanceName_Is_Null()
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
        public void TemporarySqlLocalDbInstance_Constructor_Throws_If_Provider_Is_Null()
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
        public void TemporarySqlLocalDbInstance_Constructor_Creates_Temporary_Instance()
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

                Assert.IsFalse(target.DeleteFiles, "TemporarySqlLocalDbInstance.DeleteFiles is incorrect.");
                Assert.AreEqual(target.Instance.Name, target.Name, "TemporarySqlLocalDbInstance.Name is incorrect.");
                Assert.AreEqual(target.Instance.NamedPipe, target.NamedPipe, "TemporarySqlLocalDbInstance.NamedPipe is incorrect.");
            }

            // The instance should have been deleted
            AssertExistence(instanceName, exists: false);
            AssertFileExistence(instanceName, shouldFilesExist: true);
        }

        [TestMethod]
        [Description("Tests .ctor(string, ISqlLocalDbProvider).")]
        public void TemporarySqlLocalDbInstance_Constructor_Creates_Temporary_Instance_With_Specified_Provider()
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

                Assert.IsFalse(target.DeleteFiles, "TemporarySqlLocalDbInstance.DeleteFiles is incorrect.");
                Assert.AreEqual(target.Instance.Name, target.Name, "TemporarySqlLocalDbInstance.Name is incorrect.");
                Assert.AreEqual(target.Instance.NamedPipe, target.NamedPipe, "TemporarySqlLocalDbInstance.NamedPipe is incorrect.");
            }

            // The instance should have been deleted
            AssertExistence(instanceName, exists: false);
            AssertFileExistence(instanceName, shouldFilesExist: true);
        }

        [TestMethod]
        [Description("Tests .ctor(string, ISqlLocalDbProvider, bool).")]
        public void TemporarySqlLocalDbInstance_Constructor_Creates_Temporary_Instance_With_Specified_Provider_And_Does_Not_Delete_Instance_Files()
        {
            // Arrange
            var mock = new Mock<SqlLocalDbProvider>()
            {
                CallBase = true,
            };

            string instanceName = "MyTempInstance" + Guid.NewGuid().ToString();
            SqlLocalDbProvider provider = mock.Object;
            bool deleteFiles = false;

            // Act
            using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instanceName, provider, deleteFiles))
            {
                // Assert
                Mock.Get(provider).Verify((p) => p.CreateInstance(instanceName), Times.Once());

                Assert.IsNotNull(target.Instance, "TemporarySqlLocalDbInstance.Instance is null.");

                // Check the instance was created
                AssertExistence(instanceName, exists: true);

                // The instance is not running if there is no pipe open
                Assert.IsFalse(string.IsNullOrEmpty(target.Instance.NamedPipe), "The temporary SQL LocalDB instance has not been started.");

                Assert.AreEqual(deleteFiles, target.DeleteFiles, "TemporarySqlLocalDbInstance.DeleteFiles is incorrect.");
                Assert.AreEqual(target.Instance.Name, target.Name, "TemporarySqlLocalDbInstance.Name is incorrect.");
                Assert.AreEqual(target.Instance.NamedPipe, target.NamedPipe, "TemporarySqlLocalDbInstance.NamedPipe is incorrect.");
            }

            // The instance should have been deleted
            AssertExistence(instanceName, exists: false);
            AssertFileExistence(instanceName, shouldFilesExist: true);
        }

        [TestMethod]
        [Description("Tests .ctor(string, ISqlLocalDbProvider, bool) if the instance cannot be started.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TemporarySqlLocalDbInstance_Constructor_Attempts_To_Delete_Instance_If_Instance_Cannot_Be_Started()
        {
            // Arrange
            string instanceName = "MyTempInstance" + Guid.NewGuid().ToString();

            var mock = new Mock<SqlLocalDbProvider>()
            {
                CallBase = true,
            };

            // Set up the CreateInstance() method to create an SQL LocalDB
            // instance but that then throws an exception when started.
            mock.Setup((p) => p.CreateInstance(instanceName))
                .Returns(
                    () =>
                    {
                        SqlLocalDbApi.CreateInstance(instanceName);
                        return new SqlLocalDbInstanceThatCannotBeStarted(instanceName);
                    })
                .Verifiable();

            ISqlLocalDbProvider provider = mock.Object;
            bool deleteFiles = false;

            // Act
            InvalidOperationException error = ErrorAssert.Throws<InvalidOperationException>(
                () =>
                {
                    using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instanceName, provider, deleteFiles))
                    {
                    }
                });

            mock.Verify();

            ISqlLocalDbInstanceInfo instanceInfo = SqlLocalDbApi.GetInstanceInfo(instanceName);

            Assert.IsFalse(instanceInfo.Exists, "The temporary instance was not deleted.");

            throw error;
        }

        [TestMethod]
        [Description("Tests Create().")]
        public void TemporarySqlLocalDbInstance_Create_Creates_Temporary_Instance_With_Random_Name()
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

                Assert.IsFalse(target.DeleteFiles, "TemporarySqlLocalDbInstance.DeleteFiles is incorrect.");
                Assert.AreEqual(target.Instance.Name, target.Name, "TemporarySqlLocalDbInstance.Name is incorrect.");
                Assert.AreEqual(target.Instance.NamedPipe, target.NamedPipe, "TemporarySqlLocalDbInstance.NamedPipe is incorrect.");
            }

            // The instance should have been deleted
            AssertExistence(instanceName, exists: false);
            AssertFileExistence(instanceName, shouldFilesExist: true);

            // Verify that the same random name isn't used again
            using (TemporarySqlLocalDbInstance target = TemporarySqlLocalDbInstance.Create())
            {
                Assert.AreNotEqual(instanceName, target.Instance.Name, "The same random name was used to generate a temporary instance.");
            }
        }

        [TestMethod]
        [Description("Tests Create(bool) deletes the instance files if deleteFiles is true.")]
        public void TemporarySqlLocalDbInstance_Create_With_DeleteFiles_Parameter_Creates_Temporary_Instance_With_Random_Name_And_Deletes_Files_When_Disposed()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    // Set the property to false to ensure that the parameter is used to control deletion not the property
                    SqlLocalDbApi.AutomaticallyDeleteInstanceFiles = false;
                    SqlLocalDbApi.StopOptions = StopInstanceOptions.NoWait;

                    string instanceName;
                    bool deleteFiles = true;

                    // Act
                    using (TemporarySqlLocalDbInstance target = TemporarySqlLocalDbInstance.Create(deleteFiles))
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

                        Assert.IsTrue(target.DeleteFiles, "TemporarySqlLocalDbInstance.DeleteFiles is incorrect.");
                        Assert.AreEqual(target.Instance.Name, target.Name, "TemporarySqlLocalDbInstance.Name is incorrect.");
                        Assert.AreEqual(target.Instance.NamedPipe, target.NamedPipe, "TemporarySqlLocalDbInstance.NamedPipe is incorrect.");
                    }

                    // The instance should have been deleted
                    AssertExistence(instanceName, exists: false);
                    AssertFileExistence(instanceName, shouldFilesExist: false);
                });
        }

        [TestMethod]
        [Description("Tests Create() and deletes when disposed if SqlLocalDbApi.AutomaticallyDeleteInstanceFiles is true.")]
        public void TemporarySqlLocalDbInstance_Create_Creates_Temporary_Instance_With_Random_Name_And_Deletes_If_SqlLocalDbApi_AutomaticallyDeleteInstanceFiles_Is_True()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    SqlLocalDbApi.AutomaticallyDeleteInstanceFiles = true;
                    SqlLocalDbApi.StopOptions = StopInstanceOptions.NoWait;

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

                        Assert.IsTrue(target.DeleteFiles, "TemporarySqlLocalDbInstance.DeleteFiles is incorrect.");
                        Assert.AreEqual(target.Instance.Name, target.Name, "TemporarySqlLocalDbInstance.Name is incorrect.");
                        Assert.AreEqual(target.Instance.NamedPipe, target.NamedPipe, "TemporarySqlLocalDbInstance.NamedPipe is incorrect.");
                    }

                    // The instance should have been deleted
                    AssertExistence(instanceName, exists: false);
                    AssertFileExistence(instanceName, shouldFilesExist: false);

                    // Verify that the same random name isn't used again
                    using (TemporarySqlLocalDbInstance target = TemporarySqlLocalDbInstance.Create())
                    {
                        Assert.AreNotEqual(instanceName, target.Instance.Name, "The same random name was used to generate a temporary instance.");
                    }
                });
        }

        [TestMethod]
        [Description("Tests CreateConnection().")]
        public void TemporarySqlLocalDbInstance_CreateConnection_Creates_Connection()
        {
            // Arrange
            using (TemporarySqlLocalDbInstance target = TemporarySqlLocalDbInstance.Create())
            {
                // Act
                using (SqlConnection result = target.CreateConnection())
                {
                    // Assert
                    Assert.IsNotNull(result, "CreateConnection() returned null.");
                }
            }
        }

        [TestMethod]
        [Description("Tests CreateConnectionStringBuilder().")]
        public void TemporarySqlLocalDbInstance_CreateConnectionStringBuilder_Creates_SqlConnectionStringBuilder()
        {
            // Arrange
            using (TemporarySqlLocalDbInstance target = TemporarySqlLocalDbInstance.Create())
            {
                // Act
                SqlConnectionStringBuilder result = target.CreateConnectionStringBuilder();

                // Assert
                Assert.IsNotNull(result, "CreateConnectionStringBuilder() returned null.");
            }
        }

        [TestMethod]
        [Description("Tests GetInstanceInfo().")]
        public void TemporarySqlLocalDbInstance_GetInstanceInfo_Returns_ISqlLocalDbInstanceInfo()
        {
            // Arrange
            using (TemporarySqlLocalDbInstance target = TemporarySqlLocalDbInstance.Create())
            {
                // Act
                ISqlLocalDbInstanceInfo result = target.GetInstanceInfo();

                // Assert
                Assert.IsNotNull(result, "GetInstanceInfo() returned null.");
            }
        }

        [TestMethod]
        [Description("Tests Share().")]
        public void TemporarySqlLocalDbInstance_Share_Invokes_Wrapped_Instance()
        {
            // Arrange
            ISqlLocalDbInstance instance = CreateMockInstance();

            using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instance))
            {
                string sharedName = Guid.NewGuid().ToString();

                // Act
                target.Share(sharedName);

                // Assert
                Mock.Get(instance).Verify((p) => p.Share(sharedName), Times.Once());
            }
        }

        [TestMethod]
        [Description("Tests Start().")]
        public void TemporarySqlLocalDbInstance_Start_Invokes_Wrapped_Instance()
        {
            // Arrange
            ISqlLocalDbInstance instance = CreateMockInstance();

            using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instance))
            {
                // Act
                target.Start();

                // Assert
                Mock.Get(instance).Verify((p) => p.Start(), Times.Once());
            }
        }

        [TestMethod]
        [Description("Tests Stop().")]
        public void TemporarySqlLocalDbInstance_Stop_Invokes_Wrapped_Instance()
        {
            // Arrange
            ISqlLocalDbInstance instance = CreateMockInstance();

            using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instance))
            {
                // Act
                target.Stop();

                // Assert
                Mock.Get(instance).Verify((p) => p.Stop(), Times.Once());
            }
        }

        [TestMethod]
        [Description("Tests Unshare().")]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the method being tested.")]
        public void TemporarySqlLocalDbInstance_Unshare_Invokes_Wrapped_Instance()
        {
            // Arrange
            ISqlLocalDbInstance instance = CreateMockInstance();

            using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instance))
            {
                // Act
                target.Unshare();

                // Assert
                Mock.Get(instance).Verify((p) => p.Unshare(), Times.Once());
            }
        }

        [TestMethod]
        [Description("Tests CreateConnection() throws an exception if it has been disposed.")]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TemporarySqlLocalDbInstance_CreateConnection_Throws_If_Instance_Disposed()
        {
            // Act and Assert
            AssertThrowsObjectDisposedException((p) => p.CreateConnection());
        }

        [TestMethod]
        [Description("Tests CreateConnection() throws an exception if it has been disposed.")]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TemporarySqlLocalDbInstance_CreateConnectionStringBuilder_Throws_If_Instance_Disposed()
        {
            // Act and Assert
            AssertThrowsObjectDisposedException((p) => p.CreateConnectionStringBuilder());
        }

        [TestMethod]
        [Description("Tests GetInstanceInfo() throws an exception if it has been disposed.")]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TemporarySqlLocalDbInstance_GetInstanceInfo_Throws_If_Instance_Disposed()
        {
            // Act and Assert
            AssertThrowsObjectDisposedException((p) => p.GetInstanceInfo());
        }

        [TestMethod]
        [Description("Tests Share() throws an exception if it has been disposed.")]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TemporarySqlLocalDbInstance_Share_Throws_If_Instance_Disposed()
        {
            // Act and Assert
            AssertThrowsObjectDisposedException((p) => p.Share(string.Empty));
        }

        [TestMethod]
        [Description("Tests Start() throws an exception if it has been disposed.")]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TemporarySqlLocalDbInstance_Start_Throws_If_Instance_Disposed()
        {
            // Act and Assert
            AssertThrowsObjectDisposedException((p) => p.Start());
        }

        [TestMethod]
        [Description("Tests Stop() throws an exception if it has been disposed.")]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TemporarySqlLocalDbInstance_Stop_Throws_If_Instance_Disposed()
        {
            // Act and Assert
            AssertThrowsObjectDisposedException((p) => p.Stop());
        }

        [TestMethod]
        [Description("Tests Unshare() throws an exception if it has been disposed.")]
        [ExpectedException(typeof(ObjectDisposedException))]
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Naming",
            "CA1704:IdentifiersShouldBeSpelledCorrectly",
            MessageId = "Unshare",
            Justification = "Matches the name of the method being tested.")]
        public void TemporarySqlLocalDbInstance_Unshare_Throws_If_Instance_Disposed()
        {
            // Act and Assert
            AssertThrowsObjectDisposedException((p) => p.Unshare());
        }

        [TestMethod]
        [Description("Tests Dispose() does not throw if a SQL LocalDB error occurs stopping the instance that is not the error from the instance not existing.")]
        public void TemporarySqlLocalDbInstance_Dispose_Does_Not_Throw_If_Instance_Cannot_Be_Stopped_Due_To_Sql_LocalDb_Error()
        {
            // Arrange
            string instanceName = "MyTempInstance" + Guid.NewGuid().ToString();

            var mock = new Mock<SqlLocalDbProvider>()
            {
                CallBase = true,
            };

            // Set up the CreateInstance() method to create an SQL LocalDB
            // instance but that then throws an exception when stopped.
            mock.Setup((p) => p.CreateInstance(instanceName))
                .Returns(
                    () =>
                    {
                        SqlLocalDbApi.CreateInstance(instanceName);
                        return new SqlLocalDbInstanceThatCannotBeStopped(instanceName);
                    })
                .Verifiable();

            ISqlLocalDbProvider provider = mock.Object;

            // Act
            using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instanceName, provider))
            {
            }

            // Assert
            mock.Verify();

            // Tidy up as the stop intentionally failed, meaning delete would also have failed
            SqlLocalDbApi.StopInstance(instanceName);
            SqlLocalDbApi.DeleteInstance(instanceName);
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

        /// <summary>
        /// Assets that the specified SQL LocalDB instance's file(s) are in the specified state of existence.
        /// </summary>
        /// <param name="instanceName">The instance name to assert for.</param>
        /// <param name="shouldFilesExist">Whether the specified instance name's files are expected to exist.</param>
        private static void AssertFileExistence(string instanceName, bool shouldFilesExist)
        {
            string path = Path.Combine(SqlLocalDbApi.GetInstancesFolderPath(), instanceName);

            if (shouldFilesExist)
            {
                Assert.IsTrue(Directory.Exists(path), "The instance folder was deleted.");
                Assert.AreNotEqual(0, Directory.GetFiles(path).Length, "The instance file(s) were deleted.");
            }
            else
            {
                Assert.IsFalse(Directory.Exists(path), "The instance folder was not deleted.");
            }
        }

        /// <summary>
        /// Asserts that invoking the specified delegate causes an <see cref="ObjectDisposedException"/>
        /// to occur if the <see cref="TemporarySqlLocalDbInstance"/> instance has already been disposed.
        /// </summary>
        /// <param name="action">A delegate to a method to invoke for an instance of <see cref="TemporarySqlLocalDbInstance"/>.</param>
        [Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Usage",
            "CA2202:Do not dispose objects multiple times",
            Justification = "The test is designed to test this scenario.")]
        private static void AssertThrowsObjectDisposedException(Action<TemporarySqlLocalDbInstance> action)
        {
            // Arrange
            ISqlLocalDbInstance instance = CreateMockInstance();

            using (TemporarySqlLocalDbInstance target = new TemporarySqlLocalDbInstance(instance))
            {
                target.Dispose();

                // Act
                action(target);
            }
        }

        /// <summary>
        /// Creates a mock instance of <see cref="ISqlLocalDbInstance"/>.
        /// </summary>
        /// <returns>
        /// The created mock implementation of <see cref="ISqlLocalDbInstance"/>.
        /// </returns>
        private static ISqlLocalDbInstance CreateMockInstance()
        {
            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();

            mock.Setup((p) => p.Name)
                .Returns("MyInstanceName");

            return mock.Object;
        }

        /// <summary>
        /// A class representing an implementation of <see cref="ISqlLocalDbInstance"/> that cannot be started. This class cannot be inherited.
        /// </summary>
        private sealed class SqlLocalDbInstanceThatCannotBeStarted : SqlLocalDbInstance, ISqlLocalDbInstance
        {
            /// <inheritdoc />
            internal SqlLocalDbInstanceThatCannotBeStarted(string instanceName)
                : base(instanceName)
            {
            }

            /// <inheritdoc />
            void ISqlLocalDbInstance.Start()
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// A class representing an implementation of <see cref="ISqlLocalDbInstance"/> that cannot be stopped. This class cannot be inherited.
        /// </summary>
        private sealed class SqlLocalDbInstanceThatCannotBeStopped : SqlLocalDbInstance, ISqlLocalDbInstance
        {
            /// <inheritdoc />
            internal SqlLocalDbInstanceThatCannotBeStopped(string instanceName)
                : base(instanceName)
            {
            }

            /// <inheritdoc />
            void ISqlLocalDbInstance.Stop()
            {
                throw new SqlLocalDbException("Error", SqlLocalDbErrors.UnknownErrorCode);
            }
        }
    }
}
