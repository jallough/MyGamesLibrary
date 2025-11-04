using Microsoft.EntityFrameworkCore;
using Server.DBContext;
using Server.Users.Entities;

namespace Server.Users.Repositories
{
    public class UsersRepository(IDataContext _dbContext) : IUsersRepository
    {
        public async Task AddUser(UserEntity userEntity)
        {
            try {
                var existingUser = await _dbContext.Users
                    .FirstOrDefaultAsync(u => u.Username == userEntity.Username || u.Email == userEntity.Email);
                if (existingUser != null)
                {
                    throw new InvalidOperationException("User with the same username or email already exists.");
                }
                _dbContext.Users.Add(userEntity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding user: {ex.Message}");
                throw;
            }
            
        }

        public async Task UpdateUser(UserEntity userEntity)
        {
            try { 
                _dbContext.Users.Update(userEntity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw;
            }
        }

        public async Task<UserEntity?> Login(UserEntity user)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == user.Username || u.Email == user.Email);
        }

        public async Task DeleteUser(long userId)
        {
            var user = await _dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                _dbContext.Users.Remove(user);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception("User not found");
            }
        }
        public async Task<UserEntity?> GetUserById(long userId)
        {
            return await _dbContext.Users.FindAsync(userId);
        }
    }
}
