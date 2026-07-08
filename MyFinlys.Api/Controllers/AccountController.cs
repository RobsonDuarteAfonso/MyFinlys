using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Domain.Repositories;

namespace MyFinlys.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IAccountPermissionService _permissionService;
    private readonly IAccountRepository _accountRepository;

    public AccountController(
        IAccountService accountService,
        IAccountPermissionService permissionService,
        IAccountRepository accountRepository)
    {
        _accountService = accountService;
        _permissionService = permissionService;
        _accountRepository = accountRepository;
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
        var result = await _accountService.GetByIdAsync(id);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(Guid userId)
    {
        var accounts = await _accountService.GetByUserIdAsync(userId);
        return Ok(accounts);
    }

    [HttpGet("number/{number}")]
    public async Task<IActionResult> GetByNumber(string number)
    {
        var result = await _accountService.GetByNumberAsync(number);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] AccountCreateDto dto)
    {
        var created = await _accountService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] AccountUpdateDto dto)
    {
        var userId = CurrentUserId;

        var isViewer = await _permissionService.HasAccessAsync(userId, id, Domain.Enums.AccessLevel.Viewer);
        if (isViewer)
        {
            return Forbid("Viewers cannot modify accounts.");
        }

        var hasEditAccess = await _permissionService.HasAccessAsync(userId, id, Domain.Enums.AccessLevel.Owner, Domain.Enums.AccessLevel.Editor);
        if (!hasEditAccess)
        {
            return Forbid("You do not have access to this account.");
        }

        var isEditor = await _permissionService.HasAccessAsync(userId, id, Domain.Enums.AccessLevel.Editor);
        if (isEditor)
        {
            var account = await _accountRepository.GetByIdAsync(id);
            if (account != null)
            {
                var existingUsers = account.UserAccounts.Select(ua => new { ua.UserId, AccessLevel = ua.AccessLevel.ToString() }).OrderBy(u => u.UserId).ToList();
                var proposedUsers = (dto.Users ?? []).Select(u => new { u.UserId, AccessLevel = u.AccessLevel }).OrderBy(u => u.UserId).ToList();
                if (existingUsers.Count != proposedUsers.Count ||
                    !existingUsers.Zip(proposedUsers, (e, p) => e.UserId == p.UserId && e.AccessLevel == p.AccessLevel).All(match => match))
                {
                    return Forbid("Editors cannot modify the associated users list or roles of an account.");
                }
            }
        }

        var updated = await _accountService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = CurrentUserId;
        var hasAccess = await _permissionService.HasAccessAsync(userId, id, Domain.Enums.AccessLevel.Owner, Domain.Enums.AccessLevel.Editor);
        if (!hasAccess)
        {
            return Forbid("You do not have permission to delete this account.");
        }

        try
        {
            var deleted = await _accountService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (System.InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}
