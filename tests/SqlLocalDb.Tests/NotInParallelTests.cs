// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// A class containing tests that perform global operations that may cause other tests to fail if run in parallel.
/// </summary>
[CollectionDefinition("NotInParallel", DisableParallelization = true)]
public class NotInParallelTests(ITestOutputHelper outputHelper)
{
    private readonly ILoggerFactory _loggerFactory = outputHelper.ToLoggerFactory();

    [WindowsCIOnlyFact]
    public void Can_Delete_User_Instances()
    {
        // Arrange
        using var actual = new SqlLocalDbApi(_loggerFactory);
        actual.CreateInstance(Guid.NewGuid().ToString());

        IReadOnlyList<string> namesBefore = actual.GetInstanceNames();

        // Act
        int deleted = actual.DeleteUserInstances(deleteFiles: true);

        // Assert
        deleted.ShouldBeGreaterThanOrEqualTo(1);

        IReadOnlyList<string> namesAfter = actual.GetInstanceNames();

        int instancesDeleted = 0;

        foreach (string name in namesBefore)
        {
            if (!namesAfter.Contains(name))
            {
                instancesDeleted++;
            }
        }

        instancesDeleted.ShouldBeGreaterThanOrEqualTo(1);
    }
}
