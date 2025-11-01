using FiapCloudGamesTechChallenge.Domain.Entities;
using FiapCloudGamesTechChallenge.Domain.Enums;

namespace FiapCloudGamesTechChallenge.Domain.Repositories;

public interface IAuditGameUserCollectionRepository : IRepository<AuditGameUserCollection>
{
    Task<IList<AuditGameUserCollection>> GetByUserAsync(Guid userId, AuditGameUserCollectionEnum? collectionEnum);
    Task<IList<AuditGameUserCollection>> GetByGameAsync(Guid gameId, AuditGameUserCollectionEnum? collectionEnum);
}