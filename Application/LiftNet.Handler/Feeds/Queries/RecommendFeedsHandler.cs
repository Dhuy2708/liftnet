using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Contract.Views.Feeds;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Feeds.Queries.Requests;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Mappers;
using LiftNet.Utility.Utils;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Feeds.Queries
{
    public class RecommendFeedsHandler : IRequestHandler<RecommendFeedsQuery, LiftNetRes<FeedViewModel>>
    {
        private readonly IRecommendationService _recommendationService;
        private readonly IFeedIndexService _feedIndexService;
        private readonly RoleManager<Role> _roleManager;
        private readonly IUserRepo _userRepo;
        private readonly ILiftLogger<RecommendFeedsHandler> _logger;

        public RecommendFeedsHandler(IRecommendationService recommendationService, 
                                     IFeedIndexService feedIndexService, 
                                     RoleManager<Role> roleManager, 
                                     IUserRepo userRepo, 
                                     ILiftLogger<RecommendFeedsHandler> logger)
        {
            _recommendationService = recommendationService;
            _feedIndexService = feedIndexService;
            _roleManager = roleManager;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<LiftNetRes<FeedViewModel>> Handle(RecommendFeedsQuery request, CancellationToken cancellationToken)
        {
            var feeds = await _recommendationService.ListRecommendedFeedsAsync(request.UserId, 5);
            _logger.Info($"listing recommend feeds, count: {feeds.Count}");
            var feedIds = feeds.Select(f => f.Id).ToList();
            if (feedIds.IsNullOrEmpty())
            {
                return LiftNetRes<FeedViewModel>.SuccessResponse([]);
            }
            var userIds = feeds.Select(x => x.UserId).ToList();
            var userOverviewDict = await AssembleUserOverviews(userIds);

            var likeCounts = await _feedIndexService.GetFeedLikeCountsAsync(feedIds);
            var commentCounts = await _feedIndexService.GetFeedCommentCountsAsync(feedIds);
            var likeStatuses = await _feedIndexService.GetFeedLikeStatusesAsync(feedIds, request.UserId);

            var viewModels = new List<FeedViewModel>();
            foreach (var feed in feeds)
            {
                viewModels.Add(new FeedViewModel
                {
                    Id = feed.Id,
                    UserOverview = userOverviewDict[feed.UserId],
                    Content = feed.Content,
                    Medias = feed.Medias,
                    CreatedAt = feed.CreatedAt,
                    ModifiedAt = feed.ModifiedAt,
                    LikeCount = likeCounts.ContainsKey(feed.Id) ? likeCounts[feed.Id] : 0,
                    CommentCount = commentCounts.ContainsKey(feed.Id) ? commentCounts[feed.Id] : 0,
                    IsLiked = likeStatuses.ContainsKey(feed.Id) && likeStatuses[feed.Id]
                });
            }
            return LiftNetRes<FeedViewModel>.SuccessResponse(viewModels);
        }

        private async Task<Dictionary<string, UserOverview>> AssembleUserOverviews(List<string> userIds)
        {
            userIds = userIds.Distinct().ToList();
            var roles = await _roleManager.Roles.ToListAsync();
            var roleDict = roles.ToDictionary(x => x.Id, x => x.Name);

            var users = await _userRepo.GetQueryable()
                                 .Include(x => x.UserRoles)
                                 .Where(x => userIds.Contains(x.Id))
                                 .ToListAsync();
            Dictionary<string, LiftNetRoleEnum> roleEnumDict = new();

            foreach (var user in users)
            {
                if (user.UserRoles.FirstOrDefault() is { } userRole && roleDict.TryGetValue(userRole.RoleId, out var roleEnum))
                {
                    roleEnumDict[user.Id] = RoleUtil.GetRole(roleEnum!);
                }
            }
            var userOverviews = users.Select(x => x.ToOverview(roleEnumDict)).ToList();
            return userOverviews.ToDictionary(x => x.Id, v => v);
        }
    }
}