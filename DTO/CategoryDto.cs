using System.ComponentModel.DataAnnotations;

namespace ToDoApi.DTO
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [MaxLength(25, ErrorMessage = "Name must not exceed 25 characters")]
        public string Name { get; set; } = string.Empty;

        public List<ProductDto> Products { get; set; } = new();
    }
}
