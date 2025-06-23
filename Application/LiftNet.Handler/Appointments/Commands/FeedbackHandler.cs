using CloudinaryDotNet;
using LiftNet.Cloudinary.Services;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.Handler.Appointments.Commands.Validators;
using LiftNet.SharedKenel.Extensions;
using LiftNet.Utility.Extensions;
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
    public class FeedbackHandler : IRequestHandler<FeedbackCommand, LiftNetRes>
    {
        private readonly ILiftLogger<FeedbackHandler> _logger;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _uow;
        private readonly IRoleService _roleService;

        public FeedbackHandler(ILiftLogger<FeedbackHandler> logger, 
                                          ICloudinaryService cloudinaryService, 
                                          IUnitOfWork uow,
                                          IRoleService roleService)
        {
            _logger = logger;
            _cloudinaryService = cloudinaryService;
            _uow = uow;
            _roleService = roleService;
        }

        public async Task<LiftNetRes> Handle(FeedbackCommand request, CancellationToken cancellationToken)
        {
            await new AppointmentFeedbackValidator().ValidateAndThrowAsync(request);
            try
            {
                var appointment = await _uow.AppointmentRepo.GetQueryable()
                                              .Include(x => x.Participants)
                                              .FirstOrDefaultAsync(x => x.Id == request.AppointmentId);
                if (appointment == null)
                {
                    return LiftNetRes.ErrorResponse("Appointment doesnt exist");
                }
                var checkPermission = await CheckPermission(request.CallerId, request.CoachId, appointment);
                if (!checkPermission.Item1)
                {
                    return LiftNetRes.ErrorResponse(checkPermission.Item2);
                }

                var checkStatus = CheckAppointment(appointment);
                if (!checkStatus)
                {
                    return LiftNetRes.ErrorResponse("Appointment is not finished yet");
                }

                var isExist = await _uow.FeedbackRepo
                                        .GetQueryable()
                                        .AnyAsync(x => x.ApppointmentId == request.AppointmentId &&
                                                       x.ReviewerId == request.CallerId);
            
                if (isExist)
                {
                    return LiftNetRes.ErrorResponse("Already feedback this appointment");
                }
                await _uow.BeginTransactionAsync();

                List<string> mediaUrls = [];
                if (request.Medias.IsNotNullOrEmpty())
                {
                    var hostImgsTask = request.Medias!.Select(x => _cloudinaryService.HostImageAsync(x));
                    mediaUrls = (await Task.WhenAll(hostImgsTask)).ToList();
                }

                var entity = new Feedback()
                {
                    ApppointmentId = request.AppointmentId,
                    ReviewerId = request.CallerId,
                    CoachId = appointment.BookerId!,
                    Medias = JsonConvert.SerializeObject(mediaUrls),
                    Content = request.Content,
                    Star = request.Star,
                    CreatedAt = DateTime.UtcNow,
                };
                await _uow.FeedbackRepo.Create(entity);
                await UpdateCoachExtension(appointment.BookerId!, request.Star);
                await _uow.CommitAsync();
                return LiftNetRes.SuccessResponse("Feedback sucessfully");
            }
            catch (Exception e)
            {
                _logger.Error(e, "error occured while handling appointment feedback");
                return LiftNetRes.ErrorResponse("An error occured while handling request");
            }
        }

        private async Task<(bool, string)> CheckPermission(string callerId, string coachId, Appointment appointment)
        {
            if (!coachId.Eq(appointment.BookerId!))
            {
                return (false, "The PT is not the booker");
            }
            var isParticipant = appointment.Participants.Any(x => x.UserId == callerId) && !appointment.BookerId.Eq(callerId);

            if (!isParticipant)
            {
                return (false, "You are not a participant of this appointment");
            }

            var role = await _roleService.GetRoleByUserId(appointment.BookerId!);
            if (role is not LiftNetRoleEnum.Coach)
            {
                return (false, "The booker is not a PT");
            }

            return (true, string.Empty);
        }

        private bool CheckAppointment(Appointment appointment)
        {
            var status = AppointmentUtil.GetAppointmentStatus(appointment);
            if (status is not AppointmentStatus.Finished)
            {
                return false;
            }
            return true;
        }

        private async Task UpdateCoachExtension(string coachId, int star)
        {
            var coachExtension = await _uow.CoachExtensionRepo.GetQueryable()
                                    .FirstOrDefaultAsync(x => x.CoachId == coachId);

            if (coachExtension == null)
            {
                coachExtension = new CoachExtension()
                {
                    CoachId = coachId,
                    SessionTrained = 1,
                    ReviewCount = 1,
                    Star = star
                };
                await _uow.CoachExtensionRepo.Create(coachExtension);
            }
            else
            {
                var totalStar = coachExtension.Star * coachExtension.ReviewCount + star;
                coachExtension.SessionTrained += 1;
                coachExtension.ReviewCount += 1;
                coachExtension.Star = totalStar / coachExtension.ReviewCount;
                await _uow.CoachExtensionRepo.Update(coachExtension);
            }
        }
    }
}
