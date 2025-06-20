using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Enums.Finder;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Commands.Requests;
using LiftNet.ServiceBus.Contracts;
using LiftNet.ServiceBus.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Finders.Commands
{
    public class ResponseApplicantHandler : IRequestHandler<ResponseApplicantCommand, LiftNetRes>
    {
        private readonly ILiftLogger<ResponseApplicantHandler> _logger;
        private readonly IFinderPostRepo _postRepo;
        private readonly IFinderPostApplicantRepo _applicantRepo;
        private readonly IUserRepo _userRepo;
        private readonly IUserService _userService;
        private readonly IEventBusService _eventBusService;

        public ResponseApplicantHandler(ILiftLogger<ResponseApplicantHandler> logger,
                                        IFinderPostRepo postRepo,
                                        IFinderPostApplicantRepo applicantRepo,
                                        IUserRepo userRepo,
                                        IUserService userService,
                                      IEventBusService eventBusService)
        {
            _logger = logger;
            _postRepo = postRepo;
            _applicantRepo = applicantRepo;
            _userRepo = userRepo;
            _userService = userService;
            _eventBusService = eventBusService;
        }

        public async Task<LiftNetRes> Handle(ResponseApplicantCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryable = _applicantRepo.GetQueryable()
                                              .Include(x => x.Post);

                var applicant = await queryable.FirstOrDefaultAsync(x => x.Id == request.ApplicantId);
                if (applicant == null)
                {
                    return LiftNetRes.ErrorResponse("Applicant not found");
                }

                if (applicant.Post.UserId != request.UserId)
                {
                    return LiftNetRes.ErrorResponse("You don't have permission to respond to this applicant");
                }

                if (applicant.Status != (int)FinderPostApplyingStatus.Applying)
                {
                    return LiftNetRes.ErrorResponse("Applicant is not in a pending state");
                }

                if (request.Status == FinderPostResponseType.Accept)
                {
                    applicant.Status = (int)FinderPostApplyingStatus.Accepted;
                    applicant.Post.Status = (int)FinderPostStatus.Matched;

                    var otherApplicants = await _applicantRepo.GetQueryable()
                        .Where(x => x.PostId == applicant.PostId
                                    && x.Id != applicant.Id
                                    && x.Status == (int)FinderPostApplyingStatus.Applying)
                        .ToListAsync(cancellationToken);

                    foreach (var other in otherApplicants)
                    {
                        other.Status = (int)FinderPostApplyingStatus.Rejected;
                    }

                    await _applicantRepo.UpdateRange(otherApplicants);
                    await _applicantRepo.Update(applicant);
                    await _postRepo.Update(applicant.Post);
                    await _applicantRepo.SaveChangesAsync();
                }
                else if (request.Status == FinderPostResponseType.Reject)
                {
                    applicant.Status = (int)FinderPostApplyingStatus.Rejected;
                    await _applicantRepo.Update(applicant);
                    await _applicantRepo.SaveChangesAsync();
                }
                else
                {
                    return LiftNetRes.ErrorResponse("Invalid status");
                }

                await SendNoti(applicant.PostId, request.UserId, applicant.TrainerId, request.Status);
                return LiftNetRes.SuccessResponse();
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error while responding to applicant");
                return LiftNetRes.ErrorResponse("An error occurred while processing your request.");
            }
        }

        private async Task SendNoti(string postId, string callerId, string applicantId, FinderPostResponseType type)
        {
            var callerName = await _userRepo.GetQueryable()
                                        .Where(x => x.Id == callerId)
                                        .Select(x => new { x.FirstName, x.LastName })
                                        .FirstOrDefaultAsync();

            NotiEventType eventType = type switch
            {
                FinderPostResponseType.Accept => NotiEventType.AcceptFinder,
                FinderPostResponseType.Reject => NotiEventType.RejectFinder,
                _ => NotiEventType.None,
            };

            var message = new EventMessage
            {
                Type = EventType.Noti,
                Context = JsonConvert.SerializeObject(new NotiMessageDto()
                {
                    SenderId = callerId,
                    EventType = eventType,
                    Location = NotiRefernceLocationType.Finder,
                    SenderType = NotiTarget.User,
                    RecieverId = applicantId,
                    RecieverType = NotiTarget.User,
                    CreatedAt = DateTime.UtcNow,
                    ObjectNames = [callerName!.FirstName + " " + callerName!.LastName, postId]
                })
            };


            await _eventBusService.PublishAsync(message, QueueNames.Noti);
        }
    }
}
