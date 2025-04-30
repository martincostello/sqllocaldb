# SQL LocalDB Wrapper

SQL LocalDB Wrapper is a .NET library providing interop with the
[Microsoft SQL Server LocalDB][sqllocaldb] Instance API from managed code using .NET APIs.

[![NuGet][package-badge-version]][package-download]
[![NuGet Downloads][package-badge-downloads]][package-download]

[![Build status][build-badge]][build-status]
[![codecov][coverage-badge]][coverage-report]

## IntroductionW

This library exposes types that wrap the native SQL LocalDB Instance API to perform operations on
SQL LocalDB such as for managing instances (create, delete, start, stop) and obtaining SQL
connection strings for existing instances.

Microsoft SQL Server LocalDB 2012 and later is supported for both x86 and x64 on Microsoft Windows.

While the library can be compiled and referenced in .NET applications on non-Windows operating
systems, SQL LocalDB is only supported on Windows. Non-Windows Operating Systems can query to
determine that the SQL LocalDB Instance API is not installed, but other usage will cause a
`PlatformNotSupportedException` to be thrown.

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

## Feedback

Any feedback or issues for this package can be added to the issues in [GitHub][issues].

## License

This package is licensed under the [Apache 2.0][license] license.

[build-badge]: https://github.com/martincostello/sqllocaldb/actions/workflows/build.yml/badge.svg?branch=main&event=push
[build-status]: https://github.com/martincostello/sqllocaldb/actions/workflows/build.yml?query=branch%3Amain+event%3Apush "Continuous Integration for this project"
[coverage-badge]: https://codecov.io/gh/martincostello/sqllocaldb/branch/main/graph/badge.svg
[coverage-report]: https://codecov.io/gh/martincostello/sqllocaldb "Code coverage report for this project"
[issues]: https://github.com/martincostello/sqllocaldb/issues "Issues for this project on GitHub.com"
[license]: https://www.apache.org/licenses/LICENSE-2.0.txt "The Apache 2.0 license"
[package-badge-downloads]: https://img.shields.io/nuget/dt/MartinCostello.SqlLocalDb?logo=nuget&label=Downloads&color=blue
[package-badge-version]: https://img.shields.io/nuget/v/MartinCostello.SqlLocalDb?logo=nuget&label=Latest&color=blue
[package-download]: https://www.nuget.org/packages/MartinCostello.SqlLocalDb "Download MartinCostello.SqlLocalDb from NuGet"
[sqllocaldb]: https://learn.microsoft.com/sql/relational-databases/express-localdb-instance-apis/sql-server-express-localdb-reference-instance-apis "SQL Server Express LocalDB Reference - Instance APIs"
