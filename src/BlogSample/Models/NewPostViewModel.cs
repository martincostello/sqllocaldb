using System.ComponentModel.DataAnnotations;

namespace BlogSample.Models
{
    public class NewPostViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Body { get; set; }

        public string Preview { get; set; }
    }
}
