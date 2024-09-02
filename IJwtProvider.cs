using TestTask.Models;

namespace TestTask
{
    public interface IJwtProvider
    {
        string GenerateToken(User user);
    }
}