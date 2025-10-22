using Server.Users.Entities;

namespace Server.Users.Repositories
{
    public interface IUsersRepository
    {
        Task AddUser(UserEntity userEntity);
        Task UpdateUser(UserEntity userEntity);
        Task<UserEntity?> Login(UserEntity userEntity);
        Task DeleteUser(long userId);
        Task<UserEntity?> GetUserById(long userId);
    }
}