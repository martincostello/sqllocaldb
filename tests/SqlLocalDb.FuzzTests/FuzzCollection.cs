// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

namespace MartinCostello.SqlLocalDb;

[CollectionDefinition(Name)]
public sealed class FuzzCollection : ICollectionFixture<LocalDbFixture>
{
    public const string Name = "Fuzzing";
}
