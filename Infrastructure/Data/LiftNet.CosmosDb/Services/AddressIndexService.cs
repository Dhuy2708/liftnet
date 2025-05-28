using LiftNet.Contract.Constants;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.CosmosDb.Contracts;
using LiftNet.Domain.Indexes;

namespace LiftNet.CosmosDb.Services
{
    public class AddressIndexService : IndexBaseService<AddressIndexData>, IAddressIndexService
    {
        public AddressIndexService(CosmosCredential cred)
            : base(cred, CosmosContainerId.Address)
        {
        }
    }
}
