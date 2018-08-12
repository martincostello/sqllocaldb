// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TodoApp.Data
{
    /// <summary>
    /// Defines the repository for TODO items.
    /// </summary>
    public interface ITodoRepository
    {
        /// <summary>
        /// Adds a new item as an asynchronous operation.
        /// </summary>
        /// <param name="text">The text of the item to add.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation
        /// to add the new item that returns the created item.
        /// </returns>
        Task<TodoItem> AddItemAsync(string text, CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks an item as completed as an asynchronous operation.
        /// </summary>
        /// <param name="id">The id of the item to mark as completed.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation
        /// to complete the item that <see langword="true"/> if it was completed,
        /// <see langword="false"/> if it was already completed, or <see langword="null"/>
        /// if an item with the specified Id cannot be found.
        /// </returns>
        Task<bool?> CompleteItemAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an item as an asynchronous operation.
        /// </summary>
        /// <param name="id">The id of the item to delete.</param>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous operation
        /// to delete the item that returns <see langword="true"/> if it was deleted,
        /// otherwise <see langword="false"/> if an item with the specified Id cannot be found.
        /// </returns>
        Task<bool> DeleteItemAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns all items as an asynchronous operation.
        /// </summary>
        /// <param name="cancellationToken">The optional cancellation token to use.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> representing the asynchronous
        /// operation to return all of the available items.
        /// </returns>
        Task<IList<TodoItem>> GetItemsAsync(CancellationToken cancellationToken = default);
    }
}
