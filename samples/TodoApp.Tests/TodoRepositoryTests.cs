// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MartinCostello.SqlLocalDb;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NodaTime;
using NodaTime.Testing;
using TodoApp.Data;
using Xunit;

namespace TodoApp.Tests
{
    public class TodoRepositoryTests
    {
        [Fact]
        public async Task Can_Create_Update_And_Delete_Todo_Items()
        {
            // Arrange
            var now = new DateTimeOffset(2018, 08, 12, 10, 41, 0, TimeSpan.Zero);
            var clock = new FakeClock(Instant.FromDateTimeOffset(now));

            var loggerFactory = new LoggerFactory();
            var options = new SqlLocalDbOptions()
            {
                AutomaticallyDeleteInstanceFiles = true,
                StopOptions = StopInstanceOptions.NoWait,
                StopTimeout = TimeSpan.FromSeconds(1),
            };

            using (var localDB = new SqlLocalDbApi(options, loggerFactory))
            {
                using (TemporarySqlLocalDbInstance instance = localDB.CreateTemporaryInstance(deleteFiles: true))
                {
                    var builder = new DbContextOptionsBuilder<TodoContext>()
                        .UseSqlServer(instance.ConnectionString);

                    using (var context = new TodoContext(builder.Options))
                    {
                        context.Database.EnsureCreated();
                        context.Database.Migrate();

                        var target = new TodoRepository(clock, context);

                        // Act - Verify the repository is empty
                        IList<TodoItem> items = await target.GetItemsAsync();

                        // Assert
                        Assert.NotNull(items);
                        Assert.Empty(items);

                        // Arrange - Add a new item
                        string text = "Buy cheese";

                        // Act
                        TodoItem item = await target.AddItemAsync(text);

                        // Assert
                        Assert.NotNull(item);
                        Assert.NotEmpty(item.Id);
                        Assert.Equal(text, item.Text);
                        Assert.Equal(now, item.CreatedAt);
                        Assert.Null(item.CompletedAt);

                        // Arrange - Mark the item as completed
                        string id = item.Id;

                        // Act
                        bool? completeResult = await target.CompleteItemAsync(id);

                        // Assert
                        Assert.True(completeResult);

                        // Act - Verify the repository has one item that is completed
                        items = await target.GetItemsAsync();

                        // Assert
                        Assert.NotNull(items);
                        Assert.NotEmpty(items);
                        Assert.Equal(1, items.Count);

                        item = items[0];
                        Assert.NotNull(item);
                        Assert.NotEmpty(item.Id);
                        Assert.Equal(text, item.Text);
                        Assert.Equal(now, item.CreatedAt);
                        Assert.Equal(now, item.CompletedAt);

                        // Act - Delete the item
                        bool deleteResult = await target.DeleteItemAsync(id);

                        // Assert
                        Assert.True(deleteResult);

                        // Act - Verify the repository is empty again
                        items = await target.GetItemsAsync();

                        // Assert
                        Assert.NotNull(items);
                        Assert.Empty(items);
                    }
                }
            }
        }
    }
}
