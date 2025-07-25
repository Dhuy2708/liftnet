﻿using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Commands.Requests;
using LiftNet.SharedKenel.Extensions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Commands
{
    public class CreateFinderPostHandler : IRequestHandler<CreateFinderPostCommand, LiftNetRes>
    {
        private readonly ILiftLogger<CreateFinderPostHandler> _logger;
        private readonly IFinderPostRepo _postRepo;
        private readonly IGeoService _geoService;

        public CreateFinderPostHandler(ILiftLogger<CreateFinderPostHandler> logger, IFinderPostRepo postRepo, IGeoService geoService)
        {
            _logger = logger;
            _postRepo = postRepo;
            _geoService = geoService;
        }

        public async Task<LiftNetRes> Handle(CreateFinderPostCommand request, CancellationToken cancellationToken)
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
                    UserId = request.UserId,
                    Title = request.Title,
                    IsAnonymous = request.IsAnonymous,
                    Description = request.Description,
                    StartPrice = request.StartPrice,
                    EndPrice = request.EndPrice,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    HideAddress = request.HideAddress,
                    Lat = placeDetail.Latitude,
                    Lng = placeDetail.Longitude,
                    PlaceName = placeDetail.FormattedAddress,
                    RepeatType = (int)request.RepeatType,
                    Status = (int)FinderPostApplyingStatus.Applying,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
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
