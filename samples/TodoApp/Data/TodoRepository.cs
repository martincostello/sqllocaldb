// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.EntityFrameworkCore;

namespace TodoApp.Data;

/// <summary>
/// A class representing a repository of TODO items. This class cannot be inherited.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="TodoRepository"/> class.
/// </remarks>
/// <param name="timeProvider">The <see cref="TimeProvider"/> to use.</param>
/// <param name="context">The <see cref="TodoContext"/> to use.</param>
public sealed class TodoRepository(TimeProvider timeProvider, TodoContext context) : ITodoRepository
{
    /// <inheritdoc />
    public async Task<TodoItem> AddItemAsync(string text, CancellationToken cancellationToken = default)
    {
        var item = new TodoItem()
        {
            CreatedAt = Now(),
            Text = text,
        };

        context.Add(item);

        await context.SaveChangesAsync(cancellationToken);

        return item;
    }

    /// <inheritdoc />
    public async Task<bool?> CompleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        TodoItem? item = await context.Items!.FindAsync(new object[] { id }, cancellationToken);

        if (item is null)
        {
            return null;
        }

        if (item.CompletedAt.HasValue)
        {
            return false;
        }

        item.CompletedAt = Now();

        context.Items.Update(item);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<bool> DeleteItemAsync(Guid id, CancellationToken cancellationToken = default)
    {
        TodoItem? item = await context.Items!.FindAsync([id], cancellationToken);

        if (item is null)
        {
            return false;
        }

        context.Items.Remove(item);

        await context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <inheritdoc />
    public async Task<IList<TodoItem>> GetItemsAsync(CancellationToken cancellationToken = default)
    {
        return await context.Items!
            .OrderBy((p) => p.CompletedAt.HasValue)
            .ThenBy((p) => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Returns the current date and time.
    /// </summary>
    /// <returns>
    /// The <see cref="DateTimeOffset"/> for the current date and time.
    /// </returns>
    private DateTimeOffset Now() => timeProvider.GetUtcNow();
}
