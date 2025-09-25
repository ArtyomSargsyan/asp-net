using ToDoApi.DTO;

public interface IProdcutService
{
    Task<IEnumerable<ProductDto>> GetAllProducts();
    Task<ProductDto?> GetByIdProduct(int id);
    Task<ProductDto> CreateAsync(ProductCreateDto dto);
    Task<ProductDto?> UpdateAsync(int id, ProductCreateDto dto);
    Task<bool> DeleteAsync(int id);
}
