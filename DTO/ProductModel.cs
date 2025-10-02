 namespace ToDoApi.DTO
    {
        public class ProductModelDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Color { get; set; } = string.Empty;
            public int ProductId { get; set; }
        }

        public class CreateProductModelDto
        {
            public string Name { get; set; } = string.Empty;
            public string Color { get; set; } = string.Empty;
            public int ProductId { get; set; }
        }

        public class UpdateProductModelDto
        {
            public string? Name { get; set; }
            public string? Color { get; set; }
            public int? ProductId { get; set; }
        }
    }