# Coding Agent Instructions

This file provides guidance to coding agents when working with code in this repository.

## Build, test, and lint commands

- Prefer `.\build.ps1` for validation. It bootstraps the SDK from `global.json`, packs `src\SqlLocalDb\MartinCostello.SqlLocalDb.csproj`, and then runs the test suite.
- Use `.\build.ps1 -SkipTests` when you only need a packaging/build check.
- `dotnet test --configuration Release` matches the test configuration used by `build.ps1`.
- For a single test or filtered run, disable coverage or the run will fail the repository's coverage thresholds before the filtered result is useful. Verified pattern:

  ```powershell
  dotnet test .\tests\SqlLocalDb.Tests\MartinCostello.SqlLocalDb.Tests.csproj --configuration Release -f net10.0 --filter "FullyQualifiedName~MartinCostello.SqlLocalDb.AssemblyTests" /p:CollectCoverage=false
  ```

- `dotnet build .\SqlLocalDb.slnx -c Release` is the quickest local way to run the C# analyzers wired in through the solution and shared props.
- PowerShell linting does not have a dedicated script; the repo uses the command embedded in `.github\workflows\lint.yml`:

  ```powershell
  $settings = @{
    IncludeDefaultRules = $true
    Severity = @("Error", "Warning")
  }
  Invoke-ScriptAnalyzer -Path . -Recurse -ReportSummary -Settings $settings
  ```

- Workflow and markdown linting are defined in `.github\workflows\lint.yml` with `actionlint`, `zizmor`, and `markdownlint-cli2`. If you touch workflows or Markdown, use that workflow as the source of truth for the expected tooling.
- Full LocalDB coverage is Windows-specific. The README notes that running the full suite with administrative privileges avoids skips for sharing-related tests.

## High-level architecture

- `src\SqlLocalDb` is the product library. `SqlLocalDbApi` is the main public facade for discovering installed LocalDB versions, creating/deleting instances, starting/stopping instances, and retrieving connection information.
- `src\SqlLocalDb\Interop` contains the Windows-specific boundary: registry access abstractions plus the native LocalDB loader/invoker in `LocalDbInstanceApi`. The public API goes through this layer rather than calling P/Invoke functions directly from the facade.
- Instance operations are split between state objects and mutators. `SqlLocalDbInstanceInfo` represents instance metadata, while `SqlLocalDbInstanceManager` performs mutations and then refreshes the in-memory state.
- Convenience extensions in `ISqlLocalDbApiExtensions`, `ISqlLocalDbInstanceInfoExtensions`, and related files provide the "pit of success" API used by the README examples, tests, and sample app.
- `TemporarySqlLocalDbInstance` is the test/integration bridge. It creates a throwaway LocalDB instance on demand, starts it automatically, and cleans it up on disposal. The sample tests and repository tests both rely on this pattern.
- `SqlLocalDbServiceCollectionExtensions` integrates the library with DI by registering `ISqlLocalDbApi`. `samples\TodoApp\Program.cs` shows the intended end-to-end usage: resolve `ISqlLocalDbApi`, ensure LocalDB is installed, get or create an instance, start it if needed, and feed the connection string into EF Core.
- `tests\SqlLocalDb.Tests` is the main behavioral suite and also doubles as documentation: `Examples.cs` mirrors supported usage patterns. `tests\SqlLocalDb.FuzzTests` is a separate fuzz/property-based test project. `samples\TodoApp.Tests` exercises the sample app against a temporary LocalDB-backed database.

## Key conventions

- This is a Windows-first library with graceful non-Windows behavior. The library multi-targets cross-platform TFMs, but most real LocalDB operations are guarded by platform checks and many tests are skipped outside Windows.
- Test execution is intentionally environment-aware. Custom attributes such as `WindowsOnlyFactAttribute`, `RunAsAdminFactAttribute`, `WindowsCIOnlyFactAttribute`, and `NotWindowsFactAttribute` encode whether a test should run on the current OS, with admin rights, or only in CI.
- Tests that mutate machine-wide LocalDB state are isolated from parallel execution with the `NotInParallel` collection. Preserve that pattern for any new test that could interfere with other instances or global LocalDB state.
- The repo's default `dotnet test` path collects coverage and enforces thresholds from the test project file. That is why filtered local test runs should usually add `/p:CollectCoverage=false`.
- Package versions and analyzers are centralized. `Directory.Packages.props` owns package versions and global analyzer packages, while `Directory.Build.props` applies shared build settings like the rule set, package metadata, and artifact output conventions.
- The repository expects changes to validate cleanly through `build.ps1` with no compiler warnings, per `.github\CONTRIBUTING.md`.
- The DI extension lives in the `Microsoft.Extensions.DependencyInjection` namespace on purpose so consumers can call `AddSqlLocalDB()` naturally. Keep that convention if DI registration changes.

## General guidelines

- Always ensure code compiles with no warnings or errors and tests pass locally before pushing changes.
- Do not change the public API unless specifically requested.
- Do not use APIs marked with `[Obsolete]`.
- Bug fixes should **always** include a test that would fail without the corresponding fix.
- Do not introduce new dependencies unless specifically requested.
- Do not update existing dependencies unless specifically requested.
