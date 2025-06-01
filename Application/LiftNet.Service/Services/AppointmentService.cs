using LiftNet.Cloudinary.Services;
using LiftNet.Contract.Dtos.Appointment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Utility.Mappers;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Service.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ILiftLogger<AppointmentService> _logger;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUnitOfWork _uow;

        public AppointmentService(ILiftLogger<AppointmentService> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<List<AppointmentDto>> ListUserAppointments(string userId)
        {
            var queryable = _uow.AppointmentRepo.GetQueryable();
            var appointments = await queryable.Where(x => x.Participants.Any(x => x.UserId == userId)).ToListAsync();
            return appointments.ToDtos();
        }

        public async Task<int> FeedBackAsync(AppointmentFeedbackRequestDto request)
        {
            try
            {
                var queryable = _uow.AppointmentFeedbackRepo.GetQueryable();

                var feedbackExist = await queryable.AnyAsync(x => x.ApppointmentId == request.AppointmentId && 
                                                                  x.ReviewerId == request.ReviewerId);

                if (feedbackExist)
                {
                    _logger.Warn($"Feedback already exists for appointment {request.AppointmentId} by user {request.ReviewerId}");
                    return 0;
                }

                string? hostImgUrl = null;

                if (request.Img != null)
                {
                    hostImgUrl = await _cloudinaryService.HostImageAsync(request.Img);
                }

                var feedback = new AppointmentFeedback
                {
                    ApppointmentId = request.AppointmentId,
                    ReviewerId = request.ReviewerId,
                    Content = request.Content,
                    Star = request.Star,
                    Img = hostImgUrl
                };
                await _uow.AppointmentFeedbackRepo.Create(feedback);
                var result = await _uow.CommitAsync();
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error in FeedBackAsync: {ex.Message}");
                throw;
            }
        }
    }
}
