﻿// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Xunit.Sdk;

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// Attribute that is applied to a method to indicate that it is a fact that should be run by the
/// test runner if the current operating system is Windows. This class cannot be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
[XunitTestCaseDiscoverer("MartinCostello.SqlLocalDb.RetryFactDiscoverer", "MartinCostello.SqlLocalDb.Tests")]
public sealed class WindowsOnlyFactAttribute : FactAttribute
{
    public WindowsOnlyFactAttribute()
        : base()
    {
        Skip = OperatingSystem.IsWindows() ? string.Empty : "This test can only be run on Windows.";
    }

    /// <summary>
    /// Gets or sets the number of retries allowed for a failed test.
    /// If unset (or set less than 1), will default to 3 attempts.
    /// </summary>
    public int MaxRetries { get; set; }
}
