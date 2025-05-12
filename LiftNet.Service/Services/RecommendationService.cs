using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Enums.Social;

namespace LiftNet.Service.Services
{
    internal class RecommendationService : IRecommendationService
    {
        private readonly IUnitOfWork _uow;
        private readonly IUserService _userService;

        public RecommendationService(IUnitOfWork uow, IUserService userService)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<List<UserOverview>> SearchPrioritizedUser(string currentUserId, string search, int pageSize = 10, int pageNumber = 1)
        {
            if (string.IsNullOrEmpty(currentUserId))
                throw new ArgumentException("Current user ID cannot be null or empty", nameof(currentUserId));

            if (pageSize <= 0)
                pageSize = 10;
            if (pageNumber <= 0)
                pageNumber = 1;

            var query = _uow.UserRepo.GetQueryable()
                .Include(u => u.UserRoles)
                .Where(u => !u.IsDeleted && !u.IsSuspended && u.Id != currentUserId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(u =>
                    (u.UserName != null && u.UserName.ToLower().Contains(search)) ||
                    (u.Email != null && u.Email.ToLower().Contains(search)) ||
                    (u.FirstName != null && u.FirstName.ToLower().Contains(search)) ||
                    (u.LastName != null && u.LastName.ToLower().Contains(search)) ||
                    ((u.FirstName + " " + u.LastName).ToLower().Contains(search))
                );
            }

            // Get similarity scores for the current user
            var similarityScores = await _uow.SocialSimilarityScoreRepo.GetQueryable()
                .Where(s => s.UserId1 == currentUserId)
                .ToListAsync();


            var similarityScoresDict = similarityScores.ToDictionary(s => s.UserId2, s => s.Score); 

            var users = await query
                .AsNoTracking()
                .ToListAsync();

            var usersWithScores = users
                                .Select(u => new
                                {
                                    User = u,
                                    Score = similarityScoresDict.TryGetValue(u.Id, out var score) ? score : 0
                                })
                                .OrderByDescending(x => x.Score)
                                .ToList();

            users = usersWithScores
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => x.User)
                .ToList();

            var userIds = users.Select(x => x.Id).ToList();

            var followingStatus = await _uow.SocialConnectionRepo.GetQueryable()
                .Where(c => c.UserId == currentUserId && 
                            userIds.Contains(c.TargetId) && 
                            c.Status == (int)SocialConnectionStatus.Following)
                .Select(c => c.TargetId)
                .ToListAsync();

            var roleDict = await _userService.GetUserIdRoleDict(userIds);

            return users.Select(x => new UserOverview
            {
                Id = x.Id,
                Email = x.Email ?? string.Empty,
                Username = x.UserName ?? string.Empty,
                FirstName = x.FirstName ?? string.Empty,
                LastName = x.LastName ?? string.Empty,
                Role = roleDict.GetValueOrDefault(x.Id, LiftNetRoleEnum.None),
                Avatar = x.Avatar ?? string.Empty,
                IsDeleted = x.IsDeleted,
                IsSuspended = x.IsSuspended,
                IsFollowing = followingStatus.Contains(x.Id)
            }).ToList();
        }
    }
}
