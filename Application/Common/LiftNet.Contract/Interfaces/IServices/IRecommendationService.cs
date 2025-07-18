﻿using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Indexes;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices
{
    public interface IRecommendationService : IDependency
    {
        Task<List<UserOverview>> SearchPrioritizedUser(string currentUserId,string search, int pageSize = 10, int pageNumber = 1);
        Task<List<FeedIndexData>> ListRecommendedFeedsAsync(string userId, int pageSize = 5);
        Task<List<UserOverview>> SuggestFriendsAsync(string userId, int pageSize = 10);
    }
}
