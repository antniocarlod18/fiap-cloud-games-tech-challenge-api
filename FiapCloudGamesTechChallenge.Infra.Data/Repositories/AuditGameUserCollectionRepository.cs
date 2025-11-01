using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using FiapCloudGamesTechChallenge.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesTechChallenge.Infra.Data.Repositories;

public class AuditGameUserCollectionRepository : Repository<AuditGameUserCollection>, IAuditGameUserCollectionRepository
{
    private readonly ContextDb _context;

    public AuditGameUserCollectionRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public async Task<IList<AuditGameUserCollection>> GetByUserAsync(Guid userId, AuditGameUserCollectionEnum? collectionEnum)
    {
        var query = _context.Set<AuditGameUserCollection>()
            .AsNoTracking()
            .Include(a => a.Game)
            .Include(a => a.User)
            .Where(a => a.User.Id == userId);

        if (collectionEnum.HasValue)
            query = query.Where(a => a.Collection == collectionEnum.Value);

        return await query.ToListAsync();
    }

    public async Task<IList<AuditGameUserCollection>> GetByGameAsync(Guid gameId, AuditGameUserCollectionEnum? collectionEnum)
    {
        var query = _context.Set<AuditGameUserCollection>()
            .AsNoTracking()
            .Include(a => a.Game)
            .Include(a => a.User)
            .Where(a => a.Game.Id == gameId);

        if (collectionEnum.HasValue)
            query = query.Where(a => a.Collection == collectionEnum.Value);

        return await query.ToListAsync();
    }
}
