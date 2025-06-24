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
    public class AddExerciseToPlanHandler : IRequestHandler<AddExerciseToPlanCommand, LiftNetRes>
    {
        private readonly ILiftLogger<AddExerciseToPlanHandler> _logger;
        private readonly IUnitOfWork _uow;

        public AddExerciseToPlanHandler(ILiftLogger<AddExerciseToPlanHandler> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<LiftNetRes> Handle(AddExerciseToPlanCommand request, CancellationToken cancellationToken)
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

                if (!await _uow.ExerciseRepo.GetQueryable().AnyAsync(x => x.SelfId == request.ExerciseId))
                {
                    throw new BadRequestException([$"Exercise with ID {request.ExerciseId} does not exist."]);
                }

                _logger.Info($"Adding exercise {request.ExerciseId} to plan for user {userId} on day {request.DayOfWeek}");
                await _uow.BeginTransactionAsync();

                var planRepo = _uow.TrainingPlanRepo;
                var existingPlan = await planRepo.GetQueryable()
                                .Include(p => p.Exercises)
                                .FirstOrDefaultAsync(p => p.UserId == userId && p.DayOfWeek == request.DayOfWeek, cancellationToken);

                if (existingPlan == null)
                {
                    existingPlan = new TrainingPlan
                    {
                        UserId = userId,
                        DayOfWeek = request.DayOfWeek,
                        Exercises = []
                    };
                    await planRepo.Create(existingPlan);
                    await _uow.CommitAsync();
                }

                var maxOrder = existingPlan.Exercises?.Any() == true 
                    ? existingPlan.Exercises.Max(e => e.Order) 
                    : 0;

                var newExercise = new ExerciseTrainingPlan
                {
                    ExercisesSelfId = request.ExerciseId,
                    TrainingPlansId = existingPlan.Id,
                    Order = maxOrder + 1
                };

                existingPlan.Exercises ??= [];
                existingPlan.Exercises.Add(newExercise);

                await planRepo.Update(existingPlan);
                await _uow.CommitAsync();
                return LiftNetRes.SuccessResponse("Exercise added successfully to the plan.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while adding exercise to plan");
                return LiftNetRes.ErrorResponse("An error occurred while adding the exercise to the plan.");
            }
        }
    }
} 