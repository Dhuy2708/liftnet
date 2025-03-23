using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Exceptions
{
    public class BadRequestException : BaseException
    {
        public List<ValidationFailure>? ValidationFailure { get; set; }
        public BadRequestException(List<string> errors, string message, List<ValidationFailure>? validationFailure = null) 
            : base(errors, message)
        {
            ValidationFailure = validationFailure;
        }

        public BadRequestException(List<string> errors, List<ValidationFailure>? validationFailure = null)
            : base(errors, string.Empty)
        {
            ValidationFailure = validationFailure;
        }
    }
}
