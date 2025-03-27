using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.CosmosDb.Maintainance
{
    public class CosmosMaintanceService
    {
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseId;

        public CosmosMaintanceService(CosmosClient cosmosClient, string databaseId)
        {
            _cosmosClient = cosmosClient;
            _databaseId = databaseId;
        }

        public async Task CreateIfNotExist(string containerId, string partitionKey)
        {
            var database = await _cosmosClient.CreateDatabaseIfNotExistsAsync(_databaseId);
            await database.Database.CreateContainerIfNotExistsAsync(containerId, partitionKey);
        }
    }
}
