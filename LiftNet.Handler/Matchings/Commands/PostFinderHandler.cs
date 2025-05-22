using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Matchings.Commands.Requests;
using LiftNet.SharedKenel.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Matchings.Commands
{
    public class PostFinderHandler : IRequestHandler<PostFinderCommand, LiftNetRes>
    {
        private readonly ILiftLogger<PostFinderHandler> _logger;
        private readonly IFinderPostRepo _postRepo;
        private readonly IGeoService _geoService;

        public PostFinderHandler(ILiftLogger<PostFinderHandler> logger, IFinderPostRepo postRepo, IGeoService geoService)
        {
            _logger = logger;
            _postRepo = postRepo;
            _geoService = geoService;
        }

        public async Task<LiftNetRes> Handle(PostFinderCommand request, CancellationToken cancellationToken)
        {
            await new PostFinderCommandValidator().ValidateAndThrowAsync(request);
            try
            {
                var placeDetail = await _geoService.GetPlaceDetailAsync(request.LocationId);
                if (placeDetail == null)
                {
                    return LiftNetRes.ErrorResponse("Location is invalid");
                }

                var entity = new FinderPost
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = request.Title,
                    Description = request.Description,
                    StartPrice = request.StartPrice,
                    EndPrice = request.EndPrice,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    HideAddress = request.HideAddress,
                    Lat = placeDetail.Latitude,
                    Lng = placeDetail.Longitude,
                    RepeatType = (int)request.RepeatType,
                    Status = (int)FinderPostStatus.Open,
                };

                await _postRepo.Create(entity);
                return LiftNetRes.SuccessResponse();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error occured while posting finder");
                return LiftNetRes.ErrorResponse("error occured while posting finder");
            }
        }
    }
}
