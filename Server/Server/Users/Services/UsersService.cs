using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Server.Models;
using Server.Users.Entities;
using Server.Users.Repositories;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Server.Users.Services
{
    public class UsersService(IUsersRepository _usersRepository, ILogger<UsersService> _logger, IMapper _mapper, IConfiguration _config) : IUsersService
    {
        public async Task AddUser(UserDto userDto)
        {
            try
            {
                var userEntity = _mapper.Map<UserEntity>(userDto);
                _logger.LogInformation("Adding a new user: {Username}", userEntity.Username);
                HashPassword(ref userEntity);
                userEntity.RefreshToken = GenerateRefreshToken();
                userEntity.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                await _usersRepository.AddUser(userEntity);
                _logger.LogInformation("User added successfully: {Username}", userEntity.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user: {Username}", userDto.Username);
                throw;
            }
        }
        public async Task UpdateUser(UserDto userEntity)
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
                _mapper.Map(userEntity, existingUser);
                existingUser.RefreshToken =  GenerateRefreshToken();
                existingUser.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
                await _usersRepository.UpdateUser(existingUser);
                _logger.LogInformation("User updated successfully: {Username}", userEntity.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user: {Username}", userEntity.Username);
                throw;
            }
        }

        public async Task<TokenResponseDto?> Login(UserDto userDto)
        {
            try
            {
                _logger.LogInformation("Attempting login for user: {Username}", userDto.Username);
                var user = _mapper.Map<UserEntity>(userDto);
                var existingUser = await _usersRepository.Login(user);
                if (existingUser == null)
                {
                    _logger.LogWarning("Login failed for user: {Username}", user.Username);
                    return null;
                }

                if (!VerifyPassword(existingUser, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid password for user: {Username}", user.Username);
                    return null;
                }

                TokenResponseDto response = await CreateTokenResponse(existingUser);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user: {Username}", userDto.Username);
                throw;
            }
        }

        private async Task<TokenResponseDto> CreateTokenResponse(UserEntity existingUser)
        {
            return new TokenResponseDto
            {
                AccessToken = GenerateToken(existingUser),
                RefreshToken = await GenerateAndSaveRefreshToken(existingUser)
            };
        }

        public async Task DeleteUser(UserDto userDto)
        {
            try
            {
                var user = _mapper.Map<UserEntity>(userDto);
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
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", userDto.Id);
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
        private async Task<UserEntity> ValidateRefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequest)
        {
            var user = await _usersRepository.GetUserById(refreshTokenRequest.UserId);
            if (user == null || user.RefreshToken != refreshTokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return null;
            }
            return user;
        }
        private async Task<string> GenerateAndSaveRefreshToken(UserEntity user)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            await _usersRepository.UpdateUser(user);
            return refreshToken;
        }
        private string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto refreshTokenRequest)
        {
            var user = await ValidateRefreshTokenAsync(refreshTokenRequest);
            if (user == null)
            {
                return null;
            }
            return await CreateTokenResponse(user);
        }
    }
}
