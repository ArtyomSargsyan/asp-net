using Microsoft.Extensions.DependencyInjection;
using ToDoApi.Repositories.Categories;
using ToDoApi.Repositories.Products;
using ToDoApi.Repositories.Users;
using ToDoApi.Services.Auth;
using ToDoApi.Services.Categories;
using ToDoApi.Services.Products;
using ToDoApi.Services.ProductModel;

namespace ToDoApi.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<InterfaceProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<IProdcutService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<JwtService>();
            services.AddScoped<IProductModelService, ProductModelService>();

            return services;
        }
    }
}
