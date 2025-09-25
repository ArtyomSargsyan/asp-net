using ToDoApi.DTO;
using ToDoApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToDoApi.Services.Categories
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesWithProductsAsync();
        Task<CategoryDto?> GetByIdWithProductsAsync(int id);

        Task<IEnumerable<Category>> GetAllAsync();
        Task<Category?> GetByIdAsync(int id);
        Task<Category> CreateAsync(CategoryDto dto);
        Task<Category?> UpdateAsync(int id, CategoryDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
