using System.ComponentModel.DataAnnotations;

namespace ToDoApi.DTO
{
    public class CurrencyDto
    {
        [Required]
        [MaxLength(3)]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Symbol { get; set; } = string.Empty;
    }
}
