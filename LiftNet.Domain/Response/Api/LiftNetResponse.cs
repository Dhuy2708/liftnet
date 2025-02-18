using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Response.ApiResponse
{
    public class LiftNetResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<T>? Datas { get; set; }
        public List<string>? Errors { get; set; } = new();

        public LiftNetResponse() { }

        public LiftNetResponse(List<T>? datas, bool success = true, string? message = null)
        {
            Datas = datas;
            Success = success;
            Message = message;
        }

        public static LiftNetResponse<T> SuccessResponse(List<T> datas, string? message = null)
            => new(datas, true, message);

        public static LiftNetResponse<T> ErrorResponse(List<string> errors, string? message = null)
            => new() { Success = false, Message = message, Errors = errors };
    }
}
