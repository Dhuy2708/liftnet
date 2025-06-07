using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Repositories.Core
{
    public class NotificationRepo : CrudBaseRepo<Notification>, INotificationRepo
    {
        public NotificationRepo(LiftNetDbContext dbContext, ILiftLogger<CrudBaseRepo<Notification>> logger) : base(dbContext, logger)
        {
        }
    }
}
