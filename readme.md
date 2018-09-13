# SQL LocalDB Wrapper

SQL LocalDB Wrapper is a .NET Standard 2.0 library providing interop with the [Microsoft SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/relational-databases/express-localdb-instance-apis/sql-server-express-localdb-reference-instance-apis?view=sql-server-2017 "SQL Server Express LocalDB Reference - Instance APIs") Instance API from managed code using .NET APIs.

[![NuGet](https://buildstats.info/nuget/MartinCostello.SqlLocalDb?includePreReleases=true)](http://www.nuget.org/packages/MartinCostello.SqlLocalDb "Download MartinCostello.SqlLocalDb from NuGet")

| | Windows | Linux/OS X | Linux/macOS/Windows |
|:-:|:-:|:-:|:-:|
| **Build Status** | [![Windows build status](https://img.shields.io/appveyor/ci/martincostello/sqllocaldb/master.svg)](https://ci.appveyor.com/project/martincostello/sqllocaldb) [![Code coverage](https://codecov.io/gh/martincostello/sqllocaldb/branch/master/graph/badge.svg)](https://codecov.io/gh/martincostello/sqllocaldb) | [![Linux build status](https://img.shields.io/travis-ci/com/martincostello/sqllocaldb/master.svg)](https://travis-ci.com/martincostello/sqllocaldb) | [![Azure Pipelines build status](https://dev.azure.com/martincostello/sqllocaldb/_apis/build/status/CI)](https://dev.azure.com/martincostello/sqllocaldb/_build/latest?definitionId=66) |
| **Build History** | [![Windows build history](https://buildstats.info/appveyor/chart/martincostello/sqllocaldb?branch=master&includeBuildsFromPullRequest=false)](https://ci.appveyor.com/project/martincostello/sqllocaldb) | [![Linux build history](https://buildstats.info/travisci/chart/martincostello/sqllocaldb?branch=master&includeBuildsFromPullRequest=false)](https://travis-ci.com/martincostello/sqllocaldb) | _Not supported_ |

## Introduction

This library exposes types that wrap the native SQL LocalDB Instance API to perform operations on SQL LocalDB such as for managing instances (create, delete, start, stop) and obtaining SQL connection strings for existing instances.

Microsoft SQL Server LocalDB 2012 and later is supported for both x86 and x64 on Microsoft Windows and the library targets `netstandard2.0`.

While the library can be compiled and referenced in .NET Core applications on non-Windows Operating Systems, SQL LocalDB is only supported on Windows. Non-Windows Operating Systems can query to determine that the SQL LocalDB Instance API is not installed, but other usage will cause a `PlatformNotSupportedException` to be thrown.

### Installation

To install the library from [NuGet](https://www.nuget.org/packages/MartinCostello.SqlLocalDb/ "MartinCostello.SqlLocalDb on NuGet.org") using the .NET SDK run:

```
dotnet add package MartinCostello.SqlLocalDb
```

### Basic Example

```csharp
// using MartinCostello.SqlLocalDb;

using (var localDB = new SqlLocalDbApi())
{
    ISqlLocalDbInstanceInfo instance = localDB.GetOrCreateInstance("MyInstance");
    ISqlLocalDbInstanceManager manager = instance.Manage();

    if (!instance.IsRunning)
    {
        manager.Start();
    }

    using (SqlConnection connection = instance.CreateConnection())
    {
        connection.Open();

        // Use the SQL connection...
    }

    manager.Stop();
}
```

### Further Examples

Further examples of using the library can be found by following the links below:

  1. The [wiki](https://github.com/martincostello/sqllocaldb/wiki/Examples "Examples in the SQL LocalDB Wrapper wiki").
  1. The [sample application](https://github.com/martincostello/sqllocaldb/tree/master/samples "TodoApp sample").
  1. The [examples written as tests](https://github.com/martincostello/sqllocaldb/blob/master/tests/SqlLocalDb.Tests/Examples.cs "Examples as tests").
  1. The library's [own tests](https://github.com/martincostello/sqllocaldb/tree/master/tests/SqlLocalDb.Tests "View MartinCostello.SqlLocalDb's tests").

## Migrating from System.Data.SqlLocalDb

Version `1.x.x` of this library was previously published as [`System.Data.SqlLocalDb`](https://www.nuget.org/packages/System.Data.SqlLocalDb/ "System.Data.SqlLocalDb on NuGet"). The current version (`2.x.x`) has been renamed and is a breaking change to the previous version with various changes to namespaces and types.

To migrate a project from using the previous version follow the migration guide: [Migrating to MartinCostello.SqlLocalDb from System.Data.SqlLocalDb](https://github.com/martincostello/sqllocaldb/wiki/Migrating-to-MartinCostello.SqlLocalDb-from-System.Data.SqlLocalDb "Migrating to MartinCostello.SqlLocalDb from System.Data.SqlLocalDb")

## Feedback

Any feedback or issues can be added to the issues for this project in [GitHub](https://github.com/martincostello/sqllocaldb/issues "Issues for this project on GitHub.com").

## Repository

The repository is hosted in [GitHub](https://github.com/martincostello/sqllocaldb "This project on GitHub.com"): https://github.com/martincostello/sqllocaldb.git

## License

This project is licensed under the [Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0.txt "The Apache 2.0 license") license.

## Building and Testing

Compiling the library yourself requires Git and the [.NET Core SDK](https://www.microsoft.com/net/download/core "Download the .NET Core SDK") to be installed (version `2.1.402` or later).

For all of the tests to be functional you must also have at least one version of SQL LocalDB installed.

To build and test the library locally from a terminal/command-line, run one of the following set of commands:

**Windows**

```powershell
git clone https://github.com/martincostello/sqllocaldb.git
cd sqllocaldb
.\Build.ps1
```

**Note**: To run all the tests successfully, you must run either `Build.ps1` or Visual Studio with administrative privileges. This is because the SQL LocalDB APIs for sharing LocalDB instances can only be used with administrative privileges. Not running the tests with administrative privileges will cause all tests that exercise such functionality to be skipped.

**Linux/macOS**

```sh
git clone https://github.com/martincostello/sqllocaldb.git
cd sqllocaldb
./build.sh
```

**Note**: Several tests are skipped on non-Windows Operating Systems.

## Copyright and Trademarks

This library is copyright (©) Martin Costello 2012-2018.

[Microsoft SQL Server](https://www.microsoft.com/en-gb/sql-server/) is a trademark and copyright of the Microsoft Corporation.
