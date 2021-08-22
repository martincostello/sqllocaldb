// Copyright (c) Martin Costello, 2012-2018. All rights reserved.
// Licensed under the Apache 2.0 license. See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using MartinCostello.SqlLocalDb;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// A class containing extension methods for the <see cref="IServiceCollection"/> interface. This class cannot be inherited.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public static class SqlLocalDbServiceCollectionExtensions
{
    /// <summary>
    /// Adds SQL Server LocalDB services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> passed as the value of <paramref name="services"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="services"/> is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection AddSqlLocalDB(this IServiceCollection services)
        => services.AddSqlLocalDB((_) => { });

    /// <summary>
    /// Adds SQL Server LocalDB services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configure">A delegate to a method to use to configure the SQL Server LocalDB options.</param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> passed as the value of <paramref name="services"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="services"/> or <paramref name="configure"/> is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection AddSqlLocalDB(this IServiceCollection services, Action<SqlLocalDbOptions> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        return services.AddSqlLocalDB(
            (_) =>
            {
                var options = new SqlLocalDbOptions();

                configure(options);

                return options;
            });
    }

    /// <summary>
    /// Adds SQL Server LocalDB services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="configure">A delegate to a method to use to configure the SQL Server LocalDB options.</param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> passed as the value of <paramref name="services"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="services"/> or <paramref name="configure"/> is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection AddSqlLocalDB(this IServiceCollection services, Func<IServiceProvider, SqlLocalDbOptions> configure)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (configure == null)
        {
            throw new ArgumentNullException(nameof(configure));
        }

        services.TryAddSingleton(configure);

        services.TryAddSingleton<ISqlLocalDbApi>(
            (p) =>
            {
                var options = p.GetRequiredService<SqlLocalDbOptions>();
                var loggerFactory = p.GetRequiredService<ILoggerFactory>();

                return new SqlLocalDbApi(options, loggerFactory);
            });

        return services;
    }

    /// <summary>
    /// Adds SQL Server LocalDB services.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="options">The SQL Server LocalDB options to use.</param>
    /// <returns>
    /// The <see cref="IServiceCollection"/> passed as the value of <paramref name="services"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="services"/> or <paramref name="options"/> is <see langword="null"/>.
    /// </exception>
    public static IServiceCollection AddSqlLocalDB(this IServiceCollection services, SqlLocalDbOptions options)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        return services.AddSqlLocalDB((_) => options);
    }
}
