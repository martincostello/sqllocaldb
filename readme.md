# SQL LocalDB Wrapper

SQL LocalDB Wrapper is a .NET library providing interop with the [Microsoft SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/relational-databases/express-localdb-instance-apis/sql-server-express-localdb-reference-instance-apis?view=sql-server-2017 "SQL Server Express LocalDB Reference - Instance APIs") Instance API from managed code using .NET APIs. The library targets `netstandard2.0`, `net6.0` and `net8.0`.

[![NuGet](https://img.shields.io/nuget/v/MartinCostello.SqlLocalDb?logo=nuget&label=NuGet&color=blue)](https://www.nuget.org/packages/MartinCostello.SqlLocalDb "Download MartinCostello.SqlLocalDb from NuGet")

[![Build status](https://github.com/martincostello/sqllocaldb/workflows/build/badge.svg?branch=main&event=push)](https://github.com/martincostello/sqllocaldb/actions?query=workflow%3Abuild+branch%3Amain+event%3Apush)
[![codecov](https://codecov.io/gh/martincostello/sqllocaldb/branch/main/graph/badge.svg)](https://codecov.io/gh/martincostello/sqllocaldb)
[![OpenSSF Scorecard](https://api.securityscorecards.dev/projects/github.com/martincostello/sqllocaldb/badge)](https://securityscorecards.dev/viewer/?uri=github.com/martincostello/sqllocaldb)

## Introduction

This library exposes types that wrap the native SQL LocalDB Instance API to perform operations on SQL LocalDB such as for managing instances (create, delete, start, stop) and obtaining SQL connection strings for existing instances.

Microsoft SQL Server LocalDB 2012 and later is supported for both x86 and x64 on Microsoft Windows and the library targets `netstandard2.0`.

While the library can be compiled and referenced in .NET applications on non-Windows Operating Systems, SQL LocalDB is only supported on Windows. Non-Windows Operating Systems can query to determine that the SQL LocalDB Instance API is not installed, but other usage will cause a `PlatformNotSupportedException` to be thrown.

### Installation

To install the library from [NuGet](https://www.nuget.org/packages/MartinCostello.SqlLocalDb/ "MartinCostello.SqlLocalDb on NuGet.org") using the .NET SDK run:

```sh
dotnet add package MartinCostello.SqlLocalDb
```

### Basic Example

```csharp
// using MartinCostello.SqlLocalDb;

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

  1. The [wiki](https://github.com/martincostello/sqllocaldb/wiki/Examples "Examples in the SQL LocalDB Wrapper wiki").
  1. The [sample application](https://github.com/martincostello/sqllocaldb/tree/main/samples "TodoApp sample").
  1. The [examples written as tests](https://github.com/martincostello/sqllocaldb/blob/main/tests/SqlLocalDb.Tests/Examples.cs "Examples as tests").
  1. The library's [own tests](https://github.com/martincostello/sqllocaldb/tree/main/tests/SqlLocalDb.Tests "View MartinCostello.SqlLocalDb's tests").

## Migrating from System.Data.SqlLocalDb 1.x

Version `1.x.x` of this library was previously published as [`System.Data.SqlLocalDb`](https://www.nuget.org/packages/System.Data.SqlLocalDb/ "System.Data.SqlLocalDb on NuGet"). The current version (`3.x.x`) has been renamed and is a breaking change to the previous version with various changes to namespaces and types.

## Migrating from MartinCostello.SqlLocalDb 2.x

Version [`2.x.x`](https://www.nuget.org/packages/MartinCostello.SqlLocalDb/2.0.0 "MartinCostello.SqlLocalDb 2.0.0 on NuGet") of this library uses SQL types from the `System.Data.SqlClient` namespace.

The current version (`3.x.x`) uses the new [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient/) NuGet package where the same types (such as `SqlConnection`) are now in the `Microsoft.Data.SqlClient` namespace.

To migrate a project from using the previous 2.x release, you should change usage of the `System.Data.SqlClient` namespace to `Microsoft.Data.SqlClient` and recompile your project.

## Feedback

Any feedback or issues can be added to the issues for this project in [GitHub](https://github.com/martincostello/sqllocaldb/issues "Issues for this project on GitHub.com").

## Repository

The repository is hosted in [GitHub](https://github.com/martincostello/sqllocaldb "This project on GitHub.com"): [https://github.com/martincostello/sqllocaldb.git](https://github.com/martincostello/sqllocaldb.git)

## License

This project is licensed under the [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0.txt "The Apache 2.0 license") license.

## Building and Testing

Compiling the library yourself requires Git and the [.NET SDK](https://www.microsoft.com/net/download/core "Download the .NET SDK") to be installed (version `8.0.100` or later).

For all of the tests to be functional you must also have at least one version of SQL LocalDB installed.

To build and test the library locally from a terminal/command-line, run the following set of commands:

**Windows**

```powershell
git clone https://github.com/martincostello/sqllocaldb.git
cd sqllocaldb
./build.ps1
```

**Note**: To run all the tests successfully, you must run either `build.ps1` or Visual Studio with administrative privileges. This is because the SQL LocalDB APIs for sharing LocalDB instances can only be used with administrative privileges. Not running the tests with administrative privileges will cause all tests that exercise such functionality to be skipped.

**Note**: Several tests are skipped on non-Windows Operating Systems as SQL LocalDB itself is only supported on Windows.

## Copyright and Trademarks

This library is copyright (©) Martin Costello 2012-2024.

[Microsoft SQL Server](https://www.microsoft.com/sql-server/) is a trademark and copyright of the Microsoft Corporation.
