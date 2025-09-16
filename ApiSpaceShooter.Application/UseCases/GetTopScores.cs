namespace ApiSpaceShooter.Application.UseCases;

using ApiSpaceShooter.Domain.Entities;
using ApiSpaceShooter.Application.Ports;

public class GetTopScores
{
    private readonly IScoreRepository _scoreRepository;

    public GetTopScores(IScoreRepository scoreRepository)
    {
        _scoreRepository = scoreRepository ?? throw new ArgumentNullException(nameof(scoreRepository));
    }

    /// <summary>
    /// Obtiene el top N de puntajes ordenados según especificación:
    /// - Points DESC
    /// - Ante empates: menor DurationSec primero
    /// - Luego CreatedAt ASC
    /// </summary>
    public async Task<IReadOnlyList<Score>> Handle(int limit = 10, CancellationToken cancellationToken = default)
    {
        // Limitar el rango según especificación
        if (limit < 1) limit = 1;
        if (limit > 100) limit = 100;

        return await _scoreRepository.GetTopAsync(limit, cancellationToken);
    }
}