using FiapCloudGamesTechChallenge.Domain.Repositories;
using FiapCloudGamesTechChallenge.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FiapCloudGamesTechChallenge.Infra.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ContextDb _context;
        private IUserRepository _userRepository;
        private IOrderRepository _orderRepository;
        private IGameRepository _gameRepository;
        private IAuditGameUserCollectionRepository _auditGameUserRepository;
        private IPromotionRepository _promotionRepository;
        private IAuditGamePriceRepository _auditGamePriceRepository;

        public UnitOfWork(ContextDb contextDb)
        {
            _context = contextDb;
        }

        public IUserRepository UsersRepo
        {
            get
            {
                if (_userRepository == null)
                {
                    _userRepository = new UserRepository(_context);
                }
                return _userRepository;
            }
        }

        public IOrderRepository OrdersRepo
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new OrderRepository(_context);
                }
                return _orderRepository;
            }
        }

        public IGameRepository GamesRepo
        {
            get
            {
                if (_gameRepository == null)
                {
                    _gameRepository = new GameRepository(_context);
                }
                return _gameRepository;
            }
        }

        public IAuditGameUserCollectionRepository AuditGameUsersRepo
        {
            get
            {
                if (_auditGameUserRepository == null)
                {
                    _auditGameUserRepository = new AuditGameUserCollectionRepository(_context);
                }
                return _auditGameUserRepository;
            }
        }

        public IPromotionRepository PromotionsRepo
        {
            get
            {
                if (_promotionRepository == null)
                {
                    _promotionRepository = new PromotionRepository(_context);
                }
                return _promotionRepository;
            }
        }
        
        public IAuditGamePriceRepository AuditGamePriceRepo
        {
            get
            {
                if (_auditGamePriceRepository == null)
                {
                    _auditGamePriceRepository = new AuditGamePriceRepository(_context);
                }
                return _auditGamePriceRepository;
            }
        }

        public async Task Commit()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
