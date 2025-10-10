// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Diagnostics;

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// A class that can be used to manage instances of SQL LocalDB. This class cannot be inherited.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="SqlLocalDbInstanceManager"/> class.
/// </remarks>
/// <param name="instance">The SQL Server LocalDB instance to manage.</param>
/// <param name="api">The <see cref="ISqlLocalDbApi"/> instance to use.</param>
/// <exception cref="ArgumentNullException">
/// <paramref name="instance"/> or <paramref name="api"/> is <see langword="null"/>.
/// </exception>
[DebuggerDisplay("{Name}")]
public sealed class SqlLocalDbInstanceManager(
    ISqlLocalDbInstanceInfo instance,
    ISqlLocalDbApi api) : ISqlLocalDbInstanceManager, ISqlLocalDbApiAdapter
{
    /// <inheritdoc />
    public string Name => Instance.Name;

    /// <inheritdoc />
    public string NamedPipe => Instance.NamedPipe;

    /// <inheritdoc />
    ISqlLocalDbApi ISqlLocalDbApiAdapter.LocalDb => Api;

    /// <summary>
    /// Gets the <see cref="ISqlLocalDbApi"/> to use.
    /// </summary>
    private ISqlLocalDbApi Api { get; } = api ?? throw new ArgumentNullException(nameof(api));

    /// <summary>
    /// Gets the <see cref="ISqlLocalDbInstanceInfo"/> in use.
    /// </summary>
    private ISqlLocalDbInstanceInfo Instance { get; } = instance ?? throw new ArgumentNullException(nameof(instance));

    /// <summary>
    /// Gets the current state of the instance.
    /// </summary>
    /// <returns>
    /// An <see cref="ISqlLocalDbInstanceInfo"/> representing the current state of the instance being managed.
    /// </returns>
    public ISqlLocalDbInstanceInfo GetInstanceInfo() => Api.GetInstanceInfo(Name);

    /// <summary>
    /// Shares the LocalDB instance using the specified name.
    /// </summary>
    /// <param name="sharedName">The name to use to share the instance.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="sharedName"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="SqlLocalDbException">
    /// The LocalDB instance could not be shared.
    /// </exception>
    public void Share(string sharedName)
    {
        ArgumentNullException.ThrowIfNull(sharedName);

        try
        {
            Api.ShareInstance(Name, sharedName);
            UpdateState();
        }
        catch (SqlLocalDbException ex)
        {
            throw new SqlLocalDbException(
                SRHelper.Format(SR.SqlLocalDbInstanceManager_ShareFailedFormat, Name),
                ex.ErrorCode,
                ex.InstanceName,
                ex);
        }
    }

    /// <summary>
    /// Starts the SQL Server LocalDB instance.
    /// </summary>
    /// <exception cref="SqlLocalDbException">
    /// The LocalDB instance could not be started.
    /// </exception>
    public void Start()
    {
        try
        {
            Api.StartInstance(Name);
            UpdateState();
        }
        catch (SqlLocalDbException ex)
        {
            throw new SqlLocalDbException(
                SRHelper.Format(SR.SqlLocalDbInstanceManager_StartFailedFormat, Name),
                ex.ErrorCode,
                ex.InstanceName,
                ex);
        }
    }

    /// <summary>
    /// Stops the SQL Server LocalDB instance.
    /// </summary>
    /// <exception cref="SqlLocalDbException">
    /// The LocalDB instance could not be stopped.
    /// </exception>
    public void Stop()
    {
        try
        {
            Api.StopInstance(Name, null);
            UpdateState();
        }
        catch (SqlLocalDbException ex)
        {
            throw new SqlLocalDbException(
                SRHelper.Format(SR.SqlLocalDbInstanceManager_StopFailedFormat, Name),
                ex.ErrorCode,
                ex.InstanceName,
                ex);
        }
    }

    /// <summary>
    /// Stops sharing the LocalDB instance.
    /// </summary>
    /// <exception cref="SqlLocalDbException">
    /// The LocalDB instance could not be unshared.
    /// </exception>
    public void Unshare()
    {
        try
        {
            Api.UnshareInstance(Name);
            UpdateState();
        }
        catch (SqlLocalDbException ex)
        {
            throw new SqlLocalDbException(
                SRHelper.Format(SR.SqlLocalDbInstanceManager_UnshareFailedFormat, Name),
                ex.ErrorCode,
                ex.InstanceName,
                ex);
        }
    }

    /// <summary>
    /// Updates the state of <see cref="Instance"/>, if possible.
    /// </summary>
    private void UpdateState()
    {
        if (Instance is SqlLocalDbInstanceInfo info)
        {
            info.Update(Api.GetInstanceInfo(info.Name));
        }
    }
}
