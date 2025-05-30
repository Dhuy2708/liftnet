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
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Queries
{
    public class ListFinderPostApplicantsHandler : IRequestHandler<ListFinderPostApplicantsQuery, LiftNetRes<FinderPostApplicantView>>
    {
        private readonly ILiftLogger<ListFinderPostApplicantsHandler> _logger;
        private readonly IFinderPostRepo _postRepo;
        private readonly IFinderPostApplicantRepo _applicantRepo;
        private readonly IFinderPostSeenStatusRepo _seenStatusRepo;
        private readonly IUserRepo _userRepo;
        private readonly IUserService _userService;

        public ListFinderPostApplicantsHandler(ILiftLogger<ListFinderPostApplicantsHandler> logger,
                                               IFinderPostRepo postRepo, 
                                               IFinderPostApplicantRepo applicantRepo, 
                                               IFinderPostSeenStatusRepo seenStatusRepo,
                                               IUserRepo userRepo, 
                                               IUserService userService)
        {
            _logger = logger;
            _postRepo = postRepo;
            _applicantRepo = applicantRepo;
            _seenStatusRepo = seenStatusRepo;
            _userRepo = userRepo;
            _userService = userService;
        }

        public async Task<LiftNetRes<FinderPostApplicantView>> Handle(ListFinderPostApplicantsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _postRepo.GetById(request.PostId);
                if (post == null)
                {
                    return LiftNetRes<FinderPostApplicantView>.ErrorResponse("Post not found");
                }

                if (post.UserId != request.UserId)
                {
                    return LiftNetRes<FinderPostApplicantView>.ErrorResponse("You don't have permission to view applicants for this post");
                }

                var applicants = await _applicantRepo.GetQueryable()
                                        .Where(x => x.PostId == request.PostId)
                                        .OrderByDescending(x => x.CreatedAt)
                                        .ToListAsync(cancellationToken);

                if (applicants.IsNullOrEmpty())
                {
                    return LiftNetRes<FinderPostApplicantView>.SuccessResponse([]);
                }

                var trainerIds = applicants.Select(x => x.TrainerId).Distinct().ToList();
                var trainers = await _userRepo.GetQueryable()
                                              .Where(x => trainerIds.Contains(x.Id))
                                              .ToListAsync();
                var userRoleDict = await _userService.GetUserIdRoleDict(trainerIds);
                var trainerOverviewDict = trainers.ToOverviews(userRoleDict).ToDictionary(k => k.Id, v => v);

                var applicantViews = applicants.Select(applicant => new FinderPostApplicantView
                {
                    Id = applicant.Id,
                    PostId = applicant.PostId,
                    Message = applicant.Message,
                    CancelReason = applicant.CancelReason,
                    Status = (FinderPostApplyingStatus)applicant.Status,
                    CreatedAt = applicant.CreatedAt,
                    ModifiedAt = applicant.ModifiedAt,
                    Trainer = trainerOverviewDict.GetValueOrDefault<string, UserOverview>(applicant.TrainerId, null)
                }).ToList();

                await UpdateSeenStatus(request.UserId, request.PostId, post.ModifiedAt);
                return LiftNetRes<FinderPostApplicantView>.SuccessResponse(applicantViews);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while listing finder post applicants");
                return LiftNetRes<FinderPostApplicantView>.ErrorResponse("Error occurred while listing finder post applicants");
            }
        }

        private async Task UpdateSeenStatus(string userId, string postId, DateTime postLastModified)
        {
            var seenStatus = await _seenStatusRepo.GetQueryable()
                                                  .FirstOrDefaultAsync(x => x.UserId == userId &&
                                                                            x.FinderPostId == postId);
            if (seenStatus == null)
            {
                seenStatus = new FinderPostSeenStatus
                {
                    UserId = userId,
                    FinderPostId = postId,
                    NotiCount = 0,
                    LastSeen = DateTime.UtcNow,
                };
                await _seenStatusRepo.Create(seenStatus);
            }
            else
            {
                seenStatus.NotiCount = 0;
                seenStatus.LastSeen = DateTime.UtcNow;
                seenStatus.LastUpdate = postLastModified;
                await _seenStatusRepo.Update(seenStatus);
            }
        }
    }
} 