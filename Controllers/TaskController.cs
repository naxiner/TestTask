using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestTask.Models;
using TestTask.Models.DTO;
using TestTask.Repositories;

namespace TestTask.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ILogger<TaskController> _logger;

        public TaskController(
            IUserRepository userRepository, 
            ITaskRepository taskRepository,
            ILogger<TaskController> logger)
        {
            _userRepository = userRepository;
            _taskRepository = taskRepository;
            _logger = logger;
        }
        
        // Get token from claims
        private Guid GetUserIdFromToken()
        {
            var test = User.Claims;

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userId");

            if (userIdClaim == null)
            {
                _logger.LogError("User ID claim not found.");
                throw new Exception("User ID claim not found.");
            }

            return Guid.Parse(userIdClaim?.Value);
        }

        [HttpPost]
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
                _logger.LogError("Failed to add task.");
                return BadRequest("Failed to add task.");
            }

            _logger.LogInformation("Task added successfully.");
            return Ok("Task added successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilter filter)
        {
            var userId = GetUserIdFromToken();

            var tasks = await _taskRepository.GetAllByUserIdAsync(userId, filter);
            if (tasks == null)
            {
                _logger.LogWarning("No tasks found for user.");
                return NotFound("Tasks not found.");
            }

            _logger.LogInformation("Tasks retrieved successfully for user.");
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var userId = GetUserIdFromToken();

            var task = await _taskRepository.GetByIdAsync(id, userId);
            if (task == null)
            {
                _logger.LogWarning("Task not found.");
                return NotFound("Task not found.");
            }

            _logger.LogInformation("Task retrieved successfully.");
            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskDto updatedTaskDto)
        {
            var userId = GetUserIdFromToken();

            var existingTask = await _taskRepository.GetByIdAsync(id, userId);
            if (existingTask == null)
            {
                _logger.LogWarning("Task not found.");
                return NotFound("Task not found.");
            }

            existingTask.Title = updatedTaskDto.Title;
            existingTask.Description = updatedTaskDto.Description;
            existingTask.DueDate = updatedTaskDto.DueDate;
            existingTask.Status = updatedTaskDto.Status;
            existingTask.Priority = updatedTaskDto.Priority;
            existingTask.UpdatedAt = DateTime.Now;

            var result = await _taskRepository.UpdateAsync(existingTask);

            if (!result)
            {
                _logger.LogError("Failed to update task.");
                return BadRequest("Failed to update task.");
            }

            _logger.LogInformation("Task updated successfully.");
            return Ok("Task updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = GetUserIdFromToken();

            var task = await _taskRepository.GetByIdAsync(id, userId);
            if (task == null)
            {
                _logger.LogWarning("Task not found.");
                return NotFound("Task not found.");
            }

            var result = await _taskRepository.DeleteAsync(id);

            if (!result)
            {
                _logger.LogError("Failed to delete task.");
                return BadRequest("Failed to delete task.");
            }

            _logger.LogInformation("Task deleted successfully.");
            return Ok("Task deleted successfully.");
        }
    }
}
