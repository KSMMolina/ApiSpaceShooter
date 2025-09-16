namespace ApiSpaceShooter.Infrastructure.Persistence;

using ApiSpaceShooter.Domain.Entities;
using ApiSpaceShooter.Application.Ports;
using ApiSpaceShooter.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class ScoreRepository : BaseRepository<Score>, IScoreRepository
{
    public ScoreRepository(AppDbContext context, ILogger<ScoreRepository> logger) 
        : base(context, logger)
    {
    }

    public async Task<int> CreateAsync(Score score, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(score);
            Logger.LogInformation("Creating new score for alias {Alias} with {Points} points", score.Alias, score.Points);

            return await AddAsync(score, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to create score for alias {Alias}", score?.Alias);
            throw;
        }
    }

    public async Task<IReadOnlyList<Score>> GetTopAsync(int limit, CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogInformation("Retrieving top {Limit} scores", limit);

            return await Context.Scores
                .AsNoTracking()
                .OrderByDescending(s => s.Points)
                .ThenBy(s => s.DurationSec)
                .ThenBy(s => s.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to retrieve top {Limit} scores", limit);
            throw;
        }
    }

    public async Task<IReadOnlyList<Score>> GetByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            Logger.LogInformation("Retrieving scores for alias {Alias}", alias);

            return await Context.Scores
                .AsNoTracking()
                .Where(s => s.Alias == alias)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to retrieve scores for alias {Alias}", alias);
            throw;
        }
    }

    public async Task<IReadOnlyList<Score>> GetTopByDateRangeAsync(int limit, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogInformation("Retrieving top {Limit} scores between {FromDate} and {ToDate}", limit, fromDate, toDate);

            return await Context.Scores
                .AsNoTracking()
                .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate)
                .OrderByDescending(s => s.Points)
                .ThenBy(s => s.DurationSec)
                .ThenBy(s => s.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to retrieve top scores for date range {FromDate} to {ToDate}", fromDate, toDate);
            throw;
        }
    }

    public async Task<Score?> GetBestScoreByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            Logger.LogInformation("Retrieving best score for alias {Alias}", alias);

            return await Context.Scores
                .AsNoTracking()
                .Where(s => s.Alias == alias)
                .OrderByDescending(s => s.Points)
                .ThenBy(s => s.DurationSec)
                .FirstOrDefaultAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to retrieve best score for alias {Alias}", alias);
            throw;
        }
    }

    public async Task<ScoreStatistics> GetStatisticsByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            Logger.LogInformation("Computing statistics for alias {Alias}", alias);

            var scores = await Context.Scores
                .AsNoTracking()
                .Where(s => s.Alias == alias)
                .ToListAsync(cancellationToken);

            if (!scores.Any())
            {
                throw new InvalidOperationException($"No scores found for alias '{alias}'");
            }

            return new ScoreStatistics(
                Alias: alias,
                TotalGames: scores.Count,
                BestScore: scores.Max(s => s.Points),
                AverageScore: scores.Average(s => s.Points),
                BestCombo: scores.Where(s => s.MaxCombo.HasValue).DefaultIfEmpty().Max(s => s?.MaxCombo) ?? 0,
                ShortestDuration: scores.Where(s => s.DurationSec.HasValue).DefaultIfEmpty().Min(s => s?.DurationSec) ?? 0,
                FirstGameDate: scores.Min(s => s.CreatedAt),
                LastGameDate: scores.Max(s => s.CreatedAt)
            );
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to compute statistics for alias {Alias}", alias);
            throw;
        }
    }

    public async Task<IReadOnlyList<Score>> GetRecentScoresAsync(int limit = 10, CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogInformation("Retrieving {Limit} most recent scores", limit);

            return await Context.Scores
                .AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to retrieve recent scores");
            throw;
        }
    }

    public async Task<bool> HasScoresForAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            
            return await ExistsAsync(s => s.Alias == alias, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to check if alias {Alias} has scores", alias);
            throw;
        }
    }

    public async Task<IReadOnlyList<string>> GetTopPlayersAsync(int limit, CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogInformation("Retrieving top {Limit} players by best score", limit);

            return await Context.Scores
                .AsNoTracking()
                .GroupBy(s => s.Alias)
                .Select(g => new { Alias = g.Key, BestScore = g.Max(s => s.Points) })
                .OrderByDescending(x => x.BestScore)
                .Take(limit)
                .Select(x => x.Alias)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to retrieve top players");
            throw;
        }
    }

    public async Task<int> GetRankingPositionAsync(int scoreId, CancellationToken cancellationToken = default)
    {
        try
        {
            Logger.LogInformation("Computing ranking position for score {ScoreId}", scoreId);

            var score = await Context.Scores
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == scoreId, cancellationToken);

            if (score == null)
            {
                throw new InvalidOperationException($"Score with ID {scoreId} not found");
            }

            var position = await Context.Scores
                .AsNoTracking()
                .Where(s => s.Points > score.Points || 
                           (s.Points == score.Points && s.DurationSec < score.DurationSec) ||
                           (s.Points == score.Points && s.DurationSec == score.DurationSec && s.CreatedAt < score.CreatedAt))
                .CountAsync(cancellationToken);

            return position + 1; // Position is 1-based
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to compute ranking position for score {ScoreId}", scoreId);
            throw;
        }
    }
}