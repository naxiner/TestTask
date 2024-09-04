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
        private readonly ILogger<UserController> _logger;


        public UserController(
            IUserRepository userRepository,
            IJwtProvider jwtProvider,
            ILogger<UserController> logger)
        {
            _userRepository = userRepository;
            _jwtProvider = jwtProvider;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegistrationDto userDto)
        {
            if (await _userRepository.GetByNameAsync(userDto.Username) != null || 
                await _userRepository.GetByEmailAsync(userDto.Email) != null)
            {
                _logger.LogError("Username or Email already exists.");
                return BadRequest("Username or Email already exists.");
            }

            if (!IsPasswordComplex(userDto.Password))
            {
                _logger.LogError("Password requirements are not met.");
                return BadRequest("Password must be at least 8 characters long, " +
                                  "and contain an uppercase letter, a lowercase letter, " +
                                  "a digit, and a special character.");
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

            _logger.LogInformation("User registered successfully.");
            return Ok("User registered successfully.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto loginDto)
        {
            var user = await _userRepository.GetByNameOrEmailAsync(loginDto.UsernameOrEmail);
            
            if (user == null)
            {
                _logger.LogError("Username or Email already exists.");
                return BadRequest("Username or Email already exists.");
            }

            var result = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

            if (result == false)
            {
                _logger.LogError("Failed to login.");
                throw new Exception("Failed to login.");
            }

            var token = _jwtProvider.GenerateToken(user);

            HttpContext.Response.Cookies.Append("token-cookie", token);

            _logger.LogInformation("User login successfully.");
            return Ok(token);
        }

        // Check password validity
        private bool IsPasswordComplex(string password)
        {
            // minimal length
            if (password.Length < 8) return false;

            bool hasUpperCase = false, hasLowerCase = false, hasDigit = false, hasSpecialChar = false;

            foreach (var c in password)
            {
                if (char.IsUpper(c)) hasUpperCase = true;
                else if (char.IsLower(c)) hasLowerCase = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else if (!char.IsLetterOrDigit(c)) hasSpecialChar = true;
            }

            return hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar;
        }
    }
}
