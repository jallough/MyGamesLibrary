using Server.Models;
using Server.Users.Entities;

namespace Server.Users.Services
{
    public interface IUsersService
    {
        Task AddUser(UserDto userEntity);
        Task UpdateUser(UserDto userEntity);
        Task<TokenResponseDto?> Login(UserDto userDto);
        Task DeleteUser(UserDto user);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto refreshTokenRequest);
    }
}