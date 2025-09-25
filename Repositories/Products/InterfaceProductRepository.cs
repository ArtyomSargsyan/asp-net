using ToDoApi.Models;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace ToDoApi.Repositories.Products
{
    public interface InterfaceProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(long id);
        Task<Product> AddAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<bool> DeleteAsync(long id);
    }
}
