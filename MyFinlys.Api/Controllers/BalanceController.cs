using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BalanceController : ControllerBase
{
    private readonly IBalanceService _service;
    private readonly IAccountPermissionService _permissionService;
    private readonly IBalanceRepository _balanceRepository;

    public BalanceController(
        IBalanceService service,
        IAccountPermissionService permissionService,
        IBalanceRepository balanceRepository)
    {
        _service = service;
        _permissionService = permissionService;
        _balanceRepository = balanceRepository;
    }

    private Guid CurrentUserId
    {
        get
        {
            var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirst("UserId")?.Value;
            return Guid.TryParse(idStr, out var id) ? id : Guid.Empty;
        }
    }

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
        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, dto.AccountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

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
        var balance = await _balanceRepository.GetByIdAsync(id);
        if (balance == null) return NotFound();

        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, balance.AccountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

        var updated = await _service.UpdateAsync(id, dto.Amount);
        return updated is null ? NotFound() : Ok(updated);
    }

    // DELETE api/balance/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var balance = await _balanceRepository.GetByIdAsync(id);
        if (balance == null) return NotFound();

        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, balance.AccountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
