using LiftNet.Contract.Dtos.Auth;
using LiftNet.Contract.Interfaces.Repositories;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Response;
using LiftNet.Handler.Auth.Commands.Requests;
using LiftNet.Handler.Auth.Commands.Validators;
using LiftNet.SharedKenel.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Auth.Commands
{
    public class RegisterHandler : IRequestHandler<RegisterCommand, LiftNetRes>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthRepo _authRepo;
        public RegisterHandler(UserManager<User> userManager, IAuthRepo authRepo)
        {
            _userManager = userManager;
            _authRepo = authRepo;
        }

        public async Task<LiftNetRes> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            await new RegisterCommandValidator(_userManager).ValidateAndThrowAsync(request);
            var registerModel = new RegisterModel()
            { 
                Email = request.Email,
                Username = request.Username,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address
            };
            var result = await _authRepo.RegisterAsync(registerModel);
            if (result.Succeeded)
            {
                return LiftNetRes.SuccessResponse(message: "User registered successfully.");
            }
            return LiftNetRes.ErrorResponse(message: "Failed to register.", errors: result.Errors.Select(x => x.Description).ToList());
        }
    }
}
