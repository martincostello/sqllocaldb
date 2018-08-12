// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// Defines an interface implemented by objects that can provide an <see cref="ISqlLocalDbApi" /> instance.
    /// </summary>
    public interface ISqlLocalDbApiAdapter
    {
        /// <summary>
        /// Gets the <see cref="ISqlLocalDbApi"/> instance.
        /// </summary>
        ISqlLocalDbApi LocalDb { get; }
    }
}
