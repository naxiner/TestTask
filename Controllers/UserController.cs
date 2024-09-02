using Microsoft.AspNetCore.Mvc;
using TestTask.Data;
using TestTask.Models;
using TestTask.Models.DTO;
using TestTask.Repositories;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto userDto)
        {
            if (_userRepository.GetByNameAsync(userDto.Username) != null || 
                _userRepository.GetByEmailAsync(userDto.Email) != null)
            {
                return BadRequest("Username or Email already exists.");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.Add(user);

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            return Ok();
        }
    }
}
