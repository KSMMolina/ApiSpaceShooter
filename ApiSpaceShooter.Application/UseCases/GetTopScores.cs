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

    public async Task<IReadOnlyList<Score>> Handle(int limit = 10, CancellationToken cancellationToken = default)
    {
        // Limitar el rango entre 1 y 100
        if (limit < 1)
            limit = 1;
        else if (limit > 100)
            limit = 100;

        return await _scoreRepository.GetTopAsync(limit, cancellationToken);
    }
}