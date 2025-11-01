using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Repositories;
using FiapCloudGamesTechChallenge.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesTechChallenge.Infra.Data.Repositories;

public class GameRepository : Repository<Game>, IGameRepository
{
    private readonly ContextDb _context;

    public GameRepository(ContextDb context) : base(context)
    {
        _context = context;
    }

    public async Task<Game?> GetByTitleAsync(string title)
    {
        return await _context.Games
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Title == title);
    }

    public async Task<IList<Game>> GetAvailableAsync()
    {
        return await _context.Games
            .AsNoTracking()
            .Where(g => g.Available == true)
            .ToListAsync();
    }

    public async Task<IList<Game>> GetByGenreAsync(string genre)
    {
        return await _context.Games
            .AsNoTracking()
            .Where(g => g.Genre == genre)
            .ToListAsync();
    }
}
