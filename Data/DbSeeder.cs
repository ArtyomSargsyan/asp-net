using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Models;
using ToDoApi.Factories;
using System.Collections.Generic;

namespace ToDoApi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
            {
                var User = new User
                {
                    UserName = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), 
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                await context.Users.AddAsync(User);
            }

            if (!await context.Users.AnyAsync(u => u.Role == "User"))
            {
                var users = FakerFactory.GenerateUsers(100);
                await context.Users.AddRangeAsync(users);
            }

            // Categories
            if (!await context.Categories.AnyAsync())
            {
                var categories = FakerFactory.GenerateCategories(20);
                await context.Categories.AddRangeAsync(categories);
            }

            await context.SaveChangesAsync();

            // Products
            if (!await context.Products.AnyAsync())
            {
                var usersList = await context.Users.ToListAsync();
                var categoriesList = await context.Categories.ToListAsync();

                var products = FakerFactory.GenerateProducts(usersList, categoriesList, 200);
                await context.Products.AddRangeAsync(products);

                await context.SaveChangesAsync();
            }
        }
    }
}
