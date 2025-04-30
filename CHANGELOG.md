# CHANGELOG

<!-- markdownlint-disable MD024 -->

## SqlLocalDb v1.0.0.0

First release.

## SqlLocalDb v1.1.0.0

Adds logging, improvements to help use with EntityFramework and new interfaces to help with mocking/abstractions.

## SqlLocalDb v1.2.0.0

Added overloads to support specifying the name of the Initial Catalog using the `ISqlLocalDbInstance.GetConnectionForModel()` and `ISqlLocalDbInstance.GetConnectionStringForModel()` extension methods.

## SqlLocalDb v1.3.0.0

* Added new extension methods to make creating and working with connection strings easier. Specifically these are:
  * `DbConnection ISqlLocalDbInstance.GetConnectionForDefaultModel()`
  * `DbConnection ISqlLocalDbInstance.GetConnectionForDefaultModel(string initialCatalog)`
  * `DbConnectionStringBuilder ISqlLocalDbInstance.GetConnectionStringForDefaultModel()`
  * `DbConnectionStringBuilder ISqlLocalDbInstance.GetConnectionStringForDefaultModel(string initialCatalog)`
  * `string DbConnectionStringBuilder.GetInitialCatalogName()`

## SqlLocalDb v1.4.0.0

* Added the following new members:
  * `void DbConnectionStringBuilder.SetInitialCatalogName(string)`
  * `SqlConnectionStringBuilder ISqlLocalDbInstance.CreateConnectionStringBuilder()`
  * `ISqlLocalDbInstance ISqlLocalDbProvider.GetOrCreateInstance(string)`
* Fixed `SqlLocalDbProvider.GetInstances()` only ever returning the default instance.
* Added sample application that shows use of the assembly with EntityFramework.

## SqlLocalDb v1.5.0.0

* Added the following new members:
  * `string DbConnectionStringBuilder.GetPhysicalFileName()`
  * `void DbConnectionStringBuilder.SetPhysicalFileName(string)`

## SqlLocalDb v1.6.0.0

* Changes to have the managed code automatically use the native library with the appropriate bitness for the executing processor architecture.
* Fixed typo in XML documentation.
* Improvements to build automation.
* Updated to the latest versions of EntityFramework and Moq.
* The non-core projects now target .NET Framework 4.5.1.

## SqlLocalDb v1.7.0.0

* Removed processor-specific native code from the solution. All native dependencies are now only those related to SQL LocaDB itself. This means the wrapper now ships as a single .NET assembly (`System.Data.SqlLocalDb.dll`) which is compiled as **AnyCPU**.
* Added ability to control the behaviour when shutting down a SQL LocalDB instance using the new `StopInstanceOptions` enumeration via the `SqlLocalDbApi.StopOptions` property and the `SqlLocalDbApi.StopInstance(string, StopInstanceOptions, TimeSpan)` method overload.

## SqlLocalDb v1.8.0.0

### New Features

* Support for Microsoft SQL LocalDB 2014.
* Added functionality to the `SqlLocalDbProvider` class to allow the version used to create instances to be changed easily via the new `Version` property.
* Added the `DeleteUserInstances()` method to the `SqlLocalDbApi` class to allow throw-away instances on the local machine to be easily cleaned-up.
* The `SQLLocalDB:OverrideVersion` application setting can now be used to override the version of the SQL LocalDB native API loaded, instead of the default behaviour of loading the latest version installed on the local machine.

### Bug Fixes

* Fixed `SqlLocalDbApi.ShareInstance()` not passing the marshalled value for `ownerSid` to the underlying native LocalDB API function.
* Fixed `SqlLocalDbApi.LatestVersion` returning the oldest version number, not the latest version, when SQL LocalDB 2012 and 2014 are installed on the same machine.
* Fixed incorrect trace event Id being used if an SQL LocalDB instance was successfully deleted.
* Fixed typo in resource string.
* FxCop and SecAnnotate fixes to NativeMethods for targeting .NET 3.5 and .NET 4.0.

## SqlLocalDb v1.9.0.0

### New Features

* Added new `TemporarySqlLocalDbInstance` class which helps manage temporary instances used for a limited period of time, such as for testing purposes. The instances created by this class are automatically started and are deleted when the instances are the class are disposed.
* Added the `Restart()` convenience extension method to the `ISqlLocalDbInstance` interface.

