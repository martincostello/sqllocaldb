using System;
using System.Data.Entity;

namespace BlogSample.Models
{
    public class BlogInitializer : DropCreateDatabaseIfModelChanges<BlogContext>
    {
        protected override void Seed(BlogContext context)
        {
            if (context != null)
            {
                DateTime now = DateTime.UtcNow;

                context.Posts.Add(new BlogPost() { Body = "I've started a blog. Watch this space for exciting posts!", Preview = "This is my first blog post...", Title = "First Post!", PublishedAt = now });
                context.Posts.Add(new BlogPost() { Body = "This blog post will teach you about how to use SQL Local DB Wrapper to test data access code with MSTest for an ASP.NET MVC website.", Preview = "How to use SQL LocalDB Wrapper to test your data access code.", Title = "Testing With SQL Server LocalDB Wrapper", PublishedAt = now.AddSeconds(1) });

                context.SaveChanges();
            }
        }
    }
}
