using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditCardController : ControllerBase
    {
        private readonly ICreditCardService _service;
        private readonly IAccountPermissionService _permissionService;

        public CreditCardController(
            ICreditCardService service,
            IAccountPermissionService permissionService)
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

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CreditCardDto>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();

            if (item.UserId != CurrentUserId)
            {
                return Forbid("You do not have access to this card.");
            }

            return Ok(item);
        }

        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<CreditCardDto>>> GetByUser()
        {
            var items = await _service.GetByUserIdAsync(CurrentUserId);
            return Ok(items);
        }

        [HttpGet("account/{accountId:guid}")]
        public async Task<ActionResult<IEnumerable<CreditCardDto>>> GetByAccount(Guid accountId)
        {
            var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, accountId, AccessLevel.Owner, AccessLevel.Editor, AccessLevel.Viewer);
            if (!hasAccess)
            {
                return Forbid("You do not have access to this account's cards.");
            }

            var items = await _service.GetByAccountAsync(accountId);
            return Ok(items);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CreditCardCreateDto dto)
        {
            if (dto.AccountId.HasValue)
            {
                var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, dto.AccountId.Value, AccessLevel.Owner, AccessLevel.Editor);
                if (!hasAccess)
                {
                    return Forbid("You do not have write access to this account.");
                }
            }

            var id = await _service.CreateAsync(dto, CurrentUserId);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        [HttpPut("{id:guid}")]
        public async Task<ActionResult<CreditCardDto>> Update(Guid id, [FromBody] CreditCardUpdateDto dto)
        {
            var card = await _service.GetByIdAsync(id);
            if (card == null) return NotFound();

            if (card.UserId != CurrentUserId)
            {
                return Forbid("You do not have access to this card.");
            }

            if (dto.AccountId.HasValue)
            {
                var hasAccess = await _permissionService.HasAccessAsync(CurrentUserId, dto.AccountId.Value, AccessLevel.Owner, AccessLevel.Editor);
                if (!hasAccess)
                {
                    return Forbid("You do not have write access to this account.");
                }
            }

            var updated = await _service.UpdateAsync(id, dto, CurrentUserId);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var card = await _service.GetByIdAsync(id);
            if (card == null) return NotFound();

            if (card.UserId != CurrentUserId)
            {
                return Forbid("You do not have access to this card.");
            }

            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
    }
}
