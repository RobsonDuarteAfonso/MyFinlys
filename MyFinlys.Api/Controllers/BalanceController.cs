using Microsoft.AspNetCore.Mvc;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BalanceController : ControllerBase
{
    private readonly IBalanceService _service;

    public BalanceController(IBalanceService service)
        => _service = service;

    // GET api/balance/account/{accountId}/year/{year}/month/{month}
    [HttpGet("account/{accountId:guid}/year/{year:int}/month/{month}")]
    public async Task<ActionResult<BalanceDto>> GetByAccountMonthYear(
        Guid accountId, int year, string month)
    {
        if (!Enum.TryParse<Month>(month, true, out var m))
            return BadRequest("Mês inválido.");

        var dto = await _service.GetByAccountMonthYearAsync(accountId, m, year);
        return dto is null ? NotFound() : Ok(dto);
    }

    // GET api/balance/account/{accountId}
    [HttpGet("account/{accountId:guid}")]
    public async Task<ActionResult<IEnumerable<BalanceDto>>> GetByAccount(Guid accountId)
    {
        var items = await _service.GetByAccountAsync(accountId);
        return Ok(items);
    }

    // POST api/balance
    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] BalanceCreateDto dto)
    {
        if (!Enum.TryParse<Month>(dto.Month, true, out var m))
            return BadRequest("Mês inválido.");

        var id = await _service.CreateAsync(dto.AccountId, dto.Year, m, dto.Amount);
        return CreatedAtAction(nameof(GetByAccountMonthYear),
            new { accountId = dto.AccountId, year = dto.Year, month = dto.Month }, id);
    }

    // PUT api/balance/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<BalanceDto>> Update(
        Guid id, [FromBody] BalanceUpdateDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto.Amount);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE api/balance/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
