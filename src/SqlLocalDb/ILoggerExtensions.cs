// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class containing extension methods for the <see cref="ILogger"/> interface. This class cannot be inherited.
    /// </summary>
    internal static class ILoggerExtensions
    {
        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance has been created.
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> _createdInstance = LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            EventIds.CreatedInstance,
            SR.ILoggerExtensions_CreatedInstanceFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance is being created.
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> _creatingInstance = LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            EventIds.CreatingInstance,
            SR.ILoggerExtensions_CreatingInstanceFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance has been deleted.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _deletedInstance = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.DeletedInstance,
            SR.ILoggerExtensions_DeletedInstanceFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance is being deleting.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _deletingInstance = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.DeletingInstance,
            SR.ILoggerExtensions_DeletingFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance that could not be deleted.
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> _deletingInstanceFailed = LoggerMessage.Define<string, string>(
            LogLevel.Error,
            EventIds.DeletingInstanceFailed,
            SR.ILoggerExtensions_DeleteFailedFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance that could not be deleted as it is still in use.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _deletingInstanceFailedAsInUse = LoggerMessage.Define<string>(
            LogLevel.Warning,
            EventIds.DeletingInstanceFailedAsInUse,
            SR.ILoggerExtensions_DeleteFailedAsInUseFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance that could not be deleted because it could not be found.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _deletingInstanceFailedAsInstanceNotFound = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.DeletingInstanceFailedAsCannotBeNotFound,
            SR.ILoggerExtensions_InstanceDoesNotExistFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance files have been deleted.
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> _deletedInstanceFiles = LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            EventIds.DeletedInstanceFiles,
            SR.ILoggerExtensions_DeletedInstanceFilesFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance files are being deleted.
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> _deletingInstanceFiles = LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            EventIds.DeletingInstanceFiles,
            SR.ILoggerExtensions_DeletingInstanceFilesFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance files could not be deleted.
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> _deletingInstanceFilesFailed = LoggerMessage.Define<string, string>(
            LogLevel.Error,
            EventIds.DeletingInstanceFilesFailed,
            SR.ILoggerExtensions_DeletingInstanceFilesFailedFormat);

        /// <summary>
        /// Logging delegate for when getting information about a SQL LocalDB instance.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _gettingInstanceInfo = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.GettingInstanceInfo,
            SR.ILoggerExtensions_GettingInfoFormat);

        /// <summary>
        /// Logging delegate for when information about a SQL LocalDB instance was got.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _gotInstanceInfo = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.GotInstanceInfo,
            SR.ILoggerExtensions_GotInfoFormat);

        /// <summary>
        /// Logging delegate for when getting SQL LocalDB instance names.
        /// </summary>
        private static readonly Action<ILogger, Exception> _gettingInstanceNames = LoggerMessage.Define(
            LogLevel.Debug,
            EventIds.GettingInstanceNames,
            SR.ILoggerExtensions_GetInstances);

        /// <summary>
        /// Logging delegate for when SQL LocalDB instance names were got.
        /// </summary>
        private static readonly Action<ILogger, int, Exception> _gotInstanceNames = LoggerMessage.Define<int>(
            LogLevel.Debug,
            EventIds.GotInstanceNames,
            SR.ILoggerExtensions_GotInstancesFormat);

        /// <summary>
        /// Logging delegate for when getting information about a SQL LocalDB version.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _gettingVersionInfo = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.GettingVersionInfo,
            SR.ILoggerExtensions_GetVersionInfoFormat);

        /// <summary>
        /// Logging delegate for when information about a SQL LocalDB version was got.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _gotVersionInfo = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.GotVersionInfo,
            SR.ILoggerExtensions_GotVersionInfoFormat);

        /// <summary>
        /// Logging delegate for when getting SQL LocalDB versions.
        /// </summary>
        private static readonly Action<ILogger, Exception> _gettingVersions = LoggerMessage.Define(
            LogLevel.Debug,
            EventIds.GettingVersions,
            SR.ILoggerExtensions_GetVersions);

        /// <summary>
        /// Logging delegate for when SQL LocalDB versions were got.
        /// </summary>
        private static readonly Action<ILogger, int, Exception> _gotVersions = LoggerMessage.Define<int>(
            LogLevel.Debug,
            EventIds.GotVersions,
            SR.ILoggerExtensions_GotVersionsFormat);

        /// <summary>
        /// Logging delegate for when a invalid Language Id is used.
        /// </summary>
        private static readonly Action<ILogger, int, Exception> _invalidLanguageId = LoggerMessage.Define<int>(
            LogLevel.Warning,
            EventIds.InvalidLanguageId,
            SR.ILoggerExtensions_InvalidLanguageIdFormat);

        /// <summary>
        /// Logging delegate for when a invalid SQL LocalDB Instance API registry key was found.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _invalidRegistryKey = LoggerMessage.Define<string>(
            LogLevel.Warning,
            EventIds.InvalidRegistryKey,
            SR.ILoggerExtensions_InvalidRegistryKeyNameFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB Instance API function could not be found.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _nativeApiFunctionNotFound = LoggerMessage.Define<string>(
            LogLevel.Error,
            EventIds.NativeFunctionNotFound,
            SR.ILoggerExtensions_FunctionNotFoundFormat);

        /// <summary>
        /// Logging delegate for when the SQL LocalDB Instance API DLL was loaded.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _nativeApiLoaded = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.NativeApiLoaded,
            SR.ILoggerExtensions_NativeApiLoadedFormat);

        /// <summary>
        /// Logging delegate for when the SQL LocalDB Instance API DLL could not loaded.
        /// </summary>
        private static readonly Action<ILogger, string, int, Exception> _nativeApiLoadFailed = LoggerMessage.Define<string, int>(
            LogLevel.Error,
            EventIds.NativeApiLoadFailed,
            SR.ILoggerExtensions_NativeApiLoadFailedFormat);

        /// <summary>
        /// Logging delegate for when the SQL LocalDB Instance API could not be found.
        /// </summary>
        private static readonly Action<ILogger, Exception> _nativeApiNotFound = LoggerMessage.Define(
            LogLevel.Warning,
            EventIds.NoNativeApiFound,
            SR.ILoggerExtensions_NoNativeApiFound);

        /// <summary>
        /// Logging delegate for when the SQL LocalDB Instance API DLL was not loaded.
        /// </summary>
        private static readonly Action<ILogger, Exception> _nativeApiNotLoaded = LoggerMessage.Define(
            LogLevel.Warning,
            EventIds.NativeApiNotLoaded,
            SR.ILoggerExtensions_NativeApiNotLoaded);

        /// <summary>
        /// Logging delegate for when the SQL LocalDB Instance API DLL could not be found at the configured path.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _nativeApiPathNotFound = LoggerMessage.Define<string>(
            LogLevel.Error,
            EventIds.NativeApiPathNotFound,
            SR.ILoggerExtensions_NativeApiNotFoundFormat);

        /// <summary>
        /// Logging delegate for when the SQL LocalDB Instance API DLL was unloaded.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _nativeApiUnloaded = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.NativeApiUnloaded,
            SR.ILoggerExtensions_NativeApiUnloadedFormat);

        /// <summary>
        /// Logging delegate for when the version of the SQL LocalDB Instance API to use was overridden by the user.
        /// </summary>
        private static readonly Action<ILogger, Version, Exception> _nativeApiVersionOverriddenByUser = LoggerMessage.Define<Version>(
            LogLevel.Debug,
            EventIds.NativeApiVersionOverriddenByUser,
            SR.ILoggerExtensions_NativeApiVersionOverriddenByUserFormat);

        /// <summary>
        /// Logging delegate for when the version of the SQL LocalDB Instance API specified by the user could not be found.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _nativeApiVersionOverrideNotFound = LoggerMessage.Define<string>(
            LogLevel.Warning,
            EventIds.NativeApiVersionOverrideNotFound,
            SR.ILoggerExtensions_OverrideVersionNotFoundFormat);

        /// <summary>
        /// Logging delegate for when SQL LocalDB is not installed.
        /// </summary>
        private static readonly Action<ILogger, Exception> _notInstalled = LoggerMessage.Define(
            LogLevel.Warning,
            EventIds.NotInstalled,
            SR.ILoggerExtensions_NotInstalled);

        /// <summary>
        /// Logging delegate for when the SQL LocalDB Instance API registry key cannot be found.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _registryKeyNotFound = LoggerMessage.Define<string>(
            LogLevel.Warning,
            EventIds.RegistryKeyNotFound,
            SR.ILoggerExtensions_RegistryKeyNotFoundFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance was shared.
        /// </summary>
        private static readonly Action<ILogger, string, string, string, Exception> _sharedInstance = LoggerMessage.Define<string, string, string>(
            LogLevel.Debug,
            EventIds.SharedInstance,
            SR.ILoggerExtensions_SharedInstanceFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance is shared.
        /// </summary>
        private static readonly Action<ILogger, string, string, string, Exception> _sharingInstance = LoggerMessage.Define<string, string, string>(
            LogLevel.Debug,
            EventIds.SharingInstance,
            SR.ILoggerExtensions_SharingInstanceFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance is starting.
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> _startedInstance = LoggerMessage.Define<string, string>(
            LogLevel.Debug,
            EventIds.StartedInstance,
            SR.ILoggerExtensions_StartedFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance is starting.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _startingInstance = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.StartingInstance,
            SR.ILoggerExtensions_StartingFormat);

        /// <summary>
        /// Logging delegate for when SQL LocalDB tracing has started.
        /// </summary>
        private static readonly Action<ILogger, Exception> _startedTracing = LoggerMessage.Define(
            LogLevel.Debug,
            EventIds.StartedTracing,
            SR.ILoggerExtensions_StartedTracing);

        /// <summary>
        /// Logging delegate for when SQL LocalDB tracing is starting.
        /// </summary>
        private static readonly Action<ILogger,  Exception> _startingTracing = LoggerMessage.Define(
            LogLevel.Debug,
            EventIds.StartingTracing,
            SR.ILoggerExtensions_StartTracing);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance was stopped.
        /// </summary>
        private static readonly Action<ILogger, string, TimeSpan, Exception> _stoppedInstance = LoggerMessage.Define<string, TimeSpan>(
            LogLevel.Debug,
            EventIds.StoppedInstance,
            SR.ILoggerExtensions_StoppedFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance is stopping.
        /// </summary>
        private static readonly Action<ILogger, string, TimeSpan, StopInstanceOptions, Exception> _stoppingInstance = LoggerMessage.Define<string, TimeSpan, StopInstanceOptions>(
            LogLevel.Debug,
            EventIds.StoppingInstance,
            SR.ILoggerExtensions_StoppingFormat);

        /// <summary>
        /// Logging delegate for when a temporary SQL LocalDB instance failed to stop.
        /// </summary>
        private static readonly Action<ILogger, string, string, Exception> _stoppingTemporaryInstanceFailed = LoggerMessage.Define<string, string>(
            LogLevel.Error,
            EventIds.StopTemporaryInstanceFailed,
            SR.ILoggerExtensions_StopFailedFormat);

        /// <summary>
        /// Logging delegate for when SQL LocalDB instance is stopped.
        /// </summary>
        private static readonly Action<ILogger, Exception> _stoppedTracing = LoggerMessage.Define(
            LogLevel.Debug,
            EventIds.StoppedTracing,
            SR.ILoggerExtensions_StoppedTracing);

        /// <summary>
        /// Logging delegate for when SQL LocalDB instance is stopping.
        /// </summary>
        private static readonly Action<ILogger, Exception> _stoppingTracing = LoggerMessage.Define(
            LogLevel.Debug,
            EventIds.StoppingTracing,
            SR.ILoggerExtensions_StoppingTracing);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance was unshared.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _unsharedInstance = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.UnsharedInstance,
            SR.ILoggerExtensions_StoppedSharingFormat);

        /// <summary>
        /// Logging delegate for when a SQL LocalDB instance is unshared.
        /// </summary>
        private static readonly Action<ILogger, string, Exception> _unsharingInstance = LoggerMessage.Define<string>(
            LogLevel.Debug,
            EventIds.UnsharingInstance,
            SR.ILoggerExtensions_StoppingSharingFormat);

        /// <summary>
        /// Logs that a SQL LocalDB instance has been created.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that was created.</param>
        /// <param name="version">The LocalDB version the instance that was created with.</param>
        internal static void CreatedInstance(this ILogger logger, string instanceName, string version)
            => _createdInstance(logger, instanceName, version, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance is being created.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that is being created.</param>
        /// <param name="version">The LocalDB version to create the instance with.</param>
        internal static void CreatingInstance(this ILogger logger, string instanceName, string version)
            => _creatingInstance(logger, instanceName, version, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance has been deleted.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that was created.</param>
        internal static void DeletedInstance(this ILogger logger, string instanceName)
            => _deletedInstance(logger, instanceName, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance is being deleted.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that is being deleted.</param>
        internal static void DeletingInstance(this ILogger logger, string instanceName)
            => _deletingInstance(logger, instanceName, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance could not be deleted.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that could not be deleted.</param>
        /// <param name="error">The error code.</param>
        internal static void DeletingInstanceFailed(this ILogger logger, string instanceName, int error)
            => _deletingInstanceFailed(logger, instanceName, error.ToString("X", CultureInfo.InvariantCulture), null);

        /// <summary>
        /// Logs that a SQL LocalDB instance could not be deleted because it is still in use.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="exception">The exception that was thrown.</param>
        /// <param name="instanceName">The name of the instance that could not be deleted.</param>
        internal static void DeletingInstanceFailedAsInUse(this ILogger logger, Exception exception, string instanceName)
            => _deletingInstanceFailedAsInUse(logger, instanceName, exception);

        /// <summary>
        /// Logs that a SQL LocalDB instance could not be deleted because it could not be found.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that could not be deleted.</param>
        internal static void DeletingInstanceFailedAsNotFound(this ILogger logger, string instanceName)
            => _deletingInstanceFailedAsInstanceNotFound(logger, instanceName, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance's files have been deleted.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance whose files were deleted.</param>
        /// <param name="instancePath">The path of the files that were deleted.</param>
        internal static void DeletedInstanceFiles(this ILogger logger, string instanceName, string instancePath)
            => _deletedInstanceFiles(logger, instanceName, instancePath, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance's files are being deleted.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance whose files are being deleted.</param>
        /// <param name="instancePath">The path of the files to be deleted.</param>
        internal static void DeletingInstanceFiles(this ILogger logger, string instanceName, string instancePath)
            => _deletingInstanceFiles(logger, instanceName, instancePath, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance's files could not be deleted.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance whose files could not be deleted.</param>
        /// <param name="instancePath">The path of the files that failed to be deleted.</param>
        internal static void DeletingInstanceFilesFailed(this ILogger logger, string instanceName, string instancePath)
            => _deletingInstanceFilesFailed(logger, instanceName, instancePath, null);

        /// <summary>
        /// Logs that information about a SQL LocalDB instance is being retrieved.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance information is being retrieved for.</param>
        internal static void GettingInstanceInfo(this ILogger logger, string instanceName)
            => _gettingInstanceInfo(logger, instanceName, null);

        /// <summary>
        /// Logs that information about a SQL LocalDB instance was retrieved.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance information was retrieved for.</param>
        internal static void GotInstanceInfo(this ILogger logger, string instanceName)
            => _gotInstanceInfo(logger, instanceName, null);

        /// <summary>
        /// Logs that SQL LocalDB instance names are being retrieved.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        internal static void GettingInstanceNames(this ILogger logger)
            => _gettingInstanceNames(logger, null);

        /// <summary>
        /// Logs that SQL LocalDB instance names were retrieved.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="count">The number of SQL LocalDB instances retrieved.</param>
        internal static void GotInstanceNames(this ILogger logger, int count)
            => _gotInstanceNames(logger, count, null);

        /// <summary>
        /// Logs that information about a SQL LocalDB version is being retrieved.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="version">The version information is being retrieved for.</param>
        internal static void GettingVersionInfo(this ILogger logger, string version)
            => _gettingVersionInfo(logger, version, null);

        /// <summary>
        /// Logs that information about a SQL LocalDB version was retrieved.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="version">The version information was retrieved for.</param>
        internal static void GotVersionInfo(this ILogger logger, string version)
            => _gotVersionInfo(logger, version, null);

        /// <summary>
        /// Logs that SQL LocalDB versions are being retrieved.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        internal static void GettingVersions(this ILogger logger)
            => _gettingVersions(logger, null);

        /// <summary>
        /// Logs that SQL LocalDB versions were retrieved.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="count">The number of SQL LocalDB versions retrieved.</param>
        internal static void GotVersions(this ILogger logger, int count)
            => _gotVersions(logger, count, null);

        /// <summary>
        /// Logs that an invalid Language Id is configured.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="languageId">The version used.</param>
        internal static void InvalidLanguageId(this ILogger logger, int languageId)
            => _invalidLanguageId(logger, languageId, null);

        /// <summary>
        /// Logs that an invalid SQL LocalDB Instance API registry key was found.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="version">The version used.</param>
        internal static void InvalidRegistryKey(this ILogger logger, string version)
            => _invalidRegistryKey(logger, version, null);

        /// <summary>
        /// Logs that a SQL LocalDB Instance API function could not be found.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="functionName">The name of the function that could not be found.</param>
        internal static void NativeApiFunctionNotFound(this ILogger logger, string functionName)
            => _nativeApiFunctionNotFound(logger, functionName, null);

        /// <summary>
        /// Logs that the SQL LocalDB Instance API DLL could not be found at the configured path.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="path">The path to the file that could not be found.</param>
        internal static void NativeApiLibraryNotFound(this ILogger logger, string path)
            => _nativeApiPathNotFound(logger, path, null);

        /// <summary>
        /// Logs that the SQL LocalDB Instance API DLL was loaded.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="fileName">The full path to the DLL that was loaded.</param>
        internal static void NativeApiLoaded(this ILogger logger, string fileName)
            => _nativeApiLoaded(logger, fileName, null);

        /// <summary>
        /// Logs that the SQL LocalDB Instance API DLL could not loaded.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="fileName">The full path to the DLL that could not be loaded.</param>
        /// <param name="error">The error code.</param>
        internal static void NativeApiLoadFailed(this ILogger logger, string fileName, int error)
            => _nativeApiLoadFailed(logger, fileName, error, null);

        /// <summary>
        /// Logs that the SQL LocalDB Instance API could not be found.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        internal static void NativeApiNotFound(this ILogger logger)
            => _nativeApiNotFound(logger, null);

        /// <summary>
        /// Logs that the SQL LocalDB Instance API DLL was not loaded.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        internal static void NativeApiNotLoaded(this ILogger logger)
            => _nativeApiNotLoaded(logger, null);

        /// <summary>
        /// Logs that the SQL LocalDB Instance API DLL was unloaded.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="fileName">The full path to the DLL that was unloaded.</param>
        internal static void NativeApiUnloaded(this ILogger logger, string fileName)
            => _nativeApiUnloaded(logger, fileName, null);

        /// <summary>
        /// Logs that the version of the SQL LocalDB Instance API to use was overridden by the user.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="version">The version used.</param>
        internal static void NativeApiVersionOverriddenByUser(this ILogger logger, Version version)
            => _nativeApiVersionOverriddenByUser(logger, version, null);

        /// <summary>
        /// Logs that the version of the SQL LocalDB Instance API specified by the user could not be found.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="version">The version specified to be used.</param>
        internal static void NativeApiVersionOverrideNotFound(this ILogger logger, string version)
            => _nativeApiVersionOverrideNotFound(logger, version, null);

        /// <summary>
        /// Logs that SQL LocalDB is not installed.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        internal static void NotInstalled(this ILogger logger)
            => _notInstalled(logger, null);

        /// <summary>
        /// Logs that the SQL LocalDB Instance API registry key cannot be found.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="keyName">The name of the registry key that was not found.</param>
        internal static void RegistryKeyNotFound(this ILogger logger, string keyName)
            => _registryKeyNotFound(logger, keyName, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance is being shared.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that is being shared.</param>
        /// <param name="ownerSid">The SID of the instance owner.</param>
        /// <param name="sharedInstanceName">The shared name for the LocalDB instance to share as.</param>
        internal static void SharingInstance(this ILogger logger, string instanceName, string ownerSid, string sharedInstanceName)
            => _sharingInstance(logger, instanceName, ownerSid, sharedInstanceName, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance was shared.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that was shared.</param>
        /// <param name="ownerSid">The SID of the instance owner.</param>
        /// <param name="sharedInstanceName">The shared name for the LocalDB instance to share as.</param>
        internal static void SharedInstance(this ILogger logger, string instanceName, string ownerSid, string sharedInstanceName)
            => _sharedInstance(logger, instanceName, ownerSid, sharedInstanceName, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance was started.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that was started.</param>
        /// <param name="namedPipe">The named pipe the instance is listening on.</param>
        internal static void StartedInstance(this ILogger logger, string instanceName, string namedPipe)
            => _startedInstance(logger, instanceName, namedPipe, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance is starting.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that is starting.</param>
        internal static void StartingInstance(this ILogger logger, string instanceName)
            => _startingInstance(logger, instanceName, null);

        /// <summary>
        /// Logs that SQL LocalDB tracing was started.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        internal static void StartedTracing(this ILogger logger)
            => _startedTracing(logger, null);

        /// <summary>
        /// Logs that SQL LocalDB tracing is starting.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        internal static void StartingTracing(this ILogger logger)
            => _startingTracing(logger, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance was stopped.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that was stopped.</param>
        /// <param name="elapsed">The time taken for the instance to stop.</param>
        internal static void StoppedInstance(this ILogger logger, string instanceName, TimeSpan elapsed)
            => _stoppedInstance(logger, instanceName, elapsed, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance is stopping.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that is stopping.</param>
        /// <param name="timeout">The timeout used.</param>
        /// <param name="options">The stop options used.</param>
        internal static void StoppingInstance(this ILogger logger, string instanceName, TimeSpan timeout, StopInstanceOptions options)
            => _stoppingInstance(logger, instanceName, timeout, options, null);

        /// <summary>
        /// Logs that a temporary SQL LocalDB instance failed to stop.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that failed to stop.</param>
        /// <param name="error">The error code.</param>
        internal static void StoppingTemporaryInstanceFailed(this ILogger logger, string instanceName, int error)
            => _stoppingTemporaryInstanceFailed(logger, instanceName, error.ToString("X", CultureInfo.InvariantCulture), null);

        /// <summary>
        /// Logs that SQL LocalDB tracing was stopped.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        internal static void StoppedTracing(this ILogger logger)
            => _stoppedTracing(logger, null);

        /// <summary>
        /// Logs that SQL LocalDB tracing is stopping.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        internal static void StoppingTracing(this ILogger logger)
            => _stoppingTracing(logger, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance is being unshared.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that is being shared.</param>
        internal static void UnsharingInstance(this ILogger logger, string instanceName)
            => _unsharingInstance(logger, instanceName, null);

        /// <summary>
        /// Logs that a SQL LocalDB instance was shared.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="instanceName">The name of the instance that was shared.</param>
        internal static void UnsharedInstance(this ILogger logger, string instanceName)
            => _unsharedInstance(logger, instanceName, null);
    }
}
