using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoApi.Data;
using ToDoApi.Models;
using Microsoft.Extensions.Logging;
using ToDoApi.DTO;

namespace ToDoApi.Controllers
{
    [ApiController]
    [Route("api/currencies")]
    public class CurrencyController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<CurrencyController> _logger;

        public CurrencyController(AppDbContext context, ILogger<CurrencyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Currency>>> GetCurrencies()
        {
            _logger.LogInformation("Fetching all currencies at {Time}", DateTime.UtcNow);
            var currencies = await _context.Currencies.AsNoTracking().ToListAsync();
            return Ok(currencies);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCurrency([FromBody] CurrencyDto currencyDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for creating currency at {Time}", DateTime.UtcNow);
                return BadRequest(ModelState);
            }

            var currency = new Currency
            {
                Code = currencyDto.Code,
                Name = currencyDto.Name,
                Symbol = currencyDto.Symbol
            };

            await _context.Currencies.AddAsync(currency);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new currency {Code} at {Time}", currency.Code, DateTime.UtcNow);

            return Ok(new
            {
                success = true,
                message = "Currency created successfully"
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCurrency(int id, [FromBody] CurrencyDto currencyDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state for updating currency at {Time}", DateTime.UtcNow);
                return BadRequest(ModelState);
            }

            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                _logger.LogWarning("Currency with ID {Id} not found for update at {Time}", id, DateTime.UtcNow);
                return NotFound(new { success = false, message = "Currency not found" });
            }

            currency.Code = currencyDto.Code;
            currency.Name = currencyDto.Name;
            currency.Symbol = currencyDto.Symbol;

            _context.Currencies.Update(currency);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated currency {Code} at {Time}", currency.Code, DateTime.UtcNow);

            return Ok(new
            {
                success = true,
                message = "Currency updated successfully"
            });
        }

        [HttpDelete("{id}")]
         public async Task<IActionResult> DeleteCurrency(int id)
        {
            var currency = await _context.Currencies.FindAsync(id);
            if (currency == null)
            {
                _logger.LogWarning("Currency with ID {Id} not found for deletion at {Time}", id, DateTime.UtcNow);
                return NotFound(new { success = false, message = "Currency not found" });
            }

            _context.Currencies.Remove(currency);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted currency {Code} at {Time}", currency.Code, DateTime.UtcNow);

            return Ok(new
            {
                success = true,
                message = "Currency deleted successfully"
            });

        }
    }
}