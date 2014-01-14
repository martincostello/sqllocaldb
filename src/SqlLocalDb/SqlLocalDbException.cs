// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlLocalDbException.cs" company="http://sqllocaldb.codeplex.com">
//   Martin Costello (c) 2012-2014
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   SqlLocalDbException.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.Common;
using System.Runtime.Serialization;
#if !NET40
using System.Security.Permissions;
#endif

namespace System.Data.SqlLocalDb
{
    /// <summary>
    /// The exception that is thrown when SQL Server LocalDB returns an error.
    /// </summary>
    [Serializable]
    public class SqlLocalDbException : DbException
    {
        #region Constants

        /// <summary>
        /// The serialization key for the <see cref="ErrorCode"/> property.
        /// </summary>
        private const string ErrorCodeKey = "SqlLocalDbException_ErrorCode";

        /// <summary>
        /// The serialization key for the <see cref="InstanceName"/> property.
        /// </summary>
        private const string InstanceNameKey = "SqlLocalDbException_InstanceName";

        #endregion

        #region Fields

        /// <summary>
        /// The error code associated with the exception, if any.
        /// </summary>
        private readonly int? _errorCode;

        #endregion

        #region Constructors

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
        public SqlLocalDbException(string message, int errorCode, string instanceName)
            : base(message, errorCode)
        {
            this.InstanceName = instanceName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbException"/> class with
        /// the specified error message and error code.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="errorCode">The error code for the exception.</param>
        /// <param name="instanceName">The name of the LocalDB instance that caused the exception, if any.</param>
        /// <param name="innerException">The inner exception reference.</param>
        public SqlLocalDbException(string message, int errorCode, string instanceName, Exception innerException)
            : base(message, innerException)
        {
            // Set local value as no way to pass both errorCode and innerException to base class
            _errorCode = errorCode;
            this.InstanceName = instanceName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlLocalDbException"/> class with
        /// the specified serialization information and context.
        /// </summary>
        /// <param name="info">
        /// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds
        /// the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains
        /// contextual information about the source or destination.
        /// </param>
        protected SqlLocalDbException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _errorCode = (int?)info.GetValue(ErrorCodeKey, typeof(int?));
            this.InstanceName = info.GetString(InstanceNameKey);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the HRESULT of the error.
        /// </summary>
        public override int ErrorCode
        {
            get { return _errorCode.HasValue ? _errorCode.Value : base.ErrorCode; }
        }

        /// <summary>
        /// Gets or sets the name of the SQL Server LocalDB
        /// instance that caused the exception, if any.
        /// </summary>
        public string InstanceName
        {
            get;
            protected set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="info"/> parameter is <see langword="null"/>.
        /// </exception>
        /// <PermissionSet>
        /// <IPermission class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*"/>
        /// <IPermission class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089" version="1" Flags="SerializationFormatter"/>
        /// </PermissionSet>
#if NET40
        [System.Security.SecurityCritical]
#else
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
#endif
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            base.GetObjectData(info, context);

            info.AddValue(ErrorCodeKey, _errorCode);
            info.AddValue(InstanceNameKey, this.InstanceName);
        }

        #endregion
    }
}