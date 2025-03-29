using Azure;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Utils;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.CosmosDb.Services
{
    public class IndexBaseService<T> : IIndexBaseService<T> where T : IndexData
    {
        private readonly Container _container;
        public IndexBaseService(CosmosCredential cred, string containerId)
        {
            var client = cred.Client;
            _container = client.GetContainer(cred.DatabaseId, containerId);
        }

        public async Task<T?> GetAsync(string id, string? partitionKey = null)
        {
            try
            {
                ItemResponse<T> response = await _container.ReadItemAsync<T>(id, new PartitionKey(partitionKey));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<(List<T> Items, string? NextPageToken)> QueryAsync(QueryCondition condition)
        {
            var queryParam = QueryUtil.BuildSqlQuery("c", condition);
            var pageSize = condition.PageSize;
            var continuationToken = condition.NextPageToken;

            var queryDefinition = new QueryDefinition(queryParam.Query);

            foreach (var param in queryParam.Params)
            {
                queryDefinition.WithParameter(param.Key, Convert.ChangeType(param.Value.value, param.Value.type));
            }

            var queryRequestOptions = new QueryRequestOptions { MaxItemCount = pageSize };

            var iterator = _container.GetItemQueryIterator<T>(queryDefinition, continuationToken, queryRequestOptions);

            List<T> results = [];
            string nextPageToken = null;

            if (iterator.HasMoreResults)
            {
                FeedResponse<T> response = await iterator.ReadNextAsync();
                results.AddRange(response);
                nextPageToken = response.ContinuationToken;
            }

            return (results, nextPageToken);
        }

        public async Task<(List<T> Items, string? NextPageToken)> QueryAsync(string query, int pageSize = 10, string? nextPageToken = null)
        {
            var queryDefinition = new QueryDefinition(query);
            var queryRequestOptions = new QueryRequestOptions { MaxItemCount = pageSize };

            var iterator = _container.GetItemQueryIterator<T>(queryDefinition, nextPageToken, queryRequestOptions);

            List<T> results = new List<T>();
            string newContinuationToken = null;

            if (iterator.HasMoreResults)
            {
                FeedResponse<T> response = await iterator.ReadNextAsync();
                results.AddRange(response);
                newContinuationToken = response.ContinuationToken;
            }

            return (results, newContinuationToken);
        }

        public async Task<T> UpsertAsync(T item)
        {
            ItemResponse<T> response = await _container.UpsertItemAsync(item);
            return response.Resource;
        }

        public async Task BulkUpsertAsync(IEnumerable<T> items)
        {
            List<Task> tasks = new List<Task>();

            foreach (var item in items)
            {
                tasks.Add(_container.UpsertItemAsync(item));
            }

            await Task.WhenAll(tasks);
        }

        public async Task<bool> PatchAsync(string id, string partitionKey, string fieldName, object fieldValue)
        {
            try
            {
                var patchOperations = new List<PatchOperation>
                {
                    PatchOperation.Set($"/{fieldName}", fieldValue)
                };

                await _container.PatchItemAsync<T>(id, new PartitionKey(partitionKey), patchOperations);
                return true;
            }
            catch (CosmosException ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string id, string partitionKey)
        {
            try
            {
                await _container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey));
                return true;
            }
            catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task BulkDeleteAsync(IEnumerable<(string id, string partitionKey)> items)
        {
            List<Task> tasks = new List<Task>();

            foreach (var (id, partitionKey) in items)
            {
                tasks.Add(_container.DeleteItemAsync<T>(id, new PartitionKey(partitionKey)));
            }

            await Task.WhenAll(tasks);
        }
    }
}
