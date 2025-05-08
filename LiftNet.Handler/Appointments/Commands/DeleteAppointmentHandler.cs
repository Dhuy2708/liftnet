using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Appointments.Commands
{
    public class DeleteAppointmentHandler : IRequestHandler<DeleteAppointmentCommand, LiftNetRes>
    {
        private readonly ILiftLogger<DeleteAppointmentHandler> _logger;
        private readonly IAppointmentRepo _appointmentRepo;
        private readonly IEventIndexService _eventIndexService;

        public DeleteAppointmentHandler(
            IAppointmentRepo appointmentRepo,
            IEventIndexService eventIndexService,
            ILiftLogger<DeleteAppointmentHandler> logger)
        {
            _appointmentRepo = appointmentRepo;
            _eventIndexService = eventIndexService;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.Info($"begin delete appointment {request.AppointmentId}");

                // Get appointment with participants
                var appointment = await _appointmentRepo.GetQueryable()
                    .Include(x => x.Participants)
                    .FirstOrDefaultAsync(x => x.Id == request.AppointmentId);

                if (appointment == null)
                {
                    return LiftNetRes.ErrorResponse("Appointment not found");
                }

                // Check if user is a participant
                if (!appointment.Participants.Any(p => p.UserId == request.UserId))
                {
                    return LiftNetRes.ErrorResponse("You are not authorized to delete this appointment");
                }

                // Delete events from Cosmos DB
                var condition = new QueryCondition();
                condition.AddCondition(new ConditionItem("appointmentid", new List<string> { request.AppointmentId }, Contract.Enums.FilterType.String));
                
                try
                {
                    await _eventIndexService.DeleteByConditionAsync(condition);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error deleting events for appointment {request.AppointmentId}");
                    return LiftNetRes.ErrorResponse("Failed to delete appointment events");
                }

                // Delete appointment (this will cascade delete participants)
                var result = await _appointmentRepo.HardDelete(appointment);
                if (result == 0)
                {
                    return LiftNetRes.ErrorResponse("Failed to delete appointment");
                }

                _logger.Info($"delete appointment {request.AppointmentId} successfully");
                return LiftNetRes.SuccessResponse("Appointment deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in DeleteAppointmentHandler for appointment {request.AppointmentId}");
                return LiftNetRes.ErrorResponse("Internal server error");
            }
        }
    }
} 