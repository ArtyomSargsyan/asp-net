using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoApi.DTO;
using ToDoApi.Services.Categories;
using ToDoApi.Services.Loger;
using System.Text.Json;

namespace ToDoApi.Controllers
{
   [ApiController]
   [Route("api/categories")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service,)
        {
            _service = service;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categoriesDto = await _service.GetAllCategoriesWithProductsAsync();
            
           int count = categoriesDto?.Count() ?? 0;
           FileLogger.Instance.Info($"Fetched all categories. Count: {count}");
           
           return Ok(categoriesDto);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var categoryDto = await _service.GetByIdWithProductsAsync(id);
            if (categoryDto == null) return NotFound();
            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryDto dto)
        {
            var category = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }
}
