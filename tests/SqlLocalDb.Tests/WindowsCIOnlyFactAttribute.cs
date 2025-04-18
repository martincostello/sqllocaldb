﻿// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// Attribute that is applied to a method to indicate that it is a fact that should be run by the
/// test runner if the current operating system is Windows. This class cannot be inherited.
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public sealed class WindowsCIOnlyFactAttribute : FactAttribute
{
    public WindowsCIOnlyFactAttribute()
        : base()
    {
        bool isWindowsCI =
#if NET
            OperatingSystem.IsWindows() &&
#endif
            !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("CI"));

        Skip = isWindowsCI ? null : "This test can only be run on Windows CI.";
    }
}
