using ToDoApi.Models;
using ToDoApi.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToDoApi.Repositories.Products
{
    public interface InterfaceProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<IEnumerable<Product>> GetProductSmoll();
        Task<IEnumerable<ProductSmallDto>> GetProductNamesAndPricesAsync();
        Task<List<CategoryProductCountDto>> GetProductCountPerCategory();
        Task<Product?> GetByIdAsync(long id);
        Task<Product> AddAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<bool> DeleteAsync(long id);
    }
}
