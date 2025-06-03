
using System.ComponentModel.DataAnnotations;

public class ResetPasswordDto
{
    [Required]
    public string ResetToken { get; set; } = default!;
    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
       ErrorMessage = "Password must be at least 8 characters long and contain a mix of letters, numbers, and special characters")]
    public string Password { get; set; } = default!;
}

