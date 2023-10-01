// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace TodoApp.Data;

/// <summary>
/// A class representing the database context for TodoApp.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TodoContext"/> class.
/// </remarks>
/// <param name="options">The options for this context.</param>
public class TodoContext(DbContextOptions<TodoContext> options) : DbContext(options)
{
    /// <summary>
    /// Gets or sets the database set containing the Todo items.
    /// </summary>
    public DbSet<TodoItem>? Items { get; set; }
}
