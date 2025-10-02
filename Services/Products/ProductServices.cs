using ToDoApi.DTO;
using ToDoApi.Models;
using ToDoApi.Repositories.Products;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace ToDoApi.Services.Products
{
    public class ProductService : IProdcutService
    {
        private readonly InterfaceProductRepository _repo;
        private readonly IWebHostEnvironment _env;

        public ProductService(InterfaceProductRepository repo, IWebHostEnvironment env)
            => (_repo, _env) = (repo, env);

        public async Task<IEnumerable<ProductDto>> GetAllProducts()
            => (await _repo.GetAllAsync()).Select(MapToDto);
            
        public async Task<IEnumerable<Product>> GetProductSmoll()
            => await _repo.GetProductSmoll();
        public async Task<IEnumerable<ProductSmallDto>> GetProductNamesAndPricesAsync()
            => await _repo.GetProductNamesAndPricesAsync();

        public async Task<IEnumerable<CategoryProductCountDto>> GetProductCountPerCategory()
            => await _repo.GetProductCountPerCategory();    
        public async Task<ProductDto?> GetByIdProduct(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            return p == null ? null : MapToDto(p);
        }

        public async Task<ProductDto> CreateAsync(ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                Color = dto.Color,
                CategoryId = dto.CategoryId,
                ImageUrl = await SaveImageAsync(dto.ImageFile)
            };
            var created = await _repo.AddAsync(product);
            return MapToDto(created);
        }

        public async Task<ProductDto?> UpdateAsync(int id, ProductCreateDto dto)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null) return null;

            if (dto.ImageFile != null) existing.ImageUrl = await SaveImageAsync(dto.ImageFile);
            existing.Name = dto.Name;
            existing.Price = dto.Price;
            existing.Color = dto.Color;
            existing.CategoryId = dto.CategoryId;

            var updated = await _repo.UpdateAsync(existing);
            return updated == null ? null : MapToDto(updated);
        }

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

        private async Task<string?> SaveImageAsync(IFormFile? image)
        {
            if (image == null || image.Length == 0) return null;
            var root = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var folder = Path.Combine(root, "images", "products");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var fullPath = Path.Combine(folder, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await image.CopyToAsync(stream);

            return Path.Combine("images", "products", fileName).Replace("\\", "/");
        }

        private static ProductDto MapToDto(Product p) => new()
        {
            Id = (int) p.Id,
            Name = p.Name,
            Price = p.Price,
            Color = p.Color,
            CategoryId = p.CategoryId,
            ImageUrl = p.ImageUrl
        };
    }
}
