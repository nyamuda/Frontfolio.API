
using Frontfolio.API.Dtos.Auth;

public interface IUserService
{
    Task<UserDto> GetAsync(int id);
}

