using LiftNet.Domain.Indexes;
using LiftNet.Ioc;

namespace LiftNet.Contract.Interfaces.IServices.Indexes
{
    public interface IAddressIndexService : IIndexBaseService<AddressIndexData>, IDependency
    {
    }
}
