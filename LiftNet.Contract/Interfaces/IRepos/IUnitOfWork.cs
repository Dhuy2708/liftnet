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
        Task BeginTransactionAsync();
        Task<int> CommitAsync();
        Task RollbackAsync();
    }
}
