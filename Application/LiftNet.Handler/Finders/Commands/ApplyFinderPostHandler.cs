using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Commands.Requests;
using LiftNet.ServiceBus.Contracts;
using LiftNet.ServiceBus.Interfaces;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LiftNet.Handler.Finders.Commands
{
    public class ApplyFinderPostHandler : IRequestHandler<ApplyFinderPostCommand, LiftNetRes>
    {
        private readonly ILiftLogger<ApplyFinderPostHandler> _logger;
        private readonly IFinderPostRepo _postRepo;
        private readonly IUnitOfWork _uow;
        private readonly IEventBusService _eventBusService;

        public ApplyFinderPostHandler(ILiftLogger<ApplyFinderPostHandler> logger, 
                                      IFinderPostRepo postRepo, 
                                      IUnitOfWork uow,
                                      IEventBusService eventBusService)
        {
            _logger = logger;
            _postRepo = postRepo;
            _uow = uow;
            _eventBusService = eventBusService;
        }

        public async Task<LiftNetRes> Handle(ApplyFinderPostCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var post = await _postRepo.GetById(request.PostId);
                if (post == null)
                {
                    return LiftNetRes.ErrorResponse("Post not found");
                }

                if (post.StartTime != null && post.StartTime < DateTime.UtcNow)
                {
                    return LiftNetRes.ErrorResponse("Post has been expired");
                }

                if (post.Status != (int)FinderPostStatus.Open)
                {
                    return LiftNetRes.ErrorResponse("Post is not open for applications");
                }

                await _uow.BeginTransactionAsync();

                var existingApplication = await _uow.FinderPostApplicantRepo.GetQueryable()
                                                    .FirstOrDefaultAsync(x => x.PostId == request.PostId && 
                                                                              x.TrainerId == request.UserId); 

                if (existingApplication != null)
                {
                    return LiftNetRes.ErrorResponse("You have already applied to this post");
                }

                var application = new FinderPostApplicant
                {
                    PostId = request.PostId,
                    TrainerId = request.UserId,
                    Message = request.Message,
                    Status = (int)FinderPostApplyingStatus.Applying,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };

                await _uow.FinderPostApplicantRepo.Create(application);
                var updateTime = DateTime.UtcNow;
                await UpdateSeenStatus(post.UserId, request.PostId, updateTime);
                post.ModifiedAt = updateTime;
                await _uow.FinderPostRepo.Update(post);

                await _uow.CommitAsync();
                await SendNoti(request.PostId, request.UserId, post.UserId);
                return LiftNetRes.SuccessResponse();
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                _logger.Error(ex, "Error occurred while applying to finder post");
                return LiftNetRes.ErrorResponse("Error occurred while applying to finder post");
            }
        }

        private async Task UpdateSeenStatus(string posterId, string postId, DateTime updateTime)
        {
            var queryable = _uow.FinderPostSeenStatusRepo.GetQueryable();
            var seenStatus = await queryable
                                    .FirstOrDefaultAsync(x => x.UserId == posterId &&
                                                              x.FinderPostId == postId);

            if (seenStatus != null)
            {
                seenStatus.NotiCount += 1;
                seenStatus.LastUpdate = updateTime;
                await _uow.FinderPostSeenStatusRepo.Update(seenStatus);
            }
            else
            {
                seenStatus = new FinderPostSeenStatus
                {
                    UserId = posterId,
                    FinderPostId = postId,
                    NotiCount = 1,
                    LastSeen = null,
                    LastUpdate = updateTime
                };
                await _uow.FinderPostSeenStatusRepo.Create(seenStatus);
            }
        }

        private async Task SendNoti(string postId, string callerId, string posterId)
        {
            var callerName = await _uow.UserRepo.GetQueryable()
                                        .Where(x => x.Id == callerId)
                                        .Select(x => new { x.FirstName, x.LastName })
                                        .FirstOrDefaultAsync();

            var message = new EventMessage
            {
                Type = EventType.Noti,
                Context = JsonConvert.SerializeObject(new NotiMessageDto()
                {
                    SenderId = callerId,
                    EventType = NotiEventType.ApplyFinder,
                    Location = NotiRefernceLocationType.Finder,
                    SenderType = NotiTarget.User,
                    RecieverId = posterId,
                    RecieverType = NotiTarget.User,
                    CreatedAt = DateTime.UtcNow,
                    ObjectNames = [callerName!.FirstName + " " + callerName!.LastName, postId]
                })
            };


            await _eventBusService.PublishAsync(message, QueueNames.Noti);
        }
    }
} 