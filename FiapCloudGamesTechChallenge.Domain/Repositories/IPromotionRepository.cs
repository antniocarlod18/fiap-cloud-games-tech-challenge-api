using FiapCloudGamesTechChallenge.Domain.Entities;

namespace FiapCloudGamesTechChallenge.Domain.Repositories;

public interface IPromotionRepository : IRepository<Promotion>
{
    Task<IList<Promotion>> GetActiveAsync();
}   