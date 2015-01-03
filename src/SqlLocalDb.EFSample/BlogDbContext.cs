// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BlogDbContext.cs" company="https://github.com/martincostello/sqllocaldb">
//   Martin Costello (c) 2012-2015
// </copyright>
// <license>
//   See license.txt in the project root for license information.
// </license>
// <summary>
//   BlogDbContext.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace System.Data.SqlLocalDb
{
    [DbConfigurationType(typeof(BlogDbConfiguration))]
    public class BlogDbContext : DbContext
    {
        public BlogDbContext()
            : base()
        {
        }

        public BlogDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
}