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
            services.AddSingleton<IClock>((_) => SystemClock.Instance);
            services.AddSingleton<ISqlLocalDbApi, SqlLocalDbApi>();
            services.AddScoped<ITodoRepository, TodoRepository>();
            services.AddScoped<ITodoService, TodoService>();

            services.AddMvc()
                    .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

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

            TodoInitializer.Initialize(app.ApplicationServices);
        }

        private static void AddTodoContext(IServiceProvider serviceProvider, DbContextOptionsBuilder options)
        {
            IConfiguration config = serviceProvider.GetRequiredService<IConfiguration>();
            ISqlLocalDbApi localDB = serviceProvider.GetRequiredService<ISqlLocalDbApi>();

            if (!localDB.IsLocalDBInstalled())
            {
                throw new NotSupportedException("SQL LocalDB is not installed.");
            }

            string instanceName = config["SqlLocalDbInstance"];
            ISqlLocalDbInstanceInfo instance = localDB.GetOrCreateInstance(instanceName);

            if (!instance.IsRunning)
            {
                var manager = new SqlLocalDbInstanceManager(instance, localDB);
                manager.Start();
            }

            string connectionString = instance.GetConnectionString();

            options.UseSqlServer(connectionString);
        }
    }
}
