using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IRepos
{
    public interface IUnitOfWork : IDisposable
    {
        IAppointmentRepo AppointmentRepo { get; }
        ISocialConnectionRepo SocialConnectionRepo { get;}
        IUserRepo UserRepo { get;}

        Task BeginTransactionAsync();
        Task<int> CommitAsync();
        Task RollbackAsync();
    }
}
