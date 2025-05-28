
public class UserOtp
{
    public int Id { get; set; }

    public string Email { get; set; } = default!;

    public string OtpCode { get; set; }= default!;

    public DateTime ExpirationTime { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int UserId { get; set; }

    public User User { get; set; } = default!;
}

