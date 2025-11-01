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
            .Where(p => p.StartDate <= now && p.EndDate >= now)
            .ToListAsync();
    }
}
