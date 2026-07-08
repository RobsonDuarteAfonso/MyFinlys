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
    public class CardPurchaseController : ControllerBase
    {
        private readonly ICardPurchaseService _service;
        private readonly ICreditCardService _cardService;
        private readonly IAccountPermissionService _permissionService;

        public CardPurchaseController(
            ICardPurchaseService service,
            ICreditCardService cardService,
            IAccountPermissionService permissionService)
        {
            _service = service;
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

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CardPurchaseDto>> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();

            var card = await _cardService.GetByIdAsync(item.CardId);
            if (card == null) return NotFound("Credit card not found.");

            var hasAccess = (card.UserId == CurrentUserId) || (card.AccountId.HasValue && await _permissionService.HasAccessAsync(CurrentUserId, card.AccountId.Value, AccessLevel.Owner, AccessLevel.Editor, AccessLevel.Viewer));
            if (!hasAccess)
            {
                return Forbid("You do not have access to this account's credit card records.");
            }

            return Ok(item);
        }

        [HttpGet("card/{cardId:guid}")]
        public async Task<ActionResult<IEnumerable<CardPurchaseDto>>> GetByCard(Guid cardId)
        {
            var card = await _cardService.GetByIdAsync(cardId);
            if (card == null) return NotFound("Credit card not found.");

            var hasAccess = (card.UserId == CurrentUserId) || (card.AccountId.HasValue && await _permissionService.HasAccessAsync(CurrentUserId, card.AccountId.Value, AccessLevel.Owner, AccessLevel.Editor, AccessLevel.Viewer));
            if (!hasAccess)
            {
                return Forbid("You do not have access to this account's credit card records.");
            }

            var items = await _service.GetByCardAsync(cardId);
            return Ok(items);
        }



        [AllowAnonymous]
        [HttpGet("diagnostic")]
        public async Task<IActionResult> Diagnostic()
        {
            var dbContext = HttpContext.RequestServices.GetRequiredService<MyFinlys.Infrastructure.Context.MyFinlysDbContext>();
            string trace = "No execution";
            try
            {
                var cardId = Guid.Parse("82a6fec5-2b49-43c3-aebe-d5b78e178051");
                await _service.AdjustClosingDayAsync(cardId, "June", 2026, 19);
                trace = "Successfully executed AdjustClosingDayAsync";
            }
            catch (Exception ex)
            {
                trace = "Error: " + ex.ToString();
            }

            var purchases = dbContext.CardPurchases.ToList();
            var installments = dbContext.CardInstallments.ToList();
            var cards = dbContext.CreditCards.ToList();
            return Ok(new { trace, purchases, installments, cards });
        }

        [HttpGet("card/{cardId:guid}/month/{monthName}/year/{year:int}")]
        public async Task<ActionResult<IEnumerable<CardInstallmentDto>>> GetInstallmentsByCardAndMonth(
            Guid cardId, string monthName, int year)
        {
            var card = await _cardService.GetByIdAsync(cardId);
            if (card == null) return NotFound("Credit card not found.");

            var hasAccess = (card.UserId == CurrentUserId) || (card.AccountId.HasValue && await _permissionService.HasAccessAsync(CurrentUserId, card.AccountId.Value, AccessLevel.Owner, AccessLevel.Editor, AccessLevel.Viewer));
            if (!hasAccess)
            {
                return Forbid("You do not have access to this account's credit card records.");
            }

            try
            {
                var items = await _service.GetInstallmentsByCardAndMonthAsync(cardId, monthName, year);
                return Ok(items);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> Create([FromBody] CardPurchaseCreateDto dto)
        {
            var card = await _cardService.GetByIdAsync(dto.CardId);
            if (card == null) return NotFound("Credit card not found.");

            var hasAccess = (card.UserId == CurrentUserId) || (card.AccountId.HasValue && await _permissionService.HasAccessAsync(CurrentUserId, card.AccountId.Value, AccessLevel.Owner, AccessLevel.Editor));
            if (!hasAccess)
            {
                return Forbid("You do not have write access to this account.");
            }

            try
            {
                var id = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id }, id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();

            var card = await _cardService.GetByIdAsync(item.CardId);
            if (card == null) return NotFound("Credit card not found.");

            var hasAccess = (card.UserId == CurrentUserId) || (card.AccountId.HasValue && await _permissionService.HasAccessAsync(CurrentUserId, card.AccountId.Value, AccessLevel.Owner, AccessLevel.Editor));
            if (!hasAccess)
            {
                return Forbid("You do not have write access to this account.");
            }

            var deleted = await _service.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpPut("installment/{installmentId:guid}/realized/{realized}")]
        public async Task<ActionResult<CardInstallmentDto>> UpdateInstallmentRealized(Guid installmentId, string realized)
        {
            var installment = await _service.UpdateInstallmentRealizedAsync(installmentId, realized);
            if (installment == null) return NotFound();

            return Ok(installment);
        }

        [HttpPost("installment/{installmentId:guid}/transfer-next-month")]
        public async Task<IActionResult> TransferInstallmentToNextMonth(Guid installmentId)
        {
            var success = await _service.TransferInstallmentToNextMonthAsync(installmentId);
            if (!success) return NotFound();

            return Ok();
        }

        [HttpPost("card/{cardId:guid}/adjust-closing-day")]
        public async Task<IActionResult> AdjustClosingDay(Guid cardId, [FromBody] AdjustClosingDayRequest request)
        {
            var card = await _cardService.GetByIdAsync(cardId);
            if (card == null) return NotFound("Credit card not found.");

            var hasAccess = (card.UserId == CurrentUserId) || (card.AccountId.HasValue && await _permissionService.HasAccessAsync(CurrentUserId, card.AccountId.Value, AccessLevel.Owner, AccessLevel.Editor));
            if (!hasAccess)
            {
                return Forbid("You do not have write access to this account.");
            }

            try
            {
                await _service.AdjustClosingDayAsync(cardId, request.Month, request.Year, request.ClosingDay);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpGet("card/{cardId:guid}/plans")]
        public async Task<ActionResult<IEnumerable<CardPurchasePlanDto>>> GetPlansByCard(Guid cardId)
        {
            var card = await _cardService.GetByIdAsync(cardId);
            if (card == null) return NotFound("Credit card not found.");

            var hasAccess = (card.UserId == CurrentUserId) || (card.AccountId.HasValue && await _permissionService.HasAccessAsync(CurrentUserId, card.AccountId.Value, AccessLevel.Owner, AccessLevel.Editor, AccessLevel.Viewer));
            if (!hasAccess)
            {
                return Forbid("You do not have access to this account's credit card records.");
            }

            var plans = await _service.GetPlansByCardAsync(cardId);
            return Ok(plans);
        }
    }

    public class AdjustClosingDayRequest
    {
        public string Month { get; set; } = null!;
        public int Year { get; set; }
        public int ClosingDay { get; set; }
    }
}
