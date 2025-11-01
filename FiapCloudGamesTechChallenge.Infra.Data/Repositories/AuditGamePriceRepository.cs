using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using FiapCloudGamesTechChallenge.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesTechChallenge.Infra.Data.Repositories;

public class AuditGamePriceRepository : Repository<AuditGamePrice>, IAuditGamePriceRepository
{
    private readonly ContextDb _context;

    public AuditGamePriceRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public async Task<IList<AuditGamePrice>> GetByGameAsync(Guid gameId)
    {
        var query = _context.Set<AuditGamePrice>()
            .AsNoTracking()
            .Include(a => a.Game)
            .Where(a => a.Game.Id == gameId);

        return await query.ToListAsync();
    }
}
