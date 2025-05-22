using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Finders;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Queries
{
    public class ExploreFinderPostsHandler : IRequestHandler<ExploreFinderPostsQuery, LiftNetRes<List<FinderPostView>>>
    {
        private readonly ILiftLogger<ExploreFinderPostsHandler> _logger;
        private readonly IFinderPostRepo _postRepo;
        private readonly IUserRepo _userRepo;

        public ExploreFinderPostsHandler(
            ILiftLogger<ExploreFinderPostsHandler> logger,
            IFinderPostRepo postRepo,
            IUserRepo userRepo)
        {
            _logger = logger;
            _postRepo = postRepo;
            _userRepo = userRepo;
        }

        public async Task<LiftNetRes<List<FinderPostView>>> Handle(ExploreFinderPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var coach = await _userRepo.GetById(request.UserId, new[] { "Address" });
                if (coach == null)
                {
                    return LiftNetRes<List<FinderPostView>>.ErrorResponse("User not found");
                }

                if (coach.Address == null)
                {
                    return LiftNetRes<List<FinderPostView>>.ErrorResponse("Coach must have an address to use this feature");
                }

                var coachLat = coach.Address.Lat;
                var coachLng = coach.Address.Lng;

                var posts = await _postRepo.FromRawSql(@"
                    SELECT * FROM (
                        SELECT *, 
                        6371 * acos(
                            cos(radians({0})) * cos(radians(Lat)) *
                            cos(radians(Lng) - radians({1})) +
                            sin(radians({0})) * sin(radians(Lat))
                        ) AS DistanceInKm
                        FROM FinderPosts
                        WHERE Status = {2}
                    ) AS fp
                    ORDER BY DistanceInKm, CreatedAt DESC
                    LIMIT 10
                ", coachLat, coachLng, (int)FinderPostStatus.Open)
                .ToListAsync(cancellationToken);

                var userIds = posts.Select(p => p.UserId).Distinct().ToList();
                var users = await _userRepo.GetAll(new[] { "Id", "FirstName", "LastName", "Avatar" });
                var userDict = users.ToDictionary(u => u.Id, u => u);

                var postViews = posts.Select(post => new FinderPostView
                {
                    Id = post.Id,
                    Title = post.Title,
                    Description = post.Description,
                    StartTime = post.StartTime,
                    EndTime = post.EndTime,
                    StartPrice = post.StartPrice,
                    EndPrice = post.EndPrice,
                    Lat = post.Lat,
                    Lng = post.Lng,
                    HideAddress = post.HideAddress,
                    RepeatType = (RepeatingType)post.RepeatType,
                    Status = (FinderPostStatus)post.Status,
                    CreatedAt = post.CreatedAt,
                    Poster = userDict.ContainsKey(post.UserId) ? new UserOverview
                    {
                        Id = userDict[post.UserId].Id,
                        FirstName = userDict[post.UserId].FirstName,
                        LastName = userDict[post.UserId].LastName,
                        Avatar = userDict[post.UserId].Avatar
                    } : null
                })
                .ToList();

                return LiftNetRes<List<FinderPostView>>.SuccessResponse(postViews);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while exploring finder posts");
                return LiftNetRes<List<FinderPostView>>.ErrorResponse("Error occurred while exploring finder posts");
            }
        }
    }
} 