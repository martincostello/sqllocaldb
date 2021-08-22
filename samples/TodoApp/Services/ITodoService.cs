// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using TodoApp.Models;

namespace TodoApp.Services;

/// <summary>
/// Defines a service for managing TODO items.
/// </summary>
public interface ITodoService
{
    /// <summary>
    /// Adds a new item as an asynchronous operation.
    /// </summary>
    /// <param name="text">The text of the item to add.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation to add the new item.
    /// </returns>
    Task AddItemAsync(string text, CancellationToken cancellationToken);

    /// <summary>
    /// Marks an item as completed as an asynchronous operation.
    /// </summary>
    /// <param name="id">The id of the item to mark as completed.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation
    /// to complete the item that <see langword="true"/> if it was completed,
    /// <see langword="false"/> if it was already completed, or <see langword="null"/>
    /// if an item with the specified Id cannot be found.
    /// </returns>
    Task<bool?> CompleteItemAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Deletes an item as an asynchronous operation.
    /// </summary>
    /// <param name="id">The id of the item to delete.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous operation
    /// to delete the item that returns <see langword="true"/> if it was deleted,
    /// otherwise <see langword="false"/> if an item with the specified Id cannot be found.
    /// </returns>
    Task<bool> DeleteItemAsync(string id, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all the TODO items as an asynchronous operation.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>
    /// A <see cref="Task{TResult}"/> representing the asynchronous
    /// operation to return all of the available TODO items.
    /// </returns>
    Task<TodoListViewModel> GetListAsync(CancellationToken cancellationToken);
}
