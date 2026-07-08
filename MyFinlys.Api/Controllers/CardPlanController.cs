using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MyFinlys.Application.DTOs;
using MyFinlys.Application.Services.Interfaces;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CardPlanController : ControllerBase
    {
        private readonly ICardPlanService _planService;
        private readonly ICreditCardService _cardService;
        private readonly IAccountPermissionService _permissionService;

        public CardPlanController(
            ICardPlanService planService,
            ICreditCardService cardService,
            IAccountPermissionService permissionService)
        {
            _planService = planService;
            _cardService = cardService;
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

        /// <summary>Returns all plans for a card.</summary>
        [HttpGet("card/{cardId:guid}")]
        public async Task<ActionResult<IEnumerable<CardPlanDto>>> GetByCard(Guid cardId)
        {
            var card = await _cardService.GetByIdAsync(cardId);
            if (card == null) return NotFound("Credit card not found.");

            var hasAccess = (card.UserId == CurrentUserId) ||
                (card.AccountId.HasValue && await _permissionService.HasAccessAsync(
                    CurrentUserId, card.AccountId.Value,
                    AccessLevel.Owner, AccessLevel.Editor, AccessLevel.Viewer));
            if (!hasAccess) return Forbid();

            var plans = await _planService.GetByCardAsync(cardId);
            return Ok(plans);
        }

        /// <summary>Creates a new installment plan for a card.</summary>
        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CardPlanCreateDto dto)
        {
            var card = await _cardService.GetByIdAsync(dto.CardId);
            if (card == null) return NotFound("Credit card not found.");

            var hasAccess = (card.UserId == CurrentUserId) ||
                (card.AccountId.HasValue && await _permissionService.HasAccessAsync(
                    CurrentUserId, card.AccountId.Value,
                    AccessLevel.Owner, AccessLevel.Editor));
            if (!hasAccess) return Forbid();

            try
            {
                var id = await _planService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetByCard), new { cardId = dto.CardId }, id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>Deletes (soft) a plan.</summary>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _planService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        /// <summary>
        /// Called when the user opens a billing month view for a card.
        /// Auto-generates missing monthly installments for all active plans.
        /// Returns all installments (CardPurchase + CardPlan) for that card+month.
        /// </summary>
        [HttpPost("card/{cardId:guid}/generate-month")]
        public async Task<ActionResult<IEnumerable<CardInstallmentDto>>> GenerateMonth(
            Guid cardId, [FromBody] GenerateMonthRequest request)
        {
            var card = await _cardService.GetByIdAsync(cardId);
            if (card == null) return NotFound("Credit card not found.");

            var hasAccess = (card.UserId == CurrentUserId) ||
                (card.AccountId.HasValue && await _permissionService.HasAccessAsync(
                    CurrentUserId, card.AccountId.Value,
                    AccessLevel.Owner, AccessLevel.Editor, AccessLevel.Viewer));
            if (!hasAccess) return Forbid();

            try
            {
                var installments = await _planService.GetOrGenerateInstallmentsForMonthAsync(
                    cardId, request.Month, request.Year, request.ClosingDay);
                return Ok(installments);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class GenerateMonthRequest
    {
        public string Month { get; set; } = null!;
        public int Year { get; set; }
        public int ClosingDay { get; set; }
    }
}
