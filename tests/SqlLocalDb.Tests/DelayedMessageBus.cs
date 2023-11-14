// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using Xunit.Sdk;

namespace MartinCostello.SqlLocalDb;

internal sealed class DelayedMessageBus(IMessageBus inner) : IMessageBus
{
    private readonly List<IMessageSinkMessage> _messages = new();

    public bool QueueMessage(IMessageSinkMessage message)
    {
        lock (_messages)
        {
            _messages.Add(message);
        }

        // No way to ask the inner bus if they want to cancel without sending them the message, so
        // we just go ahead and continue always.
        return true;
    }

    public void Dispose()
    {
        foreach (var message in _messages)
        {
            inner.QueueMessage(message);
        }
    }
}
