using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Domain.Response.ApiResponse
{
    public class PaginatedResponse<T> : BaseResponse<T>
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

        public PaginatedResponse(int pageNumber, int pageSize, int totalCount, string? nextPageToken)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = totalCount;
            NextPageToken = nextPageToken;
        }
    }
}
