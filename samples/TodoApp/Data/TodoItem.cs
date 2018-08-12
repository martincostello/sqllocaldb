// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;

namespace TodoApp.Data
{
    /// <summary>
    /// A class representing the database entity for a TODO item.
    /// </summary>
    public class TodoItem
    {
        /// <summary>
        /// Gets or sets the Id of the item.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the text of the item.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the date and time the item was created.
        /// </summary>
        public DateTimeOffset CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the date and time the item was completed.
        /// </summary>
        public DateTimeOffset? CompletedAt { get; set; }
    }
}
