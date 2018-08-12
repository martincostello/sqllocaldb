// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class that contains examples for using <c>MartinCostello.SqlLocalDb</c>.
    /// </summary>
    public static class Examples
    {
        [Fact]
        public static void Create_A_Sql_LocalDB_Instance()
        {
            using (var localDB = new SqlLocalDbApi())
            {
                ISqlLocalDbInstanceInfo instance = localDB.GetOrCreateInstance("MyInstance");
                ISqlLocalDbInstanceManager manager = instance.Manage();

                if (!instance.IsRunning)
                {
                    manager.Start();
                }

                using (SqlConnection connection = instance.CreateConnection())
                {
                    connection.Open();

                    // Use the SQL connection...
                }

                manager.Stop();
            }
        }

        [Fact]
        public static void Create_A_Temporary_Sql_LocalDB_Instance()
        {
            using (var localDB = new SqlLocalDbApi())
            {
                using (TemporarySqlLocalDbInstance instance = localDB.CreateTemporaryInstance(deleteFiles: true))
                {
                    using (var connection = new SqlConnection(instance.ConnectionString))
                    {
                        connection.Open();

                        // Use the SQL connection...
                    }
                }
            }
        }

        [Fact]
        public static void Use_With_Dependency_Injection()
        {
            // Register with SQL LocalDB services
            var services = new ServiceCollection()
                .AddLogging()
                .AddSqlLocalDB();

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                ISqlLocalDbApi localDB = scope.ServiceProvider.GetService<ISqlLocalDbApi>();
                ISqlLocalDbInstanceInfo instance = localDB.GetDefaultInstance();
                ISqlLocalDbInstanceManager manager = instance.Manage();

                if (!instance.IsRunning)
                {
                    manager.Start();
                }

                using (SqlConnection connection = instance.CreateConnection())
                {
                    connection.Open();

                    // Use the SQL connection...
                }
            }
        }
    }
}
