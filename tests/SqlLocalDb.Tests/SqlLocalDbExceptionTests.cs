﻿// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Runtime.Serialization;

namespace MartinCostello.SqlLocalDb;

public static class SqlLocalDbExceptionTests
{
    [Fact]
    public static void SqlLocalDbException_Constructor_Default_Sets_Properties()
    {
        // Act
        var target = new SqlLocalDbException();

        // Assert
        target.ErrorCode.ShouldBe(-2147467259);
        target.InstanceName.ShouldBeNull();
        target.Message.ShouldBe("An error occurred with a SQL Server LocalDB instance.");
    }

    [Fact]
    public static void SqlLocalDbException_Constructor_With_Message_Sets_Properties()
    {
        // Arrange
        string message = Guid.NewGuid().ToString();

        // Act
        var target = new SqlLocalDbException(message);

        // Assert
        target.ErrorCode.ShouldBe(-2147467259);
        target.InstanceName.ShouldBeNull();
        target.Message.ShouldBe(message);
    }

    [Fact]
    public static void SqlLocalDbException_Constructor_With_Message_And_InnerException_Sets_Properties()
    {
        // Arrange
        var innerException = new InvalidOperationException();
        string message = Guid.NewGuid().ToString();

        // Act
        var target = new SqlLocalDbException(message, innerException);

        // Assert
        target.ErrorCode.ShouldBe(-2147467259);
        target.InnerException.ShouldBeSameAs(innerException);
        target.InstanceName.ShouldBeNull();
        target.Message.ShouldBe(message);
    }

    [Fact]
    public static void SqlLocalDbException_Constructor_With_Message_And_ErrorCode_Sets_Properties()
    {
        // Arrange
        const int ErrorCode = 337519;
        string message = Guid.NewGuid().ToString();

        // Act
        var target = new SqlLocalDbException(message, ErrorCode);

        // Assert
        target.ErrorCode.ShouldBe(ErrorCode);
        target.InstanceName.ShouldBeNull();
        target.Message.ShouldBe(message);
    }

    [Fact]
    public static void SqlLocalDbException_Constructor_With_Message_ErrorCode_And_InstanceName_Sets_Properties()
    {
        // Arrange
        const int ErrorCode = 337519;
        string instanceName = Guid.NewGuid().ToString();
        string message = Guid.NewGuid().ToString();

        // Act
        var target = new SqlLocalDbException(message, ErrorCode, instanceName);

        // Assert
        target.ErrorCode.ShouldBe(ErrorCode);
        target.InstanceName.ShouldBe(instanceName);
        target.Message.ShouldBe(message);
    }

    [Fact]
    public static void SqlLocalDbException_Constructor_With_Message_ErrorCode_InstanceName_And_InnerException_Sets_Properties()
    {
        // Arrange
        var innerException = new InvalidOperationException();
        const int ErrorCode = 337519;
        string instanceName = Guid.NewGuid().ToString();
        string message = Guid.NewGuid().ToString();

        // Act
        var target = new SqlLocalDbException(message, ErrorCode, instanceName, innerException);

        // Assert
        target.ErrorCode.ShouldBe(ErrorCode);
        target.InnerException.ShouldBeSameAs(innerException);
        target.InstanceName.ShouldBe(instanceName);
        target.Message.ShouldBe(message);
    }

    [Fact]
    [Obsolete("Obsolete members are still tested.")]
    public static void SqlLocalDbException_GetObjectData_Throws_If_Info_Is_Null()
    {
        // Arrange
        var target = new SqlLocalDbException();

        SerializationInfo? info = null;
        var context = new StreamingContext();

        // Act and Assert
        Assert.Throws<ArgumentNullException>("info", () => target.GetObjectData(info!, context));
    }

#if NETFRAMEWORK
    [Fact]
    [Obsolete("Obsolete members are still tested.")]
    public static void SqlLocalDbException_Can_Be_Roundtrip_Serialized()
    {
        // Arrange
        var expected = new SqlLocalDbException("Some exception.", 123, "my-instance", new SqlLocalDbException());

        SqlLocalDbException actual;

        // Act
        using (var stream = Serialize(expected))
        {
            actual = Deserialize(stream);
        }

        // Assert
        actual.ShouldNotBeNull();
        actual.Message.ShouldBe(expected.Message);
        actual.ErrorCode.ShouldBe(expected.ErrorCode);
        actual.InstanceName.ShouldBe(expected.InstanceName);
        actual.InnerException.ShouldNotBeNull();
        actual.InnerException.ShouldBeOfType<SqlLocalDbException>();

        static Stream Serialize(object graph)
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            var stream = new MemoryStream();

            try
            {
                formatter.Serialize(stream, graph);

                stream.Seek(0L, SeekOrigin.Begin);

                return stream;
            }
            catch (Exception)
            {
                stream?.Dispose();
                throw;
            }
        }

        static SqlLocalDbException Deserialize(Stream stream)
        {
            var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            return (SqlLocalDbException)formatter.Deserialize(stream);
        }
    }
#endif
}
