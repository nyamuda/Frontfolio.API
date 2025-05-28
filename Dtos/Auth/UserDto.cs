namespace Frontfolio.API.Dtos.Auth
{
    public class UserDto
    {
        public required int Id { get; set; }

        public required string Name { get; set; }
        public required string Email { get; set; }

        public required bool isVerified { get; set; }

        public required UserRole Role { get; set; }


        public static UserDto MapFrom(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                isVerified = user.isVerified
            };
        }
    }
}
