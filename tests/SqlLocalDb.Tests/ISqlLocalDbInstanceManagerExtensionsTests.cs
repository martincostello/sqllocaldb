// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace MartinCostello.SqlLocalDb
{
    public class ISqlLocalDbInstanceManagerExtensionsTests
    {
        private readonly ILoggerFactory _loggerFactory;

        public ISqlLocalDbInstanceManagerExtensionsTests(ITestOutputHelper outputHelper)
        {
            _loggerFactory = outputHelper.AsLoggerFactory();
        }

        [Fact]
        public void CreateConnection_Throws_If_Manager_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstanceManager manager = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("manager", () => manager.CreateConnection());
        }

        [Fact]
        public async Task CreateConnection_Creates_A_Sql_Connection()
        {
            // Arrange
            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                using (TemporarySqlLocalDbInstance temporary = api.CreateTemporaryInstance(deleteFiles: true))
                {
                    ISqlLocalDbInstanceManager manager = temporary.Manage();

                    manager.ShouldNotBeNull();
                    manager.Name.ShouldBe(temporary.Name);

                    // Act
                    using (SqlConnection actual = manager.CreateConnection())
                    {
                        // Assert
                        actual.ShouldNotBeNull();
                        actual.ConnectionString.ShouldNotBeNull();
                        actual.State.ShouldBe(ConnectionState.Closed);

                        await actual.OpenAsync();
                        actual.Close();
                    }
                }
            }
        }

        [Fact]
        public void Restart_Throws_If_Manager_Is_Null()
        {
            // Arrange
            ISqlLocalDbInstanceManager manager = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("manager", () => manager.Restart());
        }

        [Fact]
        public void Restart_Stops_And_Starts_Instance()
        {
            // Arrange
            using (var api = new SqlLocalDbApi(_loggerFactory))
            {
                using (TemporarySqlLocalDbInstance temporary = api.CreateTemporaryInstance(deleteFiles: true))
                {
                    ISqlLocalDbInstanceManager manager = temporary.Manage();

                    // Act
                    manager.Restart();

                    // Assert
                    temporary.GetInstanceInfo().IsRunning.ShouldBeTrue();
                }
            }
        }
    }
}
