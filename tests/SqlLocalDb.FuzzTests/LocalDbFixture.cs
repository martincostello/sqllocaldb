// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.Runtime.InteropServices;
using MartinCostello.SqlLocalDb.Interop;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;

namespace MartinCostello.SqlLocalDb;

public sealed class LocalDbFixture : IDisposable
{
    private readonly LocalDbInstanceApi _target = CreateLocalDbApi();

    internal LocalDbInstanceApi Target => _target;

    public void Dispose()
    {
        _target?.Dispose();
        GC.SuppressFinalize(this);
    }

    internal static LocalDbInstanceApi CreateLocalDbApi(string? apiVersion = default)
    {
        IRegistry registry =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
            new WindowsRegistry() :
            Substitute.For<IRegistry>();

        var logger = NullLogger<LocalDbInstanceApi>.Instance;

        return new(apiVersion ?? string.Empty, registry, logger);
    }
}
