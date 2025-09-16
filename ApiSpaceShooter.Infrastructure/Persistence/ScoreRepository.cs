namespace ApiSpaceShooter.Infrastructure.Persistence;

using ApiSpaceShooter.Domain.Entities;
using ApiSpaceShooter.Application.Ports;
using ApiSpaceShooter.Application.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

public class ScoreRepository : IScoreRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<ScoreRepository> _logger;

    public ScoreRepository(AppDbContext context, ILogger<ScoreRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Métodos principales del reto
    public async Task<int> CreateAsync(Score score, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(score);
            _logger.LogInformation("Creando puntaje para {Alias} con {Points} puntos", score.Alias, score.Points);

            _context.Scores.Add(score);
            await _context.SaveChangesAsync(cancellationToken);
            return score.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al crear puntaje para {Alias}", score?.Alias);
            throw;
        }
    }

    public async Task<IReadOnlyList<Score>> GetTopAsync(int limit, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Obteniendo top {Limit} puntajes", limit);

            return await _context.Scores
                .AsNoTracking()
                .OrderByDescending(s => s.Points)
                .ThenBy(s => s.DurationSec ?? int.MaxValue)
                .ThenBy(s => s.CreatedAt)
                .Take(limit)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener top {Limit} puntajes", limit);
            throw;
        }
    }

    public async Task<IReadOnlyList<Score>> GetByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(alias);
            _logger.LogInformation("Obteniendo historial para {Alias}", alias);

            return await _context.Scores
                .AsNoTracking()
                .Where(s => s.Alias == alias)
                .OrderByDescending(s => s.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener historial para {Alias}", alias);
            throw;
        }
    }

    // Implementaciones de IBaseRepository<Score>
    public async Task<Score?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Scores
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener score por ID {Id}", id);
            throw;
        }
    }

    public async Task<IReadOnlyList<Score>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Scores
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener todos los scores");
            throw;
        }
    }

    public async Task<IReadOnlyList<Score>> FindAsync(Expression<Func<Score, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Scores
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en búsqueda con predicado");
            throw;
        }
    }

    public async Task<Score?> SingleOrDefaultAsync(Expression<Func<Score, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Scores
                .AsNoTracking()
                .SingleOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en SingleOrDefault con predicado");
            throw;
        }
    }

    public async Task<int> AddAsync(Score entity, CancellationToken cancellationToken = default)
    {
        return await CreateAsync(entity, cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<Score> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            _context.Scores.AddRange(entities);
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al agregar múltiples scores");
            throw;
        }
    }

    public void Update(Score entity)
    {
        try
        {
            _context.Scores.Update(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar score");
            throw;
        }
    }

    public void Delete(Score entity)
    {
        try
        {
            _context.Scores.Remove(entity);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al eliminar score");
            throw;
        }
    }

    public async Task<bool> ExistsAsync(Expression<Func<Score, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.Scores.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al verificar existencia");
            throw;
        }
    }

    public async Task<int> CountAsync(Expression<Func<Score, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return predicate == null
                ? await _context.Scores.CountAsync(cancellationToken)
                : await _context.Scores.CountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al contar scores");
            throw;
        }
    }

    // Métodos adicionales (implementación básica para evitar errores)
    public async Task<IReadOnlyList<Score>> GetTopByDateRangeAsync(int limit, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
    {
        return await _context.Scores
            .AsNoTracking()
            .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate)
            .OrderByDescending(s => s.Points)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<Score?> GetBestScoreByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        return await _context.Scores
            .AsNoTracking()
            .Where(s => s.Alias == alias)
            .OrderByDescending(s => s.Points)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ScoreStatistics> GetStatisticsByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        var scores = await _context.Scores
            .AsNoTracking()
            .Where(s => s.Alias == alias)
            .ToListAsync(cancellationToken);

        if (!scores.Any())
            throw new InvalidOperationException($"No scores found for alias '{alias}'");

        return new ScoreStatistics(
            Alias: alias,
            TotalGames: scores.Count,
            BestScore: scores.Max(s => s.Points),
            AverageScore: scores.Average(s => s.Points),
            BestCombo: scores.Where(s => s.MaxCombo.HasValue).Max(s => s.MaxCombo) ?? 0,
            ShortestDuration: scores.Where(s => s.DurationSec.HasValue).Min(s => s.DurationSec) ?? 0,
            FirstGameDate: scores.Min(s => s.CreatedAt),
            LastGameDate: scores.Max(s => s.CreatedAt)
        );
    }

    public async Task<IReadOnlyList<Score>> GetRecentScoresAsync(int limit = 10, CancellationToken cancellationToken = default)
    {
        return await _context.Scores
            .AsNoTracking()
            .OrderByDescending(s => s.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> HasScoresForAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        return await _context.Scores.AnyAsync(s => s.Alias == alias, cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetTopPlayersAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Scores
            .AsNoTracking()
            .GroupBy(s => s.Alias)
            .Select(g => new { Alias = g.Key, BestScore = g.Max(s => s.Points) })
            .OrderByDescending(x => x.BestScore)
            .Take(limit)
            .Select(x => x.Alias)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetRankingPositionAsync(int scoreId, CancellationToken cancellationToken = default)
    {
        var score = await _context.Scores.FindAsync([scoreId], cancellationToken);
        if (score == null) throw new InvalidOperationException($"Score {scoreId} not found");

        var position = await _context.Scores
            .CountAsync(s => s.Points > score.Points ||
                           (s.Points == score.Points && s.DurationSec < score.DurationSec) ||
                           (s.Points == score.Points && s.DurationSec == score.DurationSec && s.CreatedAt < score.CreatedAt),
                       cancellationToken);

        return position + 1;
    }
}