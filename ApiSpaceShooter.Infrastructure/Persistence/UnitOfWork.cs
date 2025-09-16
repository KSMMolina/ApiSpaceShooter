namespace ApiSpaceShooter.Infrastructure.Persistence;

using ApiSpaceShooter.Application.Ports;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ILogger<UnitOfWork> _logger;
    private IDbContextTransaction? _transaction;
    private readonly IScoreRepository _scoreRepository;

    public UnitOfWork(AppDbContext context, ILogger<UnitOfWork> logger, IScoreRepository scoreRepository)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _scoreRepository = scoreRepository ?? throw new ArgumentNullException(nameof(scoreRepository));
    }

    public IScoreRepository Scores => _scoreRepository;

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var changes = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Saved {Changes} changes to database", changes);
            return changes;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changes to database");
            throw;
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogInformation("Database transaction started");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start database transaction");
            throw;
        }
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction == null)
                throw new InvalidOperationException("No active transaction to commit");

            await _transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Database transaction committed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to commit database transaction");
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                _logger.LogInformation("Database transaction rolled back");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to rollback database transaction");
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context?.Dispose();
    }
}