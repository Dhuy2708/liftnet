using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.Repositories;
using LiftNet.Domain.Interfaces;
using LiftNet.Persistence.Context;
using LiftNet.Utility.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;
using System.Text;

namespace LiftNet.Repositories.Core
{
    public class CrudBaseRepo<TEntity> : ICrudBaseRepo<TEntity> where TEntity : class
    {
        protected readonly LiftNetDbContext _dbContext;
        protected readonly ILiftLogger<CrudBaseRepo<TEntity>> _logger;
        public bool AutoSave
        {
            get; set;
        } = true;

        public CrudBaseRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<TEntity>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        #region query
        public async Task<int> ExecuteRawQuery(string sql)
        {
            return await _dbContext.Database.ExecuteSqlRawAsync(sql);
        }

        public async Task<int> ExecuteRawQuery(string sql, params object[] parameters)
        {
            return await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public IQueryable<TEntity> GetQueryable(QueryCondition? conditions = null, bool dontAssignNotDeleted = true)
        {
            if (conditions != null)
            {
                var hasIsDeletedProperty = typeof(TEntity).GetProperty("IsDeleted") != null;

                if (!dontAssignNotDeleted && hasIsDeletedProperty)
                {
                    AssignIsDeletedCondition(conditions);
                }

                return BuildQueryableByConditions(conditions);
            }

            return _dbContext.Set<TEntity>();
        }

        public async Task<IEnumerable<TEntity>> QueryByConditions(QueryCondition conditions, string[]? includeProps = null)
        {
            if (conditions == null)
            {
                throw new ArgumentException("Conditions are null or empty.");
            }
            var query = BuildQueryableByConditions(conditions);

            if (includeProps != null && includeProps.Any())
            {
                foreach (var prop in includeProps)
                {
                    query = query.Include(prop);
                }
            }

            return await query.ToListAsync();
        }

        #endregion

        #region get
        public async Task<IEnumerable<TEntity>> GetAll(string[]? includeProps = null)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.GetAll]: Assembling datas...");
            IQueryable<TEntity> query = _dbContext.Set<TEntity>().AsNoTracking();

            if (includeProps != null)
            {
                foreach (var include in includeProps)
                {
                    query = query.Include(include);
                }
            }
            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetById<Tid>(Tid id, string[]? includeProps = null, bool includeTrashed = false)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.GetById]: Finding record with id: {id}");

            IQueryable<TEntity> query = _dbContext.Set<TEntity>();

            if (includeProps != null)
            {
                foreach (var include in includeProps)
                {
                    query = query.Include(include);
                }
            }

            var hasIsDeletedProperty = typeof(TEntity).GetProperty("IsDeleted") != null;
            if (!includeTrashed && hasIsDeletedProperty)
            {
                query = query.Where(e => EF.Property<bool>(e, "IsDeleted") == false);
            }

            var entity = await query.FirstOrDefaultAsync(e => EF.Property<Tid>(e, "Id")!.Equals(id));

