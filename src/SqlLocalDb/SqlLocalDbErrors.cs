// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class containing SQL Server LocalDB errors.  This class cannot be inherited.
    /// </summary>
    public static class SqlLocalDbErrors
    {
        /// <summary>
        /// Cannot create folder for the LocalDB instance at: <c>&#37;LOCALAPPDATA&#37;\Microsoft\Microsoft SQL Server Local DB\Instances\&lt;instance name&gt;</c>.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_CANNOT_CREATE_INSTANCE_FOLDER</c> error.
        /// </remarks>
        public const int CannotCreateInstanceFolder = unchecked((int)0x89c50100);

        /// <summary>
        /// The parameter for the LocalDB Instance API method is incorrect.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INVALID_PARAMETER</c> error.
        /// </remarks>
        public const int InvalidParameter = unchecked((int)0x89c50101);

        /// <summary>
        /// Unable to create the LocalDB instance with specified version.
        /// An instance with the same name already exists, but it has lower
        /// version than the specified version.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INSTANCE_EXISTS_WITH_LOWER_VERSION</c> error.
        /// </remarks>
        public const int InstanceExistsWithLowerVersion = unchecked((int)0x89c50102);

        /// <summary>
        /// Cannot access the user profile folder for local application data (<c>&#37;LOCALAPPDATA&#37;</c>).
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_CANNOT_GET_USER_PROFILE_FOLDER</c> error.
        /// </remarks>
        public const int CannotGetUserProfileFolder = unchecked((int)0x89c50103);

        /// <summary>
        /// The full path length of the LocalDB instance folder is longer than
        /// MAX_PATH. The instance must be stored in folder: <c>&#37;LOCALAPPDATA&#37;\Microsoft\Microsoft SQL Server Local DB\Instances\&lt;instance name&gt;</c>.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INSTANCE_FOLDER_PATH_TOO_LONG</c> error.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long", Justification = "Usage is safe.")]
        public const int InstanceFolderPathTooLong = unchecked((int)0x89c50104);

        /// <summary>
        /// Cannot access LocalDB instance folder: <c>&#37;LOCALAPPDATA&#37;\Microsoft\Microsoft SQL Server Local DB\Instances\&lt;instance name&gt;</c>.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_CANNOT_ACCESS_INSTANCE_FOLDER</c> error.
        /// </remarks>
        public const int CannotAccessInstanceFolder = unchecked((int)0x89c50105);

        /// <summary>
        /// Unexpected error occurred while trying to access the LocalDB instance
        /// registry configuration. See the Windows Application event log for error details.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_CANNOT_ACCESS_INSTANCE_REGISTRY</c> error.
        /// </remarks>
        public const int CannotAccessInstanceRegistry = unchecked((int)0x89c50106);

        /// <summary>
        /// The specified LocalDB instance does not exist.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_UNKNOWN_INSTANCE</c> error.
        /// </remarks>
        public const int UnknownInstance = unchecked((int)0x89c50107);

        /// <summary>
        /// Unexpected error occurred inside a LocalDB instance API method call.
        /// See the Windows Application event log for error details.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INTERNAL_ERROR</c> error.
        /// </remarks>
        public const int InternalError = unchecked((int)0x89c50108);

        /// <summary>
        /// Unexpected error occurred while trying to modify the registry
        /// configuration for the LocalDB instance. See the Windows Application
        /// event log for error details.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_CANNOT_MODIFY_INSTANCE_REGISTRY</c> error.
        /// </remarks>
        public const int CannotModifyInstanceRegistry = unchecked((int)0x89c50109);

        /// <summary>
        /// Error occurred during LocalDB instance startup: SQL Server process failed to start.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_SQL_SERVER_STARTUP_FAILED</c> error.
        /// </remarks>
        public const int ServerStartupFailed = unchecked((int)0x89c5010a);

        /// <summary>
        /// LocalDB instance is corrupted. See the Windows Application event log for error details.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INSTANCE_CONFIGURATION_CORRUPT</c> error.
        /// </remarks>
        public const int InstanceConfigurationCorrupt = unchecked((int)0x89c5010b);

        /// <summary>
        /// Error occurred during LocalDB instance startup: unable to create the SQL Server process.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_CANNOT_CREATE_SQL_PROCESS</c> error.
        /// </remarks>
        public const int CannotCreateSqlProcess = unchecked((int)0x89c5010c);

        /// <summary>
        /// The specified LocalDB version is not available on this computer.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_UNKNOWN_VERSION</c> error.
        /// </remarks>
        public const int UnknownVersion = unchecked((int)0x89c5010d);

        /// <summary>
        /// Error getting the localized error message.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_UNKNOWN_LANGUAGE_ID</c> error.
        /// </remarks>
        public const int UnknownLanguageId = unchecked((int)0x89c5010e);

        /// <summary>
        /// Stop operation for LocalDB instance failed to complete within the specified time.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INSTANCE_STOP_FAILED</c> error.
        /// </remarks>
        public const int InstanceStopFailed = unchecked((int)0x89c5010f);

        /// <summary>
        /// Error getting the localized error message. The specified error code is unknown.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_UNKNOWN_ERROR_CODE</c> error.
        /// </remarks>
        public const int UnknownErrorCode = unchecked((int)0x89c50110);

        /// <summary>
        /// The LocalDB version available on this workstation is lower than
        /// the requested LocalDB version.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_VERSION_REQUESTED_NOT_INSTALLED</c> error.
        /// </remarks>
        public const int VersionNotInstalled = unchecked((int)0x89c50111);

        /// <summary>
        /// Requested operation on LocalDB instance cannot be performed because
        /// specified instance is currently in use. Stop the instance and try again.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INSTANCE_BUSY</c> error.
        /// </remarks>
        public const int InstanceBusy = unchecked((int)0x89c50112);

        /// <summary>
        /// Default LocalDB instances cannot be created, stopped or deleted manually.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INVALID_OPERATION</c> error.
        /// </remarks>
        public const int InvalidOperation = unchecked((int)0x89c50113);

        /// <summary>
        /// The buffer passed to the LocalDB instance API method has insufficient size.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INSUFFICIENT_BUFFER</c> error.
        /// </remarks>
        public const int InsufficientBuffer = unchecked((int)0x89c50114);

        /// <summary>
        /// Timeout occurred inside the LocalDB instance API method.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_WAIT_TIMEOUT</c> error.
        /// </remarks>
        public const int WaitTimeout = unchecked((int)0x89c50115);

        /// <summary>
        /// SQL Server LocalDB is not installed.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_NOT_INSTALLED</c> error.
        /// </remarks>
        public const int NotInstalled = unchecked((int)0x89c50116);

        /// <summary>
        /// Failed to start XEvent engine within the LocalDB Instance API.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_XEVENT_FAILED</c> error.
        /// </remarks>
        public const int XEventFailed = unchecked((int)0x89c50117);

        /// <summary>
        /// Cannot create an automatic instance. See the Windows Application event log for error details.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_AUTO_INSTANCE_CREATE_FAILED</c> error.
        /// </remarks>
        public const int AutoInstanceCreateFailed = unchecked((int)0x89c50118);

        /// <summary>
        /// Cannot create a shared instance. The specified shared instance name is already in use.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_SHARED_NAME_TAKEN</c> error.
        /// </remarks>
        public const int SharedNameTaken = unchecked((int)0x89c50119);

        /// <summary>
        /// API caller is not LocalDB instance owner.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_CALLER_IS_NOT_OWNER</c> error.
        /// </remarks>
        public const int CallerIsNotOwner = unchecked((int)0x89c5011a);

        /// <summary>
        /// Specified LocalDB instance name is invalid.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INVALID_INSTANCE_NAME</c> error.
        /// </remarks>
        public const int InvalidInstanceName = unchecked((int)0x89c5011b);

        /// <summary>
        /// The specified LocalDB instance is already shared with different shared name.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INSTANCE_ALREADY_SHARED</c> error.
        /// </remarks>
        public const int InstanceAlreadyShared = unchecked((int)0x89c5011c);

        /// <summary>
        /// The specified LocalDB instance is not shared.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_INSTANCE_NOT_SHARED</c> error.
        /// </remarks>
        public const int InstanceNotShared = unchecked((int)0x89c5011d);

        /// <summary>
        /// Administrator privileges are required in order to execute this operation.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_ADMIN_RIGHTS_REQUIRED</c> error.
        /// </remarks>
        public const int AdminRightsRequired = unchecked((int)0x89c5011e);

        /// <summary>
        /// Unable to share a LocalDB instance - maximum number of shared LocalDB instances reached.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_TOO_MANY_SHARED_INSTANCES</c> error.
        /// </remarks>
        public const int TooManySharedInstances = unchecked((int)0x89c5011f);

        /// <summary>
        /// The "<c>Parent Instance</c>" registry value is missing in the LocalDB instance registry key.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_CANNOT_GET_LOCAL_APP_DATA_PATH</c> error.
        /// </remarks>
        public const int CannotGetLocalAppDataPath = unchecked((int)0x89c50120);

        /// <summary>
        /// Cannot load resources for this DLL. Resources for this DLL should
        /// be stored in a subfolder Resources, with the same file name as this DLL
        /// and the extension "<c>.RLL</c>".
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_ERROR_CANNOT_LOAD_RESOURCES</c> error.
        /// </remarks>
        public const int CannotLoadResources = unchecked((int)0x89c50121);

        /// <summary>
        /// The "<c>DataDirectory</c>" registry value is missing in the LocalDB instance registry key.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_DATADIRECTORY_IS_MISSING</c> error.
        /// </remarks>
        public const int DataDirectoryMissing = unchecked((int)0x89c50200);

        /// <summary>
        /// Cannot access LocalDB instance folder.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_CANNOT_ACCESS_INSTANCE_FOLDER</c> error.
        /// </remarks>
        public const int CannotAccessInstanceFolderDetail = unchecked((int)0x89c50201);

        /// <summary>
        /// The "<c>DataDirectory</c>" registry value is too long in the LocalDB instance registry key.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_DATADIRECTORY_IS_TOO_LONG</c> error.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long", Justification = "Usage is safe.")]
        public const int DataDirectoryIsTooLong = unchecked((int)0x89c50202);

        /// <summary>
        /// The "<c>Parent Instance</c>" registry value is missing in the LocalDB instance registry key.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_PARENT_INSTANCE_IS_MISSING</c> error.
        /// </remarks>
        public const int ParentInstanceIsMissing = unchecked((int)0x89c50203);

        /// <summary>
        /// The "<c>Parent Instance</c>" registry value is too long in the LocalDB instance registry key.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_PARENT_INSTANCE_IS_TOO_LONG</c> error.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long", Justification = "Usage is safe.")]
        public const int ParentInstanceIsTooLong = unchecked((int)0x89c50204);

        /// <summary>
        /// Data directory for LocalDB instance is invalid.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_DATA_DIRECTORY_INVALID</c> error.
        /// </remarks>
        public const int DataDirectoryInvalid = unchecked((int)0x89c50205);

        /// <summary>
        /// LocalDB instance API: XEvent engine assert.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_XEVENT_ASSERT</c> error.
        /// </remarks>
        public const int XEventAssert = unchecked((int)0x89c50206);

        /// <summary>
        /// LocalDB instance API: XEvent error.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_XEVENT_ERROR</c> error.
        /// </remarks>
        public const int XEventError = unchecked((int)0x89c50207);

        /// <summary>
        /// LocalDB installation is corrupted. Reinstall the LocalDB.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_INSTALLATION_CORRUPTED</c> error.
        /// </remarks>
        public const int InstallationCorrupted = unchecked((int)0x89c50208);

        /// <summary>
        /// LocalDB XEvent error: cannot determine &#37;ProgramFiles&#37; folder location.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_CANNOT_GET_PROGRAM_FILES_LOCATION</c> error.
        /// </remarks>
        public const int CannotGetProgramFilesLocation = unchecked((int)0x89c50209);

        /// <summary>
        /// LocalDB XEvent error: Cannot initialize XEvent engine.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_XEVENT_CANNOT_INITIALIZE</c> error.
        /// </remarks>
        public const int CannotInitializeXEvent = unchecked((int)0x89c5020a);

        /// <summary>
        /// LocalDB XEvent error: Cannot find XEvents configuration file.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_XEVENT_CANNOT_FIND_CONF_FILE</c> error.
        /// </remarks>
        public const int CannotFindXEventConfigFile = unchecked((int)0x89c5020b);

        /// <summary>
        /// LocalDB XEvent error: Cannot configure XEvents engine with the configuration file.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_XEVENT_CANNOT_CONFIGURE</c> error.
        /// </remarks>
        public const int CannotConfigureXEvent = unchecked((int)0x89c5020c);

        /// <summary>
        /// LocalDB XEvent error: XEvents engine configuration file too long.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_XEVENT_CONF_FILE_NAME_TOO_LONG</c> error.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "long", Justification = "Usage is safe.")]
        public const int XEventConfigFileTooLong = unchecked((int)0x89c5020d);

        /// <summary>
        /// <c>CoInitializeEx</c> API failed.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_COINITIALIZEEX_FAILED</c> error.
        /// </remarks>
        [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Co", Justification = "Is part of a function name.")]
        public const int CoInitializeExFailed = unchecked((int)0x89c5020e);

        /// <summary>
        /// LocalDB parent instance version is invalid.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_PARENT_INSTANCE_VERSION_INVALID</c> error.
        /// </remarks>
        public const int ParentInstanceVersionInvalid = unchecked((int)0x89c5020f);

        /// <summary>
        /// A Windows API call returned an error.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_WINAPI_ERROR</c> error.
        /// </remarks>
        public const int WindowsApiError = unchecked((int)0x89c50210);

        /// <summary>
        /// Unexpected result.
        /// </summary>
        /// <remarks>
        /// Maps to the <c>LOCALDB_EDETAIL_UNEXPECTED_RESULT</c> error.
        /// </remarks>
        public const int UnexpectedResult = unchecked((int)0x89c50211);
    }
}
