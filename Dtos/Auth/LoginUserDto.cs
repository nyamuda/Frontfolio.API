
using System.ComponentModel.DataAnnotations;

public class LoginUserDto
    {
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and contain a mix of letters, numbers, and special characters")]
    public string Password { get; set; } = default!;


}