## SqlLocalDb v1.10.0.0

### New Features

* Added the ability to change the logging implementation by setting an `ILogger` instance via the `Logger.SetLogger(ILogger)` method.
* The assembly no longer directly references the System.Data.Entity assembly, and now creates the types used from it using reflection instead.
* `SqlLocalDbApi.DeleteUserInstances()` now ignores any errors caused by instances that cannot be deleted because the instance is in use.

### Bug Fixes

* Changed some log messages to use "Obtaining"/"Obtained" instead of "Getting"/"Obtained".
* Fixed two log messages with quoted format values that only had a start quote and no end quote.

## SqlLocalDb v1.11.0.0

### New Features

* Added the following new members to the `SqlLocalDbApi` class to allow the files associated with a SQL LocalDB instance to be deleted when the instance is deleted. The previous behaviour is retained when using the existing deletion methods. The default value of the `SqlLocalDbApi.AutomaticallyDeleteInstanceFiles` property can be overridden using the `SQLLocalDB:AutomaticallyDeleteInstanceFiles` application configuration setting.
  * `bool SqlLocalDbApi.AutomaticallyDeleteInstanceFiles`
  * `void SqlLocalDbApi.DeleteInstance(string instanceName, bool deleteFiles)`
  * `int SqlLocalDbApi.DeleteUserInstances(bool deleteFiles)`
* Added `SqlLocalDbApi.GetInstancesFolderPath()` method to obtain the path to which the SQL LocalDB instance files are stored on the local machine for the current user.

### Bug Fixes

* Small change to P/Invoke calls that receive strings as output parameters.
* Fixes to some XML documentation comments.

## SqlLocalDb v1.12.0.0

### New Features

* Added the following new members to the `TemporarySqlLocalDbInstance` class to make managing the files for temporary instances easier.
  * `TemporarySqlLocalDbInstance .ctor(string instanceName, ISqlLocalDbProvider provider, bool deleteFiles)`
  * `TemporarySqlLocalDbInstance Create(bool deleteFiles)`

### Bug Fixes

* Fixed code path where the value of the `SqlLocalDbApi.AutomaticallyDeleteInstanceFiles` property was not honoured when deleting instances.

## SqlLocalDb v1.13.0.0

### New Features

