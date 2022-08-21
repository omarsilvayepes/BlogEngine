using System.ComponentModel.DataAnnotations;

namespace BlogEngine.Models
{
    public class Comment
    {
        [Required]
        public string IdPost { get; set; }
        public DateTime Date { get; set; }
        [Required]
        public string comment { get; set; }
        [Required]
        public string Author { get; set; }
    }
}
