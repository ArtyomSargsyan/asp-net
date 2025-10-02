using Microsoft.AspNetCore.Mvc;
using ToDoApi.Services.ProductModel;
namespace ToDoApi.Controller
{
    public class ProductModelController : ControllerBase
    {
       protected readonly ILogger<ProductModelController> _logger;
       protected readonly IProductModelService _service;
       public ProductModelController(ILogger<ProductModelController> logger, IProductModelService service)
       {
            _logger = logger;
            _service = service;
       }
    }
}