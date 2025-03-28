using LiftNet.Contract.Dtos.Query;
using LiftNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IRepos
{
    public interface ICrudBaseRepo<T> where T : class
    {
        bool AutoSave { get; set; }
        IQueryable<T> GetQueryable(QueryCondition? conditions = null, bool dontAssignNotDeleted = true);
        Task<IEnumerable<T>> QueryByConditions(QueryCondition condition, string[]? includeProps = null);
        Task<IEnumerable<T>> GetAll(string[]? includeProps = null);
        Task<T?> GetById<TId>(TId id, string[]? includeProps = null, bool dontAssignNotDeleted = true);
        Task<bool> IsExisted<TValue>(string key, TValue value);
        Task<bool> IsNotDeletedAsync<TId>(TId id);
        Task<bool> IsExistedByConditions(Dictionary<string, object> conditions, bool isAnd = true);
        Task<bool> IsExistedForUpdating<TId>(TId id, string key, string value);
        Task<int> Create(T model);
        Task<int> CreateRange(IEnumerable<T> model);
        Task<int> Update(T model);
        Task<int> UpdateRange(IEnumerable<T> model);
        Task<int> DeleteByRawSql(object model);
        Task<int> DeleteRangeRawSql(IEnumerable<object> models);
        Task<int> HardDelete(T model);
        Task<int> HardDeleteRange(IEnumerable<T> models);
        Task<int> GetCount();
        Task<int> SaveChangesAsync();
    }
}
