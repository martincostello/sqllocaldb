// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

#if NETFRAMEWORK
using System.Runtime.CompilerServices;

#pragma warning disable IDE0130
namespace System;

internal static class ArgumentNullExceptionExtensions
{
    extension(ArgumentNullException)
    {
        public static void ThrowIfNull(object? argument, [CallerArgumentExpression(nameof(argument))] string? paramName = null)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
#endif
