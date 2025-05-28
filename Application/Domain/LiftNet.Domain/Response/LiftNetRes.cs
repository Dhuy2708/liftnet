using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Response
{
    public class LiftNetRes : BaseResult
    {
        public static LiftNetRes SuccessResponse(string? message = null)
          => new() { Success = true, Message = message};

        public static LiftNetRes ErrorResponse(string? message = null, List<string>? errors = null, List<ValidationFailure>? validationFailure = null)
            => new() { Success = false, Message = message, Errors = errors, ValidationFailure = validationFailure };
    }

    public class LiftNetRes<T> : LiftNetRes
    {
        public List<T>? Datas { get; set; }

        public LiftNetRes() { }

        public LiftNetRes(List<T>? datas, bool success = true, string? message = null)
        {
            Datas = datas;
            Success = success;
            Message = message;
        }
        public static LiftNetRes<T> SuccessResponse(List<T> datas, string? message = null)
            => new(datas, true, message);
        public static LiftNetRes<T> SuccessResponse(T datas, string? message = null)
           => new([datas], true, message);
        public new static LiftNetRes<T> ErrorResponse(string? message = null, List<string>? errors = null, List<ValidationFailure>? validationFailure = null)
    => new() { Success = false, Message = message, Errors = errors, ValidationFailure = validationFailure };
    }

    public class PaginatedLiftNetRes<T> : LiftNetRes<T>
    {
        public int PageNumber
        {
            get; set;
        } = 1;

        private int _pageSize;
        public int PageSize
        {
            get => Datas?.Count ?? _pageSize;
            set => _pageSize = value;
        }

        public int TotalCount
        {
            get; set;
        }

        public string? NextPageToken
        {
            get; set;
        }

        public PaginatedLiftNetRes()
        {
        }

        public PaginatedLiftNetRes(int? pageNumber = null, int? pageSize = null, int? totalCount = null, string? nextPageToken = null)
        {
            PageNumber = pageNumber ?? 0;
            PageSize = pageSize ?? 0;
            TotalCount = totalCount ?? 0;
            NextPageToken = nextPageToken;
        }

        public static PaginatedLiftNetRes<T> SuccessResponse(List<T> datas, int? pageNumber = null, int? pageSize = null, int? totalCount = null, string? nextPageToken = null, string? message = null)
            => new(pageNumber, pageSize, totalCount, nextPageToken)
            {
                Datas = datas,
                Success = true,
                Message = message
            };

        public static new PaginatedLiftNetRes<T> ErrorResponse(string? message = null, List<string>? errors = null, List<ValidationFailure>? validationFailure = null)
            => new()
            {
                Success = false,
                Message = message,
                Errors = errors,
                ValidationFailure = validationFailure
            };
    }
}
