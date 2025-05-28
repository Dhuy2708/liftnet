using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Profiles.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace LiftNet.Handler.Profiles.Queries
{
    public class GetAddressHandler : IRequestHandler<GetAddressQuery, LiftNetRes<AddressView>>
    {
        private readonly ILiftLogger<GetAddressHandler> _logger;
        private readonly IUserRepo _userRepo;

        public GetAddressHandler(ILiftLogger<GetAddressHandler> logger, IUserRepo userRepo)
        {
            _logger = logger;
            _userRepo = userRepo;
        }

        public async Task<LiftNetRes<AddressView>> Handle(GetAddressQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Info("begin get address");
                if (string.IsNullOrEmpty(request.UserId))
                {
                    return LiftNetRes<AddressView>.ErrorResponse("UserId is required");
                }

                var address = (await _userRepo.GetQueryable()
                                             .Include(x => x.Address)
                                             .Select(x => new {x.Id, x.Address})
                                             .FirstOrDefaultAsync(x => x.Id == request.UserId, cancellationToken))?
                               .Address;

                if (address == null)
                {
                    return LiftNetRes<AddressView>.ErrorResponse(null);
                }

                var addressView = new AddressView
                {
                    PlaceName = address.FormattedAddress,
                    ShortPlaceName = address.PlaceName,
                    Lat = address.Lat,
                    Lng = address.Lng,
                    PlaceId = address.PlaceId,
                };
                return LiftNetRes<AddressView>.SuccessResponse(addressView);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting address");
                return LiftNetRes<AddressView>.ErrorResponse("An error occurred while processing your request");
            }
        }
    }
}
