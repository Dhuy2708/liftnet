using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Interfaces
{
    public interface ILiftLogger<T>
    {
        void Info(string message);
        void Warning(string message);
        void Error(string message);
        void Error(Exception e, string message);
    }
}
