using CloudinaryDotNet;
using LiftNet.Cloudinary.Services;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Commands.Requests;
using LiftNet.Handler.Appointments.Commands.Validators;
using LiftNet.SharedKenel.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
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

        public FeedbackHandler(ILiftLogger<FeedbackHandler> logger, 
                                          ICloudinaryService cloudinaryService, 
                                          IUnitOfWork uow)
        {
            _logger = logger;
            _cloudinaryService = cloudinaryService;
            _uow = uow;
        }

        public async Task<LiftNetRes> Handle(FeedbackCommand request, CancellationToken cancellationToken)
        {
            await new AppointmentFeedbackValidator().ValidateAndThrowAsync(request);
            try
            {
                if (!await CheckPermission(request.CallerId, request.AppointmentId))
                {
                    return LiftNetRes.ErrorResponse("User doesnt have permission to feedback this appointment");
                }

                var isExist = await _uow.FeedbackRepo
                                        .GetQueryable()
                                        .AnyAsync(x => x.ApppointmentId == request.AppointmentId &&
                                                       x.ReviewerId == request.CallerId);

                if (isExist)
                {
                    return LiftNetRes.ErrorResponse("Already feedback this appointment");
                }

                string? imgUrl = null;
                if (request.Medias != null)
                {
                    imgUrl = await _cloudinaryService.HostImageAsync(request.Medias);
                }

                var entity = new Feedback()
                {
                    ApppointmentId = request.AppointmentId,
                    ReviewerId = request.CallerId,
                    Medias = imgUrl,
                    Content = request.Content,
                    Star = request.Star,
                };
                await _uow.FeedbackRepo.Create(entity);
                await _uow.CommitAsync();
                return LiftNetRes.SuccessResponse(imgUrl);
            }
            catch (Exception e)
            {
                _logger.Error(e, "error occured while handling appointment feedback");
                return LiftNetRes.ErrorResponse("An error occured while handling request");
            }
        }

        private async Task<bool> CheckPermission(string callerId, string appointmentId)
        {
            var queryable = _uow.AppointmentRepo.GetQueryable();

            var any = await queryable.AnyAsync(x => x.Id == appointmentId &&
                                                x.Participants.Any(x => x.UserId == callerId));
            return any;
        }
        
    }
}
