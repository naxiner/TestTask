using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestTask.Models.DTO;
using TestTask.Repositories;

namespace TestTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;

        public TaskController(IUserRepository userRepository, ITaskRepository taskRepository)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
        }
        
        private Guid GetUserIdFromToken()
        {
            var test = User.Claims;

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");

            if (userIdClaim == null)
            {
                throw new Exception("User ID claim not found");
            }

            return Guid.Parse(userIdClaim?.Value);
        }

        [Authorize]
        [HttpPost("add")]
        public async Task<IActionResult> AddTask(TaskDto taskDto)
        {
            var userId = GetUserIdFromToken();

            var task = new Models.Task
            {
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                UserId = userId,
            };

            var result = await _taskRepository.AddAsync(task);

            if (result == false)
            {
                return BadRequest("Failed to add task.");
            }
            return Ok("Task added successfully.");
        }
    }
}
