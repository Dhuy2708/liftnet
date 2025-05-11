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
using LiftNet.Handler.Auths.Commands.Requests;
using LiftNet.Handler.Auths.Commands.Validators;
using LiftNet.Ioc;
using LiftNet.SharedKenel.Extensions;
using LiftNet.Utility.Extensions;
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
        private readonly IGeoService _geoService;
        private readonly IAddressIndexService _addressIndexService;

        public RegisterHandler(UserManager<User> userManager, 
                               IAuthRepo authRepo, 
                               IUnitOfWork uow,
                               ILiftLogger<RegisterHandler> logger,
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

        public async Task<LiftNetRes> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            await new RegisterCommandValidator().ValidateAndThrowAsync(request);
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user != null)
            {
                throw new BadRequestException(["Email is already registered with account."], "Email is already registered with account.");
            }
            PlaceDetailDto? placeDetail = null;
            Address? address = null;
            if (request.Address != null)
            {
                var provinceCode = request.Address!.ProvinceCode;
                var districtCode = request.Address!.DistrictCode;
                var wardCode = request.Address!.WardCode;
                var rawLocation = request.Address.Location;
                
                if (provinceCode == 0 || districtCode == 0 || wardCode == 0)
                {
                    throw new BadRequestException(["Location codes are fully required"], "Location codes are fully required.");
                }
           
                var province = await _uow.ProvinceRepo.GetQueryable()
                    .Where(p => p.Code == provinceCode)
                    .Select(p => p.Name)
                    .FirstOrDefaultAsync();

                var district = await _uow.DistrictRepo.GetQueryable()
                    .Where(d => d.Code == districtCode && d.ProvinceCode == provinceCode)
                    .Select(d => d.Name)
                    .FirstOrDefaultAsync();

                var ward = await _uow.WardRepo.GetQueryable()
                    .Where(w => w.Code == wardCode && w.DistrictCode == districtCode)
                    .Select(w => w.Name)
                    .FirstOrDefaultAsync();

                if (province == null || district == null || ward == null)
                {
                    throw new BadRequestException(["Invalid administrative division codes"], "Invalid administrative division codes.");
                }

                var fullAddress = string.Join(", ", ward, district, province);
                if (rawLocation.IsNotNullOrEmpty())
                {
                    fullAddress = string.Join(", ", rawLocation, fullAddress);
                }
                var coordinates = await _geoService.GetCoordinatesByProvinceCodeAsync(provinceCode);
                
                var (lat, lng) = await _geoService.FowardGeoCodeAsync(fullAddress);
                
                if (lat == 0 && lng == 0)
                {
                    throw new BadRequestException(["Location not found"], "Could not find the specified location.");
                }

                var predictions = await _geoService.AutocompleteSearchAsync(fullAddress, lat, lng);
                
                if (predictions == null || !predictions.Any())
                {
                    throw new BadRequestException(["Location not found"], "Could not find the specified location.");
                }

                placeDetail = await _geoService.GetPlaceDetailAsync(predictions.First().PlaceId);
                
                if (placeDetail != null)
                {
                    address = new Address
                    {
                        PlaceName = placeDetail.PlaceName,
                        FormattedAddress = placeDetail.FormattedAddress,
                        Lat = placeDetail.Latitude,
                        Lng = placeDetail.Longitude,
                        PlaceId = placeDetail.PlaceId
                    };
                    await _uow.AddressRepo.Create(address);
                    await _uow.CommitAsync();
                }
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
                Location = placeDetail?.FormattedAddress ?? string.Empty,
                AddressId = address?.Id
            };
            _logger.Info($"attempt to register, username: {request.Username}");
            var result = await _authRepo.RegisterAsync(registerModel);
            if (result.Succeeded)
            {
                if (placeDetail == null)
                {
                    _logger.Info("register user successfully");
                    return LiftNetRes.SuccessResponse(message: "User registered successfully.");
                }
                var newUser = await _userManager.FindByEmailAsync(request.Email);
                try
                {
                    var cosmosResult = await InsertCosmosAsync(newUser!.Id, placeDetail);
                    if (cosmosResult != 0)
                    {
                        _logger.Info("register user successfully");
                        return LiftNetRes.SuccessResponse(message: "User registered successfully.");
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
            _logger.Error("failed to register user");
            return LiftNetRes.ErrorResponse(message: "Failed to register.", errors: result.Errors.Select(x => x.Description).ToList());
        }

        private async Task<int> InsertCosmosAsync(string userId, PlaceDetailDto placeDetail)
        {
            if (userId.IsNullOrEmpty() || placeDetail == null)
            {
                return 0;
            }
       
            if (placeDetail == null)
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
            if (result != null)
            {
                return 1; 
            }
            return 0;
        }
    }
}
