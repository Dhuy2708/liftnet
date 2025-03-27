using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.CosmosDb.Services
{
    public class FeedIndexService : IndexBaseService<Feed>, IFeedIndexService
    {
        private readonly ILiftLogger<FeedIndexService> _logger;
        public FeedIndexService(CosmosCredential cred, 
                                ILiftLogger<IndexBaseService<Feed>> bLogger, 
                                ILiftLogger<FeedIndexService> logger) 
            : base(cred, CosmosContainerId.Feed, bLogger)
        {
            _logger = logger;
        }
    }
}
