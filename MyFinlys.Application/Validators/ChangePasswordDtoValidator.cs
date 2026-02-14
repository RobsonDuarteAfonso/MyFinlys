using FluentValidation;
using MyFinlys.Application.DTOs;

namespace MyFinlys.Application.Validators;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    public ChangePasswordDtoValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Senha atual é obrigatória");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Nova senha é obrigatória")
            .MinimumLength(6).WithMessage("Nova senha deve ter no mínimo 6 caracteres")
            .MaximumLength(100).WithMessage("Nova senha não pode exceder 100 caracteres")
            .Matches(@"[A-Z]").WithMessage("Nova senha deve conter pelo menos uma letra maiúscula")
            .Matches(@"[a-z]").WithMessage("Nova senha deve conter pelo menos uma letra minúscula")
            .Matches(@"[0-9]").WithMessage("Nova senha deve conter pelo menos um número");

        RuleFor(x => x.NewPassword)
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("Nova senha não pode ser igual à senha atual");
    }
}
