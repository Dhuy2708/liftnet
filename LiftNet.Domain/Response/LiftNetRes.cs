using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Response
{
    public class LiftNetRes : BaseResult
    {

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

        public static LiftNetRes<T> ErrorResponse(List<string>? errors, string? message = null)
            => new() { Success = false, Message = message, Errors = errors };
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

        public PaginatedLiftNetRes(int pageNumber, int pageSize, int totalCount, string? nextPageToken)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            NextPageToken = nextPageToken;
        }
    }

   
}
