using LiftNet.Contract.Dtos.Auth;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auths.Commands.Requests;
using LiftNet.Handler.Auths.Commands.Validators;
using LiftNet.Ioc;
using LiftNet.SharedKenel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Auths.Commands
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, LiftNetRes>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthRepo _authRepo;
        private readonly ILiftLogger<RegisterHandler> _logger;
        private readonly IUnitOfWork _uow;

        public RegisterHandler(UserManager<User> userManager, 
                               IAuthRepo authRepo, 
                               IUnitOfWork uow,
                               ILiftLogger<RegisterHandler> logger)
        {
            _userManager = userManager;
            _authRepo = authRepo;
            _uow = uow;
            _logger = logger;
        }

        public async Task<LiftNetRes> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            await new RegisterCommandValidator().ValidateAndThrowAsync(request);

            if (request.Address != null)
            {
                var provinceCode = request.Address!.ProvinceCode;
                var districtCode = request.Address!.DistrictCode;
                var wardCode = request.Address!.WardCode;

                if (provinceCode == 0 || districtCode == 0 || wardCode == 0)
                {
                    throw new BadRequestException(["Address codes are not valid"], "Address codes are not valid.");
                }
                var isAddressValid = await _uow.WardRepo.GetQueryable()
                                               .AnyAsync(w => w.Code == wardCode &&
                                                              w.DistrictCode == districtCode &&
                                                              w.District.ProvinceCode == provinceCode);
                if (!isAddressValid)
                {
                    throw new BadRequestException(["Address codes are not valid"], "Address codes are not valid.");
                }
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                throw new BadRequestException(["Email is already registered with account."], "Email is already registered with account.");
            }
            user = await _userManager.FindByNameAsync(request.Username);
            if (user != null)
            {
                throw new BadRequestException(["Username is already taken."], "Username is already taken.");
            }
            var registerModel = new RegisterModel()
            {
                Role = request.Role,
                Email = request.Email,
                Username = request.Username,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                ProvinceCode = request.Address?.ProvinceCode,
                DistrictCode = request.Address?.DistrictCode,
                WardCode = request.Address?.WardCode,
                Location = request.Address?.Location
            };
            _logger.Info($"attempt to register, username: {request.Username}");
            var result = await _authRepo.RegisterAsync(registerModel);
            if (result.Succeeded)
            {
                _logger.Info("register user successfully");
                return LiftNetRes.SuccessResponse(message: "User registered successfully.");
            }
            _logger.Error("failed to register user");
            return LiftNetRes.ErrorResponse(message: "Failed to register.", errors: result.Errors.Select(x => x.Description).ToList());
        }
    }
}
