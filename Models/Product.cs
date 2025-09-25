namespace ToDoApi.Models;
public class Product
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Color { get; set; } = string.Empty;

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public string? ImageUrl { get; set; } = string.Empty;
}
