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
                return BadRequest("Failed to add task.");
            }
            return Ok("Task added successfully.");
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] TaskFilter filter)
        {
            var userId = GetUserIdFromToken();

            var tasks = await _taskRepository.GetAllByUserIdAsync(userId, filter);
            if (tasks == null)
            {
                return NotFound("Tasks not found.");
            }

            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(Guid id)
        {
            var userId = GetUserIdFromToken();

            var task = await _taskRepository.GetByIdAsync(id, userId);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            return Ok(task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskDto updatedTaskDto)
        {
            var userId = GetUserIdFromToken();

            var existingTask = await _taskRepository.GetByIdAsync(id, userId);
            if (existingTask == null)
            {
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
                return BadRequest("Failed to update task.");
            }

            return Ok("Task updated successfully.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var userId = GetUserIdFromToken();

            var task = await _taskRepository.GetByIdAsync(id, userId);
            if (task == null)
            {
                return NotFound("Task not found.");
            }

            var result = await _taskRepository.DeleteAsync(id);

            if (!result)
            {
                return BadRequest("Failed to delete task.");
            }

            return Ok("Task deleted successfully.");
        }
    }
}
