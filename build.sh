#!/bin/sh
export artifacts=$(dirname "$(readlink -f "$0")")/artifacts
export configuration=Release

dotnet restore SqlLocalDb.sln --verbosity minimal || exit 1
dotnet build src/SqlLocalDb/System.Data.SqlLocalDb.csproj --output $artifacts --configuration $configuration --framework "netstandard1.6" || exit 1
dotnet test tests/SqlLocalDb.Tests/System.Data.SqlLocalDb.UnitTests.csproj --output $artifacts --configuration $configuration --framework "netcoreapp1.1" || exit 1
