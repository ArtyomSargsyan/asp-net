using ToDoApi.Data;
using ToDoApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ToDoApi.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByUsernameAsync(string username) =>
            await _context.Users.FirstOrDefaultAsync(u => u.UserName == username);

        public async Task<bool> ExistsByUsernameAsync(string username) =>
            await _context.Users.AnyAsync(u => u.UserName == username);

        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}
