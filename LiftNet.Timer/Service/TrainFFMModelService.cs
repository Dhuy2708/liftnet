using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Interfaces.IServices.Indexes;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using LiftNet.Domain.Indexes;
using LiftNet.Domain.Interfaces;
using LiftNet.Engine.Data.Feat;
using LiftNet.Engine.Engine;
using LiftNet.Timer.Service.Common;
using LiftNet.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LiftNet.Timer.Service
{
    public class TrainFFMModelService : BaseSystemJob
    {
        private ILiftLogger<TrainFFMModelService> _logger => _provider.GetRequiredService<ILiftLogger<TrainFFMModelService>>();
        private IFeedEngine _feedEngine => _provider.GetRequiredService<IFeedEngine>();
        private IFeedIndexService _feedIndexService => _provider.GetRequiredService<IFeedIndexService>();
        private IUserRepo _userRepo => _provider.GetRequiredService<IUserRepo>();
        private IUserService _userService => _provider.GetRequiredService<IUserService>();
        private IRoleService _roleService => _provider.GetRequiredService<IRoleService>();

        private const int BATCH_SIZE = 100;

        public TrainFFMModelService(IServiceProvider provider) : base(JobType.TrainFFMModel, provider)
        {
        }

        protected override async Task<JobStatus> KickOffJobServiceAsync()
        {
            try
            {
                _logger.Info("begin to train ffm model of all feeds");
                await TrainModel();
                return JobStatus.Finished;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while training ffm models of all feeds");
                return JobStatus.Failed;
            }
        }

        private async Task TrainModel()
        {
            var condition = new QueryCondition();
            condition.PageSize = -1; // query max
            condition.AddCondition(new ConditionItem("schema", new List<string> { $"{(int)DataSchema.Feed}" }, FilterType.Integer));

            var trainingData = new List<UserFeedFeature>();
            string? nextPageToken = null;
            var allFeeds = new List<FeedIndexData>();

            do
            {
                var (feeds, token) = await _feedIndexService.QueryAsync(condition);
                nextPageToken = token;
                allFeeds.AddRange(feeds);
            } while (nextPageToken != null);

            var feedIds = allFeeds.Select(f => f.Id).ToList();
            var feedLikes = await _feedIndexService.GetFeedLikesAsync(feedIds);

            // Get all users who liked feeds and feed creators
            var userIds = feedLikes.Values.SelectMany(x => x).Distinct().ToList();

            // Get all users for potential negative examples
            var allUsers = await _userRepo.GetQueryable()
                                    .Include(x => x.Address)
                                    .ToListAsync();

            var roleDict = await _roleService.GetAllRoleDictAsync();
            var adminRoleId = roleDict.Keys.FirstOrDefault(x => roleDict[x] is LiftNetRoleEnum.Admin) ?? string.Empty;

            if (adminRoleId.IsNotNullOrEmpty())
            {
                allUsers.RemoveAll(x => x.UserRoles.Any(r => r.RoleId.Eq(adminRoleId)));
            }

            var userRoleDict = await _userService.GetUserIdRoleDict(allUsers.Select(x => x.Id).ToList(), roleDict);


            var userDict = allUsers.ToDictionary(u => u.Id);


            foreach (var feed in allFeeds)
            {
                var positiveCount = 0;

                if (feedLikes.TryGetValue(feed.Id, out var likers))
                {
                    foreach (var likerId in likers)
                    {
                        if (userDict.TryGetValue(likerId, out var liker))
                        {
                            trainingData.Add(CreateFeature(liker, feed, userRoleDict, feedLikes, 1.0f));
                            positiveCount++;
                        }
                    }
                }

                if (positiveCount == 0)
                {
                    continue;
                }

                var nonLikers = allUsers
                    .Where(u => u.Id != feed.UserId && (likers == null || !likers.Contains(u.Id)))
                    .OrderBy(x => Guid.NewGuid()) 
                    .Take(positiveCount); 

                foreach (var nonLiker in nonLikers)
                {
                    trainingData.Add(CreateFeature(nonLiker, feed, userRoleDict, feedLikes, 0.0f));
                }
            }

            if (trainingData.Any())
            {
                try
                {
                    await _feedEngine.Train(trainingData);
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"error while training the datas, training data count: {trainingData.Count}");
                }
            }
        }

        private UserFeedFeature CreateFeature(User user, FeedIndexData feed, Dictionary<string, LiftNetRoleEnum> userRoleDict, Dictionary<string, List<string>> feedLikes, float label)
        {
            return new UserFeedFeature
            {
                User = new UserFieldAware
                {
                    UserId = user.Id,
                    Age = user.Age,
                    Role = userRoleDict.GetValueOrDefault(user.Id, LiftNetRoleEnum.None),
                    Gender = (LiftNetGender)user.Gender,
                    Location = user.Address != null ? (user.Address.Lat, user.Address.Lng) : null
                },
                Feed = new FeedFieldAware
                {
                    FeedId = feed.Id,
                    CreatedAt = feed.CreatedAt
                },
                Label = label
            };
        }
    }
}
