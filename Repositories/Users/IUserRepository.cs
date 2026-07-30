using ToDoApi.Models;

namespace ToDoApi.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<User> AddAsync(User user);
    }
}
