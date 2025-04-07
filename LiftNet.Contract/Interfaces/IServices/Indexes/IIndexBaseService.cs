using LiftNet.Contract.Dtos.Query;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices.Indexes
{
    public interface IIndexBaseService<T>
    {
        Task<T?> GetAsync(string id, string partitionKey);
        Task<(List<T> Items, string? NextPageToken)> QueryAsync(QueryCondition condition);
        Task<(List<T> Items, string? NextPageToken)> QueryAsync(string query, int pageSize = 10, string? nextPageToken = null);
        Task<T> UpsertAsync(T item);
        Task BulkUpsertAsync(IEnumerable<T> items);
        Task<bool> PatchAsync(string id, string partitionKey, string fieldName, object fieldValue);
        Task<bool> DeleteAsync(string id, string partitionKey);
        Task BulkDeleteAsync(IEnumerable<(string id, string partitionKey)> items);
        Task<int> CountAsync(QueryCondition condition);
        Task<bool> AnyAsync(QueryCondition condition);
    }
}
