using TestTask.Models;

namespace TestTask.Repositories
{
    public interface ITaskRepository
    {
        Task<Models.Task> GetByIdAsync(Guid taskId, Guid userId);
        Task<IEnumerable<Models.Task>> GetAllByUserIdAsync(Guid userId, TaskFilter filter);
        Task<bool> AddAsync(Models.Task task);
        Task<bool> UpdateAsync(Models.Task task);
        Task<bool> DeleteAsync(Guid taskId);
    }
}