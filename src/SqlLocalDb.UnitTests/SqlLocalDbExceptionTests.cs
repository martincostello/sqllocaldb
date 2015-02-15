// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbExceptionTests.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbExceptionTests.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// A class containing unit tests for the <see cref="SqlLocalDbException"/> class.
    /// </summary>
    [TestClass]
    public class SqlLocalDbExceptionTests
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbExceptionTests"/> class.
        /// </summary>
        public SqlLocalDbExceptionTests()
        {
        }

        [TestMethod]
        [Description("Tests .ctor().")]
        public void SqlLocalDbException_Constructor_Default_Sets_Properties()
        {
            // Act
            SqlLocalDbException target = new SqlLocalDbException();

            // Assert
            Assert.AreEqual(-2147467259, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(SR.SqlLocalDbException_DefaultMessage, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string).")]
        public void SqlLocalDbException_Constructor_With_Message_Sets_Properties()
        {
            // Arrange
            string message = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbException target = new SqlLocalDbException(message);

            // Assert
            Assert.AreEqual(-2147467259, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, Exception).")]
        public void SqlLocalDbException_Constructor_With_Message_And_InnerException_Sets_Properties()
        {
            // Arrange
            InvalidOperationException innerException = new InvalidOperationException();
            string message = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbException target = new SqlLocalDbException(message, innerException);

            // Assert
            Assert.AreEqual(-2147467259, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreSame(innerException, target.InnerException, "The InnerException property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, int).")]
        public void SqlLocalDbException_Constructor_With_Message_And_ErrorCode_Sets_Properties()
        {
            // Arrange
            const int ErrorCode = 337519;
            string message = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbException target = new SqlLocalDbException(message, ErrorCode);

            // Assert
            Assert.AreEqual(ErrorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, int, string).")]
        public void SqlLocalDbException_Constructor_With_Message_ErrorCode_And_InstanceName_Sets_Properties()
        {
            // Arrange
            const int ErrorCode = 337519;
            string instanceName = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbException target = new SqlLocalDbException(message, ErrorCode, instanceName);

            // Assert
            Assert.AreEqual(ErrorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(instanceName, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, int, string, Exception).")]
        public void SqlLocalDbException_Constructor_With_Message_ErrorCode_InstanceName_And_InnerException_Sets_Properties()
        {
            // Arrange
            InvalidOperationException innerException = new InvalidOperationException();
            const int ErrorCode = 337519;
            string instanceName = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbException target = new SqlLocalDbException(message, ErrorCode, instanceName, innerException);

            // Assert
            Assert.AreEqual(ErrorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreSame(innerException, target.InnerException, "The InnerException property is incorrect.");
            Assert.AreEqual(instanceName, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(SerializationInfo, StreamingContext).")]
        public void SqlLocalDbException_Constructor_For_Serialization_Can_Be_Serialized()
        {
            // Arrange
            InvalidOperationException innerException = new InvalidOperationException();
            const int ErrorCode = 337519;
            string instanceName = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();

            // Act
            SqlLocalDbException target = new SqlLocalDbException(message, ErrorCode, instanceName, innerException);

            BinaryFormatter formatter = new BinaryFormatter();

            SqlLocalDbException deserialized;

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, target);
                stream.Seek(0L, SeekOrigin.Begin);
                deserialized = formatter.Deserialize(stream) as SqlLocalDbException;
            }

            // Assert
            Assert.IsNotNull(deserialized, "The exception was not deserialized.");
            Assert.AreEqual(deserialized.ErrorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(deserialized.InstanceName, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(deserialized.Message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests GetObjectData() if info is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SqlLocalDbException_GetObjectData_Throws_If_Info_Is_Null()
        {
            // Arrange
            SqlLocalDbException target = new SqlLocalDbException();

            SerializationInfo info = null;
            StreamingContext context = new StreamingContext();

            // Act and Assert
            throw ErrorAssert.Throws<ArgumentNullException>(
                () => target.GetObjectData(info, context),
                "info");
        }
    }
}