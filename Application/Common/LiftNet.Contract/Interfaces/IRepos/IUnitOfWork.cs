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
        IFinderPostRepo FinderPostRepo { get; }
        IFinderPostApplicantRepo FinderPostApplicantRepo { get; }
        IChatBotConversationRepo ChatBotConversationRepo { get; }
        IChatBotMessageRepo ChatBotMessageRepo { get; }
        Task BeginTransactionAsync();
        Task<int> CommitAsync();
        Task RollbackAsync();
    }
}
