using ToDoApi.Models;

namespace ToDoApi.Repositories.Users
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<bool> ExistsByUsernameAsync(string username);
        Task<User> AddAsync(User user);
    }
}
