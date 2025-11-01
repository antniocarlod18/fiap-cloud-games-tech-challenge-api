using FiapCloudGamesTechChallenge.Domain.Entities;

namespace FiapCloudGamesTechChallenge.Domain.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<IList<User>> GetActiveAsync();
    Task<User?> GetDetailedByIdAsync(Guid userId);
}   