# SQL LocalDB Wrapper

[![Build Status](https://ci.appveyor.com/api/projects/status/github/martincostello/sqllocaldb?branch=master&svg=true)](https://ci.appveyor.com/project/martincostello/sqllocaldb) [![Coverage Status](https://coveralls.io/repos/martincostello/sqllocaldb/badge.svg?branch=master)](https://coveralls.io/r/martincostello/sqllocaldb?branch=master) [![Coverity Scan Build Status](https://scan.coverity.com/projects/2424/badge.svg)](https://scan.coverity.com/projects/2424)

[![Open GitHub Issues](https://img.shields.io/github/issues/martincostello/sqllocaldb.svg?label=Open%20Issues)](https://github.com/martincostello/sqllocaldb/issues) [![Latest GitHub Release](https://img.shields.io/github/release/martincostello/sqllocaldb.svg?label=Latest%20Release)](https://github.com/martincostello/sqllocaldb/releases/latest) [![License](https://img.shields.io/github/license/martincostello/sqllocaldb.svg?label=License)](https://github.com/martincostello/sqllocaldb/blob/master/license.txt)

[![NuGet Downloads](https://img.shields.io/nuget/dt/System.Data.SqlLocalDb.svg?label=NuGet%20Downloads)](http://www.nuget.org/packages/System.Data.SqlLocalDb/) [![Latest NuGet Package Version](https://img.shields.io/nuget/v/System.Data.SqlLocalDb.svg?label=Latest%20NuGet%20Package)](http://www.nuget.org/packages/System.Data.SqlLocalDb/)

## Overview

SQL LocalDB Wrapper is a .NET 3.5 assembly providing interop with the [Microsoft SQL Server LocalDB](http://msdn.microsoft.com/en-us/library/hh510202.aspx) native API from managed code using .NET APIs.

It is designed to support use of dependency injection by consumers by implementing interfaces, and is also designed to fit with the other data access providers defined under the System.Data namespaces.

The assembly supports using SQL Server LocalDB 2012 and 2014 for both the x86 and x64 platforms.

## Downloads

The recommended way of obtaining the assembly is using [NuGet](https://www.nuget.org/packages/System.Data.SqlLocalDb).

Alternatively, a ZIP file containing the assembly can be downloaded from [GitHub](https://github.com/martincostello/sqllocaldb/releases/latest).

## Documentation

### Basic Usage

First install the [NuGet package](https://www.nuget.org/packages/System.Data.SqlLocalDb/):

```Install-Package System.Data.SqlLocalDb```

Add the appropriate namespace:

```
using System.Data.SqlLocalDb;
```

Then create an instance, start it and connect to it:

```
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

An example of using the API can be found [here](https://github.com/martincostello/sqllocaldb/blob/master/src/TestApp/Program.cs) in the TestApp project in the source code.

An example of using the API with [EntityFramework](http://entityframework.codeplex.com/) can be found [here](https://github.com/martincostello/sqllocaldb/blob/master/src/SqlLocalDb.EFSample/Program.cs) in the source code.

## Feedback

Any feedback or issues can be added to the issues for this project in [GitHub](https://github.com/martincostello/sqllocaldb/issues).

## Repository

The repository is hosted in [GitHub](https://github.com/martincostello/sqllocaldb): https://github.com/martincostello/sqllocaldb.git

## License

This project is licensed under the [Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0.txt) license.

## Building and Testing

Building and testing the project is supported using Microsoft Visual Studio 2013.

The simplest way to build and test the assembly from the source code is by using the [Build.cmd](https://github.com/martincostello/sqllocaldb/blob/master/Build.cmd) batch file in the root of the repository like so: ```Build.cmd```. The project can also be built and tested from Visual Studio.

Building the project from the command-line using [Build.cmd](https://github.com/martincostello/sqllocaldb/blob/master/Build.cmd) invokes MSBuild to compile the source, examples and tests (including running the configured static analysis tools), and then uses MSTest to test the compiled assembly (```System.Data.SqlLocalDb.dll```) for both the x86 and x64 platforms.

The standard build process also includes running [StyleCop](https://stylecop.codeplex.com/) and FxCop.

To only compile the source code and not run the tests, use the following command: ```Build.cmd /p:RunTests=false```

__Note__: To run all the tests, you must run either ```Build.cmd``` or Visual Studio with administrative privileges. This is because the SQL LocalDB APIs for sharing LocalDB instances can only be used with administrative privileges. Not running the tests with administrative privileges will cause all tests that exercise such functionality to be marked as Inconclusive by MSTest.