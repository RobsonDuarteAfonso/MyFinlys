using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Domain.Enums;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RegisterController : ControllerBase
{
    private readonly IRegisterService _registerService;
    private readonly IAccountPermissionService _permissionService;
    private readonly IEventRepository _eventRepository;
    private readonly IReceiptScannerService _receiptScannerService;

    public RegisterController(
        IRegisterService registerService,
        IAccountPermissionService permissionService,
        IEventRepository eventRepository,
        IReceiptScannerService receiptScannerService)
    {
        _registerService = registerService;
        _permissionService = permissionService;
        _eventRepository = eventRepository;
        _receiptScannerService = receiptScannerService;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _registerService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("event/{eventId:guid}")]
    public async Task<ActionResult<IEnumerable<RegisterDto>>> GetByEventId(Guid eventId)
    {
        var result = await _registerService.GetByEventIdAsync(eventId);
        return Ok(result);
    }

    [HttpGet("account/{accountId:guid}")]
    public async Task<ActionResult<IEnumerable<RegisterDto>>> GetByAccountId(Guid accountId)
    {
        var result = await _registerService.GetByAccountIdAsync(accountId);
        return Ok(result);
    }

    [HttpPost("initialize/account/{accountId:guid}")]
    public async Task<IActionResult> InitializeMonths(Guid accountId)
    {
        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, accountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

        await _registerService.InitializeMonthsAsync(accountId);
        return Ok();
    }

    [HttpGet("account/{accountId:guid}/month/{month}/year/{year:int}")]
    public async Task<ActionResult<IEnumerable<RegisterDto>>> GetByAccountAndMonth(Guid accountId, Month month, int year)
    {
        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, accountId, AccessLevel.Owner, AccessLevel.Editor, AccessLevel.Viewer);
        if (!hasAccess)
        {
            return Forbid("You do not have access to this account.");
        }

        var result = await _registerService.GetByAccountAndMonthAsync(accountId, month, year);
        return Ok(result);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] RegisterCreateDto dto)
    {
        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, dto.AccountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

        var created = await _registerService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] RegisterUpdateDto dto)
    {
        var register = await _registerService.GetByIdAsync(id);
        if (register == null) return NotFound();

        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, dto.AccountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

        var updated = await _registerService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var register = await _registerService.GetByIdAsync(id);
        if (register == null) return NotFound();

        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, register.AccountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

        var deleted = await _registerService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("close/account/{accountId:guid}/month/{month}/year/{year:int}")]
    public async Task<IActionResult> CloseMonth(Guid accountId, Month month, int year)
    {
        var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, accountId, AccessLevel.Owner, AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have write access to this account.");
        }

        await _registerService.CloseMonthAsync(accountId, month, year);
        return Ok();
    }

    [HttpPost("scan")]
    public async Task<IActionResult> ScanReceipt(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Nenhum arquivo enviado.");
        }

        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        var imageBytes = ms.ToArray();

        try
        {
            var result = await _receiptScannerService.ScanReceiptAsync(imageBytes, file.ContentType);
            if (result == null)
            {
                return BadRequest("Não foi possível extrair os dados do recibo.");
            }
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erro interno no servidor: {ex.Message}");
        }
    }
}
