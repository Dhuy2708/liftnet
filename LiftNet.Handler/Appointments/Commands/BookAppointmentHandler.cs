using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.Utility.Mappers;
using LiftNet.Utility.Utils;
using MediatR;
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
        private readonly IScheduleIndexService _indexService;
        private readonly IRoleService _roleService;
        private readonly IUserService _userService;

        public BookAppointmentHandler(IAppointmentRepo appointmentRepo, 
                                      IScheduleIndexService indexService, 
                                      IUserService userService,
                                      ILiftLogger<BookAppointmentHandler> logger,
                                      IRoleService roleService)
        {
            _appointmentRepo = appointmentRepo;
            _indexService = indexService;
            _userService = userService;
            _roleService = roleService;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(BookAppointmentCommand request, CancellationToken cancellationToken)
        {
            _logger.Info("begin create a appointment");
            request.Appointment.Status = Contract.Enums.AppointmentStatus.Pending;
            var entity = request.Appointment.ToEntity();
            entity.Created = DateTime.UtcNow;
            entity.Modified = DateTime.UtcNow;
            var result = await _appointmentRepo.Create(entity);

            if (result == 0)
            {
                return LiftNetRes.ErrorResponse($"Create appointment {request.Appointment.Name} failed");
            }
            
            var participantIds = request.Appointment.Participants.Select(x => x.Id).ToList();
            var participants = await _userService.GetByIdsAsync(participantIds);
            var userIdRoleDict = await _userService.GetUserIdRoleDict(participantIds);

            var bookerEvent = new EventIndexData()
            {
                UserId = request.Appointment.Booker.Id,
                Schema = DataSchema.Event,
                Title = request.Appointment.Name,
                Description = request.Appointment.Description,
                StartTime = request.Appointment.StartTime,
                EndTime = request.Appointment.EndTime,
                Rule = Domain.Enums.Indexes.RepeatRule.None,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
            };

            //participantIds.ForEach(p =>
            //{

            //});

            //var participantEvent = new EventIndexData()
            //{
            //    UserId = request.Appointment.Participants,
            //    Schema = DataSchema.Event,
            //    Title = request.Appointment.Name,
            //    Description = request.Appointment.Description,
            //    StartTime = request.Appointment.StartTime,
            //    EndTime = request.Appointment.EndTime,
            //    Rule = Domain.Enums.Indexes.RepeatRule.None,
            //    CreatedAt = DateTime.UtcNow,
            //    ModifiedAt = DateTime.UtcNow,
            //}

            List<ParticipantIndexData> participantIndex = [];
            participants.ForEach(p =>
            {
                if (userIdRoleDict.TryGetValue(p.Id, out var role))
                {
                    participantIndex.Add(new ParticipantIndexData()
                    {
                        Id = p.Id,
                        Username = p.UserName!,
                        Email = p.Email!,
                        FirstName = p.FirstName,
                        LastName = p.LastName,
                        Role = role,
                        Avatar = p.Avatar,
                        Status = Domain.Enums.Indexes.ParticipantStatus.Pending
                    });
                }
            });
            bookerEvent.Participants = participantIndex;
            var resultIndex = await _indexService.UpsertAsync(bookerEvent);

            if (resultIndex != null)
            {
                return LiftNetRes.SuccessResponse($"Create appointment {request.Appointment.Name} successfully");
            }

            return LiftNetRes.ErrorResponse($"Create appointment {request.Appointment.Name} failed");
        }
    }
}
