using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IJobs
{
    public interface IJob
    {
        Task KickOffAsync();
    }

    public interface ISystemJob : IJob
    {
    }

    public interface IActionJob : IJob
    {
    }

    public interface ICustomerJob : IJob
    {
    }
}
