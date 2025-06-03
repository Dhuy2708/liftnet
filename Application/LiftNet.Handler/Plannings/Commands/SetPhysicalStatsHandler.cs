using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Plannings.Commands.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Plannings.Commands
{
    public class SetPhysicalStatsHandler : IRequestHandler<SetPhysicalStatsCommand, LiftNetRes>
    {
        private readonly ILiftLogger<SetPhysicalStatsHandler> _logger;
        private readonly IPhysicalStatRepo _physicalStatRepo;

        public SetPhysicalStatsHandler(ILiftLogger<SetPhysicalStatsHandler> logger, IPhysicalStatRepo physicalStatRepo)
        {
            _logger = logger;
            _physicalStatRepo = physicalStatRepo;
        }

        public async Task<LiftNetRes> Handle(SetPhysicalStatsCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var existingStat = await _physicalStatRepo.GetQueryable()
                    .FirstOrDefaultAsync(x => x.UserId == request.UserId, cancellationToken);

                if (existingStat == null)
                {
                    existingStat = new UserPhysicalStat
                    {
                        UserId = request.UserId
                    };
                    await _physicalStatRepo.Create(existingStat);
                }

                existingStat.Age = request.Age;
                existingStat.Gender = request.Gender;
                existingStat.Height = request.Height;
                existingStat.Mass = request.Mass;
                existingStat.Bdf = request.Bdf;
                existingStat.ActivityLevel = request.ActivityLevel;
                existingStat.Goal = request.Goal;

                await _physicalStatRepo.SaveChangesAsync(cancellationToken);
                return LiftNetRes.SuccessResponse();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while setting physical stats");
                return LiftNetRes.ErrorResponse("Error occurred while setting physical stats");
            }
        }
    }
} 