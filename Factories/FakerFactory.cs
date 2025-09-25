using Bogus;
using System.Collections.Generic;
using ToDoApi.Models;
using System;

namespace ToDoApi.Factories
{
    public static class FakerFactory
    {
        public static IEnumerable<User> GenerateUsers(int count = 10)
        {
            return new Faker<User>()
                .RuleFor(u => u.UserName, f => f.Person.UserName)
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.PasswordHash, f => f.Internet.Password())
                .RuleFor(u => u.CreatedAt, f => f.Date.Past(1))
                .RuleFor(u => u.UpdatedAt, f => f.Date.Recent(30))
                .Generate(count);
        }

        public static IEnumerable<Category> GenerateCategories(int count = 5)
        {
            return new Faker<Category>()
                .RuleFor(c => c.Name, f => f.Commerce.Categories(1)[0])
                .RuleFor(c => c.CreatedAt, f => f.Date.Past(1))
                .RuleFor(c => c.UpdatedAt, f => f.Date.Recent(30))
                .Generate(count);
        }

        public static IEnumerable<Product> GenerateProducts(IEnumerable<User> users, IEnumerable<Category> categories, int productsPerUser = 3)
        {
            var faker = new Faker<Product>()
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => decimal.Parse(f.Commerce.Price()))
                .RuleFor(p => p.Color, f => f.Commerce.Color())
                .RuleFor(p => p.CreatedAt, f => f.Date.Past(1))
                .RuleFor(p => p.UpdatedAt, f => f.Date.Recent(30));

            var products = new List<Product>();
            var random = new Random();

            foreach (var user in users)
            {
                for (int i = 0; i < productsPerUser; i++)
                {
                    var category = categories.ElementAt(random.Next(categories.Count()));
                    var product = faker.Generate();
                    product.UserId = user.Id;
                    product.CategoryId = category.Id;
                    products.Add(product);
                }
            }

            return products;
        }
    }
}
