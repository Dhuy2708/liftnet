using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Enums.Indexes;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.ServiceBus.Contracts;
using LiftNet.ServiceBus.Interfaces;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Mappers;
using LiftNet.Utility.Utils;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands
{
    public class BookAppointmentHandler : IRequestHandler<BookAppointmentCommand, LiftNetRes>
    {
        private readonly ILiftLogger<BookAppointmentHandler> _logger;
        private readonly IAppointmentRepo _appointmentRepo;
        private readonly IUserRepo _userRepo;
        private readonly IAppointmentSeenStatusRepo _seenRepo;
        private readonly IEventIndexService _indexService;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;
        private readonly IEventBusService _eventBusService;

        public BookAppointmentHandler(ILiftLogger<BookAppointmentHandler> logger,
                                      IAppointmentRepo appointmentRepo,
                                      IUserRepo userRepo,
                                      IAppointmentSeenStatusRepo seenRepo, 
                                      IEventIndexService indexService, 
                                      IRoleService roleService, 
                                      IUserService userService,
                                      IEventBusService eventBusService)
        {
            _logger = logger;
            _appointmentRepo = appointmentRepo;
            _userRepo = userRepo;
            _seenRepo = seenRepo;
            _indexService = indexService;
            _roleService = roleService;
            _userService = userService;
            _eventBusService = eventBusService;
        }

        public async Task<LiftNetRes> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
        {
            _logger.Info("begin create a appointment");
            var entity = request.Appointment.ToEntity();
            foreach (var part in entity.Participants)
            {
                if (part.IsBooker)
                {
                    part.Status = (int)AppointmentParticipantStatus.Accepted;
                }
                else
                {
                    part.Status = (int)AppointmentParticipantStatus.Pending;
                }
            }
            if (request.Appointment.StartTime >= request.Appointment.EndTime)
            {
                throw new BadRequestException(["Time is invalid"]);
            }
            if (request.Appointment.StartTime < DateTime.UtcNow)
            {
                throw new BadRequestException(["Time is invalid"]);
            }
            entity.Created = DateTime.UtcNow;
            entity.Modified = DateTime.UtcNow;
            AsssignAllAcceptedStatus(entity);
            var result = await _appointmentRepo.Create(entity);
            await AsignNotiCount(entity.Participants.Select(x => x.UserId).ToList(), entity.Id, request.CallerId);   
            if (result == 0)
            {
                return LiftNetRes.ErrorResponse($"Create appointment {request.Appointment.Name} failed");
            }
            
            var participantIds = request.Appointment.Participants.Select(x => x.Id).ToList();
            var participants = await _userService.GetByIdsAsync(participantIds);
            var userIdRoleDict = await _userService.GetUserIdRoleDict(participantIds);

            List<EventIndexData> events = [];
            foreach (var item in participantIds)
            {
                var eventItem = new EventIndexData()
                {
                    UserId = item,
                    Schema = DataSchema.Event,
                    Title = request.Appointment.Name,
                    Description = request.Appointment.Description,
                    StartTime = request.Appointment.StartTime,
                    AppointmentId = entity.Id,
                    EndTime = request.Appointment.EndTime,
                    Rule = (RepeatRule)(int)request.Appointment.RepeatingType,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    Location = request.Appointment.PlaceDetail?.ToLocationIndexData(),
                };
                try
                {
                    await _indexService.InsertEvent(eventItem);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error creating event for user {item}");
                    await _appointmentRepo.HardDelete(entity);
                    return LiftNetRes.ErrorResponse($"Create appointment {request.Appointment.Name} failed");
                }
            }
            participantIds.Remove(request.CallerId);
            await SendNoti(entity.Id, request.CallerId, participantIds);
            _logger.Info("create appointment successfully");
            return LiftNetRes.SuccessResponse($"Create appointment {request.Appointment.Name} successfully");
        }

        private async Task AsignNotiCount(List<string> participantIds, string appointmentId, string callerId)
        {
            List<AppointmentSeenStatus> entitiesToCreate = [];
            foreach (var participantId in participantIds)
            {
                var seenStatus = new AppointmentSeenStatus()
                {
                    AppointmentId = appointmentId,
                    UserId = participantId,
                    NotiCount = participantId.Eq(callerId) ? 0 : 1,
                    LastUpdate = DateTime.UtcNow
                };
                entitiesToCreate.Add(seenStatus);
            }
            await _seenRepo.CreateRange(entitiesToCreate);
        }

        private void AsssignAllAcceptedStatus(Appointment appointment)
        {
            if (appointment.Participants.All(x => x.Status == (int)AppointmentParticipantStatus.Accepted))
            {
                appointment.AllAccepted = true;
            }
            else
            {
                appointment.AllAccepted = false;
            }
        }

        private async Task SendNoti(string appointmentId, string callerId, List<string> otherParticipantIds)
        {
            if (otherParticipantIds.IsNullOrEmpty())
            {
                return;
            }

            var callerName = await _userRepo.GetQueryable()
                                        .Where(x => x.Id == callerId)
                                        .Select(x => new { x.FirstName, x.LastName })
                                        .FirstOrDefaultAsync();
            List<EventMessage> messages = [];

            otherParticipantIds.ForEach(x =>
            {
                messages.Add(new EventMessage
                {
                    Type = EventType.Noti,
                    Context = JsonConvert.SerializeObject(new NotiMessageDto()
                    {
                        SenderId = callerId,
                        EventType = NotiEventType.BookAppointment,
                        Location = NotiRefernceLocationType.Appointment,
                        SenderType = NotiTarget.User,
                        RecieverId = x,
                        RecieverType = NotiTarget.User,
                        CreatedAt = DateTime.UtcNow,
                        ObjectNames = [callerName!.FirstName + " " + callerName!.LastName, appointmentId]
                    })
                });
                   
            });

            var sendTasks = messages.Select(x => _eventBusService.PublishAsync(x, QueueNames.Noti)).ToList();
            await Task.WhenAll(sendTasks);
        }
    }
}
