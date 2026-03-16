using FluentValidation;

namespace Application.Features.Clients.Create;

public class CreateClientCommandValidation : AbstractValidator<CreateClientCommand>
{
    public CreateClientCommandValidation()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(500).WithMessage("Máximo 500 caracteres.");

        RuleFor(x => x.LegalName)
            .MaximumLength(500).WithMessage("Máximo 500 caracteres.");

        RuleFor(x => x.Nit)
            .MaximumLength(50).WithMessage("Máximo 50 caracteres.");
    }
}
