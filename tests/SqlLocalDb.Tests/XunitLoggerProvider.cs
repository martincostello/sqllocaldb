// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class representing an <see cref="ILoggerProvider"/> to use with xunit. This class cannot be inherited.
    /// </summary>
    internal sealed class XunitLoggerProvider : ILoggerProvider
    {
        private readonly ITestOutputHelper _outputHelper;

        internal XunitLoggerProvider(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
        }

        /// <inheritdoc />
        public ILogger CreateLogger(string categoryName) => new XunitLogger(categoryName, _outputHelper);

        /// <inheritdoc />
        public void Dispose()
        {
            // Nothing to dispose of
        }
    }
}
