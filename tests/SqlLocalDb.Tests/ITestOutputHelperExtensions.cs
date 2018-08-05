// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class containing extension methods for the <see cref="ITestOutputHelper"/> interface. This class cannot be inherited.
    /// </summary>
    internal static class ITestOutputHelperExtensions
    {
        /// <summary>
        /// Returns an <see cref="ILoggerFactory"/> that logs to the output helper.
        /// </summary>
        /// <param name="outputHelper">The <see cref="ITestOutputHelper"/> to create the logger factory from.</param>
        /// <returns>
        /// An <see cref="ILoggerFactory"/> that writes messages to the test output helper.
        /// </returns>
        public static ILoggerFactory AsLoggerFactory(this ITestOutputHelper outputHelper)
        {
            return new LoggerFactory().AddXunit(outputHelper);
        }
    }
}
