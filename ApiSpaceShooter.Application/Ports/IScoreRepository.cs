namespace ApiSpaceShooter.Application.Ports;

using ApiSpaceShooter.Domain.Entities;
using ApiSpaceShooter.Application.DTOs;

public interface IScoreRepository : IBaseRepository<Score>
{
    // Métodos existentes mejorados
    Task<int> CreateAsync(Score score, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Score>> GetTopAsync(int limit, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Score>> GetByAliasAsync(string alias, CancellationToken cancellationToken = default);
    
    // Nuevos métodos para mejor funcionalidad
    Task<IReadOnlyList<Score>> GetTopByDateRangeAsync(int limit, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    Task<Score?> GetBestScoreByAliasAsync(string alias, CancellationToken cancellationToken = default);
    Task<ScoreStatistics> GetStatisticsByAliasAsync(string alias, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Score>> GetRecentScoresAsync(int limit = 10, CancellationToken cancellationToken = default);
    Task<bool> HasScoresForAliasAsync(string alias, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetTopPlayersAsync(int limit, CancellationToken cancellationToken = default);
    Task<int> GetRankingPositionAsync(int scoreId, CancellationToken cancellationToken = default);
}