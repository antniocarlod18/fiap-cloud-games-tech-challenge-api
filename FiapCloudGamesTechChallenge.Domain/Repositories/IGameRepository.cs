using FiapCloudGamesTechChallenge.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FiapCloudGamesTechChallenge.Domain.Repositories;

public interface IGameRepository : IRepository<Game>
{
    Task<Game?> GetByTitleAsync(string title);
    Task<IList<Game>> GetAvailableAsync();
    Task<IList<Game>> GetByGenreAsync(string genre);
    Task<Game?> GetWithPromotionsByIdAsync(Guid idGame);
    new Task<IList<Game>> GetAllAsync();
}