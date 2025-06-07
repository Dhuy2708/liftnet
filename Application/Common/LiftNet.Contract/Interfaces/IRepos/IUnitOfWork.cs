using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IRepos
{
    public interface IUnitOfWork : IDisposable, IDependency
    {
        IAppointmentRepo AppointmentRepo { get; }
        IAppointmentParticipantRepo AppointmentParticipantRepo { get; }
        IAppointmentSeenStatusRepo AppointmentSeenStatusRepo { get; }
        IAppointmentConfirmationRepo AppointmentConfirmationRepo { get; }
        IAppointmentFeedbackRepo AppointmentFeedbackRepo { get; }
        ISocialConnectionRepo SocialConnectionRepo { get;}
        IUserRepo UserRepo { get;}
        IProvinceRepo ProvinceRepo { get;}
        IDistrictRepo DistrictRepo { get;}
        IWardRepo WardRepo { get;}
        IVersionRepo VersionRepo { get;}
        IAddressRepo AddressRepo { get; }
        ISocialSimilarityScoreRepo SocialSimilarityScoreRepo { get; }
        IConversationRepo ConversationRepo { get; }
        IConversationUserRepo ConversationUserRepo { get; }
        IWalletRepo WalletRepo { get; }
        ITransactionRepo TransactionRepo { get; }
        ILiftNetTransactionRepo LiftNetTransactionRepo { get; }
        IFinderPostRepo FinderPostRepo { get; }
        IFinderPostSeenStatusRepo FinderPostSeenStatusRepo { get; }
        IFinderPostApplicantRepo FinderPostApplicantRepo { get; }
        IChatBotConversationRepo ChatBotConversationRepo { get; }
        IChatBotMessageRepo ChatBotMessageRepo { get; }
        IPhysicalStatRepo PhysicalStatRepo { get; }
        INotificationRepo NotificationRepo { get; }
        Task BeginTransactionAsync();
        Task<int> CommitAsync();
        Task RollbackAsync();
    }
}
