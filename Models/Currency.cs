namespace ToDoApi.Models;
using System.ComponentModel.DataAnnotations;  

public class Currency
{
    public int Id { get; set; }
    [Required]
    [MaxLength(3)]
    public string Code { get; set; } = string.Empty;
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(10)]
    public string Symbol { get; set; } = string.Empty;
}