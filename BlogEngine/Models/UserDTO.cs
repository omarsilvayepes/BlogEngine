using System.ComponentModel.DataAnnotations;

namespace BlogEngine.Models
{
    public class UserDTO
    {
        [Required, MinLength(3)]
        public string UserName { get; set; }
        [Required, MinLength(3)]
        public string PassWord { get; set; }
        [Required, MinLength(3)]
        public string Role { get; set; }

    }
}
