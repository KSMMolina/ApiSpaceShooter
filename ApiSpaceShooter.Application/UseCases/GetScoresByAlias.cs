namespace ApiSpaceShooter.Application.UseCases;

using ApiSpaceShooter.Domain.Entities;
using ApiSpaceShooter.Application.Ports;

public class GetScoresByAlias
{
    private readonly IScoreRepository _scoreRepository;

    public GetScoresByAlias(IScoreRepository scoreRepository)
    {
        _scoreRepository = scoreRepository ?? throw new ArgumentNullException(nameof(scoreRepository));
    }

    public async Task<IReadOnlyList<Score>> Handle(string alias, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(alias))
            throw new ArgumentException("El alias no puede estar vacío.", nameof(alias));

        return await _scoreRepository.GetByAliasAsync(alias.Trim(), cancellationToken);
    }
}