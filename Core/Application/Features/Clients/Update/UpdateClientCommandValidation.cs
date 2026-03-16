using FluentValidation;

namespace Application.Features.Clients.Update;

public class UpdateClientCommandValidation : AbstractValidator<UpdateClientCommand>
{
    public UpdateClientCommandValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El id es requerido.");

        RuleFor(x => x.Nit)
            .NotEmpty().WithMessage("El NIT es requerido.")
            .MaximumLength(50).WithMessage("Máximo 50 caracteres.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(500).WithMessage("Máximo 500 caracteres.");
    }
}
