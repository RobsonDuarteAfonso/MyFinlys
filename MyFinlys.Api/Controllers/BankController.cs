using Microsoft.AspNetCore.Mvc;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;

namespace MyFinlys.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankController : ControllerBase
    {
        private readonly IBankService _service;

        public BankController(IBankService service)
            => _service = service;

        // GET api/bank
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankDto>>> GetAll()
        {
            var items = await _service.GetAllAsync();
            return Ok(items);
        }

        // GET api/bank/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BankDto>> GetById(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            return dto is null ? NotFound() : Ok(dto);
        }

        // POST api/bank
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] BankCreateDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        // PUT api/bank/{id}
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<BankDto>> Update(Guid id, [FromBody] BankUpdateDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }

        // DELETE api/bank/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
