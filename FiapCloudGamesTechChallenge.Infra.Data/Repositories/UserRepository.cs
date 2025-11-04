using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using FiapCloudGamesTechChallenge.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesTechChallenge.Infra.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    private readonly ContextDb _context;

    public UserRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IList<User>> GetActiveAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.Active)
            .ToListAsync();
    }

    public async Task<User?> GetDetailedByIdAsync(Guid userId)
    {
        return await _context.Users
            .Include(u => u.GameCart)
                .ThenInclude(g => g.Promotions)
            .Include(u => u.GameLibrary)
            .Include(u => u.Orders)
                .ThenInclude(ug => ug.Games)
                    .ThenInclude(og => og.Game)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }
}
