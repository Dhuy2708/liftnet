using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Exceptions;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Plannings.Commands.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LiftNet.Handler.Plannings.Commands
{
    public class UpdateSpecificDaysPlanHandler : IRequestHandler<UpdateSpecificDaysPlanCommand, LiftNetRes>
    {
        private readonly ILiftLogger<UpdateSpecificDaysPlanHandler> _logger;
        private readonly IUnitOfWork _uow;

        public UpdateSpecificDaysPlanHandler(ILiftLogger<UpdateSpecificDaysPlanHandler> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<LiftNetRes> Handle(UpdateSpecificDaysPlanCommand request, CancellationToken cancellationToken)
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

                _logger.Info($"Updating plan for user {userId} for days {string.Join(", ", request.DaysOfWeek)}");
                await _uow.BeginTransactionAsync();

                var planRepo = _uow.TrainingPlanRepo;
                var existingPlans = await planRepo.GetQueryable()
                                .Where(p => p.UserId == request.UserId && request.DaysOfWeek.Contains(p.DayOfWeek))
                                .ToListAsync(cancellationToken);

                if (existingPlans.Any())
                {
                    await planRepo.HardDeleteRange(existingPlans);
                }

                var planDict = request.DayWithExerciseIds;
                List<TrainingPlan> newPlans = [];
                foreach (var day in planDict)
                {
                    if (!request.DaysOfWeek.Contains(day.Key))
                        continue;

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
                return LiftNetRes.SuccessResponse("Plan updated successfully for specified days.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while updating plan for specific days");
                return LiftNetRes.ErrorResponse("An error occurred while updating the plan.");
            }
        }
    }
} 