// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace TodoApp.Data;

/// <summary>
/// A class representing the database context for TodoApp.
/// </summary>
public class TodoContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TodoContext"/> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    public TodoContext(DbContextOptions<TodoContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Gets or sets the database set containing the Todo items.
    /// </summary>
    public DbSet<TodoItem>? Items { get; set; }
}
