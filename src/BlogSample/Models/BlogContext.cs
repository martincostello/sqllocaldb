using System.Data.Entity;
using System.Diagnostics.CodeAnalysis;

namespace BlogSample.Models
{
    public class BlogContext : DbContext
    {
        [ExcludeFromCodeCoverage]
        public BlogContext()
            : base("name=BlogContext")
        {
        }

        public BlogContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
        }

        public virtual DbSet<BlogPost> Posts { get; set; }

        [ExcludeFromCodeCoverage]
        public static BlogContext Create()
        {
            return new BlogContext();
        }
    }
}
