// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using MartinCostello.SqlLocalDb;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using TodoApp.Data;
using TodoApp.Services;

namespace TodoApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSqlLocalDB();

            services.AddSingleton<IClock>((_) => SystemClock.Instance);
            services.AddScoped<ITodoRepository, TodoRepository>();
            services.AddScoped<ITodoService, TodoService>();

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddDbContext<TodoContext>(AddTodoContext);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseMvcWithDefaultRoute();

            // Ensure that the database and schema exists
            TodoInitializer.Initialize(app.ApplicationServices);
        }

        private static void AddTodoContext(IServiceProvider serviceProvider, DbContextOptionsBuilder options)
        {
            // Check that SQL Server LocalDB is installed
            ISqlLocalDbApi localDB = serviceProvider.GetRequiredService<ISqlLocalDbApi>();

            if (!localDB.IsLocalDBInstalled())
            {
                throw new NotSupportedException("SQL LocalDB is not installed.");
            }

            // Get the configured SQL LocalDB instance to store the TODO items in, creating it if it does not exist
            IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();
            ISqlLocalDbInstanceInfo instance = localDB.GetOrCreateInstance(config["SqlLocalDbInstance"]);

            // Ensure that the SQL LocalDB instance is running and start it if not already running
            if (!instance.IsRunning)
            {
                instance.Manage().Start();
            }

            // Get the SQL connection string to use to connect to the LocalDB instance
            string connectionString = instance.GetConnectionString();
            options.UseSqlServer(connectionString);
        }
    }
}
