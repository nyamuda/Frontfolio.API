using System.ComponentModel.DataAnnotations;

namespace Frontfolio.API.Dtos.Auth
{
    public class AddUserDto
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = default!;

        [Required]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long and contain a mix of letters, numbers, and special characters")]
        public string Password { get; set; } = default!;
    }
}
