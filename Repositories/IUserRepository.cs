using TestTask.Models;

namespace TestTask.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid userId);
        Task<User> GetByNameAsync(string username);
        Task<User> GetByEmailAsync(string email);
        Task<bool> Add(User user);
    }
}