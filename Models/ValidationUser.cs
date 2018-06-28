using System.ComponentModel.DataAnnotations;
using LoginReg.Models;

namespace LoginReg
{
    public class ValidUser
    {
        public int UserID { get; set; }

        [Required]
        [DataType("string")]
        [MinLength(2)]
        public string FirstName { get; set; }

        [Required]
        [DataType("string")]
        [MinLength(2)]
        public string LastName { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        public string Username { get; set; }

        [Required]
        [DataType("string")]
        [MinLength(8)]
        public string Password { get; set; }

        [Required]
        [MinLength(8)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}