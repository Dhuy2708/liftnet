using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Profiles.Commands.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Profiles.Commands
{
    public class UpdateAdressHandler : IRequestHandler<UpdateAdressCommand, LiftNetRes>
    {
        private readonly ILiftLogger<UpdateAdressHandler> _logger;
        private readonly IUserRepo _userRepo;
        private readonly IGeoService _geoService;

        public UpdateAdressHandler(ILiftLogger<UpdateAdressHandler> logger, IUserRepo userRepo, IGeoService geoService)
        {
            _logger = logger;
            _userRepo = userRepo;
            _geoService = geoService;
        }

        public async Task<LiftNetRes> Handle(UpdateAdressCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.UserId.IsNullOrEmpty())
                {
                    throw new BadRequestException(["UserId is required"]);
                }

                var user = await _userRepo.GetById(request.UserId);
                if (user == null)
                {
                    throw new BadRequestException(["User not found"]);
                }
                var placeDetail = await _geoService.GetPlaceDetailAsync(request.LocationId);
                if (placeDetail == null)
                {
                    return LiftNetRes.ErrorResponse("place id doesnt valid");
                }
                _logger.Info($"updating user address, userId: {request.UserId}");
                var address = new Address()
                {
                    Id = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    FormattedAddress = placeDetail.FormattedAddress,
                    Lat = placeDetail.Latitude,
                    Lng = placeDetail.Longitude,
                    ModifiedAt = DateTime.UtcNow,
                    PlaceId = request.LocationId,
                    PlaceName = placeDetail.PlaceName,
                };

                user.Address = address;
                await _userRepo.SaveChangesAsync();
                return LiftNetRes.SuccessResponse();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error updating address");
                return LiftNetRes.ErrorResponse("Error updating address");
            }
        }
    }
}
