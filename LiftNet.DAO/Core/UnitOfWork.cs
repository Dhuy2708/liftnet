using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using LiftNet.Ioc;
using LiftNet.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Repositories.Core
{
    public class UnitOfWork : IUnitOfWork, IDependency
    {
        private readonly LiftNetDbContext _context;
        private IDbContextTransaction? _transaction;
        private readonly ILiftLogger<UnitOfWork> _logger;
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(LiftNetDbContext context, ILiftLogger<UnitOfWork> logger, IServiceProvider serviceProvider)
        {
            _context = context;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        #region singleton
        private IAppointmentRepo? _appointmentRepo = null;
        public IAppointmentRepo AppointmentRepo
        {
            get
            {
                _appointmentRepo ??= _serviceProvider.GetRequiredService<IAppointmentRepo>();
                _appointmentRepo.AutoSave = false;
                return _appointmentRepo;
            }
        }

        private ISocialConnectionRepo? _socialConnectionRepo = null;
        public ISocialConnectionRepo SocialConnectionRepo
        {
            get
            {
                _socialConnectionRepo ??= _serviceProvider.GetRequiredService<ISocialConnectionRepo>();
                _socialConnectionRepo.AutoSave = false;
                return _socialConnectionRepo;
            }
        }

        private IUserRepo? _userRepo = null;
        public IUserRepo UserRepo
        {
            get
            {
                _userRepo ??= _serviceProvider.GetRequiredService<IUserRepo>();
                _userRepo.AutoSave = false;
                return _userRepo;
            }
        }

        #endregion

        public async Task BeginTransactionAsync()
        {
            if (_transaction != null)
            {
                _logger.Warn("A transaction is already in progress.");
                throw new InvalidOperationException("A transaction is already in progress.");
            }

            _transaction = await _context.Database.BeginTransactionAsync();
            _logger.Info("Mysql transaction started.");
        }

        public async Task<int> CommitAsync()
        {
            int rowEffect = 0;
            try
            {
                rowEffect += await _context.SaveChangesAsync();
                if (_transaction != null)
                {
                    await _transaction.CommitAsync();
                    _logger.Info("Transaction committed.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred during commit. Rolling back.");
                await RollbackAsync();
                rowEffect = 0;
            }
            finally
            {
                if (_transaction != null)
                {
                    await _transaction.DisposeAsync();
                    _transaction = null;
                }
            }
            return rowEffect;
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _logger.Info("Transaction rolled back.");
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
