// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Security.Principal;
using Xunit.Sdk;

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// Attribute that is applied to a method to indicate that it is a fact that should be run by the test runner
/// if the user account running the test has administrative privileges. This class cannot be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("MartinCostello.SqlLocalDb.RetryFactDiscoverer", "MartinCostello.SqlLocalDb.Tests")]
public sealed class RunAsAdminFactAttribute : FactAttribute
{
    public RunAsAdminFactAttribute()
        : base()
    {
        Skip = IsCurrentUserAdmin(out string name) ? string.Empty : $"The current user '{name}' does not have administrative privileges.";
    }

    /// <summary>
    /// Gets or sets the number of retries allowed for a failed test.
    /// If unset (or set less than 1), will default to 3 attempts.
    /// </summary>
    public int MaxRetries { get; set; }

    /// <summary>
    /// Returns whether the current user has Administrative privileges.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the current user has Administrative
    /// privileges; otherwise <see langword="false"/>.
    /// </returns>
    internal static bool IsCurrentUserAdmin() => IsCurrentUserAdmin(out string _);

    /// <summary>
    /// Returns whether the current user has Administrative privileges.
    /// </summary>
    /// <param name="name">When the method returns, contains the name of the current user.</param>
    /// <returns>
    /// <see langword="true"/> if the current user has Administrative
    /// privileges; otherwise <see langword="false"/>.
    /// </returns>
    private static bool IsCurrentUserAdmin(out string name)
    {
        if (!OperatingSystem.IsWindows())
        {
            name = Environment.UserName;
            return false;
        }

        using (var identity = WindowsIdentity.GetCurrent())
        {
            name = identity.Name;
            return new WindowsPrincipal(identity).IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
