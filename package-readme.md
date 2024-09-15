# SQL LocalDB Wrapper

SQL LocalDB Wrapper is a .NET library providing interop with the [Microsoft SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/relational-databases/express-localdb-instance-apis/sql-server-express-localdb-reference-instance-apis?view=sql-server-2017 "SQL Server Express LocalDB Reference - Instance APIs") Instance API from managed code using .NET APIs. The library targets `netstandard2.0`, `net6.0` and `net8.0`.

[![NuGet](https://img.shields.io/nuget/v/MartinCostello.SqlLocalDb?logo=nuget&label=Latest&color=blue)](https://www.nuget.org/packages/MartinCostello.SqlLocalDb "Download MartinCostello.SqlLocalDb from NuGet")
[![NuGet Downloads](https://img.shields.io/nuget/dt/MartinCostello.SqlLocalDb?logo=nuget&label=Downloads&color=blue)](https://www.nuget.org/packages/MartinCostello.SqlLocalDb "Download MartinCostello.SqlLocalDb from NuGet")

[![Build status](https://github.com/martincostello/sqllocaldb/workflows/build/badge.svg?branch=main&event=push)](https://github.com/martincostello/sqllocaldb/actions?query=workflow%3Abuild+branch%3Amain+event%3Apush)
[![codecov](https://codecov.io/gh/martincostello/sqllocaldb/branch/main/graph/badge.svg)](https://codecov.io/gh/martincostello/sqllocaldb)

## Introduction

This library exposes types that wrap the native SQL LocalDB Instance API to perform operations on SQL LocalDB such as for managing instances (create, delete, start, stop) and obtaining SQL connection strings for existing instances.

Microsoft SQL Server LocalDB 2012 and later is supported for both x86 and x64 on Microsoft Windows and the library targets `netstandard2.0`.

While the library can be compiled and referenced in .NET applications on non-Windows Operating Systems, SQL LocalDB is only supported on Windows. Non-Windows Operating Systems can query to determine that the SQL LocalDB Instance API is not installed, but other usage will cause a `PlatformNotSupportedException` to be thrown.

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

## Feedback

Any feedback or issues for this package can be added to the issues in [GitHub](https://github.com/martincostello/sqllocaldb/issues "Issues for this project on GitHub.com").

## License

This package is licensed under the [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0.txt "The Apache 2.0 license") license.
