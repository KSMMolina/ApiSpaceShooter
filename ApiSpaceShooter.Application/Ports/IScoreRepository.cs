namespace ApiSpaceShooter.Application.Ports;

using ApiSpaceShooter.Domain.Entities;

public interface IScoreRepository
{
    Task<int> CreateAsync(Score score, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Score>> GetTopAsync(int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Score>> GetByAliasAsync(string alias, CancellationToken cancellationToken = default);
}