using FluentValidation;

namespace Application.Features.SystemSettings.Update;

public class UpdateSystemSettingCommandValidation : AbstractValidator<UpdateSystemSettingCommand>
{
    public UpdateSystemSettingCommandValidation()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("El id es requerido.");

        RuleFor(x => x.Value)
            .NotNull().WithMessage("El valor es requerido.")
            .MaximumLength(500).WithMessage("El valor no puede exceder 500 caracteres.");
    }
}
