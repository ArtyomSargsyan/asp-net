using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.DTO;
using ToDoApi.Models;

namespace ToDoApi.Services.ProductModel
{
    public class ProductModelService : IProductModelService
    {
        private readonly AppDbContext _context;

        public ProductModelService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductModelDto>> GetAllAsync()
        {
            return await _context.ProductModels
                .Select(p => new ProductModelDto
                {
                    Id =(int) p.Id,
                    Name = p.Name,
                    Color = p.Color,
                    ProductId = (int) p.ProductId
                })
                .ToListAsync();
        }

        public async Task<ProductModelDto?> GetByIdAsync(int id)
        {
            var product = await _context.ProductModels.FindAsync(id);
            if (product == null) return null;

            return new ProductModelDto
            {
                Id = (int)product.Id,
                Name = product.Name,
                Color = product.Color,
                ProductId =(int) product.ProductId
            };
        }

        public async Task<ProductModelDto> CreateAsync(CreateProductModelDto dto)
        {
            var product = new Models.ProductModel
            {
                Name = dto.Name,
                Color = dto.Color,
                ProductId = dto.ProductId
            };

            _context.ProductModels.Add(product);
            await _context.SaveChangesAsync();

            return new ProductModelDto
            {
                Id = (int) product.Id,
                Name = product.Name,
                Color = product.Color,
                ProductId = (int) product.ProductId
            };
        }

       public async Task<ProductModelDto?> UpdateAsync(int id, UpdateProductModelDto dto)
        {
            var product = await _context.ProductModels.FindAsync(id);
            if (product == null) return null;

            if (!string.IsNullOrEmpty(dto.Name)) product.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Color)) product.Color = dto.Color;
            if (dto.ProductId.HasValue) product.ProductId = dto.ProductId.Value;

            await _context.SaveChangesAsync();

            return new ProductModelDto
            {
                Id = (int) product.Id,
                Name = product.Name,
                Color = product.Color,
                ProductId = (int) product.ProductId
            };
        }


        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.ProductModels.FindAsync(id);
            if (product == null) return false;

            _context.ProductModels.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}