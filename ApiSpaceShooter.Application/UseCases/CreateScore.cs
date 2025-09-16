namespace ApiSpaceShooter.Application.UseCases;

using ApiSpaceShooter.Domain.Entities;
using ApiSpaceShooter.Application.Ports;

public class CreateScore
{
    private readonly IScoreRepository _scoreRepository;

    public CreateScore(IScoreRepository scoreRepository)
    {
        _scoreRepository = scoreRepository ?? throw new ArgumentNullException(nameof(scoreRepository));
    }

    public async Task<int> Handle(
        string alias, 
        int points, 
        int? maxCombo = null, 
        int? durationSec = null, 
        string? metadata = null, 
        CancellationToken cancellationToken = default)
    {
        // Validar alias
        if (string.IsNullOrWhiteSpace(alias))
            throw new ArgumentException("El alias es obligatorio.", nameof(alias));
        
        if (alias.Length < 3 || alias.Length > 30)
            throw new ArgumentException("El alias debe tener entre 3 y 30 caracteres.", nameof(alias));

        // Validar points
        if (points < 0)
            throw new ArgumentOutOfRangeException(nameof(points), "Los puntos deben ser mayor o igual a 0.");

        // Validar maxCombo
        if (maxCombo.HasValue && maxCombo.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(maxCombo), "El combo m�ximo debe ser mayor o igual a 0.");

        // Validar durationSec
        if (durationSec.HasValue && durationSec.Value < 0)
            throw new ArgumentOutOfRangeException(nameof(durationSec), "La duraci�n debe ser mayor o igual a 0.");

        // Validar metadata
        if (!string.IsNullOrEmpty(metadata) && metadata.Length > 400)
            throw new ArgumentException("Los metadatos no pueden exceder 400 caracteres.", nameof(metadata));

        var score = new Score
        {
            Alias = alias.Trim(),
            Points = points,
            MaxCombo = maxCombo,
            DurationSec = durationSec,
            Metadata = metadata?.Trim(),
            CreatedAt = DateTime.UtcNow
        };

        return await _scoreRepository.CreateAsync(score, cancellationToken);
    }
}