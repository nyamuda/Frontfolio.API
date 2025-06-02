
using System.ComponentModel.DataAnnotations;

public class PasswordResetRequestDto
{

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;
}


