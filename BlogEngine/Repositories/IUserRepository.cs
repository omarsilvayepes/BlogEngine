using BlogEngine.Models;

namespace BlogEngine.Repositories
{
    public interface IUserRepository
    {
        Task<string> Register(User user, string password);
        Task<string> Login(string userName, string password,string role);
        Task<bool> IsRegister(string userName);
    }
}
