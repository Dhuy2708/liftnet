using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Interfaces
{
    public interface ILiftLogger<T>
    {
        void LogInformation(string message);
        void LogWarning(string message);
        void LogError(string message);
        void LogError(Exception e, string message);
    }
}
