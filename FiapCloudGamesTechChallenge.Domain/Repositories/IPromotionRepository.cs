using FiapCloudGamesTechChallenge.Domain.Entities;

namespace FiapCloudGamesTechChallenge.Domain.Repositories;

public interface IPromotionRepository : IRepository<Promotion>
{
    Task<IList<Promotion>> GetActiveAsync();
    Task<Promotion?> GetDetailedByIdAsync(Guid id);
    new Task<IList<Promotion>> GetAllAsync();
}   