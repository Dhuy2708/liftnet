﻿using LiftNet.Cloudinary.Services;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Commands
{
    public class RequestAppointmentConfirmationHandler : IRequestHandler<RequestAppointmentConfirmationCommand, LiftNetRes>
    {
        private readonly ILiftLogger<RequestAppointmentConfirmationHandler> _logger;
        private readonly IAppointmentService _appointmentService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _uow;

        public RequestAppointmentConfirmationHandler(ILiftLogger<RequestAppointmentConfirmationHandler> logger, 
                                                     IAppointmentService appointmentService,
                                                     ICloudinaryService cloudinaryService,
                                                     IUnitOfWork uow)
        {
            _logger = logger;
            _appointmentService = appointmentService;
            _cloudinaryService = cloudinaryService;
            _uow = uow;
        }

        public async Task<LiftNetRes> Handle(RequestAppointmentConfirmationCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var queryable = _uow.AppointmentConfirmationRepo.GetQueryable();

                var isExist = await queryable.AnyAsync(x => x.AppointmentId == request.AppointmentId);

                if (isExist)
                {
                    _logger.Warn($"Confirmation already exists for appointment {request.AppointmentId} by user {request.CallerId}");
                    return LiftNetRes.ErrorResponse("Confirmation already exists for this appointment.");
                }

                if (!await CheckPermission(request.CallerId, request.AppointmentId))
                {
                    _logger.Warn($"User {request.CallerId} does not have permission request confirmation {request.AppointmentId}");
                    return LiftNetRes.ErrorResponse("You do not have permission to request confirmation this appointment.");
                }

                var endTime = await _uow.AppointmentRepo.GetQueryable()
                                            .Where(x => x.Id == request.AppointmentId)
                                            .Select(x => x.EndTime)
                                            .FirstOrDefaultAsync();

                if (endTime > DateTime.UtcNow)
                {
                    return LiftNetRes.ErrorResponse("You can only request confirmation before the appointment ends.");
                }

                string? imgUrl = null;
                if (request.Image != null)
                {
                    imgUrl = await _cloudinaryService.HostImageAsync(request.Image);
                }

                var createdTime = DateTime.UtcNow;
                var entity = new AppointmentConfirmation
                {
                    AppointmentId = request.AppointmentId,
                    Img = imgUrl,
                    Content = request.Content,
                    Status = (int)AppointmentConfirmationStatus.Requested,
                    CreatedAt = createdTime,
                    ModifiedAt = createdTime,
                    ExpiredAt = createdTime.AddHours(1)
                };

                await _uow.AppointmentConfirmationRepo.Create(entity);
                var result = await _uow.CommitAsync();
                if (result > 0)
                {
                    await _appointmentService.PingAppointmentNotiCount(request.AppointmentId, request.CallerId);
                }
                return LiftNetRes.SuccessResponse("Confirmation request created successfully.");
            }
            catch (Exception e)
            {
                _logger.Error($"Error confirming appointment {request.AppointmentId}: {e.Message}");
                return LiftNetRes.ErrorResponse("An error occurred while confirming the appointment.");
            }
        }

        private async Task<bool> CheckPermission(string callerId, string appoinmentId)
        {
            var isBooker = await _uow.AppointmentRepo.GetQueryable()
                                       .AnyAsync(x => x.Id == appoinmentId && x.BookerId == callerId);
            return isBooker;
        }
    }
}
