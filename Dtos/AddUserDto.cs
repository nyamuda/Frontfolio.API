namespace Frontfolio.API.Dtos
{
    public class AddUserDto
    {
        public string Name { get; set; } = default!;
        public string Email { get; set; } =default!;
        public string Password { get; set; } = default!;
    }
}
