using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Users.Entities;
using Server.Users.Services;

namespace Server.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUsersService _usersService) : ControllerBase
    {

        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] UserEntity userEntity)
        {
            if (userEntity == null)
            {
                return BadRequest("User data is required.");
            }
            try
            {
                await _usersService.AddUser(userEntity);
                return Ok("User added successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error adding user: {ex.Message}");
            }
        }
        [Authorize]
        [HttpPut("update")]
        public async Task<IActionResult> UpdateUser([FromBody] UserEntity userEntity)
        {
            if (userEntity == null)
            {
                return BadRequest("User data is required.");
            }
            try
            {
                await _usersService.UpdateUser(userEntity);
                return Ok("User updated successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("User not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserEntity user)
        {
            if (user == null)
            {
                return BadRequest("User data is required.");
            }
            try
            {
                var token = await _usersService.Login(user);
                if (token == null)
                {
                    return Unauthorized("Invalid username or password.");
                }
                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error during login: {ex.Message}");
            }
        }

        [Authorize]
        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromBody] UserEntity user)
        {
            try
            {
                await _usersService.DeleteUser(user);
                return Ok("User deleted successfully.");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Credentials are false");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting user: {ex.Message}");
            }
        }
    }
}
