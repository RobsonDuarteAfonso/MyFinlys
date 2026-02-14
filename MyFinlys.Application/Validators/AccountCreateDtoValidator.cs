using FluentValidation;
using MyFinlys.Application.DTOs;
using MyFinlys.Domain.Enums;

namespace MyFinlys.Application.Validators;

public class AccountCreateDtoValidator : AbstractValidator<AccountCreateDto>
{
    public AccountCreateDtoValidator()
    {
        RuleFor(x => x.Number)
            .NotEmpty().WithMessage("Número da conta é obrigatório")
            .MinimumLength(3).WithMessage("Número deve ter no mínimo 3 caracteres")
            .MaximumLength(20).WithMessage("Número não pode exceder 20 caracteres");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Tipo de conta é obrigatório")
            .Must(IsValidAccountType).WithMessage("Tipo de conta inválido");

        RuleFor(x => x.BankId)
            .NotEmpty().WithMessage("ID do banco é obrigatório");

        RuleFor(x => x.UserIds)
            .NotEmpty().WithMessage("Pelo menos um usuário é obrigatório");
    }

    private static bool IsValidAccountType(string type)
    {
        return Enum.TryParse<AccountType>(type, true, out _);
    }
}
