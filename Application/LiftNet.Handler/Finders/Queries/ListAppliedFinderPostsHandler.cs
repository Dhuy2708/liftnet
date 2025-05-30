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
using LiftNet.Utility.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Queries
{
    public class ListAppliedFinderPostsHandler : IRequestHandler<ListAppliedFinderPostsQuery, LiftNetRes<ExploreFinderPostView>>
    {
        private readonly ILiftLogger<ListAppliedFinderPostsHandler> _logger;
        private readonly IFinderPostApplicantRepo _postRepo;
        private readonly IUserRepo _userRepo;
        private readonly IUserService _userService;

        public ListAppliedFinderPostsHandler(ILiftLogger<ListAppliedFinderPostsHandler> logger,
                                             IFinderPostApplicantRepo postRepo, 
                                             IUserRepo userRepo, 
                                             IUserService userService)
        {
            _logger = logger;
            _postRepo = postRepo;
            _userRepo = userRepo;
            _userService = userService;
        }

        public async Task<LiftNetRes<ExploreFinderPostView>> Handle(ListAppliedFinderPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var trainerId = request.UserId;

                var trainerAddress = (await _userRepo.GetQueryable()
                                                    .Include(x => x.Address)
                                                    .Select(x => new { x.Address, x.Id})
                                                    .FirstOrDefaultAsync(x => x.Id == trainerId, cancellationToken))?.Address;
                if (trainerAddress == null)
                {
                    return LiftNetRes<ExploreFinderPostView>.ErrorResponse("Trainer address not found");
                }

                _logger.Info($"begin listing applied posts");
                var applicants = await _postRepo.GetQueryable()
                                          .Include(x => x.Post)
                                          .Where(x => x.TrainerId == trainerId &&
                                                      (x.Status == (int)FinderPostApplyingStatus.Applying ||
                                                       x.Status == (int)FinderPostApplyingStatus.Accepted ||
                                                       x.Status == (int)FinderPostApplyingStatus.Rejected ||
                                                       x.Status == (int)FinderPostApplyingStatus.Canceled))
                                          .ToListAsync(cancellationToken);
                if (applicants.IsNullOrEmpty())
                {
                    return LiftNetRes<ExploreFinderPostView>.SuccessResponse([]);
                }

                List<ExploreFinderPostView> results = [];

                var posts = applicants.Select(x => x.Post)
                                            .Where(x => x != null)
                                            .ToList();
                var posterIds = posts.Select(x => x.UserId)
                                     .Distinct()
                                     .ToList();

                var userOverviewDict = (await _userService.Convert2Overviews(posterIds))
                                            .ToDictionary(k => k.Id, v => v);

                foreach (var post in posts)
                {
                    var distanceAway = GeoUtil.CalculateDistance(
                                                trainerAddress.Lat,
                                                trainerAddress.Lng,
                                                post.Lat,
                                                post.Lng
                                            );

                    var applyingStatusInt = applicants.FirstOrDefault(x => x.PostId == post.Id)?.Status;
                    var applyingStatus = applyingStatusInt != null ? (FinderPostApplyingStatus)applyingStatusInt.Value 
                                                                   : FinderPostApplyingStatus.None;

                    if ((applyingStatus is FinderPostApplyingStatus.Applying || 
                        applyingStatus is FinderPostApplyingStatus.Accepted) &&
                        post.StartTime < DateTime.UtcNow)
                    {
                        applyingStatus = FinderPostApplyingStatus.Canceled;
                    }
                    if (post.StartTime < DateTime.UtcNow ||
                        post.Status == (int)FinderPostStatus.Closed)
                    {
                        applyingStatus = FinderPostApplyingStatus.Canceled;
                    }

                    UserOverview? poster = null;
                    if (applyingStatus is FinderPostApplyingStatus.Accepted || !post.IsAnonymous)
                    {
                        poster = userOverviewDict.GetValueOrDefault<string, UserOverview>(post.UserId);
                        if (poster == null)
                        {
                            _logger.Warn($"Poster with ID {post.UserId} not found for post ID {post.Id}");
                        }
                    }


                    var result = new ExploreFinderPostView
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
                        Lat = (applyingStatus is FinderPostApplyingStatus.Accepted) ? post.Lat
                                    : post.HideAddress ? null : post.Lat,
                        Lng = (applyingStatus is FinderPostApplyingStatus.Accepted) ? post.Lng
                                    : post.HideAddress ? null : post.Lng,
                        IsAnonymous = post.IsAnonymous,
                        HideAddress = (applyingStatus is FinderPostApplyingStatus.Accepted) ? false : post.HideAddress,
                        ApplyingStatus = applyingStatus,
                        RepeatType = (RepeatingType)post.RepeatType,
                        Status = post.StartTime < DateTime.UtcNow ? FinderPostStatus.Closed:
                                            (FinderPostStatus)post.Status,
                        CreatedAt = post.CreatedAt.ToOffSet(),
                        Poster = poster
                    };
                    results.Add(result);
                }
                results = results.OrderByDescending(x => x.CreatedAt).ToList();

                return LiftNetRes<ExploreFinderPostView>.SuccessResponse(results);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while listing applied finder posts");
                return LiftNetRes<ExploreFinderPostView>.ErrorResponse("An error occurred while processing your request");
            }
        }
    }
}
