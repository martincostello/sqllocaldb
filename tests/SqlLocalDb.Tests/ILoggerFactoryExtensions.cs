// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MartinCostello.SqlLocalDb
{
    /// <summary>
    /// A class containing extension methods for the <see cref="ILoggerFactory"/> interface. This class cannot be inherited.
    /// </summary>
    internal static class ILoggerFactoryExtensions
    {
        /// <summary>
        /// Adds an xunit logger to the factory.
        /// </summary>
        /// <param name="factory">The <see cref="ILoggerFactory"/> to use.</param>
        /// <param name="outputHelper">The <see cref="ITestOutputHelper"/> to use.</param>
        /// <returns>
        /// The instance of <see cref="ILoggerFactory"/> specified by <paramref name="factory"/>.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="factory"/> or <paramref name="outputHelper"/> is <see langword="null"/>.
        /// </exception>
        internal static ILoggerFactory AddXunit(this ILoggerFactory factory, ITestOutputHelper outputHelper)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            if (outputHelper == null)
            {
                throw new ArgumentNullException(nameof(outputHelper));
            }

            var provider = new XunitLoggerProvider(outputHelper);

            factory.AddProvider(provider);

            return factory;
        }
    }
}
