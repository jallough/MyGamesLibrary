using Server.Models;
using Server.Users.Entities;

namespace Server.Users.Services
{
    public interface IUsersService
    {
        Task AddUser(UserEntity userEntity);
        Task UpdateUser(UserEntity userEntity);
        Task<TokenResponseDto?> Login(UserDto userDto);
        Task DeleteUser(UserEntity user);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto refreshTokenRequest);
    }
}