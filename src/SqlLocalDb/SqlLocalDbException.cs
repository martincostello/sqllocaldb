// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Data.Common;
using System.Runtime.Serialization;

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// The exception that is thrown when SQL Server LocalDB returns an error.
/// </summary>
[Serializable]
public class SqlLocalDbException : DbException
{
    /// <summary>
    /// The serialization key for the <see cref="ErrorCode"/> property.
    /// </summary>
    private const string ErrorCodeKey = "SqlLocalDbException_ErrorCode";

    /// <summary>
    /// The serialization key for the <see cref="InstanceName"/> property.
    /// </summary>
    private const string InstanceNameKey = "SqlLocalDbException_InstanceName";

    /// <summary>
    /// The error code associated with the exception, if any.
    /// </summary>
    private readonly int? _errorCode;

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLocalDbException"/> class.
    /// </summary>
    public SqlLocalDbException()
        : base(SR.SqlLocalDbException_DefaultMessage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLocalDbException"/> class with
    /// the specified error message.
    /// </summary>
    /// <param name="message">The message to display for this exception.</param>
    public SqlLocalDbException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLocalDbException"/> class with
    /// the specified error message and a reference to the inner exception that is
    /// the cause of this exception.
    /// </summary>
    /// <param name="message">The error message string.</param>
    /// <param name="innerException">The inner exception reference.</param>
    public SqlLocalDbException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLocalDbException"/> class with
    /// the specified error message and error code.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorCode">The error code for the exception.</param>
    public SqlLocalDbException(string message, int errorCode)
        : base(message, errorCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLocalDbException"/> class with
    /// the specified error message and error code.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorCode">The error code for the exception.</param>
    /// <param name="instanceName">The name of the LocalDB instance that caused the exception, if any.</param>
    public SqlLocalDbException(string message, int errorCode, string? instanceName)
        : base(message, errorCode)
    {
        InstanceName = instanceName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLocalDbException"/> class with
    /// the specified error message and error code.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="errorCode">The error code for the exception.</param>
    /// <param name="instanceName">The name of the LocalDB instance that caused the exception, if any.</param>
    /// <param name="innerException">The inner exception reference.</param>
    public SqlLocalDbException(string message, int errorCode, string? instanceName, Exception innerException)
        : base(message, innerException)
    {
        // Set local value as no way to pass both errorCode and innerException to base class
        _errorCode = errorCode;
        InstanceName = instanceName;
    }

#pragma warning disable SYSLIB0051
    /// <summary>
    /// Initializes a new instance of the <see cref="SqlLocalDbException"/> class with
    /// the specified serialization information and context.
    /// </summary>
    /// <param name="info">
    /// The <see cref="SerializationInfo"/> that holds
    /// the serialized object data about the exception being thrown.
    /// </param>
    /// <param name="context">
    /// The <see cref="StreamingContext"/> that contains
    /// contextual information about the source or destination.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="info"/> is <see langword="null"/>.
    /// </exception>
    protected SqlLocalDbException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
        _errorCode = (int?)info.GetValue(ErrorCodeKey, typeof(int?));
        InstanceName = info.GetString(InstanceNameKey);
    }
#pragma warning restore SYSLIB0051

    /// <summary>
    /// Gets the HRESULT of the error.
    /// </summary>
    public override int ErrorCode => _errorCode ?? base.ErrorCode;

    /// <summary>
    /// Gets or sets the name of the SQL Server LocalDB
    /// instance that caused the exception, if any.
    /// </summary>
    public string? InstanceName { get; protected set; }

    /// <summary>
    /// Sets the <see cref="SerializationInfo"/> with information about the exception.
    /// </summary>
    /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
    /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="info"/> parameter is <see langword="null"/>.
    /// </exception>
#pragma warning disable IDE0055
#if NET8_0
    [Obsolete("This API supports obsolete formatter-based serialization. It should not be called or extended by application code.")]
#endif
#pragma warning restore IDE0055
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        if (info == null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        base.GetObjectData(info, context);

        info.AddValue(ErrorCodeKey, _errorCode);
        info.AddValue(InstanceNameKey, InstanceName);
    }
}
