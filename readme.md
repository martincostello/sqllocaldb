# SQL LocalDB Wrapper

SQL LocalDB Wrapper is a .NET library providing interop with the
[Microsoft SQL Server LocalDB][sqllocaldb] Instance API from managed code using .NET APIs.

[![NuGet][package-badge-version]][package-download]
[![NuGet Downloads][package-badge-downloads]][package-download]

[![Build status][build-badge]][build-status]
[![codecov][coverage-badge]][coverage-report]
[![OpenSSF Scorecard][scorecard-badge]][scorecard-report]

## Introduction

This library exposes types that wrap the native SQL LocalDB Instance API to perform operations on
SQL LocalDB such as for managing instances (create, delete, start, stop) and obtaining SQL
connection strings for existing instances.

Microsoft SQL Server LocalDB 2012 and later is supported for both x86 and x64 on Microsoft Windows.

> [!IMPORTANT]
> While the library can be compiled and referenced in .NET applications on non-Windows operating
> systems, SQL LocalDB is only supported on Windows. Non-Windows Operating Systems can query to
> determine that the SQL LocalDB Instance API is not installed, but other usage will cause a
> `PlatformNotSupportedException` to be thrown.

### Installation

To install the library from [NuGet][package-download] using the .NET SDK run:

```text
dotnet add package MartinCostello.SqlLocalDb
```

### Basic Example

```csharp
using MartinCostello.SqlLocalDb;

using var localDB = new SqlLocalDbApi();

ISqlLocalDbInstanceInfo instance = localDB.GetOrCreateInstance("MyInstance");
ISqlLocalDbInstanceManager manager = instance.Manage();

if (!instance.IsRunning)
{
    manager.Start();
}

using SqlConnection connection = instance.CreateConnection();
connection.Open();

// Use the SQL connection...

manager.Stop();
```

### Further Examples

Further examples of using the library can be found by following the links below:

1. The [wiki][wiki-examples].
1. The [sample application][sample-app].
1. The [examples written as tests][test-examples].
1. The library's [own tests][tests].

## Migrating from System.Data.SqlLocalDb 1.x

Version `1.x.x` of this library was previously published as [`System.Data.SqlLocalDb`][legacy-package].
Subsequent versions (`3.x.x` and later) have been renamed and is a breaking change to the previous
versions, with various changes to namespaces and types.

## Migrating from MartinCostello.SqlLocalDb 2.x

Version `2.x.x` of this library uses SQL types from the `System.Data.SqlClient` namespace.

Subsequent versions (`3.x.x` and later) use the new [Microsoft.Data.SqlClient][sqlclient] NuGet package
where the same types (such as `SqlConnection`) are now in the `Microsoft.Data.SqlClient` namespace.

To migrate a project from using the previous `2.x.x` releases, you should change usage of the `System.Data.SqlClient`
namespace to `Microsoft.Data.SqlClient` and recompile your project.

## Feedback

Any feedback or issues can be added to the issues for this project in [GitHub][issues].

## Repository

The repository is hosted in [GitHub][repo]: <https://github.com/martincostello/sqllocaldb.git>

## License

This project is licensed under the [Apache 2.0][license] license.

## Building and Testing

Compiling the library yourself requires Git and the latest release of the [.NET SDK][dotnet-sdk] to be installed.

For all of the tests to be functional you must also have at least one version of [SQL LocalDB][sqllocaldb] installed.

To build and test the library locally from a terminal using PowerShell, run the following set of commands:

```text
git clone https://github.com/martincostello/sqllocaldb.git
cd sqllocaldb
./build.ps1
```

> [!NOTE]
> To run all the tests successfully, you must run either `build.ps1` or Visual Studio with administrative privileges.
> This is because the SQL LocalDB APIs for sharing LocalDB instances can only be used with administrative privileges.
> Not running the tests with administrative privileges will cause all tests that exercise such functionality to be skipped.

<!-- -->

> [!IMPORTANT]
> Most tests are skipped on non-Windows Operating Systems as SQL LocalDB itself is only supported on Windows.

## Copyright and Trademarks

This library is copyright (©) Martin Costello 2012-2025.

[Microsoft SQL Server][sqlserver] is a trademark and copyright of the Microsoft Corporation.

[build-badge]: https://github.com/martincostello/sqllocaldb/actions/workflows/build.yml/badge.svg?branch=main&event=push
[build-status]: https://github.com/martincostello/sqllocaldb/actions/workflows/build.yml?query=branch%3Amain+event%3Apush "Continuous Integration for this project"
[coverage-badge]: https://codecov.io/gh/martincostello/sqllocaldb/branch/main/graph/badge.svg
[coverage-report]: https://codecov.io/gh/martincostello/sqllocaldb "Code coverage report for this project"
[dotnet-sdk]: https://dotnet.microsoft.com/download "Download the .NET SDK"
[issues]: https://github.com/martincostello/sqllocaldb/issues "Issues for this project on GitHub.com"
[legacy-package]: https://www.nuget.org/packages/System.Data.SqlLocalDb/ "System.Data.SqlLocalDb on NuGet"
[license]: https://www.apache.org/licenses/LICENSE-2.0.txt "The Apache 2.0 license"
[package-badge-downloads]: https://img.shields.io/nuget/dt/MartinCostello.SqlLocalDb?logo=nuget&label=Downloads&color=blue
[package-badge-version]: https://img.shields.io/nuget/v/MartinCostello.SqlLocalDb?logo=nuget&label=Latest&color=blue
[package-download]: https://www.nuget.org/packages/MartinCostello.SqlLocalDb "Download MartinCostello.SqlLocalDb from NuGet"
[repo]: https://github.com/martincostello/sqllocaldb "This project on GitHub.com"
[sample-app]: https://github.com/martincostello/sqllocaldb/tree/main/samples "TodoApp sample"
[scorecard-badge]: https://api.securityscorecards.dev/projects/github.com/martincostello/sqllocaldb/badge
[scorecard-report]: https://securityscorecards.dev/viewer/?uri=github.com/martincostello/sqllocaldb "OpenSSF Scorecard for this project"
[sqlclient]: https://www.nuget.org/packages/Microsoft.Data.SqlClient/ "Microsoft.Data.SqlClient on NuGet"
[sqllocaldb]: https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/sql-server-express-localdb-reference-instance-apis "SQL Server Express LocalDB Reference - Instance APIs"
[sqlserver]: https://www.microsoft.com/sql-server/ "Microsoft SQL Server"
[test-examples]: https://github.com/martincostello/sqllocaldb/blob/main/tests/SqlLocalDb.Tests/Examples.cs "Examples as tests"
[tests]: https://github.com/martincostello/sqllocaldb/tree/main/tests/SqlLocalDb.Tests "View MartinCostello.SqlLocalDb's tests"
[wiki-examples]: https://github.com/martincostello/sqllocaldb/wiki/Examples "Examples in the SQL LocalDB Wrapper wiki"
