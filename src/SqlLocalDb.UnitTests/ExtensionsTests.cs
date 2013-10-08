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

        #region Methods

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
                Assert.AreEqual(ConnectionState.Closed, result.State, "DbConnection.State is incorrect.");
            }
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
        }

        #endregion
    }
}