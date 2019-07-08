// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace MartinCostello.SqlLocalDb
{
    public sealed class RetryFactDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly IMessageSink _sink;

        public RetryFactDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _sink = diagnosticMessageSink;
        }

        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            int maxRetries = factAttribute.GetNamedArgument<int>("MaxRetries");

            if (maxRetries < 1)
            {
                maxRetries = 3;
            }

            yield return new RetryTestCase(
                _sink,
                discoveryOptions.MethodDisplayOrDefault(),
                discoveryOptions.MethodDisplayOptionsOrDefault(),
                testMethod,
                maxRetries);
        }
    }
}
