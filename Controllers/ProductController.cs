using Microsoft.AspNetCore.Mvc;
using ToDoApi.Models;
using ToDoApi.Services.Products;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

using System.Threading.Tasks;
using ToDoApi.DTO;

namespace ToDoApi.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly IProdcutService _service;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IProdcutService service, ILogger<ProductsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _service.GetAllProducts();
            _logger.LogInformation("Fetched {count} products-test", items.Count());
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _service.GetByIdProduct(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] ProductCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);

             _logger.LogInformation("Product created with Id {id}", created.Id);

            return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] ProductCreateDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
