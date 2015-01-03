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
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbExceptionTests"/> class.
        /// </summary>
        public SqlLocalDbExceptionTests()
        {
        }

        #endregion

        #region Methods

        [TestMethod]
        [Description("Tests .ctor().")]
        public void Constructor_Default()
        {
            SqlLocalDbException target = new SqlLocalDbException();

            Assert.AreEqual(-2147467259, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(SR.SqlLocalDbException_DefaultMessage, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string).")]
        public void Constructor_Message()
        {
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message);

            Assert.AreEqual(-2147467259, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, Exception).")]
        public void Constructor_MessageInnerException()
        {
            InvalidOperationException innerException = new InvalidOperationException();
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message, innerException);

            Assert.AreEqual(-2147467259, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreSame(innerException, target.InnerException, "The InnerException property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, int).")]
        public void Constructor_MessageErrorCode()
        {
            const int ErrorCode = 337519;
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message, ErrorCode);

            Assert.AreEqual(ErrorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(null, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, int, string).")]
        public void Constructor_MessageErrorCodeInstanceName()
        {
            const int ErrorCode = 337519;
            string instanceName = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message, ErrorCode, instanceName);

            Assert.AreEqual(ErrorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(instanceName, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(string, int, string, Exception).")]
        public void Constructor_MessageErrorCodeInstanceNameInnerException()
        {
            InvalidOperationException innerException = new InvalidOperationException();
            const int ErrorCode = 337519;
            string instanceName = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message, ErrorCode, instanceName, innerException);

            Assert.AreEqual(ErrorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreSame(innerException, target.InnerException, "The InnerException property is incorrect.");
            Assert.AreEqual(instanceName, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests .ctor(SerializationInfo, StreamingContext).")]
        public void Constructor_Serialization()
        {
            InvalidOperationException innerException = new InvalidOperationException();
            const int ErrorCode = 337519;
            string instanceName = Guid.NewGuid().ToString();
            string message = Guid.NewGuid().ToString();

            SqlLocalDbException target = new SqlLocalDbException(message, ErrorCode, instanceName, innerException);

            BinaryFormatter formatter = new BinaryFormatter();

            SqlLocalDbException deserialized;

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, target);
                stream.Seek(0L, SeekOrigin.Begin);
                deserialized = formatter.Deserialize(stream) as SqlLocalDbException;
            }

            Assert.IsNotNull(deserialized, "The exception was not deserialized.");
            Assert.AreEqual(deserialized.ErrorCode, target.ErrorCode, "The ErrorCode property is incorrect.");
            Assert.AreEqual(deserialized.InstanceName, target.InstanceName, "The InstanceName property is incorrect.");
            Assert.AreEqual(deserialized.Message, target.Message, "The Message property is incorrect.");
        }

        [TestMethod]
        [Description("Tests GetObjectData() if info is null.")]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetObjectData_ThrowsIfInfoIsNull()
        {
            SqlLocalDbException target = new SqlLocalDbException();

            throw ErrorAssert.Throws<ArgumentNullException>(
                () => target.GetObjectData(null, new Runtime.Serialization.StreamingContext()),
                "info");
        }

        #endregion
    }
}