            return entity;
        }
        public async Task<IEnumerable<TEntity>> GetByIds<Tid>(IEnumerable<Tid> ids)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.GetByIds]: Assembling data...");

            var keyProperty = typeof(TEntity).GetProperties()
                .FirstOrDefault(p => p.Name.Equals("Id", StringComparison.OrdinalIgnoreCase));

            if (keyProperty == null)
            {
                _logger.Error($"[BaseRepo<{typeof(TEntity).Name}>.GetByIds]: Key property not found.");
                throw new InvalidOperationException($"Key property 'Id' not found for entity {typeof(TEntity).Name}");
            }

            IQueryable<TEntity> query = _dbContext.Set<TEntity>()
                .Where(e => ids.Contains((Tid)keyProperty!.GetValue(e)!))
                .AsNoTracking();

            return await query.ToListAsync();
        }
        #endregion

        #region check
        public async Task<bool> IsExisted<Tvalue>(string key, Tvalue value)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, key);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(equality, parameter);

            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.IsExists]: Checking data existed with key: {key}, value: {value}");
            return await _dbContext.Set<TEntity>().AnyAsync(lambda);
        }

        public async Task<bool> IsNotDeletedAsync<TId>(TId id)
        {
            var entity = await _dbContext.Set<TEntity>().FindAsync(id);

            if (entity == null || !HasIsDeletedProperty(entity))
            {
                return false;
            }

            var isDeletedValue = (bool)entity.GetType().GetProperty("IsDeleted")!.GetValue(entity)!;
            return !isDeletedValue;
        }

        // check exist by multiple condition
        public async Task<bool> IsExistedByConditions(Dictionary<string, object> conditions, bool isAnd = true)
        {
            if (conditions == null || conditions.Count == 0)
            {
                throw new ArgumentException("Conditions cannot be null or empty.");
            }

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            Expression? combinedExpression = null;

            foreach (var condition in conditions)
            {
                var key = condition.Key;
                var value = condition.Value;
                var property = Expression.Property(parameter, key);
                var constant = Expression.Constant(value);
                var equality = Expression.Equal(property, constant);

                if (combinedExpression == null)
                {
                    combinedExpression = equality;
                }
                else
                {
                    combinedExpression = isAnd
                        ? Expression.AndAlso(combinedExpression, equality)
                        : Expression.OrElse(combinedExpression, equality);
                }
            }

            var lambda = Expression.Lambda<Func<TEntity, bool>>(combinedExpression!, parameter);

            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.IsExisted]: Checking data existence with conditions: {string.Join(", ", conditions)}");
            return await _dbContext.Set<TEntity>().AnyAsync(lambda);
        }

        //Before update existence check
        public async Task<bool> IsExistedForUpdating<Tid>(Tid id, string key, string value)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, key);
            var constant = Expression.Constant(value);
            var equality = Expression.Equal(property, constant);

            var idProperty = Expression.Property(parameter, "Id");
            var idEquality = Expression.NotEqual(idProperty, Expression.Constant(id));

            var combinedExpression = Expression.AndAlso(equality, idEquality);
            var lambda = Expression.Lambda<Func<TEntity, bool>>(combinedExpression, parameter);

            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.IsExistedForUpdating]: Checking data existed with key: {key}, value: {value} and not with id: {id}");
            return await _dbContext.Set<TEntity>().AnyAsync(lambda);
        }
        #endregion

        #region create
        public async Task<int> Create(TEntity model)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.Create]: Attempt creating data...");
            await _dbContext.Set<TEntity>().AddAsync(model);
            return await SaveChangesAsync();
        }

        public async Task<int> CreateRange(IEnumerable<TEntity> model)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.CreateRange]: Attempt creating list datas...");
            await _dbContext.Set<TEntity>().AddRangeAsync(model);
            return await SaveChangesAsync();
        }
        #endregion

        #region update
        public async Task<int> Update(TEntity model)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.Update]: Attempt updating data...");
            _dbContext.Set<TEntity>().Update(model);
            return await SaveChangesAsync();
        }
        public async Task<int> UpdateRange(IEnumerable<TEntity> model)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.UpdateRange]: Attempt updating list datas...");
            _dbContext.Set<TEntity>().UpdateRange(model);
            return await SaveChangesAsync();
        }
        #endregion

        #region delete
        public async Task<int> DeleteByRawSql(object id)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.Delete]: Attempt to soft-delete data...");
            string? tableName = GetTableName();
            if (string.IsNullOrEmpty(tableName))
            {
                throw new InvalidOperationException($"Cannot determine the table name for {typeof(TEntity).Name}.");
            }
            string keyName = "Id";
            string sqlQuery = $"UPDATE `{tableName}` SET IsDeleted = 1 WHERE `{keyName}` = @id";

            var result = await _dbContext.Database.ExecuteSqlRawAsync(sqlQuery, new SqlParameter("@id", id));
            return result;
        }

        public async Task<int> DeleteRangeRawSql(IEnumerable<object> ids)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.DeleteRange]: Attempt to soft-delete a list of data...");

            if (!ids.Any())
            {
                throw new ArgumentException("The list of IDs is empty.", nameof(ids));
            }

            string? tableName = GetTableName();

            if (string.IsNullOrEmpty(tableName))
            {
                throw new InvalidOperationException($"Cannot determine the table name for {typeof(TEntity).Name}.");
            }
            string keyName = "Id";
            var idList = string.Join(",", ids.Select(id => $"'{id}'"));
            string sqlQuery = $"UPDATE `{tableName}` SET IsDeleted = 1 WHERE `{keyName}` IN ({idList})";

            var result = await _dbContext.Database.ExecuteSqlRawAsync(sqlQuery);
            return result;
        }
        public async Task<int> Delete(object id)
        {
            var entity = await GetById(id);
            if (entity == null)
            {
                return 0;
            }

            var isDeletedProperty = entity.GetType().GetProperty("isDeleted");

            if (isDeletedProperty != null && isDeletedProperty.CanWrite)
            {
                isDeletedProperty.SetValue(entity, true);

                return await Update(entity);
            }
            return 0;
        }

        public async Task<int> DeleteRange(IEnumerable<object> ids)
        {
            var entities = await GetByIds(ids);
            if (entities == null || !entities.Any())
            {
                return 0;
            }
            foreach (var entity in entities)
            {
                var isDeletedProperty = entity.GetType().GetProperty("isDeleted");
                if (isDeletedProperty != null && isDeletedProperty.CanWrite)
                {
                    isDeletedProperty.SetValue(entity, true);
                }
            }
            return await UpdateRange(entities);
        }

        public async Task<int> HardDelete(TEntity model)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.HardDelete]: Attempt to hard-delete data...");

            _dbContext.Set<TEntity>().Remove(model);
            return await SaveChangesAsync();
        }

        public async Task<int> HardDeleteRange(IEnumerable<TEntity> models)
        {
            _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.HardDeleteRange]: Attempt to hard-delete a list of data...");

            _dbContext.Set<TEntity>().RemoveRange(models);
            return await SaveChangesAsync();
        }
        #endregion

        #region count
        public async Task<int> GetCount()
        {
            return await _dbContext.Set<TEntity>().CountAsync();
        }
        #endregion


        #region helper
        public async Task<int> SaveChangesAsync()
        {
            if (AutoSave)
            {
                _logger.Info($"[BaseRepo<{typeof(TEntity).Name}>.SaveChangeAsync]: Saving changes...");
                return await _dbContext.SaveChangesAsync();
            }
            return 0;
        }

        private string? GetTableName()
        {
            return _dbContext.Model.FindEntityType(typeof(TEntity))?.GetTableName();
        }

        private object? GetEntityId(TEntity entity)
        {
            var idProperty = typeof(TEntity).GetProperty("Id");
            if (idProperty == null)
            {
                throw new ArgumentException($"{typeof(TEntity).Name} does not have a property named 'Id'.");
            }

            return idProperty.GetValue(entity);
        }

        private void AssignIsDeletedCondition(QueryCondition conds)
        {
            var isDeletedCond = conds.FindConditions("isDeleted");
            if (isDeletedCond == null || !isDeletedCond.Any())
            {
                if (conds.ConditionItems.Any())
                {
                    conds.ConditionItems.Last().Logic = QueryLogic.And;
                }
                conds.AddCondition(new ConditionItem()
                {
                    Property = "isDeleted",
                    Operator = QueryOperator.Equal,
                    Type = FilterType.Boolean,
                    Values = ["false"],
                    Logic = QueryLogic.And
                });
            }
        }

        private bool HasIsDeletedProperty(object entity)
        {
            return entity.GetType().GetProperty("IsDeleted") != null;
        }

        private IQueryable<TEntity> BuildQueryableByConditions(QueryCondition conditions, bool assignNotDeleted = true)
        {
            if (assignNotDeleted)
            {
                AssignIsDeletedCondition(conditions);
            }
            var tableName = GetTableName() ?? nameof(TEntity);

            var queryParam = QueryUtil.BuildSqlQuery(tableName, conditions);

            IQueryable<TEntity> query;
            if (queryParam.Params != null && queryParam.Params.Count > 0)
            {
                List<SqlParameter> sqlParams = new();

                foreach (var kvp in queryParam.Params)
                {
                    sqlParams.Add(new SqlParameter(kvp.Key, Convert.ChangeType(kvp.Value.value, kvp.Value.type)));
                }

                query = _dbContext.Set<TEntity>().FromSqlRaw(queryParam.Query, sqlParams.ToArray()).AsNoTracking();
            }
            else
            {
                query = _dbContext.Set<TEntity>().FromSqlRaw(queryParam.Query);
            }
            return query;
        }
        #endregion
    }
}
