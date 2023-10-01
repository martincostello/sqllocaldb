// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace TodoApp.Data;

/// <summary>
/// A class representing the initializer for the TodoApp database context.
/// </summary>
public static class TodoInitializer
{
    /// <summary>
    /// Initializes the database for TodoApp.
    /// </summary>
    /// <param name="serviceProvider">The <see cref="IServiceProvider"/> to use.</param>
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope!.ServiceProvider!.GetService<TodoContext>();
        context!.Database.Migrate();
    }
}
