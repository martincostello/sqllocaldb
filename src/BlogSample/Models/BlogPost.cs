using System;
using System.ComponentModel.DataAnnotations;

namespace BlogSample.Models
{
    public class BlogPost
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        [Required]
        public DateTime PublishedAt { get; set; }

        public string Preview { get; set; }
    }
}
