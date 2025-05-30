
using System.ComponentModel.DataAnnotations;

public class VerifyEmailDto
    {
    [Required]
    [MinLength(6)]
    [MaxLength(6)]
    public string OtpCode { get; set; } = default!;
    }

