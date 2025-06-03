using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Plannings;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Plannings.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Plannings.Queries
{
    public class GetPhysicalStatsHandler : IRequestHandler<GetPhysicalStatsQuery, LiftNetRes<UserPhysicalStatView>>
    {
        private readonly ILiftLogger<GetPhysicalStatsHandler> _logger;
        private readonly IPhysicalStatRepo _physicalStatRepo;

        public GetPhysicalStatsHandler(ILiftLogger<GetPhysicalStatsHandler> logger, IPhysicalStatRepo physicalStatRepo)
        {
            _logger = logger;
            _physicalStatRepo = physicalStatRepo;
        }

        public async Task<LiftNetRes<UserPhysicalStatView>> Handle(GetPhysicalStatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var physicalStat = await _physicalStatRepo.GetQueryable()
                    .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);

                if (physicalStat == null)
                {
                    var newStat = new UserPhysicalStat
                    {
                        UserId = request.UserId,
                    };
                    var creatingResult = await _physicalStatRepo.Create(newStat);
                    if (creatingResult > 0)
                    {
                        return LiftNetRes<UserPhysicalStatView>.SuccessResponse(new UserPhysicalStatView { Id = newStat.Id});
                    }
                    return LiftNetRes<UserPhysicalStatView>.ErrorResponse("Failed to get physical stats");
                }

                var view = new UserPhysicalStatView
                {
                    Id = physicalStat.Id,
                    Age = physicalStat.Age,
                    Gender = physicalStat.Gender,
                    Height = physicalStat.Height,
                    Mass = physicalStat.Mass,
                    Bdf = physicalStat.Bdf,
                    ActivityLevel = physicalStat.ActivityLevel,
                    Goal = physicalStat.Goal
                };

                return LiftNetRes<UserPhysicalStatView>.SuccessResponse(view);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while getting physical stats");
                return LiftNetRes<UserPhysicalStatView>.ErrorResponse("Error occurred while getting physical stats");
            }
        }
    }
} 