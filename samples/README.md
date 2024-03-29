# SQL LocalDB Wrapper Samples

This folder contains a sample application for showing usage of `MartinCostello.SqlLocalDb`.

## TodoApp

_TodoApp_ is an [ASP.NET](https://learn.microsoft.com/aspnet/core/ "Introduction to ASP.NET") MVC application that uses [Entity Framework Core](https://learn.microsoft.com/ef/core/ "Entity Framework Core") to manage a simple Todo list.

The list is stored in a SQL Server database backed by a SQL Server LocalDB Instance on the local machine.

The application is configured to create the LocalDB instance if it does not already exist, start it if necessary, and then [configure the Entity Framework data context](https://github.com/martincostello/sqllocaldb/blob/ef6c1e2918ce084274cbc1e7d173371a7fbaebd3/samples/TodoApp/Startup.cs#L62-L85 "View Startup.cs") with the connection string for the LocalDB instance.

## TodoApp.Tests

A test project for the _TodoApp_ application that uses the `TemporarySqlLocalDbInstance` class in `MartinCostello.SqlLocalDb` to [setup a temporary SQL database](https://github.com/martincostello/sqllocaldb/blob/a6c901eec68c05c78ad26eef7c41bc2fc37e564f/samples/TodoApp.Tests/TodoRepositoryTests.cs#L36 "View TodoRepositoryTests.cs") to run functional tests against the TodoApp's data-access logic.

The test sets up a test instance, database and schema, then tests the query, create, update and delete functionality of the `TodoRepository` class.

Once the test is run, the test database and LocalDB instance are deleted, also cleaning up the instance's files from the local machine.
