// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsTests.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   ExtensionsTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Data.Common;
using System.Data.EntityClient;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="Extensions"/> class.
    /// </summary>
    [TestClass]
    public class ExtensionsTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionsTests"/> class.
        /// </summary>
        public ExtensionsTests()
        {
        }

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get;
            set;
        }

        [TestMethod]
        [Description("Tests GetConnectionForDefaultModel() if instance is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extensions_GetConnectionForDefaultModel_Throws_If_Instance_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstance instance = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => instance.GetConnectionForDefaultModel(),
                "instance");
        }

        [TestMethod]
        [Description("Tests GetConnectionForDefaultModel() returns a connection.")]
        public void Extensions_GetConnectionForDefaultModel_Returns_Connection()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;

            // Act
            using (DbConnection result = instance.GetConnectionForDefaultModel())
            {
                // Assert
                Assert.IsNotNull(result, "GetConnectionForDefaultModel() returned null.");
                Assert.IsInstanceOfType(result, typeof(EntityConnection), "GetConnectionForDefaultModel() returned incorrect type.");
                StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
                StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
                Assert.AreEqual(ConnectionState.Closed, result.State, "DbConnection.State is incorrect.");
            }
        }

        [TestMethod]
        [Description("Tests GetConnectionForDefaultModel() returns a connection if initialCatalog is null.")]
        public void Extensions_GetConnectionForDefaultModel_Returns_Connection_If_InitialCatalog_Is_Null()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = string.Empty;

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;

            // Act
            using (DbConnection result = instance.GetConnectionForDefaultModel(initialCatalog))
            {
                // Assert
                Assert.IsNotNull(result, "GetConnectionForDefaultModel() returned null.");
                Assert.IsInstanceOfType(result, typeof(EntityConnection), "GetConnectionForDefaultModel() returned incorrect type.");
                StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
                StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
                Assert.AreEqual(ConnectionState.Closed, result.State, "DbConnection.State is incorrect.");
            }
        }

        [TestMethod]
        [Description("Tests GetConnectionForDefaultModel() returns a connection if initialCatalog is the empty string.")]
        public void Extensions_GetConnectionForDefaultModel_Returns_Connection_If_InitialCatalog_Is_Empty()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = string.Empty;

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;

            // Act
            using (DbConnection result = instance.GetConnectionForDefaultModel(initialCatalog))
            {
                // Assert
                Assert.IsNotNull(result, "GetConnectionForDefaultModel() returned null.");
                Assert.IsInstanceOfType(result, typeof(EntityConnection), "GetConnectionForDefaultModel() returned incorrect type.");
                StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
                StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
                Assert.AreEqual(ConnectionState.Closed, result.State, "DbConnection.State is incorrect.");
            }
        }

        [TestMethod]
        [Description("Tests GetConnectionForDefaultModel() returns a connection if an Initial Catalog is specified.")]
        public void Extensions_GetConnectionForDefaultModel_Returns_Connection_If_InitialCatalog_Is_Specified()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = "MyOtherDatabase";

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;

            // Act
            using (DbConnection result = instance.GetConnectionForDefaultModel(initialCatalog))
            {
                // Assert
                Assert.IsNotNull(result, "GetConnectionForDefaultModel() returned null.");
                Assert.IsInstanceOfType(result, typeof(EntityConnection), "GetConnectionForDefaultModel() returned incorrect type.");
                StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
                StringAssert.Contains(result.ConnectionString, initialCatalog, "The Initial Catalog in the connection string was not changed.");
                Assert.AreEqual(ConnectionState.Closed, result.State, "DbConnection.State is incorrect.");
            }
        }

        [TestMethod]
        [Description("Tests GetConnectionForModel() if instance is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extensions_GetConnectionForModel_Throws_If_Instance_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstance instance = null;
            string modelConnectionStringName = "MyConnectionString";

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => instance.GetConnectionForModel(modelConnectionStringName),
                "instance");
        }

        [TestMethod]
        [Description("Tests GetConnectionForModel() returns a connection.")]
        public void Extensions_GetConnectionForModel_Returns_Connection()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;
            string modelConnectionStringName = "MyDataContext";

            // Act
            using (DbConnection result = instance.GetConnectionForModel(modelConnectionStringName))
            {
                // Assert
                Assert.IsNotNull(result, "GetConnectionForModel() returned null.");
                Assert.IsInstanceOfType(result, typeof(EntityConnection), "GetConnectionForModel() returned incorrect type.");
                StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
                StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
                Assert.AreEqual(ConnectionState.Closed, result.State, "DbConnection.State is incorrect.");
            }
        }

        [TestMethod]
        [Description("Tests GetConnectionForModel() returns a connection if initialCatalog is null.")]
        public void Extensions_GetConnectionForModel_Returns_Connection_If_InitialCatalog_Is_Null()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = string.Empty;

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;
            string modelConnectionStringName = "MyDataContext";

            // Act
            using (DbConnection result = instance.GetConnectionForModel(modelConnectionStringName, initialCatalog))
            {
                // Assert
                Assert.IsNotNull(result, "GetConnectionForModel() returned null.");
                Assert.IsInstanceOfType(result, typeof(EntityConnection), "GetConnectionForModel() returned incorrect type.");
                StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
                StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
                Assert.AreEqual(ConnectionState.Closed, result.State, "DbConnection.State is incorrect.");
            }
        }

        [TestMethod]
        [Description("Tests GetConnectionForModel() returns a connection if initialCatalog is the empty string.")]
        public void Extensions_GetConnectionForModel_Returns_Connection_If_InitialCatalog_Is_Empty()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = string.Empty;

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;
            string modelConnectionStringName = "MyDataContext";

            // Act
            using (DbConnection result = instance.GetConnectionForModel(modelConnectionStringName, initialCatalog))
            {
                // Assert
                Assert.IsNotNull(result, "GetConnectionForModel() returned null.");
                Assert.IsInstanceOfType(result, typeof(EntityConnection), "GetConnectionForModel() returned incorrect type.");
                StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
                StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
                Assert.AreEqual(ConnectionState.Closed, result.State, "DbConnection.State is incorrect.");
            }
        }

        [TestMethod]
        [Description("Tests GetConnectionForModel() returns a connection if an Initial Catalog is specified.")]
        public void Extensions_GetConnectionForModel_Returns_Connection_If_InitialCatalog_Is_Specified()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = "MyOtherDatabase";

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;
            string modelConnectionStringName = "MyDataContext";

            // Act
            using (DbConnection result = instance.GetConnectionForModel(modelConnectionStringName, initialCatalog))
            {
                // Assert
                Assert.IsNotNull(result, "GetConnectionForModel() returned null.");
                Assert.IsInstanceOfType(result, typeof(EntityConnection), "GetConnectionForModel() returned incorrect type.");
                StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
                StringAssert.Contains(result.ConnectionString, initialCatalog, "The Initial Catalog in the connection string was not changed.");
                Assert.AreEqual(ConnectionState.Closed, result.State, "DbConnection.State is incorrect.");
            }
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForDefaultModel() if instance is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extensions_GetConnectionStringForDefaultModel_Throws_If_Instance_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstance instance = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => instance.GetConnectionStringForDefaultModel(),
                "instance");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForDefaultModel() returns a connection string.")]
        public void Extensions_GetConnectionStringForDefaultModel_Returns_Connection_String()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;

            // Act
            DbConnectionStringBuilder result = instance.GetConnectionStringForDefaultModel();

            // Assert
            Assert.IsNotNull(result, "GetConnectionStringForDefaultModel() returned null.");
            Assert.IsInstanceOfType(result, typeof(EntityConnectionStringBuilder), "GetConnectionStringForDefaultModel() returned incorrect type.");
            StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
            StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForDefaultModel() returns a connection string if initialCatalog is null.")]
        public void Extensions_GetConnectionStringForDefaultModel_Returns_Connection_String_If_InitialCatalog_Is_Null()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = null;

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;

            // Act
            DbConnectionStringBuilder result = instance.GetConnectionStringForDefaultModel(initialCatalog);

            // Assert
            Assert.IsNotNull(result, "GetConnectionStringForDefaultModel() returned null.");
            Assert.IsInstanceOfType(result, typeof(EntityConnectionStringBuilder), "GetConnectionStringForDefaultModel() returned incorrect type.");
            StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
            StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForDefaultModel() returns a connection string if initialCatalog is the empty string.")]
        public void Extensions_GetConnectionStringForDefaultModel_Returns_Connection_String_If_InitialCatalog_Is_Empty()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = string.Empty;

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;

            // Act
            DbConnectionStringBuilder result = instance.GetConnectionStringForDefaultModel(initialCatalog);

            // Assert
            Assert.IsNotNull(result, "GetConnectionStringForDefaultModel() returned null.");
            Assert.IsInstanceOfType(result, typeof(EntityConnectionStringBuilder), "GetConnectionStringForDefaultModel() returned incorrect type.");
            StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
            StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForDefaultModel() returns a connection string if a different Initial Catalog is specified.")]
        public void Extensions_GetConnectionStringForDefaultModel_Returns_Connection_String_If_Initial_Catalog_Specified()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = "MyOtherDatabase";

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;

            // Act
            DbConnectionStringBuilder result = instance.GetConnectionStringForDefaultModel(initialCatalog);

            // Assert
            Assert.IsNotNull(result, "GetConnectionStringForDefaultModel() returned null.");
            Assert.IsInstanceOfType(result, typeof(EntityConnectionStringBuilder), "GetConnectionStringForDefaultModel() returned incorrect type.");
            StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
            StringAssert.Contains(result.ConnectionString, initialCatalog, "The Initial Catalog in the connection string was not changed.");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForDefaultModel() if no connection strings are configured.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extensions_GetConnectionStringForDefaultModel_Throws_If_No_Connection_Strings_Configured()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    ISqlLocalDbInstance instance = Mock.Of<ISqlLocalDbInstance>();

                    // Act and Assert
                    throw ErrorAssert.Throws<InvalidOperationException>(
                        () => instance.GetConnectionStringForDefaultModel());
                },
                configurationFile: "ExtensionsTests.NoConnectionStrings.config");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForDefaultModel() if more than one connection string is configured.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extensions_GetConnectionStringForDefaultModel_Throws_If_Multiple_Connection_Strings_Are_Configured()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    ISqlLocalDbInstance instance = Mock.Of<ISqlLocalDbInstance>();

                    // Act and Assert
                    throw ErrorAssert.Throws<InvalidOperationException>(
                        () => instance.GetConnectionStringForDefaultModel());
                },
                configurationFile: "ExtensionsTests.MultipleConnectionStrings.config");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForModel() if instance is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extensions_GetConnectionStringForModel_Throws_If_Instance_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstance instance = null;
            string modelConnectionStringName = "MyConnectionString";

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => instance.GetConnectionStringForModel(modelConnectionStringName),
                "instance");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForModel() if modelConnectionStringName cannot be found.")]
        [ExpectedException(typeof(ArgumentException))]
        public void Extensions_GetConnectionStringForModel_Throws_If_ConnectionString_Cannot_Be_Found()
        {
            // Arrange
            ISqlLocalDbInstance instance = Mock.Of<ISqlLocalDbInstance>();
            string modelConnectionStringName = "NonExistentContext";

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => instance.GetConnectionStringForModel(modelConnectionStringName),
                "modelConnectionStringName");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForModel() returns a connection string.")]
        public void Extensions_GetConnectionStringForModel_Returns_Connection_String()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;
            string modelConnectionStringName = "MyDataContext";

            // Act
            DbConnectionStringBuilder result = instance.GetConnectionStringForModel(modelConnectionStringName);

            // Assert
            Assert.IsNotNull(result, "GetConnectionStringForModel() returned null.");
            Assert.IsInstanceOfType(result, typeof(EntityConnectionStringBuilder), "GetConnectionStringForModel() returned incorrect type.");
            StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
            StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForModel() returns a connection string if initialCatalog is null.")]
        public void Extensions_GetConnectionStringForModel_Returns_Connection_String_If_InitialCatalog_Is_Null()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = null;

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;
            string modelConnectionStringName = "MyDataContext";

            // Act
            DbConnectionStringBuilder result = instance.GetConnectionStringForModel(modelConnectionStringName, initialCatalog);

            // Assert
            Assert.IsNotNull(result, "GetConnectionStringForModel() returned null.");
            Assert.IsInstanceOfType(result, typeof(EntityConnectionStringBuilder), "GetConnectionStringForModel() returned incorrect type.");
            StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
            StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForModel() returns a connection string if initialCatalog is the empty string.")]
        public void Extensions_GetConnectionStringForModel_Returns_Connection_String_If_InitialCatalog_Is_Empty()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = string.Empty;

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;
            string modelConnectionStringName = "MyDataContext";

            // Act
            DbConnectionStringBuilder result = instance.GetConnectionStringForModel(modelConnectionStringName, initialCatalog);

            // Assert
            Assert.IsNotNull(result, "GetConnectionStringForModel() returned null.");
            Assert.IsInstanceOfType(result, typeof(EntityConnectionStringBuilder), "GetConnectionStringForModel() returned incorrect type.");
            StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
            StringAssert.Contains(result.ConnectionString, "MyDatabase", "The Initial Catalog in the connection string was changed.");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForModel() returns a connection string if a different Initial Catalog is specified.")]
        public void Extensions_GetConnectionStringForModel_Returns_Connection_String_If_Initial_Catalog_Specified()
        {
            // Arrange
            string namedPipe = @"np:\\.\pipe\LOCALDB#C0209E6A\tsql\query";
            string initialCatalog = "MyOtherDatabase";

            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            mock.Setup((p) => p.NamedPipe).Returns(namedPipe);

            ISqlLocalDbInstance instance = mock.Object;
            string modelConnectionStringName = "MyDataContext";

            // Act
            DbConnectionStringBuilder result = instance.GetConnectionStringForModel(modelConnectionStringName, initialCatalog);

            // Assert
            Assert.IsNotNull(result, "GetConnectionStringForModel() returned null.");
            Assert.IsInstanceOfType(result, typeof(EntityConnectionStringBuilder), "GetConnectionStringForModel() returned incorrect type.");
            StringAssert.Contains(result.ConnectionString, namedPipe, "The named pipe is not present in the connection string.");
            StringAssert.Contains(result.ConnectionString, initialCatalog, "The Initial Catalog in the connection string was not changed.");
        }

        [TestMethod]
        [Description("Tests GetInitialCatalogName() throws an exception if value is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extensions_GetInitialCatalogName_Throws_If_Value_Is_Null()
        {
            // Arrange
            DbConnectionStringBuilder value = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => value.GetInitialCatalogName(),
                "value");
        }

        [TestMethod]
        [Description("Tests GetInitialCatalogName() if the Initial Catalog name cannot be found.")]
        public void Extensions_GetInitialCatalogName_Returns_Null_If_Not_Found()
        {
            // Arrange
            DbConnectionStringBuilder value = new DbConnectionStringBuilder();

            // Act
            string result = value.GetInitialCatalogName();

            // Assert
            Assert.IsNull(result, "GetInitialCatalogName() returned incorrect value.");
        }

        [TestMethod]
        [Description("Tests GetInitialCatalogName() returns the correct Initial Catalog name.")]
        [DataSource(
            "Microsoft.VisualStudio.TestTools.DataSource.XML",
            @"|DataDirectory|\GetInitialCatalogNameTestCases.xml",
            "testCase",
            DataAccessMethod.Sequential)]
        public void Extensions_GetInitialCatalogName_Returns_Correct_Initial_Catalog()
        {
            // Arrange
            string connectionString = Convert.ToString(this.TestContext.DataRow["connectionString"], CultureInfo.InvariantCulture);
            string expected = Convert.ToString(this.TestContext.DataRow["expected"], CultureInfo.InvariantCulture);

            // Arrange
            DbConnectionStringBuilder value = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString,
            };

            // Act
            string actual = value.GetInitialCatalogName();

            // Assert
            Assert.AreEqual(expected, actual, "GetInitialCatalogName() returned incorrect value.");
        }

        [TestMethod]
        [Description("Tests GetInitialCatalogName() if the connection string is an entity connection string.")]
        public void Extensions_GetInitialCatalogName_Returns_Initial_Catalog_If_Entity_Connection_String()
        {
            // Arrange
            DbConnectionStringBuilder value = new System.Data.EntityClient.EntityConnectionStringBuilder(
                @"metadata=res://*/MyData.csdl|res://*/MyData.ssdl|res://*/MyData.msl;provider=System.Data.SqlClient;provider connection string="";data source=.;initial catalog=MyDatabase;integrated security=True;MultipleActiveResultSets=True""");

            // Act
            string actual = value.GetInitialCatalogName();

            // Assert
            Assert.AreEqual("MyDatabase", actual, "GetInitialCatalogName() returned incorrect value.");
        }

        [TestMethod]
        [Description("Tests GetInitialCatalogName() if the connection string is an entity connection string with no Initial Catalog.")]
        public void Extensions_GetInitialCatalogName_Returns_Null_If_No_Initial_Catalog_In_Entity_Connection_String()
        {
            // Arrange
            DbConnectionStringBuilder value = new System.Data.EntityClient.EntityConnectionStringBuilder(
                @"metadata=res://*/MyData.csdl|res://*/MyData.ssdl|res://*/MyData.msl;provider=System.Data.SqlClient;provider connection string="";data source=.;integrated security=True;MultipleActiveResultSets=True""");

            // Act
            string actual = value.GetInitialCatalogName();

            // Assert
            Assert.IsNull(actual, "GetInitialCatalogName() returned incorrect value.  Value: {0}", actual);
        }

        [TestMethod]
        [Description("Tests GetInitialCatalogName() if the connection string is an SQL connection string.")]
        public void Extensions_GetInitialCatalogName_Returns_Initial_Catalog_If_Sql_Connection_String()
        {
            // Arrange
            DbConnectionStringBuilder value = new System.Data.SqlClient.SqlConnectionStringBuilder(
                "data source=.;initial catalog=MyDatabase;integrated security=True;MultipleActiveResultSets=True");

            // Act
            string actual = value.GetInitialCatalogName();

            // Assert
            Assert.AreEqual("MyDatabase", actual, "GetInitialCatalogName() returned incorrect value.");
        }

        [TestMethod]
        [Description("Tests GetInitialCatalogName() if the connection string is an SQL connection string with no Initial Catalog.")]
        public void Extensions_GetInitialCatalogName_Returns_Null_If_No_Initial_Catalog_In_Sql_Connection_String()
        {
            // Arrange
            DbConnectionStringBuilder value = new System.Data.SqlClient.SqlConnectionStringBuilder(
                "data source=.;integrated security=True;MultipleActiveResultSets=True");

            // Act
            string actual = value.GetInitialCatalogName();

            // Assert
            Assert.IsNull(actual, "GetInitialCatalogName() returned incorrect value.  Value: {0}", actual);
        }

        [TestMethod]
        [Description("Tests GetPhysicalFileName() throws an exception if value is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extensions_GetPhysicalFileName_Throws_If_Value_Is_Null()
        {
            // Arrange
            DbConnectionStringBuilder value = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => value.GetPhysicalFileName(),
                "value");
        }

        [TestMethod]
        [Description("Tests GetPhysicalFileName() if the file name cannot be found.")]
        public void Extensions_GetPhysicalFileName_Returns_Null_If_Not_Found()
        {
            // Arrange
            DbConnectionStringBuilder value = new DbConnectionStringBuilder();

            // Act
            string result = value.GetPhysicalFileName();

            // Assert
            Assert.IsNull(result, "GetPhysicalFileName() returned incorrect value.");
        }

        [TestMethod]
        [Description("Tests GetPhysicalFileName() returns the correct file name.")]
        [DataSource(
            "Microsoft.VisualStudio.TestTools.DataSource.XML",
            @"|DataDirectory|\GetPhysicalFileNameTestCases.xml",
            "testCase",
            DataAccessMethod.Sequential)]
        public void Extensions_GetPhysicalFileName_Returns_Correct_FileName()
        {
            // Arrange
            string connectionString = Convert.ToString(this.TestContext.DataRow["connectionString"], CultureInfo.InvariantCulture);
            string expected = Convert.ToString(this.TestContext.DataRow["expected"], CultureInfo.InvariantCulture);

            // Arrange
            DbConnectionStringBuilder value = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString,
            };

            // Act
            string actual = value.GetPhysicalFileName();

            // Assert
            Assert.AreEqual(expected, actual, "GetPhysicalFileName() returned incorrect value.");
        }

        [TestMethod]
        [Description("Tests GetPhysicalFileName() if the connection string is an entity connection string.")]
        public void Extensions_GetPhysicalFileName_Returns_FileName_If_Entity_Connection_String()
        {
            // Arrange
            DbConnectionStringBuilder value = new System.Data.EntityClient.EntityConnectionStringBuilder(
                @"metadata=res://*/MyData.csdl|res://*/MyData.ssdl|res://*/MyData.msl;provider=System.Data.SqlClient;provider connection string="";data source=.;AttachDBFilename=C:\MyDatabase.mdf;integrated security=True;MultipleActiveResultSets=True""");

            // Act
            string actual = value.GetPhysicalFileName();

            // Assert
            Assert.AreEqual(@"C:\MyDatabase.mdf", actual, "GetPhysicalFileName() returned incorrect value.");
        }

        [TestMethod]
        [Description("Tests GetPhysicalFileName() if the connection string is an entity connection string with no file name.")]
        public void Extensions_GetPhysicalFileName_Returns_Null_If_No_FileName_In_Entity_Connection_String()
        {
            // Arrange
            DbConnectionStringBuilder value = new System.Data.EntityClient.EntityConnectionStringBuilder(
                @"metadata=res://*/MyData.csdl|res://*/MyData.ssdl|res://*/MyData.msl;provider=System.Data.SqlClient;provider connection string="";data source=.;integrated security=True;MultipleActiveResultSets=True""");

            // Act
            string actual = value.GetPhysicalFileName();

            // Assert
            Assert.IsNull(actual, "GetPhysicalFileName() returned incorrect value.  Value: {0}", actual);
        }

        [TestMethod]
        [Description("Tests GetPhysicalFileName() if the connection string is an SQL connection string.")]
        public void Extensions_GetPhysicalFileName_Returns_FileName_If_Sql_Connection_String()
        {
            // Arrange
            DbConnectionStringBuilder value = new System.Data.SqlClient.SqlConnectionStringBuilder(
                @"data source=.;AttachDBFilename=""C:\MyDatabase.mdf"";integrated security=True;MultipleActiveResultSets=True");

            // Act
            string actual = value.GetPhysicalFileName();

            // Assert
            Assert.AreEqual(@"C:\MyDatabase.mdf", actual, "GetPhysicalFileName() returned incorrect value.");
        }

        [TestMethod]
        [Description("Tests GetPhysicalFileName() if the connection string is an SQL connection string with no file name.")]
        public void Extensions_GetPhysicalFileName_Returns_Null_If_No_FileName_In_Sql_Connection_String()
        {
            // Arrange
            DbConnectionStringBuilder value = new System.Data.SqlClient.SqlConnectionStringBuilder(
                "data source=.;integrated security=True;MultipleActiveResultSets=True");

            // Act
            string actual = value.GetPhysicalFileName();

            // Assert
            Assert.IsNull(actual, "GetPhysicalFileName() returned incorrect value.  Value: {0}", actual);
        }

        [TestMethod]
        [Description("Tests GetOrCreateInstance() if value is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extensions_GetOrCreateInstance_Throws_If_Value_Is_Null()
        {
            // Arrange
            ISqlLocalDbProvider value = null;
            string instanceName = Guid.NewGuid().ToString();

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => value.GetOrCreateInstance(instanceName),
                "value");
        }

        [TestMethod]
        [Description("Tests GetOrCreateInstance() if instanceName is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extensions_GetOrCreateInstance_Throws_If_InstanceName_Is_Null()
        {
            // Arrange
            ISqlLocalDbProvider value = Mock.Of<ISqlLocalDbProvider>();
            string instanceName = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => value.GetOrCreateInstance(instanceName),
                "instanceName");
        }

        [TestMethod]
        [Description("Tests GetOrCreateInstance() if instanceName does not exist.")]
        public void Extensions_GetOrCreateInstance_If_InstanceName_Does_Not_Exist()
        {
            // Arrange
            ISqlLocalDbProvider value = new SqlLocalDbProvider();
            string instanceName = Guid.NewGuid().ToString();

            // Act
            ISqlLocalDbInstance result = value.GetOrCreateInstance(instanceName);

            try
            {
                // Assert
                Assert.IsNotNull(result, "GetOrCreateInstance() returned null.");
                Assert.AreEqual(instanceName, result.Name, "ISqlLocalDbInstance.Name is incorrect.");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [Description("Tests GetOrCreateInstance() if instanceName exists.")]
        public void Extensions_GetOrCreateInstance_If_InstanceName_Exists()
        {
            // Arrange
            ISqlLocalDbProvider value = new SqlLocalDbProvider();
            string instanceName = Guid.NewGuid().ToString();

            // Act
            ISqlLocalDbInstance result = value.GetOrCreateInstance(instanceName);

            try
            {
                // Assert
                Assert.IsNotNull(result, "GetOrCreateInstance() returned null.");
                Assert.AreEqual(instanceName, result.Name, "ISqlLocalDbInstance.Name is incorrect.");
            }
            finally
            {
                SqlLocalDbApi.DeleteInstance(instanceName);
            }
        }

        [TestMethod]
        [TestCategory(TestCategories.SqlServer2012)]
        [Description("Tests GetOrCreateInstance() if instanceName is the SQL LocalDB 2012 default instance name.")]
        public void Extensions_GetOrCreateInstance_For_2012_Default_Instance_Name()
        {
            // Arrange
            ISqlLocalDbProvider value = new SqlLocalDbProvider();
            string instanceName = "v11.0";

            // Act
            ISqlLocalDbInstance result = value.GetOrCreateInstance(instanceName);

            // Assert
            Assert.IsNotNull(result, "GetOrCreateInstance() returned null.");
            Assert.AreEqual(instanceName, result.Name, "ISqlLocalDbInstance.Name is incorrect.");
        }

        [TestMethod]
        [TestCategory(TestCategories.SqlServer2014)]
        [TestCategory(TestCategories.SqlServer2016)]
        [Description("Tests GetOrCreateInstance() if instanceName is the SQL LocalDB 2014 and 2016 default instance name.")]
        public void Extensions_GetOrCreateInstance_For_2014_And_2016_Default_Instance_Name()
        {
            // Arrange
            ISqlLocalDbProvider value = new SqlLocalDbProvider();
            string instanceName = "MSSQLLocalDB";

            // Act
            ISqlLocalDbInstance result = value.GetOrCreateInstance(instanceName);

            // Assert
            Assert.IsNotNull(result, "GetOrCreateInstance() returned null.");
            Assert.AreEqual(instanceName, result.Name, "ISqlLocalDbInstance.Name is incorrect.");
        }

        [TestMethod]
        [Description("Tests SetInitialCatalogName() if value is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extensions_SetInitialCatalogName_Throws_If_Value_Is_Null()
        {
            // Arrange
            DbConnectionStringBuilder value = null;
            string initialCatalog = "MyCatalog";

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => value.SetInitialCatalogName(initialCatalog),
                "value");
        }

        [TestMethod]
        [Description("Tests SetInitialCatalogName() sets the correct Initial Catalog name.")]
        [DataSource(
            "Microsoft.VisualStudio.TestTools.DataSource.XML",
            @"|DataDirectory|\SetInitialCatalogNameTestCases.xml",
            "testCase",
            DataAccessMethod.Sequential)]
        public void Extensions_SetInitialCatalogName_Returns_Correct_Initial_Catalog()
        {
            // Arrange
            string connectionString = Convert.ToString(this.TestContext.DataRow["connectionString"], CultureInfo.InvariantCulture);
            string initialCatalog = Convert.ToString(this.TestContext.DataRow["initialCatalog"], CultureInfo.InvariantCulture);

            // Arrange
            DbConnectionStringBuilder value = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString,
            };

            // Act
            value.SetInitialCatalogName(initialCatalog);

            // Assert
            string result = value.GetInitialCatalogName();
            Assert.AreEqual(initialCatalog, result, "SetInitialCatalogName() did not set the correct value.");
        }

        [TestMethod]
        [Description("Tests SetPhysicalFileName() sets the correct physical file name.")]
        [DataSource(
            "Microsoft.VisualStudio.TestTools.DataSource.XML",
            @"|DataDirectory|\SetPhysicalFileNameTestCases.xml",
            "testCase",
            DataAccessMethod.Sequential)]
        public void Extensions_SetPhysicalFileName_Returns_Correct_File_Name()
        {
            // Arrange
            string connectionString = Convert.ToString(this.TestContext.DataRow["connectionString"], CultureInfo.InvariantCulture);
            string fileName = Convert.ToString(this.TestContext.DataRow["physicalFileName"], CultureInfo.InvariantCulture);

            // Arrange
            DbConnectionStringBuilder value = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString,
            };

            // Act
            value.SetPhysicalFileName(fileName);

            // Assert
            string result = value.GetPhysicalFileName();
            Assert.AreEqual(fileName, result, "SetPhysicalFileName() did not set the correct value.");
        }

        [TestMethod]
        [Description("Tests SetPhysicalFileName() sets the correct physical file name if a relative path is used.")]
        public void Extensions_SetPhysicalFileName_Sets_Correct_Value_If_Relative_Path_Used()
        {
            // Arrange
            string connectionString = @"data source=.;attachdbfilename=MyDatabase.mdf;integrated security=True;MultipleActiveResultSets=True";
            string fileName = @".\MyDatabase.mdf";
            string expected = Path.GetFullPath(fileName);

            // Arrange
            DbConnectionStringBuilder value = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString,
            };

            // Act
            value.SetPhysicalFileName(fileName);

            // Assert
            string result = value.GetPhysicalFileName();
            Assert.AreEqual(expected, result, "SetPhysicalFileName() did not set the correct value.");
        }

        [TestMethod]
        [Description("Tests SetPhysicalFileName() sets the correct physical file name if the |DataDirectory| AppDomain value is set.")]
        public void Extensions_SetPhysicalFileName_Sets_Correct_Value_If_Data_Directory_Used_And_Set()
        {
            // Arrange
            var appDomainData = new Dictionary<string, object>()
            {
                { "DataDirectory", @"C:\MyDatabases" },
            };

            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    string connectionString = @"data source=.;attachdbfilename=MyDatabase.mdf;integrated security=True;MultipleActiveResultSets=True";
                    string fileName = @"|DataDirectory|\MyDatabase.mdf";
                    string expected = @"C:\MyDatabases\MyDatabase.mdf";

                    // Arrange
                    DbConnectionStringBuilder value = new DbConnectionStringBuilder()
                    {
                        ConnectionString = connectionString,
                    };

                    // Act
                    value.SetPhysicalFileName(fileName);

                    // Assert
                    string result = value.GetPhysicalFileName();
                    Assert.AreEqual(expected, result, "SetPhysicalFileName() did not set the correct value.");
                },
                appDomainData: appDomainData);
        }

        [TestMethod]
        [Description("Tests SetPhysicalFileName() sets the correct physical file name if the |DataDirectory| AppDomain value is not set.")]
        [ExpectedException(typeof(NotSupportedException))]
        public void Extensions_SetPhysicalFileName_Sets_Correct_Value_If_Data_Directory_Used_And_Not_Set()
        {
            // Arrange
            Helpers.InvokeInNewAppDomain(
                () =>
                {
                    string connectionString = @"data source=.;attachdbfilename=MyDatabase.mdf;integrated security=True;MultipleActiveResultSets=True";
                    string fileName = @"|DataDirectory|\MyDatabase.mdf";

                    // Arrange
                    DbConnectionStringBuilder value = new DbConnectionStringBuilder()
                    {
                        ConnectionString = connectionString,
                    };

                    // Act and Assert
                    throw ErrorAssert.Throws<NotSupportedException>(
                        () => value.SetPhysicalFileName(fileName));
                });
        }

        [TestMethod]
        [Description("Tests SetPhysicalFileName() clears the physical file name if null is specified.")]
        public void Extensions_SetPhysicalFileName_Sets_Correct_Value_If_Null_Specified()
        {
            // Arrange
            string connectionString = @"data source=.;attachdbfilename=MyDatabase.mdf;integrated security=True;MultipleActiveResultSets=True";
            string fileName = null;

            // Arrange
            DbConnectionStringBuilder value = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString,
            };

            // Act
            value.SetPhysicalFileName(fileName);

            // Assert
            string result = value.GetPhysicalFileName();
            Assert.IsNull(result, "SetPhysicalFileName() did not set the correct value.");
        }

        [TestMethod]
        [Description("Tests SetPhysicalFileName() throws if fileName is invalid.")]
        [ExpectedException(typeof(ArgumentException))]
        public void Extensions_SetPhysicalFileName_Throws_If_FileName_Is_Invalid()
        {
            // Arrange
            string connectionString = @"data source=.;attachdbfilename=MyDatabase.mdf;integrated security=True;MultipleActiveResultSets=True";
            string fileName = @"\\\\\\";

            // Arrange
            DbConnectionStringBuilder value = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString,
            };

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentException>(
                () => value.SetPhysicalFileName(fileName),
                "fileName");
        }

        [TestMethod]
        [Description("Tests SetPhysicalFileName() throws if fileName is not supported.")]
        [ExpectedException(typeof(ArgumentException))]
        public void Extensions_SetPhysicalFileName_Throws_If_FileName_Is_Not_Supported()
        {
            // Arrange
            string connectionString = @"data source=.;attachdbfilename=MyDatabase.mdf;integrated security=True;MultipleActiveResultSets=True";
            string fileName = @"\database:mdf";

            // Arrange
            DbConnectionStringBuilder value = new DbConnectionStringBuilder()
            {
                ConnectionString = connectionString,
            };

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentException>(
                () => value.SetPhysicalFileName(fileName),
                "fileName");
        }

        [TestMethod]
        [Description("Tests GetConnectionStringForDefaultModel() throws an exception if the instance has no named pipe.")]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Extensions_GetConnectionStringForDefaultModel_Throws_If_Instance_Has_No_Named_Pipe()
        {
            ISqlLocalDbInstance instance = Mock.Of<ISqlLocalDbInstance>();

            // Act and Assert
            throw ErrorAssert.Throws<InvalidOperationException>(
                () => instance.GetConnectionStringForDefaultModel());
        }

        [TestMethod]
        [Description("Tests Restart() if instance is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Extensions_Restart_Throws_If_Instance_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstance instance = null;

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => instance.Restart(),
                "instance");
        }

        [TestMethod]
        [Description("Tests Restart().")]
        public void Extensions_Restart_Restarts_Instance()
        {
            // Arrange
            Mock<ISqlLocalDbInstance> mock = new Mock<ISqlLocalDbInstance>();
            ISqlLocalDbInstance instance = mock.Object;

            // Act
            instance.Restart();

            // Assert
            mock.Verify((p) => p.Stop(), Times.Once());
            mock.Verify((p) => p.Start(), Times.Once());
        }

        [TestMethod]
        [Description("Tests GetDefaultInstance() returns default instance.")]
        public void Extensions_GetDefaultInstance_Returns_Default_Instance()
        {
            // Arrange
            ISqlLocalDbProvider value = new SqlLocalDbProvider();

            // Act
            ISqlLocalDbInstance result = value.GetDefaultInstance();

            // Assert
            Assert.IsNotNull(result, "GetDefaultInstance() returned null.");
            Assert.AreEqual(SqlLocalDbApi.DefaultInstanceName, result.Name, "ISqlLocalDbInstance.Name is incorrect.");
        }
    }
}
