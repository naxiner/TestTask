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
        private readonly IJwtProvider _jwtProvider;

        public UserController(IUserRepository userRepository, IJwtProvider jwtProvider)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto userDto)
        {
            if (await _userRepository.GetByNameAsync(userDto.Username) != null || 
                await _userRepository.GetByEmailAsync(userDto.Email) != null)
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

            await _userRepository.AddAsync(user);

            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByNameOrEmailAsync(loginDto.UsernameOrEmail);
            
            if (user == null)
            {
                return BadRequest("Username or Email already exists.");
            }

            var result = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (result == false)
            {
                throw new Exception("Failed to login");
            }

            var token = _jwtProvider.GenerateToken(user);

            HttpContext.Response.Cookies.Append("token-cookie", token);

            return Ok(token);
        }
    }
}
