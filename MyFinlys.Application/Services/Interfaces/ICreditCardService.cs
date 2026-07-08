using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Services.Interfaces
{
    public interface ICreditCardService
    {
        Task<CreditCardDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<CreditCardDto>> GetByAccountAsync(Guid accountId);
        Task<IEnumerable<CreditCardDto>> GetByUserIdAsync(Guid userId);
        Task<Guid> CreateAsync(CreditCardCreateDto dto, Guid userId);
        Task<CreditCardDto?> UpdateAsync(Guid id, CreditCardUpdateDto dto, Guid userId);
        Task<bool> DeleteAsync(Guid id);
    }
}
