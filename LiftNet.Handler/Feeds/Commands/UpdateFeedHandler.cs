using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.Domain.Response;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using System.Collections.Generic;
using System.Linq;
using LiftNet.Cloudinary.Services;

namespace LiftNet.Handler.Feeds.Commands
{
    public class UpdateFeedHandler : IRequestHandler<UpdateFeedCommand, LiftNetRes<FeedIndexData>>
    {
        private readonly IFeedIndexService _feedService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILiftLogger<UpdateFeedHandler> _logger;

        public UpdateFeedHandler(
            IFeedIndexService feedService,
            ICloudinaryService cloudinaryService,
            ILiftLogger<UpdateFeedHandler> logger)
        {
            _feedService = feedService;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<LiftNetRes<FeedIndexData>> Handle(UpdateFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                List<string>? mediaUrls = null;

                // Only process media files if they are provided
                if (request.ShouldUpdateMedia)
                {
                    mediaUrls = new List<string>();
                    foreach (var mediaFile in request.MediaFiles!)
                    {
                        var imageUrl = await _cloudinaryService.HostImageAsync(mediaFile);
                        if (!string.IsNullOrEmpty(imageUrl))
                        {
                            mediaUrls.Add(imageUrl);
                        }
                        else
                        {
                            _logger.Warn($"Failed to upload media file: {mediaFile.FileName}");
                        }
                    }
                }

                var feed = await _feedService.UpdateFeedAsync(
                    request.Id,
                    request.UserId,
                    request.ShouldUpdateContent ? request.Content : null,
                    mediaUrls
                );

                if (feed == null)
                {
                    // If feed update fails and we uploaded new images, clean them up
                    if (mediaUrls != null && mediaUrls.Any())
                    {
                        await _cloudinaryService.DeleteImageAsync(mediaUrls);
                    }
                    return LiftNetRes<FeedIndexData>.ErrorResponse("Feed not found or unauthorized");
                }

                return LiftNetRes<FeedIndexData>.SuccessResponse(feed);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in UpdateFeedHandler");
                return LiftNetRes<FeedIndexData>.ErrorResponse("Internal server error");
            }
        }
    }
} 