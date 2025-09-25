using ToDoApi.DTO;
using ToDoApi.Models;
using ToDoApi.Repositories.Categories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ToDoApi.Services.Categories
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repository;

        public CategoryService(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesWithProductsAsync()
        {
            var categories = await _repository.GetAllAsync();
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Products = c.Products.Select(p => new ProductDto
                {
                    Id = (int) p.Id,
                    Name = p.Name,
                    Price = p.Price  ,
                    Color = p.Color,
                    CategoryId = p.CategoryId
                }).ToList()
            });
        }

        public async Task<CategoryDto?> GetByIdWithProductsAsync(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Products = category.Products.Select(p => new ProductDto
                {
                    Id = (int) p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Color = p.Color,
                    CategoryId = p.CategoryId
                }).ToList()
            };
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<Category> CreateAsync(CategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            return await _repository.AddAsync(category);
        }

        public async Task<Category?> UpdateAsync(int id, CategoryDto dto)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null) return null;

            category.Name = dto.Name;
            return await _repository.UpdateAsync(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }
    }
}
