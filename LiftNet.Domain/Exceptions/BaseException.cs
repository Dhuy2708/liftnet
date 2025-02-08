using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Exceptions
{
    public class BaseException : ApplicationException
    {
        public List<string> Errors
        {
            get; set;
        } = [];

        public BaseException(List<string> errors, string message) : base(message)
        {
            Errors = errors;
        }
    }
}
