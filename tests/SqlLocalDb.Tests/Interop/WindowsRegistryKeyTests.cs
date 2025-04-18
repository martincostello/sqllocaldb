// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.SqlLocalDb.Interop;

public static class WindowsRegistryKeyTests
{
    [WindowsOnlyFact]
#if NET
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
#endif
    public static void OpenSubKey_Returns_Null_If_Key_Not_Found()
    {
        // Arrange
        var registry = new WindowsRegistry();
        using var target = registry.OpenSubKey("SOFTWARE");
        target.ShouldNotBeNull();

        // Act
        using var actual = target.OpenSubKey(Guid.NewGuid().ToString());

        // Assert
        actual.ShouldBeNull();
    }
}
