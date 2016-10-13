# SQL LocalDB Wrapper

[![Build Status](https://img.shields.io/appveyor/ci/martincostello/sqllocaldb/master.svg)](https://ci.appveyor.com/project/martincostello/sqllocaldb) [![Coverage Status](https://coveralls.io/repos/martincostello/sqllocaldb/badge.svg?branch=master)](https://coveralls.io/r/martincostello/sqllocaldb?branch=master) [![Coverity Scan Build Status](https://scan.coverity.com/projects/2424/badge.svg)](https://scan.coverity.com/projects/2424)

<!--[![Open GitHub Issues](https://img.shields.io/github/issues/martincostello/sqllocaldb.svg?label=Open%20Issues)](https://github.com/martincostello/sqllocaldb/issues) [![Latest GitHub Release](https://img.shields.io/github/release/martincostello/sqllocaldb.svg?label=Latest%20Release)](https://github.com/martincostello/sqllocaldb/releases/latest) [![License](https://img.shields.io/github/license/martincostello/sqllocaldb.svg?label=License)](https://github.com/martincostello/sqllocaldb/blob/master/license.txt)-->

[![NuGet](https://buildstats.info/nuget/System.Data.SqlLocalDb)](http://www.nuget.org/packages/System.Data.SqlLocalDb)

[![Join the chat at https://gitter.im/martincostello/sqllocaldb](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/martincostello/sqllocaldb?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

[![Build History](https://buildstats.info/appveyor/chart/martincostello/sqllocaldb?branch=master&includeBuildsFromPullRequest=false)](https://ci.appveyor.com/project/martincostello/sqllocaldb)

## Overview

SQL LocalDB Wrapper is a .NET 4.0 assembly providing interop with the [Microsoft SQL Server LocalDB](http://msdn.microsoft.com/en-us/library/hh510202.aspx) native API from managed code using .NET APIs.

It is designed to support use of dependency injection by consumers by implementing interfaces, and is also designed to fit with the other data access providers defined under the System.Data namespaces.

The assembly supports using SQL Server LocalDB 2012 and 2014 for both the x86 and x64 platforms and SQL Server LocalDB 2016 for the x64 platform.

## Downloads

The recommended way of obtaining the assembly is using [NuGet](https://www.nuget.org/packages/System.Data.SqlLocalDb).

Alternatively, a ZIP file containing the assembly can be downloaded from [GitHub](https://github.com/martincostello/sqllocaldb/releases/latest).

## Documentation

### Basic Usage

First install the [NuGet package](https://www.nuget.org/packages/System.Data.SqlLocalDb/):

```batchfile
Install-Package System.Data.SqlLocalDb
```

Add the appropriate namespace:

```csharp
using System.Data.SqlLocalDb;
```

Then create an instance, start it and connect to it:

```csharp
ISqlLocalDbProvider provider = new SqlLocalDbProvider();
ISqlLocalDbInstance instance = provider.GetOrCreateInstance("MyInstance");

instance.Start();

using (SqlConnection connection = instance.CreateConnection())
{
    connection.Open();

    // Use the connection...
}

instance.Stop();
```

### Further Details

For further documentation about the assembly and how to use it, consult the [Wiki](https://github.com/martincostello/sqllocaldb/wiki) in GitHub.

You can also check out the [examples below](https://github.com/martincostello/sqllocaldb#examples).

## Examples

  1. An example of using the API can be found [here](https://github.com/martincostello/sqllocaldb/blob/master/src/TestApp/Program.cs) in the TestApp project in the source code.
  1. An runnable example solution using the API to test an ASP.NET MVC application using SQL Server with MSTest is included as [BlogSample.sln](https://github.com/martincostello/sqllocaldb/blob/master/src/BlogSample.sln) in the source code.

## Feedback

Any feedback or issues can be added to the issues for this project in [GitHub](https://github.com/martincostello/sqllocaldb/issues).

## Repository

The repository is hosted in [GitHub](https://github.com/martincostello/sqllocaldb): https://github.com/martincostello/sqllocaldb.git

## License

This project is licensed under the [Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0.txt) license.

## Building and Testing

Building and testing the project is supported using Microsoft Visual Studio 2013 and 2015.

The simplest way to build and test the assembly from the source code is by using the [Build.cmd](https://github.com/martincostello/sqllocaldb/blob/master/Build.cmd) batch file in the root of the repository like so:

 ```batchfile
 Build.cmd
 ```

The project can also be built and tested from Visual Studio.

Building the project from the command-line using [Build.cmd](https://github.com/martincostello/sqllocaldb/blob/master/Build.cmd) invokes MSBuild to compile the source, examples and tests (including running the configured static analysis tools), and then uses MSTest to test the compiled assembly (```System.Data.SqlLocalDb.dll```) for both the x86 and x64 platforms.

The standard build process also includes running [StyleCop](https://github.com/DotNetAnalyzers/StyleCopAnalyzers) and FxCop.

To only compile the source code and not run the tests, use the following command:

```batchfile
Build.cmd /p:RunTests=false
```

__Note__: To run all the tests, you must run either ```Build.cmd``` or Visual Studio with administrative privileges. This is because the SQL LocalDB APIs for sharing LocalDB instances can only be used with administrative privileges. Not running the tests with administrative privileges will cause all tests that exercise such functionality to be marked as Inconclusive by MSTest.
