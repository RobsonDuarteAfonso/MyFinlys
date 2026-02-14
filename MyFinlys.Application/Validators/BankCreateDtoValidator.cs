using FluentValidation;
using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Validators;

public class BankCreateDtoValidator : AbstractValidator<BankCreateDto>
{
    public BankCreateDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nome do banco é obrigatório")
            .MinimumLength(3).WithMessage("Nome deve ter no mínimo 3 caracteres")
            .MaximumLength(100).WithMessage("Nome não pode exceder 100 caracteres");
    }
}
