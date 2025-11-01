using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiapCloudGamesTechChallenge.Domain.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UsersRepo { get; }
        IOrderRepository OrdersRepo { get; }
        IGameRepository GamesRepo { get; }
        IAuditGameUserCollectionRepository AuditGameUsersRepo { get; }
        IAuditGamePriceRepository AuditGamePriceRepo { get; }
        IPromotionRepository PromotionsRepo { get; }
        Task Commit();
    }
}
