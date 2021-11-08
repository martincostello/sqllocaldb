// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// A class containing trace event Ids.  This class cannot be inherited.
/// </summary>
internal static class EventIds
{
    //// Add new events to the end of the class to preserve the Ids.

    /// <summary>
    /// The <see cref="EventId"/> for when the SQL LocalDB Instance API is loaded. This field is read-only.
    /// </summary>
    internal static readonly EventId NativeApiLoaded = new(++Id, nameof(NativeApiLoaded));

    /// <summary>
    /// The <see cref="EventId"/> for when the SQL LocalDB Instance API fails to load. This field is read-only.
    /// </summary>
    internal static readonly EventId NativeApiLoadFailed = new(++Id, nameof(NativeApiLoadFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when the SQL LocalDB Instance API is not loaded. This field is read-only.
    /// </summary>
    internal static readonly EventId NativeApiNotLoaded = new(++Id, nameof(NativeApiNotLoaded));

    /// <summary>
    /// The <see cref="EventId"/> for when the SQL LocalDB Instance API version is overridden. This field is read-only.
    /// </summary>
    internal static readonly EventId NativeApiVersionOverriddenByUser = new(++Id, nameof(NativeApiVersionOverriddenByUser));

    /// <summary>
    /// The <see cref="EventId"/> for when the SQL LocalDB Instance API version specified as an override cannot be found. This field is read-only.
    /// </summary>
    internal static readonly EventId NativeApiVersionOverrideNotFound = new(++Id, nameof(NativeApiVersionOverrideNotFound));

    /// <summary>
    /// The <see cref="EventId"/> for when the SQL LocalDB Instance API cannot be found. This field is read-only.
    /// </summary>
    internal static readonly EventId NoNativeApiFound = new(++Id, nameof(NoNativeApiFound));

    /// <summary>
    /// The <see cref="EventId"/> for when the SQL LocalDB Instance API path configured in the registry cannot be found. This field is read-only.
    /// </summary>
    internal static readonly EventId NativeApiPathNotFound = new(++Id, nameof(NativeApiPathNotFound));

    /// <summary>
    /// The <see cref="EventId"/> for when a native function export from the SQL LocalDB Instance API cannot be found. This field is read-only.
    /// </summary>
    internal static readonly EventId NativeFunctionNotFound = new(++Id, nameof(NativeFunctionNotFound));

    /// <summary>
    /// The <see cref="EventId"/> for when the SQL LocalDB Instance API is not installed. This field is read-only.
    /// </summary>
    internal static readonly EventId NotInstalled = new(++Id, nameof(NotInstalled));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance is being created. This field is read-only.
    /// </summary>
    internal static readonly EventId CreatingInstance = new(++Id, nameof(CreatingInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when creating a SQL LocalDB instance fails. This field is read-only.
    /// </summary>
    internal static readonly EventId CreatingInstanceFailed = new(++Id, nameof(CreatingInstanceFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance has been created. This field is read-only.
    /// </summary>
    internal static readonly EventId CreatedInstance = new(++Id, nameof(CreatedInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance is being deleted. This field is read-only.
    /// </summary>
    internal static readonly EventId DeletingInstance = new(++Id, nameof(DeletingInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when deleting a SQL LocalDB instance fails. This field is read-only.
    /// </summary>
    internal static readonly EventId DeletingInstanceFailed = new(++Id, nameof(DeletingInstanceFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when deleting a SQL LocalDB instance fails because it cannot be found. This field is read-only.
    /// </summary>
    internal static readonly EventId DeletingInstanceFailedAsCannotBeNotFound = new(++Id, nameof(DeletingInstanceFailedAsCannotBeNotFound));

    /// <summary>
    /// The <see cref="EventId"/> for when deleting a SQL LocalDB instance fails because it is in use. This field is read-only.
    /// </summary>
    internal static readonly EventId DeletingInstanceFailedAsInUse = new(++Id, nameof(DeletingInstanceFailedAsInUse));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance has been deleted. This field is read-only.
    /// </summary>
    internal static readonly EventId DeletedInstance = new(++Id, nameof(DeletedInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when the files for a SQL LocalDB instance are being deleted. This field is read-only.
    /// </summary>
    internal static readonly EventId DeletingInstanceFiles = new(++Id, nameof(DeletingInstanceFiles));

    /// <summary>
    /// The <see cref="EventId"/> for when the files for a SQL LocalDB instance fail to be deleted. This field is read-only.
    /// </summary>
    internal static readonly EventId DeletingInstanceFilesFailed = new(++Id, nameof(DeletingInstanceFilesFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when the files for a SQL LocalDB instance have been deleted. This field is read-only.
    /// </summary>
    internal static readonly EventId DeletedInstanceFiles = new(++Id, nameof(DeletedInstanceFiles));

    /// <summary>
    /// The <see cref="EventId"/> for when getting information about a SQL LocalDB instance. This field is read-only.
    /// </summary>
    internal static readonly EventId GettingInstanceInfo = new(++Id, nameof(GettingInstanceInfo));

    /// <summary>
    /// The <see cref="EventId"/> for when getting information about a SQL LocalDB instance fails. This field is read-only.
    /// </summary>
    internal static readonly EventId GettingInstanceInfoFailed = new(++Id, nameof(GettingInstanceInfoFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when information about a SQL LocalDB instance was retrieved. This field is read-only.
    /// </summary>
    internal static readonly EventId GotInstanceInfo = new(++Id, nameof(GotInstanceInfo));

    /// <summary>
    /// The <see cref="EventId"/> for when getting SQL LocalDB instance names. This field is read-only.
    /// </summary>
    internal static readonly EventId GettingInstanceNames = new(++Id, nameof(GettingInstanceNames));

    /// <summary>
    /// The <see cref="EventId"/> for when getting SQL LocalDB instance names fails. This field is read-only.
    /// </summary>
    internal static readonly EventId GettingInstanceNamesFailed = new(++Id, nameof(GettingInstanceNamesFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when SQL LocalDB instance names are retrieved. This field is read-only.
    /// </summary>
    internal static readonly EventId GotInstanceNames = new(++Id, nameof(GotInstanceNames));

    /// <summary>
    /// The <see cref="EventId"/> for when getting information about a SQL LocalDB version. This field is read-only.
    /// </summary>
    internal static readonly EventId GettingVersionInfo = new(++Id, nameof(GettingVersionInfo));

    /// <summary>
    /// The <see cref="EventId"/> for when getting information about a SQL LocalDB version fails. This field is read-only.
    /// </summary>
    internal static readonly EventId GettingVersionInfoFailed = new(++Id, nameof(GettingVersionInfoFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when information about a SQL LocalDB version was retrieved. This field is read-only.
    /// </summary>
    internal static readonly EventId GotVersionInfo = new(++Id, nameof(GotVersionInfo));

    /// <summary>
    /// The <see cref="EventId"/> for when getting SQL LocalDB versions. This field is read-only.
    /// </summary>
    internal static readonly EventId GettingVersions = new(++Id, nameof(GettingVersions));

    /// <summary>
    /// The <see cref="EventId"/> for when getting SQL LocalDB versions fails. This field is read-only.
    /// </summary>
    internal static readonly EventId GettingVersionsFailed = new(++Id, nameof(GettingVersionsFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when SQL LocalDB instance versions are retrieved. This field is read-only.
    /// </summary>
    internal static readonly EventId GotVersions = new(++Id, nameof(GotVersions));

    /// <summary>
    /// The <see cref="EventId"/> for when a specified Language Id is invalid. This field is read-only.
    /// </summary>
    internal static readonly EventId InvalidLanguageId = new(++Id, nameof(InvalidLanguageId));

    /// <summary>
    /// The <see cref="EventId"/> for when a specified registry key name is invalid. This field is read-only.
    /// </summary>
    internal static readonly EventId InvalidRegistryKey = new(++Id, nameof(InvalidRegistryKey));

    /// <summary>
    /// The <see cref="EventId"/> for when a registry key cannot be found. This field is read-only.
    /// </summary>
    internal static readonly EventId RegistryKeyNotFound = new(++Id, nameof(RegistryKeyNotFound));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance is starting. This field is read-only.
    /// </summary>
    internal static readonly EventId StartingInstance = new(++Id, nameof(StartingInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance fails to start. This field is read-only.
    /// </summary>
    internal static readonly EventId StartingInstanceFailed = new(++Id, nameof(StartingInstanceFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance has started. This field is read-only.
    /// </summary>
    internal static readonly EventId StartedInstance = new(++Id, nameof(StartedInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance is stopping. This field is read-only.
    /// </summary>
    internal static readonly EventId StoppingInstance = new(++Id, nameof(StoppingInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance fails to stop. This field is read-only.
    /// </summary>
    internal static readonly EventId StoppingInstanceFailed = new(++Id, nameof(StoppingInstanceFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance has stopped. This field is read-only.
    /// </summary>
    internal static readonly EventId StoppedInstance = new(++Id, nameof(StoppedInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when SQL LocalDB API tracing is starting. This field is read-only.
    /// </summary>
    internal static readonly EventId StartingTracing = new(++Id, nameof(StartingTracing));

    /// <summary>
    /// The <see cref="EventId"/> for when tracing for SQL LocalDB API fails to start. This field is read-only.
    /// </summary>
    internal static readonly EventId StartingTracingFailed = new(++Id, nameof(StartingTracingFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when SQL LocalDB API tracing is started. This field is read-only.
    /// </summary>
    internal static readonly EventId StartedTracing = new(++Id, nameof(StartedTracing));

    /// <summary>
    /// The <see cref="EventId"/> for when SQL LocalDB API tracing is stopping. This field is read-only.
    /// </summary>
    internal static readonly EventId StoppedTracing = new(++Id, nameof(StoppedTracing));

    /// <summary>
    /// The <see cref="EventId"/> for when tracing for SQL LocalDB API fails to stop. This field is read-only.
    /// </summary>
    internal static readonly EventId StoppingTracingFailed = new(++Id, nameof(StoppingTracingFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when SQL LocalDB API tracing is stopped. This field is read-only.
    /// </summary>
    internal static readonly EventId StoppingTracing = new(++Id, nameof(StoppingTracing));

    /// <summary>
    /// The <see cref="EventId"/> for when a temporary SQL LocalDB API instance fails to stop. This field is read-only.
    /// </summary>
    internal static readonly EventId StopTemporaryInstanceFailed = new(++Id, nameof(StopTemporaryInstanceFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance is being shared. This field is read-only.
    /// </summary>
    internal static readonly EventId SharingInstance = new(++Id, nameof(SharingInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when sharing a SQL LocalDB instance fails. This field is read-only.
    /// </summary>
    internal static readonly EventId SharingInstanceFailed = new(++Id, nameof(SharingInstanceFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance has been shared. This field is read-only.
    /// </summary>
    internal static readonly EventId SharedInstance = new(++Id, nameof(SharedInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance is being unshared. This field is read-only.
    /// </summary>
    internal static readonly EventId UnsharingInstance = new(++Id, nameof(UnsharingInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when unsharing a SQL LocalDB instance fails. This field is read-only.
    /// </summary>
    internal static readonly EventId UnsharingInstanceFailed = new(++Id, nameof(UnsharingInstanceFailed));

    /// <summary>
    /// The <see cref="EventId"/> for when a SQL LocalDB instance has been unshared. This field is read-only.
    /// </summary>
    internal static readonly EventId UnsharedInstance = new(++Id, nameof(UnsharedInstance));

    /// <summary>
    /// The <see cref="EventId"/> for when the SQL LocalDB Instance API is unloaded. This field is read-only.
    /// </summary>
    internal static readonly EventId NativeApiUnloaded = new(++Id, nameof(NativeApiUnloaded));

    /// <summary>
    /// The <see cref="EventId"/> for when the SQL LocalDB Instance API returns a generic error. This field is read-only.
    /// </summary>
    internal static readonly EventId GenericError = new(++Id, nameof(GenericError));

    /// <summary>
    /// The base Id for the event Ids.
    /// </summary>
    private const int BaseId = 0;

    /// <summary>
    /// Gets or sets the current Id for assigning event Ids.
    /// </summary>
    private static int Id { get; set; } = BaseId;
}
