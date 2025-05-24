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
                var entities = await _postRepo.GetQueryable()
                                          .Include(x => x.Post)
                                          .Where(x => x.TrainerId == trainerId &&
                                                      x.Status == (int)FinderPostApplyingStatus.Applying)
                                          .Select(x => x.Post)
                                          .ToListAsync(cancellationToken);
                if (entities.IsNullOrEmpty())
                {
                    return LiftNetRes<ExploreFinderPostView>.SuccessResponse([]);
                }

                List<ExploreFinderPostView> results = [];
                var posterIds = entities.Where(x => x.IsAnonymous == false)
                                        .Select(x => x.UserId)
                                        .Distinct()
                                        .ToList();

                var userOverviewDict = (await _userService.Convert2Overviews(posterIds))
                                            .ToDictionary(k => k.Id, v => v);

                foreach (var post in entities)
                {
                    var distanceAway = GeoUtil.CalculateDistance(
                                                trainerAddress.Lat,
                                                trainerAddress.Lng,
                                                post.Lat,
                                                post.Lng
                                            );
                    var result = new ExploreFinderPostView
                    {
                        Id = post.Id,
                        Title = post.Title,
                        DistanceAway = distanceAway,
                        Description = post.Description,
                        StartTime = post.StartTime,
                        EndTime = post.EndTime,
                        StartPrice = post.StartPrice,
                        EndPrice = post.EndPrice,
                        PlaceName = post.HideAddress ? null : post.PlaceName,
                        Lat = post.HideAddress ? null : post.Lat,
                        Lng = post.HideAddress ? null : post.Lng,
                        IsAnonymous = post.IsAnonymous,
                        HideAddress = post.HideAddress,
                        ApplyingStatus = FinderPostApplyingStatus.Applying,
                        RepeatType = (RepeatingType)post.RepeatType,
                        Status = (FinderPostStatus)post.Status,
                        CreatedAt = post.CreatedAt,
                        Poster = post.IsAnonymous
                           ? null
                           : userOverviewDict.GetValueOrDefault<string, UserOverview>(post.UserId)
                    };
                    results.Add(result);
                }

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
