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
            if (await context.Users.AnyAsync()) return; 

            // 1️⃣ Users
            var users = FakerFactory.GenerateUsers(10);
            await context.Users.AddRangeAsync(users);

            // 2️⃣ Categories
            var categories = FakerFactory.GenerateCategories(5);
            await context.Categories.AddRangeAsync(categories);

            await context.SaveChangesAsync(); 

            // 3️⃣ Products
            var products = FakerFactory.GenerateProducts(users, categories, 4); 
            await context.Products.AddRangeAsync(products);

            await context.SaveChangesAsync();
        }
    }
}
