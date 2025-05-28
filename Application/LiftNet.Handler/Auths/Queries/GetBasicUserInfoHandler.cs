using LiftNet.Contract.Views;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Response;
using LiftNet.Domain.Interfaces;
using LiftNet.Handler.Auths.Queries.Requests;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LiftNet.Domain.Entities;

namespace LiftNet.Handler.Auths.Queries
{
    public class GetBasicUserInfoHandler : IRequestHandler<GetBasicUserInfoQuery, LiftNetRes<BasicUserInfo>>
    {
        private readonly IUserService _userService;
        private readonly ILiftLogger<GetBasicUserInfoHandler> _logger;
        private readonly UserManager<User> _userManager;

        public GetBasicUserInfoHandler(
            IUserService userService,
            ILiftLogger<GetBasicUserInfoHandler> logger,
            UserManager<User> userManager)
        {
            _userService = userService;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<LiftNetRes<BasicUserInfo>> Handle(GetBasicUserInfoQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var userInfo = await _userService.GetBasicUserInfoAsync(request.UserId);
                
                if (userInfo == null)
                {
                    _logger.Warn($"User not found or inactive with ID: {request.UserId}");
                    return LiftNetRes<BasicUserInfo>.ErrorResponse("User not found or inactive");
                }

                _logger.Info($"Successfully retrieved basic info for user: {request.UserId}");
                return LiftNetRes<BasicUserInfo>.SuccessResponse(userInfo);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"Error getting basic info for user: {request.UserId}");
                return LiftNetRes<BasicUserInfo>.ErrorResponse("Error retrieving user information");
            }
        }
    }
} 