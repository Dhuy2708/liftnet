using LiftNet.Contract.Enums.Social;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Engine.Contract;
using LiftNet.Engine.ML;
using LiftNet.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Engine.Engine.Impl
{
    public class SocialEngine : ISocialEngine
    {
        //private readonly IUnitOfWork _uow;
        //private readonly UserSimilarityModel _similarityModel;
        //private readonly IUserService _userService;
        //private readonly IRoleService _roleService;

        //private const int BATCH_SIZE = 100;
        //private Dictionary<string, LiftNetRoleEnum> UserRoleDict = [];

        //public SocialEngine(IUnitOfWork uow, IUserService userService, IRoleService roleService)
        //{
        //    _uow = uow;
        //    _userService = userService;
        //    _roleService = roleService;
        //    _similarityModel = new UserSimilarityModel();
        //}

        //public async Task CalculateAndUpdateAllUserSimilarities()
        //{
        //    var roleDict = await _roleService.GetAllRoleDictAsync();
        //    var adminRoleId = roleDict.FirstOrDefault(x => x.Value is LiftNetRoleEnum.Admin).Key;
        //    var queryable = _uow.UserRepo.GetQueryable();
        //    queryable = queryable.Include(u => u.Address);

        //    if (adminRoleId.IsNotNullOrEmpty())
        //    {
        //        queryable = queryable.Where(x => !x.UserRoles.Any(x => x.RoleId.Eq(adminRoleId)));
        //    }
        //    var activeUsers = await queryable.Where(u => !u.IsDeleted && !u.IsSuspended)
        //                                     .ToListAsync();

        //    var userIds = activeUsers.Select(u => u.Id).ToList();
        //    UserRoleDict = await _userService.GetUserIdRoleDict(userIds, roleDict);

        //    for (int i = 0; i < activeUsers.Count; i += BATCH_SIZE)
        //    {
        //        var batch = activeUsers.Skip(i).Take(BATCH_SIZE).ToList();
        //        await ProcessUserBatch(batch, activeUsers);
        //    }
        //}

        //public async Task CalculateAndUpdateUserSimilarities(string userId)
        //{
        //    var user1 = await _uow.UserRepo.GetById(userId);
        //    if (user1 == null || user1.IsDeleted || user1.IsSuspended) return;

        //    var allUsers = await _uow.UserRepo.GetAll();
        //    var activeUsers = allUsers.Where(u => !u.IsDeleted && !u.IsSuspended && u.Id != userId).ToList();
        //    var userIds = activeUsers.Select(u => u.Id).ToList();
        //    var userRoles = await _userService.GetUserIdRoleDict(userIds);

        //    await ProcessUserBatch(new List<User> { user1 }, activeUsers, userRoles);
        //}

        //private async Task ProcessUserBatch(List<User> sourceUsers, List<User> targetUsers)
        //{
        //    var similarityScores = new List<SocialSimilarityScore>();
        //    var existingScores = await _uow.SocialSimilarityScoreRepo.GetAll();
        //    var existingScoreDict = existingScores.ToDictionary(
        //        s => $"{s.UserId1}_{s.UserId2}",
        //        s => s
        //    );

        //    foreach (var user1 in sourceUsers)
        //    {
        //        var user1Feature = await CreateUserFeature(user1);

        //        foreach (var user2 in targetUsers.Where(u => u.Id != user1.Id))
        //        {
        //            var user2Feature = await CreateUserFeature(user2);
        //            var score = _similarityModel.ComputeScore(user1Feature, user2Feature);

        //            var key1 = $"{user1.Id}_{user2.Id}";
        //            var key2 = $"{user2.Id}_{user1.Id}";
        //            var existingScore = existingScoreDict.GetValueOrDefault(key1) ?? existingScoreDict.GetValueOrDefault(key2);

        //            if (existingScore != null)
        //            {
        //                existingScore.Score = score;
        //                await _uow.SocialSimilarityScoreRepo.Update(existingScore);
        //            }
        //            else
        //            {
        //                similarityScores.Add(new SocialSimilarityScore
        //                {
        //                    UserId1 = user1.Id,
        //                    UserId2 = user2.Id,
        //                    Score = score
        //                });
        //            }
        //        }
        //    }

        //    if (similarityScores.Any())
        //    {
        //        await _uow.SocialSimilarityScoreRepo.CreateRange(similarityScores);
        //    }

        //    await _uow.CommitAsync();
        //}

        //private async Task<UserSimilarityFeature> CreateUserFeature(User user)
        //{
        //    var followingIds = await GetFollowingIds(user.Id);
        //    var (lat, lng) = await GetUserLocation(user);

        //    return new UserSimilarityFeature
        //    {
        //        Id = user.Id,
        //        Age = CalculateAge(user.CreatedAt),
        //        Role = UserRoleDict.GetValueOrDefault(user.Id, LiftNetRoleEnum.None),
        //        FollowingIds = followingIds,
        //        Lat = lat,
        //        Lng = lng
        //    };
        //}

        //private async Task<List<string>> GetFollowingIds(string userId)
        //{
        //    var connections = await _uow.SocialConnectionRepo.GetAll();
        //    return connections
        //        .Where(c => c.UserId == userId && c.Status == (int)SocialConnectionStatus.Following)
        //        .Select(c => c.TargetId)
        //        .ToList();
        //}

        //private async Task<(double lat, double lng)> GetUserLocation(User user)
        //{
        //    if (user.AddressId != null)
        //    {
        //        var address = await _uow.AddressRepo.GetById(user.AddressId);
        //        if (address != null)
        //        {
        //            return (address.Lat, address.Lng);
        //        }
        //    }
        //    return (0, 0);
        //}

        //private int CalculateAge(DateTime createdAt)
        //{
        //    return DateTime.UtcNow.Year - createdAt.Year;
        //}
    }
}
