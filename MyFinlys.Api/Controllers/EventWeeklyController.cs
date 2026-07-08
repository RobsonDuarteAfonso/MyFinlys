using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventWeeklyController : ControllerBase
{
    private readonly IEventWeeklyService _service;
    private readonly IAccountPermissionService _permissionService;

    public EventWeeklyController(IEventWeeklyService service, IAccountPermissionService permissionService)
    {
        _service = service;
        _permissionService = permissionService;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<EventWeeklyDto>>> GetAll()
    {
        var items = await _service.GetAllAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EventWeeklyDto>> GetById(Guid id)
    {
        var item = await _service.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] EventWeeklyDto dto)
    {
        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, dto.AccountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

        var id = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EventWeeklyDto>> Update(Guid id, [FromBody] EventWeeklyDto dto)
    {
        var ev = await _service.GetByIdAsync(id);
        if (ev == null) return NotFound();

        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, ev.AccountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

        var updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ev = await _service.GetByIdAsync(id);
        if (ev == null) return NotFound();

        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, ev.AccountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
