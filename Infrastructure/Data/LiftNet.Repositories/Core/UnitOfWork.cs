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
    public class UnitOfWork : IUnitOfWork
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

        private IAppointmentParticipantRepo? _appointmentParticipantRepo = null;
        public IAppointmentParticipantRepo AppointmentParticipantRepo
        {
            get
            {
                _appointmentParticipantRepo ??= _serviceProvider.GetRequiredService<IAppointmentParticipantRepo>();
                _appointmentParticipantRepo.AutoSave = false;
                return _appointmentParticipantRepo;
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

        private IProvinceRepo? _provinceRepo = null;
        public IProvinceRepo ProvinceRepo
        {
            get
            {
                _provinceRepo ??= _serviceProvider.GetRequiredService<IProvinceRepo>();
                _provinceRepo.AutoSave = false;
                return _provinceRepo;
            }
        }

        private IDistrictRepo? _districtRepo = null;
        public IDistrictRepo DistrictRepo
        {
            get
            {
                _districtRepo ??= _serviceProvider.GetRequiredService<IDistrictRepo>();
                _districtRepo.AutoSave = false;
                return _districtRepo;
            }
        }

        private IWardRepo? _wardRepo = null;
        public IWardRepo WardRepo
        {
            get
            {
                _wardRepo ??= _serviceProvider.GetRequiredService<IWardRepo>();
                _wardRepo.AutoSave = false;
                return _wardRepo;
            }
        }

        private IVersionRepo? _versionRepo = null;
        public IVersionRepo VersionRepo
        {
            get
            {
                _versionRepo ??= _serviceProvider.GetRequiredService<IVersionRepo>();
                _versionRepo.AutoSave = false;
                return _versionRepo;
            }
        }

        private IAddressRepo? _addressRepo = null;
        public IAddressRepo AddressRepo
        {
            get
            {
                _addressRepo ??= _serviceProvider.GetRequiredService<IAddressRepo>();
                _addressRepo.AutoSave = false;
                return _addressRepo;
            }
        }

        private ISocialSimilarityScoreRepo? _socialSimilarityScoreRepo = null;
        public ISocialSimilarityScoreRepo SocialSimilarityScoreRepo
        {
            get
            {
                _socialSimilarityScoreRepo ??= _serviceProvider.GetRequiredService<ISocialSimilarityScoreRepo>();
                _socialSimilarityScoreRepo.AutoSave = false;
                return _socialSimilarityScoreRepo;
            }
        }

        private IConversationRepo? _conversationRepo = null;
        public IConversationRepo ConversationRepo
        {
            get
            {
                _conversationRepo ??= _serviceProvider.GetRequiredService<IConversationRepo>();
                _conversationRepo.AutoSave = false;
                return _conversationRepo;
            }
        }

        private IConversationUserRepo? _conversationUserRepo = null;
        public IConversationUserRepo ConversationUserRepo
        {
            get
            {
                _conversationUserRepo ??= _serviceProvider.GetRequiredService<IConversationUserRepo>();
                _conversationUserRepo.AutoSave = false;
                return _conversationUserRepo;
            }
        }

        private IWalletRepo? _walletRepo = null;
        public IWalletRepo WalletRepo
        {
            get
            {
                _walletRepo ??= _serviceProvider.GetRequiredService<IWalletRepo>();
                _walletRepo.AutoSave = false;
                return _walletRepo;
            }
        }

        private IFinderPostRepo? _finderPostRepo = null;
        public IFinderPostRepo FinderPostRepo
        {
            get
            {
                _finderPostRepo ??= _serviceProvider.GetRequiredService<IFinderPostRepo>();
                _finderPostRepo.AutoSave = false;
                return _finderPostRepo;
            }
        }

        private IFinderPostApplicantRepo? _finderPostApplicantRepo = null;
        public IFinderPostApplicantRepo FinderPostApplicantRepo
        {
            get
            {
                _finderPostApplicantRepo ??= _serviceProvider.GetRequiredService<IFinderPostApplicantRepo>();
                _finderPostApplicantRepo.AutoSave = false;
                return _finderPostApplicantRepo;
            }
        }

        private IChatBotConversationRepo? _chatBotConversationRepo = null;
        public IChatBotConversationRepo ChatBotConversationRepo
        {
            get
            {
                _chatBotConversationRepo ??= _serviceProvider.GetRequiredService<IChatBotConversationRepo>();
                _chatBotConversationRepo.AutoSave = false;
                return _chatBotConversationRepo;
            }
        }

        private IChatBotMessageRepo? _chatBotMessageRepo = null;
        public IChatBotMessageRepo ChatBotMessageRepo
        {
            get
            {
                _chatBotMessageRepo ??= _serviceProvider.GetRequiredService<IChatBotMessageRepo>();
                _chatBotMessageRepo.AutoSave = false;
                return _chatBotMessageRepo;
            }
        }

        private ITransactionRepo? _transactionRepo = null;
        public ITransactionRepo TransactionRepo
        {
            get
            {
                _transactionRepo ??= _serviceProvider.GetRequiredService<ITransactionRepo>();
                _transactionRepo.AutoSave = false;
                return _transactionRepo;
            }
        }

        private IFinderPostSeenStatusRepo? _finderPostSeenStatusRepo = null;
        public IFinderPostSeenStatusRepo FinderPostSeenStatusRepo
        {
            get
            {
                _finderPostSeenStatusRepo ??= _serviceProvider.GetRequiredService<IFinderPostSeenStatusRepo>();
                _finderPostSeenStatusRepo.AutoSave = false;
                return _finderPostSeenStatusRepo;
            }
        }

        private IAppointmentSeenStatusRepo? _appointmentSeenStatusRepo = null;
        public IAppointmentSeenStatusRepo AppointmentSeenStatusRepo
        {
            get
            {
                _appointmentSeenStatusRepo ??= _serviceProvider.GetRequiredService<IAppointmentSeenStatusRepo>();
                _appointmentSeenStatusRepo.AutoSave = false;
                return _appointmentSeenStatusRepo;
            }
        }

        private ILiftNetTransactionRepo? _liftNetTransactionRepo = null;
        public ILiftNetTransactionRepo LiftNetTransactionRepo
        {
            get
            {
                _liftNetTransactionRepo ??= _serviceProvider.GetRequiredService<ILiftNetTransactionRepo>();
                _liftNetTransactionRepo.AutoSave = false;
                return _liftNetTransactionRepo;
            }
        }

        private IAppointmentConfirmationRepo? _appointmentConfirmationRepo = null;
        public IAppointmentConfirmationRepo AppointmentConfirmationRepo
        {
            get
            {
                _appointmentConfirmationRepo ??= _serviceProvider.GetRequiredService<IAppointmentConfirmationRepo>();
                _appointmentConfirmationRepo.AutoSave = false;
                return _appointmentConfirmationRepo;
            }
        }

        private IFeedbackRepo? _appointmentFeedbackRepo = null;
        public IFeedbackRepo FeedbackRepo
        {
            get
            {
                _appointmentFeedbackRepo ??= _serviceProvider.GetRequiredService<IFeedbackRepo>();
                _appointmentFeedbackRepo.AutoSave = false;
                return _appointmentFeedbackRepo;
            }
        }

        private IPhysicalStatRepo? _userPhysicalStatRepo = null;
        public IPhysicalStatRepo PhysicalStatRepo
        {
            get
            {
                _userPhysicalStatRepo ??= _serviceProvider.GetRequiredService<IPhysicalStatRepo>();
                _userPhysicalStatRepo.AutoSave = false;
                return _userPhysicalStatRepo;
            }
        }

        private INotificationRepo? _notificationRepo = null;
        public INotificationRepo NotificationRepo
        {
            get
            {
                _notificationRepo ??= _serviceProvider.GetRequiredService<INotificationRepo>();
                _notificationRepo.AutoSave = false;
                return _notificationRepo;
            }
        }

        private ITrainingPlanRepo? _trainingPlanRepo = null;
        public ITrainingPlanRepo TrainingPlanRepo
        {
            get
            {
                _trainingPlanRepo ??= _serviceProvider.GetRequiredService<ITrainingPlanRepo>();
                _trainingPlanRepo.AutoSave = false;
                return _trainingPlanRepo;
            }
        }

        private IExerciseRepo? _exerciseRepo = null;
        public IExerciseRepo ExerciseRepo
        {
            get
            {
                _exerciseRepo ??= _serviceProvider.GetRequiredService<IExerciseRepo>();
                _exerciseRepo.AutoSave = false;
                return _exerciseRepo;
            }
        }

        private ICoachExtensionRepo? _coachExtensionRepo = null;
        public ICoachExtensionRepo CoachExtensionRepo
        {
            get
            {
                _coachExtensionRepo ??= _serviceProvider.GetRequiredService<ICoachExtensionRepo>();
                _coachExtensionRepo.AutoSave = false;
                return _coachExtensionRepo;
            }
        }

        private ICoachRecommendationRepo? _coachRecommendationRepo = null;
        public ICoachRecommendationRepo CoachRecommendationRepo
        {
            get
            {
                _coachRecommendationRepo ??= _serviceProvider.GetRequiredService<ICoachRecommendationRepo>();
                _coachRecommendationRepo.AutoSave = false;
                return _coachRecommendationRepo;
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
            _logger.Info("SQLServer transaction started.");
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
