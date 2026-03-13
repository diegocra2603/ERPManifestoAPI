using FluentValidation;

namespace Application.Features.Briefs.Delete;

public class DeleteBriefCommandValidation : AbstractValidator<DeleteBriefCommand>
{
    public DeleteBriefCommandValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El Id es requerido.");
    }
}
