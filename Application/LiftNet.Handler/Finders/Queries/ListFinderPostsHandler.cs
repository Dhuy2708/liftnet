using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Finders;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Queries.Requests;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Finders.Queries
{
    public class ListFinderPostsHandler : IRequestHandler<ListFinderPostsQuery, PaginatedLiftNetRes<FinderPostView>>
    {
        private readonly ILiftLogger<ListFinderPostsHandler> _logger;
        private readonly IGeoService _geoService;
        private readonly IFinderPostRepo _postRepo;
        private readonly IUserRepo _userRepo;
        private readonly IFinderPostSeenStatusRepo _seenStatusRepo;

        public ListFinderPostsHandler(ILiftLogger<ListFinderPostsHandler> logger, 
                                      IGeoService geoService, 
                                      IFinderPostRepo postRepo, 
                                      IUserRepo userRepo, 
                                      IFinderPostSeenStatusRepo seenStatusRepo)
        {
            _logger = logger;
            _geoService = geoService;
            _postRepo = postRepo;
            _userRepo = userRepo;
            _seenStatusRepo = seenStatusRepo;
        }

        public async Task<PaginatedLiftNetRes<FinderPostView>> Handle(ListFinderPostsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.Conditions == null)
                {
                    request.Conditions = new QueryCondition();
                }

                var currentUser = await _userRepo.GetById(request.UserId);
                if (currentUser == null)
                {
                    throw new Exception("current user doesnt exist");
                }

                var query = _postRepo.GetQueryable()
                                     .Where(x => x.UserId == request.UserId);

                int status = (int)FinderPostStatus.Open;
                if (request.Conditions.FindCondition("status") == null)
                {
                    query = query.Where(x => x.Status == status &&
                                             x.StartTime > DateTime.UtcNow);
                }

                var title = request.Conditions.Search;
                if (title.IsNotNullOrEmpty())
                {
                    query = query.Where(x => x.Title.Contains(title!));
                }

                var statusFilter = request.Conditions.FindCondition("status");
                if (statusFilter != null && 
                    !string.IsNullOrEmpty(statusFilter.Values.FirstOrDefault()) &&
                    int.TryParse(statusFilter.Values.First(), out status))
                {
                    if (status == (int)FinderPostStatus.Open || 
                        status == (int)FinderPostStatus.Matched)
                    {
                        query = query.Where(x => x.Status == status &&
                                                 x.StartTime > DateTime.UtcNow);
                    }
                    else if (status == (int)FinderPostStatus.Closed)
                    {
                        query = query.Where(x => x.Status == status ||
                                                 x.StartTime < DateTime.UtcNow);
                    }
                }

                query = query.OrderByDescending(x => x.CreatedAt);

                var totalCount = await query.CountAsync(cancellationToken);
                var items = await query
                    .Skip((request.Conditions.PageNumber - 1) * request.Conditions.PageSize)
                    .Take(request.Conditions.PageSize)
                    .Select(x => new FinderPostView
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Description = x.Description,
                        StartTime = x.StartTime != null ? new DateTimeOffset(x.StartTime!.Value, TimeSpan.Zero) : null,
                        EndTime = x.EndTime != null ? new DateTimeOffset(x.EndTime.Value, TimeSpan.Zero) : null,
                        StartPrice = x.StartPrice,
                        EndPrice = x.EndPrice,
                        Lat = x.Lat,
                        Lng = x.Lng,
                        PlaceName = x.PlaceName,
                        HideAddress = x.HideAddress,
                        RepeatType = (RepeatingType)x.RepeatType,
                        Status = (FinderPostStatus)x.Status,
                        CreatedAt = new DateTimeOffset(x.CreatedAt, TimeSpan.Zero),
                        LastModified = new DateTimeOffset(x.ModifiedAt, TimeSpan.Zero)
                    })
                    .ToListAsync(cancellationToken);

                var itemIds = items.Select(x => x.Id).ToList();
                var postNotiCountDict = await _seenStatusRepo.GetQueryable()
                                                          .Where(x => x.UserId == request.UserId &&
                                                                      itemIds.Contains(x.FinderPostId))
                                                          .ToDictionaryAsync(k => k.FinderPostId, v => v.NotiCount);
                items.ForEach(i =>
                {
                    i.NotiCount = postNotiCountDict.GetValueOrDefault(i.Id, 0);
                    i.Status = (FinderPostStatus)status;
                });

                items = items.OrderByDescending(x => x.LastModified).ToList();

                return PaginatedLiftNetRes<FinderPostView>.SuccessResponse(
                    items,
                    request.Conditions.PageNumber,
                    request.Conditions.PageSize,
                    totalCount);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while listing finder posts");
                return PaginatedLiftNetRes<FinderPostView>.ErrorResponse("Error occurred while listing finder posts");
            }
        }
    }
} 