using System.ComponentModel.DataAnnotations;

namespace ApiDemo01.Dto
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{6,}$",
            ErrorMessage = "password is must greater than 6 characters, at least one uppercase letter, one lowercase letter, one number and one special character")] 
        public string Password { get; set; }
    }
} 
