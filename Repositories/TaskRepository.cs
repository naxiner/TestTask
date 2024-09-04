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

        public async Task<bool> DeleteAsync(Guid taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return false;
            }

            _context.Tasks.Remove(task);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Models.Task>> GetAllByUserIdAsync(Guid userId, TaskFilter filter)
        {
            var query = _context.Tasks.AsQueryable();

            query = query.Where(t => t.UserId == userId);

            // Filter for Status
            if (filter.Status.HasValue)
            {
                query = query.Where(t => t.Status == filter.Status.Value);
            }
            
            // Filter for DueDate
            if (filter.DueDate.HasValue)
            {
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == filter.DueDate.Value.Date);
            }

            // Filter for Priority
            if (filter.Priority.HasValue)
            {
                query = query.Where(t => t.Priority == filter.Priority.Value);
            }

            // Sort by duedate or priority
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                query = filter.SortBy.ToLower() switch
                {
                    "duedate" => (filter.SortDescending ?? false)
                        ? query.OrderByDescending(t => t.DueDate)
                        : query.OrderBy(t => t.DueDate),
                    "priority" => (filter.SortDescending ?? false)
                        ? query.OrderByDescending(t => t.Priority)
                        : query.OrderBy(t => t.Priority),
                    _ => query
                };
            }

            // Pagination
            var skip = (filter.PageNumber - 1) * filter.PageSize;
            query = query.Skip(skip).Take(filter.PageSize);

            return await query.ToListAsync();
        }

        public async Task<Models.Task> GetByIdAsync(Guid taskId, Guid userId)
        {
            return await _context.Tasks
                .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
        }

        public async Task<bool> UpdateAsync(Models.Task task)
        {
            _context.Tasks.Update(task);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
