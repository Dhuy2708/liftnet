using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Response
{
    public class BaseResult
    {
        protected bool Success { get; set; }
        protected string? Message { get; set; }
        protected List<string>? Errors { get; set; } = new();

    }
}
