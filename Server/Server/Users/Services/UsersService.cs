using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Server.Users.Entities;
using Server.Users.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Server.Users.Services
{
    public class UsersService(IUsersRepository _usersRepository, ILogger<UsersService> _logger, IMapper _mapper, IConfiguration _config) : IUsersService
    {
        public async Task AddUser(UserEntity userEntity)
        {
            try
            {
                _logger.LogInformation("Adding a new user: {Username}", userEntity.Username);
                HashPassword(ref userEntity);
                await _usersRepository.AddUser(userEntity);
                _logger.LogInformation("User added successfully: {Username}", userEntity.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {Username}", userEntity.Username);
                throw;
            }
        }
        public async Task UpdateUser(UserEntity userEntity)
        {
            try
            {
                _logger.LogInformation("Updating user: {Username}", userEntity.Username);
                var existingUser = await _usersRepository.GetUserById(userEntity.Id);
                if (existingUser == null)
                {
                    _logger.LogWarning("User not found: {Username}", userEntity.Username);
                    throw new Exception("User not found");
                }
                await _usersRepository.UpdateUser(userEntity);
                _logger.LogInformation("User updated successfully: {Username}", userEntity.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Username}", userEntity.Username);
                throw;
            }
        }

        public async Task<string?> Login(UserEntity user)
        {
            try
            {
                _logger.LogInformation("Attempting login for user: {Username}", user.Username);
                var existingUser = await _usersRepository.Login(user);
                if (existingUser == null)
                {
                    _logger.LogWarning("Login failed for user: {Username}", user.Username);
                    return null;
                }
                var accepted = VerifyPassword(existingUser, user.PasswordHash);
                return accepted ? GenerateToken(existingUser) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", user.Username);
                throw;

            }
        }

        public async Task DeleteUser(UserEntity user)
        {
            try
            {
                var existingUser = await _usersRepository.Login(user);
                if(existingUser != null) { 
                    if (VerifyPassword(existingUser, user.PasswordHash))
                    {
                        _logger.LogInformation("Deleting user with ID: {UserId}", user.Id);
                        await _usersRepository.DeleteUser(user.Id);
                        _logger.LogInformation("User deleted successfully with ID: {UserId}", user.Id);
                    }
                    else
                    {
                        throw new KeyNotFoundException("Credentials are false");
                    }
                }
                else
                {
                    throw new KeyNotFoundException("Credentials are false");
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", user.Id);
                throw;
            }
        }

        private void HashPassword(ref UserEntity user)
        {
            var hashedPassword = new PasswordHasher<UserEntity>().HashPassword(user, user.PasswordHash);
            user.PasswordHash = hashedPassword;
        }
        private bool VerifyPassword(UserEntity user, string providedPassword)
        {
            var passwordVerificationResult = new PasswordHasher<UserEntity>().VerifyHashedPassword(user, user.PasswordHash, providedPassword);
            return passwordVerificationResult == PasswordVerificationResult.Success;
        }
        private string GenerateToken(UserEntity user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetValue<string>("AppSettings:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);
            var tokenDescriptor = new JwtSecurityToken(
            
                issuer: _config.GetValue<string>("AppSettings:Issuer"),
                audience: _config.GetValue<string>("AppSettings:Audience"),
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            )
            ;
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }
    }
}
