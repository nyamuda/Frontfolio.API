
public class UserOtp
{
    public int Id { get; set; }

    public required string Email { get; set; } = default!;

    public required string OtpCode { get; set; }= default!;

    public required DateTime ExpirationTime { get; set; }

    public required bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required int UserId { get; set; }

    public User? User { get; set; } 
}

