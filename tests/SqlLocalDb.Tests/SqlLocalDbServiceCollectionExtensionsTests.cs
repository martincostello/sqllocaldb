// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Shouldly;
using Xunit;

namespace MartinCostello.SqlLocalDb
{
    public static class SqlLocalDbServiceCollectionExtensionsTests
    {
        [Fact]
        public static void AddSqlLocalDB_Validates_Parameters()
        {
            // Arrange
            IServiceCollection? services = Mock.Of<IServiceCollection>();

            SqlLocalDbOptions? options = null;
            Action<SqlLocalDbOptions>? configureAction = null;
            Func<IServiceProvider, SqlLocalDbOptions>? configureFunc = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("options", () => services.AddSqlLocalDB(options!));
            Assert.Throws<ArgumentNullException>("configure", () => services.AddSqlLocalDB(configureAction!));
            Assert.Throws<ArgumentNullException>("configure", () => services.AddSqlLocalDB(configureFunc!));

            services = null;

            // Act and Assert
            Assert.Throws<ArgumentNullException>("services", () => services!.AddSqlLocalDB());
            Assert.Throws<ArgumentNullException>("services", () => services!.AddSqlLocalDB(options!));
            Assert.Throws<ArgumentNullException>("services", () => services!.AddSqlLocalDB(configureAction!));
            Assert.Throws<ArgumentNullException>("services", () => services!.AddSqlLocalDB(configureFunc!));
        }

        [Fact]
        public static void AddSqlLocalDB_Registers_Services_Using_Defaults()
        {
            // Arrange
            IServiceCollection services = CreateServiceCollection();

            // Act
            services.AddSqlLocalDB();

            // Assert
            IServiceProvider provider = services.BuildServiceProvider();

            var options = provider.GetRequiredService<SqlLocalDbOptions>();
            options.ShouldNotBeNull();

            var localDB = provider.GetRequiredService<ISqlLocalDbApi>();
            localDB.ShouldNotBeNull();
            localDB.ShouldBeOfType<SqlLocalDbApi>();
        }

        [Fact]
        public static void AddSqlLocalDB_Registers_Services_Using_Options()
        {
            // Arrange
            var options = new SqlLocalDbOptions()
            {
                StopOptions = (StopInstanceOptions)int.MinValue,
            };

            IServiceCollection services = CreateServiceCollection();

            // Act
            services.AddSqlLocalDB(options);

            // Assert
            IServiceProvider provider = services.BuildServiceProvider();

            var actualOptions = provider.GetRequiredService<SqlLocalDbOptions>();
            options.ShouldBeSameAs(actualOptions);

            var localDB = provider.GetRequiredService<ISqlLocalDbApi>();
            localDB.ShouldNotBeNull();
            localDB.ShouldBeOfType<SqlLocalDbApi>();
        }

        [Fact]
        public static void AddSqlLocalDB_Registers_Services_Using_Action()
        {
            // Arrange
            IServiceCollection services = CreateServiceCollection();

            // Act
            services.AddSqlLocalDB((p) => p.StopOptions = (StopInstanceOptions)int.MinValue);

            // Assert
            IServiceProvider provider = services.BuildServiceProvider();

            var options = provider.GetRequiredService<SqlLocalDbOptions>();
            options.ShouldNotBeNull();
            options.StopOptions.ShouldBe((StopInstanceOptions)int.MinValue);

            var localDB = provider.GetRequiredService<ISqlLocalDbApi>();
            localDB.ShouldNotBeNull();
            localDB.ShouldBeOfType<SqlLocalDbApi>();
        }

        [Fact]
        public static void AddSqlLocalDB_Registers_Services_Using_Function()
        {
            // Arrange
            IServiceCollection services = CreateServiceCollection();

            // Act
            services.AddSqlLocalDB((p) => new SqlLocalDbOptions() { StopOptions = (StopInstanceOptions)int.MinValue });

            // Assert
            IServiceProvider provider = services.BuildServiceProvider();

            var options = provider.GetRequiredService<SqlLocalDbOptions>();
            options.ShouldNotBeNull();
            options.StopOptions.ShouldBe((StopInstanceOptions)int.MinValue);

            var localDB = provider.GetRequiredService<ISqlLocalDbApi>();
            localDB.ShouldNotBeNull();
            localDB.ShouldBeOfType<SqlLocalDbApi>();
        }

        [Fact]
        public static void AddSqlLocalDB_Does_Not_Overwrite_Existing_Services()
        {
            // Arrange
            IServiceCollection services = CreateServiceCollection();

            var existingOptions = new SqlLocalDbOptions();
            var existingLocalDB = Mock.Of<ISqlLocalDbApi>();

            services.AddTransient((_) => existingOptions);
            services.AddTransient((_) => existingLocalDB);

            // Act
            services.AddSqlLocalDB();

            // Assert
            IServiceProvider provider = services.BuildServiceProvider();

            var actualOptions = provider.GetRequiredService<SqlLocalDbOptions>();
            actualOptions.ShouldBeSameAs(existingOptions);

            var actualLocalDB = provider.GetRequiredService<ISqlLocalDbApi>();
            actualLocalDB.ShouldBeSameAs(existingLocalDB);
        }

        private static IServiceCollection CreateServiceCollection()
            => new ServiceCollection().AddLogging();
    }
}
