namespace ApiSpaceShooter.Infrastructure.Persistence;

using ApiSpaceShooter.Application.Ports;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

public abstract class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext Context;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly ILogger Logger;

    protected BaseRepository(AppDbContext context, ILogger logger)
    {
        Context = context ?? throw new ArgumentNullException(nameof(context));
        DbSet = context.Set<TEntity>();
        Logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public virtual async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet.FindAsync([id], cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving entity {EntityType} with id {Id}", typeof(TEntity).Name, id);
            throw;
        }
    }

    public virtual async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet.AsNoTracking().ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving all entities of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public virtual async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error finding entities of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public virtual async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet.AsNoTracking().SingleOrDefaultAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error retrieving single entity of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public virtual async Task<int> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            
            var entry = await DbSet.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            
            // Asumiendo que la entidad tiene una propiedad Id
            var idProperty = typeof(TEntity).GetProperty("Id");
            return idProperty?.GetValue(entity) as int? ?? 0;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding entity of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entities);
            
            await DbSet.AddRangeAsync(entities, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error adding range of entities of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public virtual void Update(TEntity entity)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            DbSet.Update(entity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error updating entity of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public virtual void Delete(TEntity entity)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(entity);
            DbSet.Remove(entity);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error deleting entity of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        try
        {
            return await DbSet.AnyAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error checking existence of entity of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }

    public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return predicate == null 
                ? await DbSet.CountAsync(cancellationToken)
                : await DbSet.CountAsync(predicate, cancellationToken);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error counting entities of type {EntityType}", typeof(TEntity).Name);
            throw;
        }
    }
}