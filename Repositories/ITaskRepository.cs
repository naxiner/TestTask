namespace TestTask.Repositories
{
    public interface ITaskRepository
    {
        Task<Models.Task> GetByIdAsync(Guid taskId, Guid userId);
        Task<IEnumerable<Models.Task>> GetAllByUserIdAsync(Guid userId);
        Task<bool> AddAsync(Models.Task task);
        Task<Models.Task> UpdateAsync(Models.Task task);
        Task<Models.Task> DeleteAsync(Models.Task task);
    }
}