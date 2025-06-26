using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Finders.Commands.Requests;
using LiftNet.ServiceBus.Contracts;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiftNet.ServiceBus.Interfaces;

namespace LiftNet.Handler.Finders.Commands
{
    public class RecommendSeekerToPtHandler : IRequestHandler<RecommendSeekerToPtCommand, LiftNetRes>
    {
        private readonly ILiftLogger<RecommendSeekerToPtHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IEventBusService _eventBusService;

        public RecommendSeekerToPtHandler(ILiftLogger<RecommendSeekerToPtHandler> logger, IUnitOfWork uow, IEventBusService eventBusService)
        {
            _logger = logger;
            _uow = uow;
            _eventBusService = eventBusService;
        }

        public async Task<LiftNetRes> Handle(RecommendSeekerToPtCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (request.SeekerId.IsNullOrEmpty())
                {
                    throw new BadRequestException(["SeekerId cannot be null or empty."]);
                }

                if (request.PTIds == null || !request.PTIds.Any())
                {
                    throw new BadRequestException(["PTIds cannot be null or empty."]);
                }

                if (!await _uow.UserRepo.GetQueryable().AnyAsync(x => x.Id == request.SeekerId))
                {
                    throw new BadRequestException([$"Seeker with ID {request.SeekerId} does not exist."]);
                }

                _logger.Info($"Processing recommendations for seeker {request.SeekerId} to {request.PTIds.Count} coaches");
                await _uow.BeginTransactionAsync();

                var recommendationRepo = _uow.CoachRecommendationRepo;
                var existingRecommendations = await recommendationRepo.GetQueryable()
                    .Where(r => r.SeekerId == request.SeekerId && request.PTIds.Contains(r.CoachId))
                    .ToListAsync(cancellationToken);

                var existingCoachIds = existingRecommendations.Select(r => r.CoachId).ToHashSet();
                var newCoachIds = request.PTIds.Where(ptId => !existingCoachIds.Contains(ptId)).ToList();

                var recommendationsToUpdate = new List<CoachRecommendation>();
                var recommendationsToCreate = new List<CoachRecommendation>();

                // Update existing recommendations
                foreach (var existingRec in existingRecommendations)
                {
                    existingRec.Description = request.Description;
                    existingRec.LastUpdated = DateTime.UtcNow;
                    recommendationsToUpdate.Add(existingRec);
                }

                // Create new recommendations
                foreach (var coachId in newCoachIds)
                {
                    var newRecommendation = new CoachRecommendation
                    {
                        SeekerId = request.SeekerId,
                        CoachId = coachId,
                        Description = request.Description,
                        LastUpdated = DateTime.UtcNow
                    };
                    recommendationsToCreate.Add(newRecommendation);
                }

                if (recommendationsToUpdate.Any())
                {
                    await recommendationRepo.UpdateRange(recommendationsToUpdate);
                }

                if (recommendationsToCreate.Any())
                {
                    await recommendationRepo.CreateRange(recommendationsToCreate);
                }

                await _uow.CommitAsync();
                await SendNoti(request.SeekerId, request.PTIds);

                var message = $"Successfully processed {recommendationsToUpdate.Count} updates and {recommendationsToCreate.Count} new recommendations for seeker {request.SeekerId}";
                _logger.Info(message);
                return LiftNetRes.SuccessResponse(message);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error while recommending seeker to PT");
                return LiftNetRes.ErrorResponse("An error occurred while processing your request.");
            }
        }

        private async Task SendNoti(string seekerId, List<string> ptIds)
        {
            if (ptIds.IsNullOrEmpty())
            {
                return;
            }

            var callerName = await _uow.UserRepo.GetQueryable()
                                        .Where(x => x.Id == seekerId)
                                        .Select(x => new { x.FirstName, x.LastName })
                                        .FirstOrDefaultAsync();
            List<EventMessage> messages = [];

            ptIds.ForEach(x =>
            {
                messages.Add(new EventMessage
                {
                    Type = EventType.Noti,
                    Context = JsonConvert.SerializeObject(new NotiMessageDto()
                    {
                        SenderId = seekerId,
                        EventType = NotiEventType.RecommendSeekerToPt,
                        Location = NotiRefernceLocationType.SeekerRecommendation,
                        SenderType = NotiTarget.User,
                        RecieverId = x,
                        RecieverType = NotiTarget.User,
                        CreatedAt = DateTime.UtcNow,
                        ObjectNames = [callerName!.FirstName + " " + callerName!.LastName]
                    })
                });

            });

            var sendTasks = messages.Select(x => _eventBusService.PublishAsync(x, QueueNames.Noti)).ToList();
            await Task.WhenAll(sendTasks);
        }
    }
}
