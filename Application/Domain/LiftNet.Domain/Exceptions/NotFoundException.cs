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
        public NotFoundException(List<string> errors) : base(errors, string.Empty)
        {
        }
        public NotFoundException(string message) : base([], message)
        {
        }
        public NotFoundException() : base([], string.Empty)
        {
        }
    }
}
