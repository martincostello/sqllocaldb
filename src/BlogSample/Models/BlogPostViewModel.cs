using System;

namespace BlogSample.Models
{
    public class BlogPostViewModel
    {
        public string Body { get; set; }

        public int Id { get; set; }

        public DateTime PublishedAt { get; set; }

        public string Preview { get; set; }

        public string Title { get; set; }
    }
}
