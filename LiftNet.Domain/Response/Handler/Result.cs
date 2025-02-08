using LiftNet.Domain.Response.ApiResponse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Response.Handler
{
    public class Result<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public List<T>? Datas { get; set; }
        public List<string>? Errors { get; set; } = new();

        public Result() { }

        public Result(List<T> datas, bool success = true, string? message = null)
        {
            Datas = datas;
            Success = success;
            Message = message;
        }

        public static BaseResponse<T> SuccessResponse(List<T> datas, string? message = null)
            => new(datas, true, message);

        public static BaseResponse<T> ErrorResponse(List<string>? errors, string? message = null)
            => new() { Success = false, Message = message, Errors = errors };
    }
}
