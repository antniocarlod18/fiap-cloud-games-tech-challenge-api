using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using FiapCloudGamesTechChallenge.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesTechChallenge.Infra.Data.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly ContextDb _context;

    public OrderRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public async Task<IList<Order>> GetByUserAsync(Guid userId)
    {
        return await _context.Orders
            .AsNoTracking()
            .Include(o => o.Games)
            .Include(o => o.User)
            .Where(o => o.User.Id == userId)
            .ToListAsync();
    }
}
