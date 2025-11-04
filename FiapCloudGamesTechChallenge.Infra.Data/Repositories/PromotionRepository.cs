using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using FiapCloudGamesTechChallenge.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesTechChallenge.Infra.Data.Repositories;

public class PromotionRepository : Repository<Promotion>, IPromotionRepository
{
    private readonly ContextDb _context;

    public PromotionRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public async Task<IList<Promotion>> GetActiveAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Promotions
            .AsNoTracking()
            .Include(p => p.Games)
            .Where(p => p.StartDate <= now && p.EndDate >= now)
            .ToListAsync();
    }

    public async Task<Promotion?> GetDetailedByIdAsync(Guid id)
    {
        var now = DateTime.UtcNow;
        return await _context.Promotions
            .Include(p => p.Games)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public new async Task<IList<Promotion>> GetAllAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Promotions
            .Include(p => p.Games)
                .ThenInclude(g => g.Promotions)
            .ToListAsync();
    }
}
