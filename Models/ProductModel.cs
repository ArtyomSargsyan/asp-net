namespace ToDoApi.Models;
using System.ComponentModel.DataAnnotations;

public class ProductModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    public long ProductId { get; set; }
    public Product? Product { get; set; }
}