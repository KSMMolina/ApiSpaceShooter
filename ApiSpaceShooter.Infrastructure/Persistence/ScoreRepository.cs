namespace ApiSpaceShooter.Infrastructure.Persistence;

using ApiSpaceShooter.Domain.Entities;
using ApiSpaceShooter.Application.Ports;
using Microsoft.EntityFrameworkCore;

public class ScoreRepository : IScoreRepository
{
    private readonly AppDbContext _context;

    public ScoreRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<int> CreateAsync(Score score, CancellationToken cancellationToken = default)
    {
        _context.Scores.Add(score);
        await _context.SaveChangesAsync(cancellationToken);
        return score.Id;
    }

    public async Task<IReadOnlyList<Score>> GetTopAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await _context.Scores
            .AsNoTracking()
            .OrderByDescending(s => s.Points)
            .ThenBy(s => s.DurationSec)
            .ThenBy(s => s.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Score>> GetByAliasAsync(string alias, CancellationToken cancellationToken = default)
    {
        return await _context.Scores
            .AsNoTracking()
            .Where(s => s.Alias == alias)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}