using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.CosmosDb.Contracts
{
    public class CosmosCredential
    {
        public CosmosClient Client
        {
            get; set;
        }
        public string DatabaseId
        {
            get; set;
        }
    }
}
