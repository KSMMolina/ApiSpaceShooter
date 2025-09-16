using ApiSpaceShooter.Models.Requests;
using FluentValidation;

namespace ApiSpaceShooter.Validators;

public class CreateScoreRequestValidator : AbstractValidator<CreateScoreRequest>
{
    public CreateScoreRequestValidator()
    {
        RuleFor(x => x.Alias)
            .NotEmpty().WithMessage("El alias es obligatorio")
            .Length(3, 30).WithMessage("El alias debe tener entre 3 y 30 caracteres");

        RuleFor(x => x.Points)
            .GreaterThanOrEqualTo(0).WithMessage("Los puntos deben ser mayor o igual a 0");

        RuleFor(x => x.MaxCombo)
            .GreaterThanOrEqualTo(0).When(x => x.MaxCombo.HasValue)
            .WithMessage("El combo máximo debe ser mayor o igual a 0");

        RuleFor(x => x.DurationSec)
            .GreaterThanOrEqualTo(0).When(x => x.DurationSec.HasValue)
            .WithMessage("La duración debe ser mayor o igual a 0");

        RuleFor(x => x.Metadata)
            .MaximumLength(400).When(x => !string.IsNullOrEmpty(x.Metadata))
            .WithMessage("Los metadatos no pueden exceder 400 caracteres");
    }
}