* Added new `system.data.sqlLocalDb` custom configuration section. The settings are documented [here](https://github.com/martincostello/sqllocaldb/wiki/Configuration).
* Added static `DefaultInstanceName` property to the `SqlLocalDbApi` class.
* Added `GetDefaultInstance()` extension method for the `ISqlLocalDbProvider` interface.
* All references to [CodePlex](https://sqllocaldb.codeplex.com/) in the source code have been updated to [GitHub](https://github.com/martincostello/sqllocaldb).

### Bug Fixes

* Fixed inability to create the default named instances of SQL LocalDB (i.e. `v11.0` or `MSSQLLocalDB`).
* Fixed the `GetOrCreateInstance()` extension method for the `ISqlLocalDbInstance` interface not being able to correctly determine if the instance already exists if the instance name is one of the default instance names.
* Fixed directory traversal flaw in the internal `SqlLocalDbApi.DeleteInstanceFiles(string)` method. This was not exploitable by the public API as the native SQL LocalDB Instance API rejects calls to `LocalDBDeleteInstance()` with such names so `DeleteInstanceFiles()` was never called.
* The path to the native SQL LocalDB Instance API DLL read from the registry is now canonicalized using `System.IO.Path.GetFullPath()` before being used.

## SqlLocalDb v1.14.0.0

### New Features

* Added support for specifying a custom `ILogger` implementation to use in the application configuration file.
* Added new `LanguageId` property to the `SqlLocalDbApi` class to allow integrators to override the default behaviour used to select the language used to format error messages from the SQL LocalDB Instance API.
* Added `en-GB` resources.

### Bug Fixes

* Fixed the HRESULT in an exception message not being formatted in hexadecimal.
* Fixed some incorrect XML documentation.

## SqlLocalDb v1.15.0.0

### Changes

* The assembly now targets .NET 4.0 instead of .NET 3.5 SP1.
* The HRESULT values from the SQL LocalDB Instance API logged in error messages are now formatted as hexadecimal values.
* A warning is logged if the configured version of the SQL LocalDB Instance API to load instead of the latest version cannot be found.
* Fixed ```TemporarySqlLocalDbInstance``` so it no longer throws an exception when disposed if the temporary SQL LocalDB instance could not be stopped or deleted.
* The internals now use the Windows ```LoadLibraryEx``` function instead of ```LoadLibrary```.

## SqlLocalDb v2.0.0.0

### Changes

* The NuGet package name is now `MartinCostello.SqlLocalDb`.
* Types are now in the `MartinCostello.SqlLocalDb` namespace.
* Core classes and interfaces are now:
  * `ISqlLocalDbApi`
  * `ISqlLocalDbInstanceInfo`
  * `ISqlLocalDbInstanceManager`
  * `ISqlLocalDbVersionInfo`
  * `SqlLocalDbApi`
  * `SqlLocalDbException`
  * `SqlLocalDbInstanceManager`
  * `SqlLocalDbOptions`
  * `TemporarySqlLocalDbInstance`
* Various methods on previous classes and interfaces are now extension methods.
* Configuration is now via the `SqlLocalDbOptions` class for use with the `SqlLocalDbApi` class.
* Uses .NET Standard logging abstractions for logging rather than `TraceSource`.
* Now compiled using the .NET Core SDK.
* Tests now use xunit instead of MSTest.

### Added

* Added new `bool InstanceExists(string)` method to `ISqlLocalDbApi`.
* Add support for registration with the built-in ASP.NET Core service container.
* Added new [sample Todo list ASP.NET Core web application](https://github.com/martincostello/sqllocaldb/tree/main/samples "Samples") that uses SQL Server LocalDB.
* [Continuous integration using Travis CI](https://travis-ci.com/martincostello/sqllocaldb) for Linux and macOS.
* [Continuous integration using Azure Pipelines](https://dev.azure.com/martincostello/sqllocaldb/_build?definitionId=66) for Windows, Linux and macOS.

### Fixed/Improved

* Improved behaviour when testing for the existence of LocalDB instances.
* The SQL Server LocalDB Instance API can now be unloaded from the process.
* Logging now uses structured logging and specific event IDs.
* Several types now override `ToString()` to return object names.
* Improved behaviour if used in an application on Linux or macOS.
* Simplified build infrastructure using the .NET Core SDK.
* Use various new C# 7.2 language features to simplify the code.
* Uses `Version.TryParse()` internally.
* Added assets for working on the project in Visual Studio Code.

### Removed

* Removed the following types:
  * `ISqlLocalDbInstance`
  * `SqlLocalDbApi` (static class)
* Configuration no longer via `System.Configuration` and `.config` files.
* Removed old "Blog" sample application.
* Removed Coverity scan integration.

## SqlLocalDb v3.0.0

### Changes

* Depend on `Microsoft.Data.SqlClient` instead of `System.Data.SqlClient`.
* Add nullable reference type annotations.
* Change constants in the `SqlLocalDbErrors` class to fields.

## SqlLocalDb v3.0.1

### Changes

* Support deterministics and reproducible builds
* Publish symbol packages to NuGet.org.
* Use GitHub Actions for CI.
* Prepare for .NET 6.

## SqlLocalDb v3.1.0

### Added

* Add `net6.0` Target Framework Moniker.

## SqlLocalDb v3.2.0

### Added

* Fix incorrect minimum dependency versions when targeting `net6.0`.

## SqlLocalDb v3.3.0

### Added

* Add `net8.0` Target Framework Moniker.

## SqlLocalDb v3.4.0

### Changes

* Update Microsoft.Data.SqlClient dependency versions to `2.1.7` for
  `netstandard2.0`/`net6.0` and `5.1.5` for `net8.0` to include fixes for
  [CVE-2024-0056](https://github.com/advisories/GHSA-98g6-xh36-x2p7).
* Use interop methods that are compatible with .NET native AoT.

## SqlLocalDb v4.0.0

### Changes

* Update Microsoft.Data.SqlClient dependency to
  [6.0.2](https://github.com/dotnet/SqlClient/releases/tag/v6.0.2).
* Drop support for `netstandard2.0` and `net6.0`.
* Add support for `net472`.
* Set `IsAotCompatible=true` for `net8.0`.
