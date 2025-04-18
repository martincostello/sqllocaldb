// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace MartinCostello.SqlLocalDb;

public class ISqlLocalDbInstanceManagerExtensionsTests(ITestOutputHelper outputHelper)
{
    private readonly ILoggerFactory _loggerFactory = outputHelper.ToLoggerFactory();

    [Fact]
    public void CreateConnection_Throws_If_Manager_Is_Null()
    {
        // Arrange
        ISqlLocalDbInstanceManager? manager = null;

        // Act and Assert
        Assert.Throws<ArgumentNullException>("manager", () => manager!.CreateConnection());
    }

    [WindowsOnlyFact]
    public async Task CreateConnection_Creates_A_Sql_Connection()
    {
        // Arrange
        using var api = new SqlLocalDbApi(_loggerFactory);
        using TemporarySqlLocalDbInstance temporary = api.CreateTemporaryInstance(deleteFiles: true);

        ISqlLocalDbInstanceManager manager = temporary.Manage();

        manager.ShouldNotBeNull();
        manager.Name.ShouldBe(temporary.Name);

        // Act
        using SqlConnection actual = manager.CreateConnection();

        // Assert
        actual.ShouldNotBeNull();
        actual.ConnectionString.ShouldNotBeNull();
        actual.State.ShouldBe(ConnectionState.Closed);

        await actual.OpenAsync();

#if NET
        await actual.CloseAsync();
#else
        actual.Close();
#endif
    }

    [Fact]
    public void Restart_Throws_If_Manager_Is_Null()
    {
        // Arrange
        ISqlLocalDbInstanceManager? manager = null;

        // Act and Assert
        Assert.Throws<ArgumentNullException>("manager", () => manager!.Restart());
    }

    [WindowsOnlyFact]
    public void Restart_Stops_And_Starts_Instance()
    {
        // Arrange
        using var api = new SqlLocalDbApi(_loggerFactory);
        using TemporarySqlLocalDbInstance temporary = api.CreateTemporaryInstance(deleteFiles: true);

        ISqlLocalDbInstanceManager manager = temporary.Manage();

        // Act
        manager.Restart();

        // Assert
        temporary.GetInstanceInfo().IsRunning.ShouldBeTrue();
    }
}
