using ToDoApi.DTO;
using ToDoApi.Models;

public interface IProdcutService
{
    Task<IEnumerable<ProductDto>> GetAllProducts();
    Task<IEnumerable<Product>> GetProductSmoll();  
    Task<IEnumerable<ProductSmallDto>> GetProductNamesAndPricesAsync();
    Task<IEnumerable<CategoryProductCountDto>> GetProductCountPerCategory();
    Task<ProductDto?> GetByIdProduct(int id);
    Task<ProductDto> CreateAsync(ProductCreateDto dto);
    Task<ProductDto?> UpdateAsync(int id, ProductCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
