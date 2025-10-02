using ToDoApi.DTO;
namespace ToDoApi.Services.ProductModel
{
    public interface IProductModelService
    {
        Task<IEnumerable<ProductModelDto>> GetAllAsync();
        Task<ProductModelDto?> GetByIdAsync(int id);
        Task<ProductModelDto> CreateAsync(CreateProductModelDto dto);
        Task<ProductModelDto?> UpdateAsync(int id, UpdateProductModelDto dto);
        Task<bool> DeleteAsync(int id);
    }
}