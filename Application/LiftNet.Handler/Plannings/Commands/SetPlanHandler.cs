using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Plannings.Commands.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Plannings.Commands
{
    public class SetPlanHandler : IRequestHandler<SetPlanCommand, LiftNetRes>
    {
        private readonly ILiftLogger<SetPlanHandler> _logger;
        private readonly IUnitOfWork _uow;

        public SetPlanHandler(ILiftLogger<SetPlanHandler> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<LiftNetRes> Handle(SetPlanCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var userId = request.UserId;
                if (userId.IsNullOrEmpty())
                {
                    throw new BadRequestException(["UserId cannot be null or empty."]);
                }
                if (!await _uow.UserRepo.GetQueryable().AnyAsync(x => x.Id == userId))
                {
                    throw new BadRequestException([$"User with ID {userId} does not exist."]);
                }

                _logger.Info($"Setting plan for user {userId} with {request.DayWithExerciseIds.Count} days of exercises.");
                await _uow.BeginTransactionAsync();

                var planRepo = _uow.TrainingPlanRepo;
                var oldPlans = await planRepo.GetQueryable()
                                .Where(p => p.UserId == request.UserId)
                                .ToListAsync(cancellationToken);

                if (oldPlans.Any())
                {
                    await planRepo.HardDeleteRange(oldPlans);
                }

                var planDict = request.DayWithExerciseIds;
                List<TrainingPlan> newPlans = [];
                foreach (var day in planDict)
                {
                    List<ExerciseTrainingPlan> exercises = [];
                    float order = 1;
                    foreach (var exerciseId in day.Value)
                    {
                        var exercise = new ExerciseTrainingPlan
                        {
                            ExercisesSelfId = exerciseId,
                            Order = order++
                        };
                        exercises.Add(exercise);
                    }
                    newPlans.Add(new TrainingPlan
                    {
                        UserId = userId,
                        DayOfWeek = day.Key,
                        Exercises = exercises 
                    });
                }

                await planRepo.CreateRange(newPlans);
                await _uow.CommitAsync();
                return LiftNetRes.SuccessResponse("Plan set successfully.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while setting plan");
                return LiftNetRes.ErrorResponse("An error occurred while setting the plan.");
            }
        }
    }
}
