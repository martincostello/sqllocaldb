// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MartinCostello.SqlLocalDb;

/// <summary>
/// A class that contains examples for using <c>MartinCostello.SqlLocalDb</c>.
/// </summary>
public class Examples
{
    public Examples(ITestOutputHelper outputHelper)
    {
        OutputHelper = outputHelper;
    }

    private ITestOutputHelper OutputHelper { get; }

    [WindowsOnlyFact]
    public async Task Create_A_Sql_LocalDB_Instance()
    {
        using var localDB = new SqlLocalDbApi(OutputHelper.ToLoggerFactory());

        ISqlLocalDbInstanceInfo instance = localDB.GetOrCreateInstance("MyInstance");
        ISqlLocalDbInstanceManager manager = instance.Manage();

        if (!instance.IsRunning)
        {
            manager.Start();
        }

        await using (SqlConnection connection = instance.CreateConnection())
        {
            connection.Open();

            // Use the SQL connection...
        }

        manager.Stop();
    }

    [WindowsOnlyFact]
    public async Task Create_A_Temporary_Sql_LocalDB_Instance()
    {
        using var localDB = new SqlLocalDbApi(OutputHelper.ToLoggerFactory());
        using TemporarySqlLocalDbInstance instance = localDB.CreateTemporaryInstance(deleteFiles: true);

        await using var connection = new SqlConnection(instance.ConnectionString);
        connection.Open();

        // Use the SQL connection...
    }

    [WindowsOnlyFact]
    public async Task Use_With_Dependency_Injection()
    {
        // Register with SQL LocalDB services
        var services = new ServiceCollection()
            .AddLogging((builder) => builder.AddXUnit(OutputHelper))
            .AddSqlLocalDB();

        IServiceProvider serviceProvider = services.BuildServiceProvider();

        await using AsyncServiceScope scope = serviceProvider.CreateAsyncScope();

        ISqlLocalDbApi localDB = scope!.ServiceProvider!.GetRequiredService<ISqlLocalDbApi>();
        ISqlLocalDbInstanceInfo instance = localDB!.GetDefaultInstance();
        ISqlLocalDbInstanceManager manager = instance.Manage();

        if (!instance.IsRunning)
        {
            manager.Start();
        }

        await using SqlConnection connection = instance.CreateConnection();
        connection.Open();

        // Use the SQL connection...
    }
}
