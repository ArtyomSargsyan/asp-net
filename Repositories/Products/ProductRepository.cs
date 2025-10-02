using ToDoApi.Data;
using ToDoApi.Models;
using ToDoApi.DTO;
using Microsoft.EntityFrameworkCore;

namespace ToDoApi.Repositories.Products
{
    public class ProductRepository : InterfaceProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                                 .Include(p => p.Category)
                                 .Where(p => p.CategoryId != 0)
                                 .ToListAsync();
        }
        
        public async Task<IEnumerable<Product>> GetProductSmoll()
        {
            return await _context.Products
                                 .Where(p => p.Price > 1000)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<ProductSmallDto>> GetProductNamesAndPricesAsync()
        {
           return await _context.Products
                                .Select(p => new ProductSmallDto
                                {
                                    Name = p.Name,
                                    Price = p.Price
                                })
                                .ToListAsync();
        }
       public async Task<List<CategoryProductCountDto>> GetProductCountPerCategory()
       {
        int totalProducts = await _context.Products.CountAsync();

        return await _context.Products
        .Include(p => p.Category)
        .GroupBy(p => new { p.CategoryId, p.Category!.Name })
        .Select(g => new CategoryProductCountDto
        {
            CategoryId = g.Key.CategoryId,
            CategoryName = g.Key.Name,
            Count = g.Count(),
            ProductNames = g.Select(p => p.Name).ToList(),
            TotalProducts = totalProducts
        })
        .ToListAsync();
        } 

        public async Task<Product?> GetByIdAsync(long id) =>
            await _context.Products.Include(p => p.Category)
                                   .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Product> AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            var product1 = await _context.Products.FindAsync(product.Id);
            if (product1 == null) return null;

            product1.Name = product.Name;
            product1.Price = product.Price;
            product1.Color = product.Color;
            product1.CategoryId = product.CategoryId;
            product1.ImageUrl = product.ImageUrl;
            await _context.SaveChangesAsync();
            return product1;
        }

        public async Task<bool> DeleteAsync(long id)
        {
            var existing = await _context.Products.FindAsync(id);
            if (existing == null) return false;

            _context.Products.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
