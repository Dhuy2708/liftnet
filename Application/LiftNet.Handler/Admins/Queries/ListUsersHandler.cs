using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Admins.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Admins.Queries
{
    public class ListUsersHandler : IRequestHandler<ListUsersQuery, LiftNetRes<UserItemView>>
    {
        private readonly ILiftLogger<ListUsersHandler> _logger;
        private readonly IUserRepo _userRepo;
        private readonly IUserService _userService;

        public ListUsersHandler(ILiftLogger<ListUsersHandler> logger, 
                              IUserRepo userRepo,
                              IUserService userService)
        {
            _logger = logger;
            _userRepo = userRepo;
            _userService = userService;
        }

        public async Task<LiftNetRes<UserItemView>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var users = await _userRepo.GetQueryable()
                    .Select(x => new UserItemView
                    {
                        Id = x.Id,
                        Email = x.Email,
                        Username = x.UserName,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        Avatar = x.Avatar,
                        IsDeleted = x.IsDeleted,
                        IsSuspended = x.IsSuspended
                    })
                    .OrderBy(x => x.Username)
                    .ToListAsync(cancellationToken);

                var userIds = users.Select(x => x.Id).ToList();
                var roleDict = await _userService.GetUserIdRoleDict(userIds);

                foreach (var user in users)
                {
                    user.Role = roleDict.GetValueOrDefault(user.Id, LiftNetRoleEnum.None);
                }

                _logger.Info($"Admin {request.CallerId} listed {users.Count} users");
                return LiftNetRes<UserItemView>.SuccessResponse(users);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while listing users");
                return LiftNetRes<UserItemView>.ErrorResponse("Error occurred while listing users");
            }
        }
    }
} 