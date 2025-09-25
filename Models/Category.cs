namespace ToDoApi.Models;
using System.ComponentModel.DataAnnotations;
public class Category
{
    public int Id { get; set; }
    [Required]
    [MaxLength(25)]
    public string Name { get; set; } = string.Empty;

    public ICollection<Product> Products { get; set; } = new List<Product>();
}