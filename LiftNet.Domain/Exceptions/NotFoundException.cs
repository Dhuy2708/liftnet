using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Exceptions
{
    public class NotFoundException : BaseException
    {
        public NotFoundException(List<string> errors, string message) : base(errors, message)
        {
        }
    }
}
