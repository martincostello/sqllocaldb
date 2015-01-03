// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   Program.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Data.Common;
using System.Data.Entity;
using System.Linq;

namespace System.Data.SqlLocalDb.EFSample
{
    /// <summary>
    /// An application that acts a sample for showing use of the <c>System.Data.SqlLocalDb</c>
    /// assembly with <c>EntityFramework</c> Code-First.  This class cannot be inherited.
    /// </summary>
    internal static class Program
    {
        /// <summary>
        /// The main entry point to the application.
        /// </summary>
        internal static void Main()
        {
            // Get or create an instance of SQL Local DB to use for our blogging context
            ISqlLocalDbProvider provider = new SqlLocalDbProvider();
            ISqlLocalDbInstance instance = provider.GetOrCreateInstance("BloggingDb");

            try
            {
                // Start the instance so it can accept requests
                instance.Start();

                try
                {
                    // Get the connection string to use to connect to the instance
                    DbConnectionStringBuilder builder = instance.CreateConnectionStringBuilder();

                    // Update the connection string to specify the name of the database
                    // and its physical location to the current application directory
                    builder.SetInitialCatalogName("Blog");
                    builder.SetPhysicalFileName(@".\Blog.mdf");

                    // Force EntityFramework to create the database
                    InitializeDatabase(builder);

                    // Connect to the database and add some content
                    using (var context = new BlogDbContext(builder.ConnectionString))
                    {
                        using (var transaction = context.Database.BeginTransaction())
                        {
                            try
                            {
                                Blog blog = new Blog()
                                {
                                    Name = "John Smith's Blog",
                                };

                                Post post = new Post()
                                {
                                    Blog = blog,
                                    Title = "My First Blog Post",
                                    Content = "This is my first blog post.",
                                    PostedAt = DateTime.UtcNow,
                                };

                                context.Blogs.Add(blog);
                                context.Posts.Add(post);

                                context.SaveChanges();
                                transaction.Commit();
                            }
                            catch (Exception)
                            {
                                transaction.Rollback();
                                throw;
                            }
                        }
                    }

                    // Use a new context to the same connection string to show that the data was persisted
                    using (var context = new BlogDbContext(builder.ConnectionString))
                    {
                        var title = context.Blogs
                            .Where((p) => p.Name == "John Smith's Blog")
                            .SelectMany((p) => p.Posts)
                            .OrderBy((p) => p.PostedAt)
                            .Select((p) => p.Title)
                            .First();

                        Console.WriteLine("The first blog post's title is: {0}", title);
                    }

                    // Delete the database from the instance
                    DeleteDatabase(builder);
                }
                finally
                {
                    instance.Stop();
                }
            }
            finally
            {
                // Tidy up
                SqlLocalDbApi.DeleteInstance(instance.Name);
            }

            Console.WriteLine("Press any key to exit...");
            Console.Read();
        }

        /// <summary>
        /// Deletes the database associated with the specified <see cref="DbConnectionStringBuilder"/>.
        /// </summary>
        /// <param name="builder">The connection string to delete the database for.</param>
        private static void DeleteDatabase(DbConnectionStringBuilder builder)
        {
            using (var context = new BlogDbContext(builder.ConnectionString))
            {
                context.Database.Delete();
            }
        }

        /// <summary>
        /// Forces the initialization of the database specified in a <see cref="DbConnectionStringBuilder"/>.
        /// </summary>
        /// <param name="builder">The connection string to initialize the database for.</param>
        /// <remarks>
        /// If the database already exists, it is deleted.
        /// </remarks>
        private static void InitializeDatabase(DbConnectionStringBuilder builder)
        {
            var strategy = new DropCreateDatabaseAlways<BlogDbContext>();
            Database.SetInitializer(strategy);

            using (var context = new BlogDbContext(builder.ConnectionString))
            {
                context.Database.Initialize(true);
            }
        }
    }
}