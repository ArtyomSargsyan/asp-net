using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ToDoApi.DTO
{
    public class ProductCreateDto
    {
        [Required, MaxLength(50)] public string Name { get; set; } = string.Empty;
        [Required] public decimal Price { get; set; }
        [Required, MaxLength(20)] public string Color { get; set; } = string.Empty;
        [Range(1, int.MaxValue)] public int CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }

    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Color { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
    }
    public class ProductSmallDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class CategoryProductCountDto
    {
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int Count { get; set; }
        public List<string> ProductNames { get; set; } = new();
        public int TotalProducts { get; set; }
    }
}
