using LiftNet.Cloudinary.Services;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Profiles.Commands.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LiftNet.Handler.Profiles.Commands
{
    public class UploadAvatarHandler : IRequestHandler<UploadAvatarCommand, LiftNetRes>
    {
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUserRepo _userRepo;
        private readonly ILiftLogger<UploadAvatarHandler> _logger;

        public UploadAvatarHandler(ICloudinaryService cloudinaryService, IUserRepo userRepo, ILiftLogger<UploadAvatarHandler> logger)
        {
            _cloudinaryService = cloudinaryService;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(UploadAvatarCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                return LiftNetRes.ErrorResponse("UserId is required");
            }
            if (request.File == null || request.File.Length == 0)
            {
                return LiftNetRes.ErrorResponse("File is required");
            }

            var user = await _userRepo.GetById(request.UserId);
            if (user == null)
            {
                return LiftNetRes.ErrorResponse("User not found");
            }

            string imageUrl;
            try
            {
                imageUrl = await _cloudinaryService.HostImageAsync(request.File);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to upload avatar to Cloudinary");
                return LiftNetRes.ErrorResponse("Failed to upload avatar");
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                return LiftNetRes.ErrorResponse("Failed to get image url from Cloudinary");
            }

            user.Avatar = imageUrl;
            await _userRepo.SaveChangesAsync();

            return LiftNetRes.SuccessResponse("Avatar updated successfully");
        }
    }
}
