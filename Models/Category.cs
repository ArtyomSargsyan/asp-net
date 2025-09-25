namespace ToDoApi.Models;
using System.ComponentModel.DataAnnotations;
public class Category
{
    public int Id { get; set; }
    [Required]
    [MaxLength(25)]
    public string Name { get; set; } = string.Empty;
     public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}