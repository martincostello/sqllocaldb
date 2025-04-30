# SQL LocalDB Wrapper Samples

This folder contains a sample application for showing usage of `MartinCostello.SqlLocalDb`.

## TodoApp

_TodoApp_ is an [ASP.NET][aspnetcore] MVC application that uses [Entity Framework Core][efcore] to manage a simple Todo list.

The list is stored in a SQL Server database backed by a SQL Server LocalDB Instance on the local machine.

The application is configured to create the LocalDB instance if it does not already exist, start it if
necessary, and then [configure the Entity Framework data context][configure] with the connection string for the LocalDB instance.

## TodoApp.Tests

A test project for the _TodoApp_ application that uses the `TemporarySqlLocalDbInstance` class in `MartinCostello.SqlLocalDb`
to [setup a temporary SQL database][temporary-instance] to run functional tests against the TodoApp's data-access logic.

The test sets up a test instance, database and schema, then tests the query, create, update and delete functionality of the `TodoRepository` class.

Once the test is run, the test database and LocalDB instance are deleted, also cleaning up the instance's files from the local machine.

[aspnetcore]: https://learn.microsoft.com/aspnet/core/ "Introduction to ASP.NET"
[efcore]: https://learn.microsoft.com/ef/core/ "Entity Framework Core"
[configure]: https://github.com/martincostello/sqllocaldb/blob/a3c3a0362e07670e46746430a4102d2cf63ea48b/samples/TodoApp/Program.cs#L19-L42
[temporary-instance]: https://github.com/martincostello/sqllocaldb/blob/a3c3a0362e07670e46746430a4102d2cf63ea48b/samples/TodoApp.Tests/TodoRepositoryTests.cs#L34
