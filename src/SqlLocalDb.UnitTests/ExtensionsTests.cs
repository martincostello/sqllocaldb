// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ExtensionsTests.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2013
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   ExtensionsTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.Common;
using System.Data.EntityClient;
using System.Globalization;
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
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ExtensionsTests"/> class.
        /// </summary>
        public ExtensionsTests()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get;
            set;
        }

        #endregion

        #region Methods

        [TestMethod]
        [Description("Tests GetConnectionForDefaultModel() if instance is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetConnectionForDefaultModel_Throws_If_Instance_Is_Null()
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
        public void GetConnectionForDefaultModel_Returns_Connection()
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
        public void GetConnectionForDefaultModel_Returns_Connection_If_InitialCatalog_Is_Null()
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
        public void GetConnectionForDefaultModel_Returns_Connection_If_InitialCatalog_Is_Empty()
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
        public void GetConnectionForDefaultModel_Returns_Connection_If_InitialCatalog_Is_Specified()
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
        public void GetConnectionForModel_Throws_If_Instance_Is_Null()
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
        public void GetConnectionForModel_Returns_Connection()
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
        public void GetConnectionForModel_Returns_Connection_If_InitialCatalog_Is_Null()
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
        public void GetConnectionForModel_Returns_Connection_If_InitialCatalog_Is_Empty()
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
        public void GetConnectionForModel_Returns_Connection_If_InitialCatalog_Is_Specified()
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
        public void GetConnectionStringForDefaultModel_Throws_If_Instance_Is_Null()
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
        public void GetConnectionStringForDefaultModel_Returns_Connection_String()
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
        public void GetConnectionStringForDefaultModel_Returns_Connection_String_If_InitialCatalog_Is_Null()
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
        public void GetConnectionStringForDefaultModel_Returns_Connection_String_If_InitialCatalog_Is_Empty()
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
        public void GetConnectionStringForDefaultModel_Returns_Connection_String_If_Initial_Catalog_Specified()
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
        [Description("Tests GetConnectionStringForModel() if instance is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetConnectionStringForModel_Throws_If_Instance_Is_Null()
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
        public void GetConnectionStringForModel_Throws_If_ConnectionString_Cannot_Be_Found()
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
        public void GetConnectionStringForModel_Returns_Connection_String()
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
        public void GetConnectionStringForModel_Returns_Connection_String_If_InitialCatalog_Is_Null()
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
        public void GetConnectionStringForModel_Returns_Connection_String_If_InitialCatalog_Is_Empty()
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
        public void GetConnectionStringForModel_Returns_Connection_String_If_Initial_Catalog_Specified()
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
        public void GetInitialCatalogName_Throws_If_Value_Is_Null()
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
        public void GetInitialCatalogName_Returns_Null_If_Not_Found()
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
        public void GetInitialCatalogName_Returns_Correct_Initial_Catalog()
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
        public void GetInitialCatalogName_Returns_Initial_Catalog_If_Entity_Connection_String()
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
        public void GetInitialCatalogName_Returns_Null_If_No_Initial_Catalog_In_Entity_Connection_String()
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
        public void GetInitialCatalogName_Returns_Initial_Catalog_If_Sql_Connection_String()
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
        public void GetInitialCatalogName_Returns_Null_If_No_Initial_Catalog_In_Sql_Connection_String()
        {
            // Arrange
            DbConnectionStringBuilder value = new System.Data.SqlClient.SqlConnectionStringBuilder(
                "data source=.;integrated security=True;MultipleActiveResultSets=True");

            // Act
            string actual = value.GetInitialCatalogName();

            // Assert
            Assert.IsNull(actual, "GetInitialCatalogName() returned incorrect value.  Value: {0}", actual);
        }

        #endregion
    }
}