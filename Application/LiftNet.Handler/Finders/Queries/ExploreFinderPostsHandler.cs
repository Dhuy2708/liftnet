using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Finders;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Queries.Requests;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Mappers;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LiftNet.Contract.Constants;
using LiftNet.Utility.Utils;
using LiftNet.Redis.Interface;

namespace LiftNet.Handler.Finders.Queries
{
    public class ExploreFinderPostsHandler : IRequestHandler<ExploreFinderPostsQuery, LiftNetRes<ExploreFinderPostView>>
    {
        private readonly ILiftLogger<ExploreFinderPostsHandler> _logger;
        private readonly IFinderPostRepo _postRepo;
        private readonly IFinderPostApplicantRepo _applicantRepo;
        private readonly IUserRepo _userRepo;
        private readonly IUserService _userService;
        private readonly IRedisCacheService _redisCacheService;

        public ExploreFinderPostsHandler(ILiftLogger<ExploreFinderPostsHandler> logger,
                                         IFinderPostRepo postRepo,
                                         IFinderPostApplicantRepo applicantRepo,
                                         IUserRepo userRepo,
                                         IUserService userService,
                                         IRedisCacheService redisCacheService)
        {
            _logger = logger;
            _postRepo = postRepo;
            _applicantRepo = applicantRepo;
            _userRepo = userRepo;
            _userService = userService;
            _redisCacheService = redisCacheService;
        }

        public async Task<LiftNetRes<ExploreFinderPostView>> Handle(ExploreFinderPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var coach = await _userRepo.GetById(request.UserId, new[] { "Address" });
                if (coach == null)
                {
                    return LiftNetRes<ExploreFinderPostView>.ErrorResponse("User not found");
                }

                if (coach.Address == null)
                {
                    return LiftNetRes<ExploreFinderPostView>.ErrorResponse("Coach must have an address to use this feature");
                }

                var coachLat = coach.Address.Lat;
                var coachLng = coach.Address.Lng;
                var maxDistanceValue = request.MaxDistance;

                var cachedPostIds = await _redisCacheService.GetObjectAsync<List<string>>(
                    string.Format(RedisCacheKeys.EXPLORED_FINDER_POST_CACHE_KEY, request.UserId)
                );

                var query = $@"
                                WITH PostsWithDistance AS (
                                    SELECT 
                                        p.*,
                                        CAST(6371 * acos(
                                            cos(radians({coachLat})) * cos(radians(p.Lat)) *
                                            cos(radians(p.Lng) - radians({coachLng})) +
                                            sin(radians({coachLat})) * sin(radians(p.Lat))
                                        ) AS FLOAT) AS DistanceAway
                                    FROM FinderPosts p
                                    WHERE p.Status = {(int)FinderPostStatus.Open}
                                    {(cachedPostIds != null && cachedPostIds.Any() ? $"AND p.Id NOT IN ({string.Join(",", cachedPostIds.Select(id => $"'{id}'"))})" : "")}
                                )
                                SELECT *
                                FROM PostsWithDistance p
                                WHERE ({maxDistanceValue} IS NULL OR DistanceAway <= {maxDistanceValue}) AND 
                                      p.Status = {(int)FinderPostStatus.Open} AND 
                                      p.StartTime > '{DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss")}'
                                ORDER BY DistanceAway, CreatedAt DESC
                                OFFSET 0 ROWS FETCH NEXT 10 ROWS ONLY;
                            ";

                var posts = await _postRepo.FromRawSql(query).ToListAsync(cancellationToken);
                if (posts.IsNullOrEmpty())
                {
                    await _redisCacheService.RemoveAsync(string.Format(RedisCacheKeys.EXPLORED_FINDER_POST_CACHE_KEY, request.UserId));
                    return LiftNetRes<ExploreFinderPostView>.SuccessResponse([]);
                }

                var postIds = posts.Select(p => p.Id).ToList();
                if (cachedPostIds != null && cachedPostIds.Any())
                {
                    postIds.AddRange(cachedPostIds);
                }

                await _redisCacheService.SetAsync(
                    string.Format(RedisCacheKeys.EXPLORED_FINDER_POST_CACHE_KEY, request.UserId),
                    postIds,
                    TimeSpan.FromHours(1)
                );

                var postStatusDict = await GetAppliedPostStatusDict(request.UserId);

                var userIds = posts.Where(x => !x.IsAnonymous)
                                   .Select(p => p.UserId).Distinct().ToList();
                var userDict = (await _userRepo.GetQueryable()
                                        .Where(u => userIds.Contains(u.Id))
                                        .ToListAsync(cancellationToken))
                             .ToDictionary(k => k.Id, v => v);
                var userRoleDict = await _userService.GetUserIdRoleDict(userIds);

                var postViews = posts.Select(post =>
                {
                    var distanceAway = GeoUtil.CalculateDistance(coachLat, coachLng, post.Lat, post.Lng);

                    return new ExploreFinderPostView
                    {
                        Id = post.Id,
                        Title = post.Title,
                        DistanceAway = distanceAway,
                        Description = post.Description,
                        StartTime = post.StartTime.ToOffSet(),
                        EndTime = post.EndTime.ToOffSet(),
                        StartPrice = post.StartPrice,
                        EndPrice = post.EndPrice,
                        PlaceName = post.HideAddress ? null : post.PlaceName,
                        Lat = post.HideAddress ? null : post.Lat,
                        Lng = post.HideAddress ? null : post.Lng,
                        IsAnonymous = post.IsAnonymous,
                        HideAddress = post.HideAddress,
                        ApplyingStatus = postStatusDict.GetValueOrDefault(post.Id, FinderPostApplyingStatus.None),
                        RepeatType = (RepeatingType)post.RepeatType,
                        Status = (FinderPostStatus)post.Status,
                        CreatedAt = post.CreatedAt.ToOffSet(),
                        Poster = post.IsAnonymous
                            ? null
                            : userDict.GetValueOrDefault<string, User>(post.UserId)?.ToOverview(userRoleDict)
                    };
                })
                .ToList();

                return LiftNetRes<ExploreFinderPostView>.SuccessResponse(postViews);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while exploring finder posts");
                return LiftNetRes<ExploreFinderPostView>.ErrorResponse("Error occurred while exploring finder posts");
            }
        }

        private async Task<Dictionary<string, FinderPostApplyingStatus>> GetAppliedPostStatusDict(string trainerId)
        {
            return (await _applicantRepo.GetQueryable()
                                       .Where(x => x.TrainerId == trainerId)
                                       .Select(x => new FinderPostApplicant
                                       {
                                           PostId = x.PostId,
                                           Status = x.Status
                                       })
                                       .ToListAsync())
                                       .ToDictionary(k => k.PostId, v => (FinderPostApplyingStatus)v.Status);
        }
    }
} 