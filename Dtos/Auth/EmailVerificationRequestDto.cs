
using System.ComponentModel.DataAnnotations;

public class EmailVerificationRequestDto
{

    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

}

