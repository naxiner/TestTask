using Microsoft.EntityFrameworkCore;
using TestTask.Data;
using TestTask.Models;

namespace TestTask.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly ApplicationDbContext _context;
        public TaskRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddAsync(Models.Task task)
        {
            _context.Tasks.Add(task);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<Models.Task> DeleteAsync(Models.Task task)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Models.Task>> GetAllByUserIdAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Models.Task> GetByIdAsync(Guid taskId, Guid userId)
        {
            throw new NotImplementedException();
        }

        public Task<Models.Task> UpdateAsync(Models.Task task)
        {
            throw new NotImplementedException();
        }
    }
}
