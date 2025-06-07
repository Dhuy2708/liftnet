using LiftNet.Contract.Dtos;
using LiftNet.Contract.Dtos.Auth;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Admins.Commands.Requests;
using LiftNet.Ioc;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Admins.Commands
{
    public class CreateUserHandler : IRequestHandler<CreateUserCommand, LiftNetRes>
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthRepo _authRepo;
        private readonly ILiftLogger<CreateUserHandler> _logger;
        private readonly IUnitOfWork _uow;
        private readonly IGeoService _geoService;
        private readonly IAddressIndexService _addressIndexService;

        public CreateUserHandler(UserManager<User> userManager,
                               IAuthRepo authRepo,
                               IUnitOfWork uow,
                               ILiftLogger<CreateUserHandler> logger,
                               IGeoService geoService,
                               IAddressIndexService addressIndexService)
        {
            _userManager = userManager;
            _authRepo = authRepo;
            _uow = uow;
            _logger = logger;
            _geoService = geoService;
            _addressIndexService = addressIndexService;
        }

        public async Task<LiftNetRes> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            if (request.Age < 1 || request.Age > 120)
            {
                throw new BadRequestException(["Age must be between 1 and 120"], "Invalid age range.");
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                throw new BadRequestException(["Email is already registered with account."], "Email is already registered with account.");
            }

            PlaceDetailDto? placeDetail = null;

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
                Age = request.Age ?? 0,
                Gender = request.Gender ?? 0,
            };

            _logger.Info($"Admin {request.CallerId} attempting to create user: {request.Username}");
            var result = await _authRepo.RegisterAsync(registerModel);
            if (result.Succeeded)
            {
                var userId = await _uow.UserRepo.GetQueryable()
                                       .Where(x => x.UserName == request.Username)
                                       .Select(x => x.Id)
                                       .FirstOrDefaultAsync();
                await InitWallet(userId!);

                if (placeDetail == null)
                {
                    _logger.Info($"Admin {request.CallerId} successfully created user: {request.Username}");
                    return LiftNetRes.SuccessResponse(message: "User created successfully.");
                }

                var newUser = await _userManager.FindByEmailAsync(request.Email);
                try
                {
                    var cosmosResult = await InsertCosmosAsync(newUser!.Id, placeDetail);
                    if (cosmosResult != 0)
                    {
                        _logger.Info($"Admin {request.CallerId} successfully created user: {request.Username}");
                        return LiftNetRes.SuccessResponse(message: "User created successfully.");
                    }
                    else
                    {
                        await _userManager.DeleteAsync(newUser!);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error inserting into Cosmos DB");
                    await _userManager.DeleteAsync(newUser!);
                }
            }

            _logger.Error($"Admin {request.CallerId} failed to create user: {request.Username}");
            return LiftNetRes.ErrorResponse(message: "Failed to create user.", errors: result.Errors.Select(x => x.Description).ToList());
        }

        private async Task InitWallet(string userId)
        {
            var wallet = new Wallet
            {
                UserId = userId,
                Balance = 0,
                LastUpdate = DateTime.UtcNow
            };
            await _uow.WalletRepo.Create(wallet);
            await _uow.CommitAsync();
        }

        private async Task<int> InsertCosmosAsync(string userId, PlaceDetailDto placeDetail)
        {
            if (userId.IsNullOrEmpty() || placeDetail == null)
            {
                return 0;
            }

            var addressIndexData = new AddressIndexData
            {
                Id = userId,
                UserId = userId,
                Location = new LocationIndexData
                {
                    PlaceId = placeDetail.PlaceId,
                    PlaceName = placeDetail.PlaceName,
                    FormattedAddress = placeDetail.FormattedAddress,
                    Latitude = placeDetail.Latitude,
                    Longitude = placeDetail.Longitude,
                },
                Schema = DataSchema.Address,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };
            var result = await _addressIndexService.UpsertAsync(addressIndexData);
            return result != null ? 1 : 0;
        }
    }
} 