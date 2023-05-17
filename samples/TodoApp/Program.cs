// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

#pragma warning disable CA1852

using MartinCostello.SqlLocalDb;
using Microsoft.EntityFrameworkCore;
using NodaTime;
using TodoApp.Data;
using TodoApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSqlLocalDB();

builder.Services.AddSingleton<IClock>((_) => SystemClock.Instance);
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
builder.Services.AddScoped<ITodoService, TodoService>();

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TodoContext>((serviceProvider, options) =>
{
    // Check that SQL Server LocalDB is installed
    ISqlLocalDbApi localDB = serviceProvider.GetRequiredService<ISqlLocalDbApi>();

    if (!localDB.IsLocalDBInstalled())
    {
        throw new NotSupportedException("SQL LocalDB is not installed.");
    }

    // Get the configured SQL LocalDB instance to store the TODO items in, creating it if it does not exist
    IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();
    ISqlLocalDbInstanceInfo instance = localDB.GetOrCreateInstance(config["SqlLocalDbInstance"] ?? string.Empty);

    // Ensure that the SQL LocalDB instance is running and start it if not already running
    if (!instance.IsRunning)
    {
        instance.Manage().Start();
    }

    // Get the SQL connection string to use to connect to the LocalDB instance
    string connectionString = instance.GetConnectionString();
    options.UseSqlServer(connectionString);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseStaticFiles();
app.UseRouting();
app.MapDefaultControllerRoute();

// Ensure that the database and schema exists
TodoInitializer.Initialize(app.Services);

app.Run();
