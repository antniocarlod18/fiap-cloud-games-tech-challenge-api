using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;

namespace FiapCloudGamesTechChallenge.Domain.Repositories;

public interface IAuditGamePriceRepository : IRepository<AuditGamePrice>
{
    Task<IList<AuditGamePrice>> GetByGameAsync(Guid gameId);
}