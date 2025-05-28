using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Feeds.Commands.Requests;
using LiftNet.Domain.Indexes;
using LiftNet.Cloudinary.Services;

namespace LiftNet.Handler.Feeds.Commands
{
    public class PostFeedHandler : IRequestHandler<PostFeedCommand, LiftNetRes<FeedIndexData>>
    {
        private readonly IFeedIndexService _feedService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly ILiftLogger<PostFeedHandler> _logger;

        public PostFeedHandler(
            IFeedIndexService feedService,
            ICloudinaryService cloudinaryService,
            ILiftLogger<PostFeedHandler> logger)
        {
            _feedService = feedService;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task<LiftNetRes<FeedIndexData>> Handle(PostFeedCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var mediaUrls = new List<string>();

                // Upload each media file
                foreach (var mediaFile in request.MediaFiles)
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

                var feed = await _feedService.PostFeedAsync(request.UserId, request.Content, mediaUrls);
                if (feed == null)
                {
                    // If feed creation fails, try to cleanup uploaded images
                    if (mediaUrls.Any())
                    {
                        await _cloudinaryService.DeleteImageAsync(mediaUrls);
                    }
                    return LiftNetRes<FeedIndexData>.ErrorResponse("Failed to post feed");
                }

                return LiftNetRes<FeedIndexData>.SuccessResponse(feed);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in PostFeedHandler");
                return LiftNetRes<FeedIndexData>.ErrorResponse("Internal server error");
            }
        }
    }
} 