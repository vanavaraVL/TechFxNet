using Microsoft.EntityFrameworkCore;
using TechFxNet.Domain.Dtos;
using TechFxNet.Domain.Entities;
using TechFxNet.Domain.Models;

namespace TechFxNet.Infrastructure.Repositories;

public interface IJournalRepository
{
    Task<long> SaveEntity(JournalEntity entity, CancellationToken ct);

    Task<JournalEntity?> GetSingleByEventId(long id, CancellationToken ct);

    Task<PaginatedResult<JournalEntity>> GetPagedResultAsync(int take, int skip, JournalFilterDto? filter, CancellationToken ct);
}

public class JournalRepository: IJournalRepository
{
    private readonly TechDbContext _context;
    private readonly DbSet<JournalEntity> _dbSet;

    public JournalRepository(TechDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _dbSet = context.Set<JournalEntity>();
    }

    public async Task<PaginatedResult<JournalEntity>> GetPagedResultAsync(int take, int skip, JournalFilterDto? filter, CancellationToken ct)
    {
        var source = _dbSet.AsNoTracking().AsQueryable();

        if (filter?.From is not null)
        {
            source = source.Where(j => j.CreatedAt >= filter.From.Value);
        }
        if (filter?.To is not null)
        {
            source = source.Where(j => j.CreatedAt <= filter.To.Value);
        }

        var count = await source.LongCountAsync(cancellationToken: ct);
        var items = await source.Skip(skip).Take(take).ToListAsync(cancellationToken: ct);

        return new PaginatedResult<JournalEntity>(items, count);
    }

    public Task<JournalEntity?> GetSingleByEventId(long id, CancellationToken ct)
    {
        return _dbSet.AsNoTracking().FirstOrDefaultAsync(j => j.EventId == id, ct);
    }

    public async Task<long> SaveEntity(JournalEntity entity, CancellationToken ct)
    {
        await _dbSet.AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);

        return entity.EventId;
    }
}