# SQL LocalDB Wrapper Samples

This folder contains a sample application for showing usage of `MartinCostello.SqlLocalDb`.

## TodoApp

_TodoApp_ is an [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/ "Introduction to ASP.NET Core") MVC application that uses [EntityFramework Core](https://docs.microsoft.com/en-us/ef/core/ "Entity Framework Core") to manage a simple Todo list.

The list is stored in a SQL Server database backed by a SQL Server LocalDB Instance on the local machine.

The application is configured to create the LocalDB instance if it does not already exist, start it if necessary, and then configure the EntityFramework data context with the connection string for the LocalDB instance.

## TodoApp.Tests

A test project for the _TodoApp_ application that uses the `TemporarySqlLocalDbInstance` class in `MartinCostello.SqlLocalDb` to setup a temporary SQL database to run functional tests against the TodoApp's data-access logic.

The test sets up a test instance, database and schema, then tests the query, create, update and delete functionality of the `TodoRepository` class.

Once the test is run, the test database and LocalDB instance are deleted, also cleaning up the instance's files from the local machine.
