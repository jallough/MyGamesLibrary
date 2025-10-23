using Server.Users.Entities;

namespace Server.Users.Services
{
    public interface IUsersService
    {
        Task AddUser(UserEntity userEntity);
        Task UpdateUser(UserEntity userEntity);
        Task<UserDto?> Login(UserEntity userEntity);
        Task DeleteUser(UserEntity user);
    }
}