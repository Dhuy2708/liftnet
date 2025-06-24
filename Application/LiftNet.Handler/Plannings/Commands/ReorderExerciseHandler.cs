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
    public class ReorderExerciseHandler : IRequestHandler<ReorderExerciseCommand, LiftNetRes>
    {
        private readonly ILiftLogger<ReorderExerciseHandler> _logger;
        private readonly IUnitOfWork _uow;

        public ReorderExerciseHandler(ILiftLogger<ReorderExerciseHandler> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<LiftNetRes> Handle(ReorderExerciseCommand request, CancellationToken cancellationToken)
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

                if (request.ExerciseId.IsNullOrEmpty())
                {
                    throw new BadRequestException(["ExerciseId cannot be null or empty."]);
                }

                if (!await _uow.ExerciseRepo.GetQueryable().AnyAsync(x => x.SelfId == request.ExerciseId))
                {
                    throw new BadRequestException([$"Exercise with ID {request.ExerciseId} does not exist."]);
                }

                if (request.SourceOrder <= 0 || request.TargetOrder <= 0)
                {
                    throw new BadRequestException(["SourceOrder and TargetOrder must be greater than 0."]);
                }

                _logger.Info($"Moving exercise {request.ExerciseId} from day {request.SourceDayOfWeek} order {request.SourceOrder} to day {request.TargetDayOfWeek} order {request.TargetOrder} for user {userId}");
                await _uow.BeginTransactionAsync();

                var planRepo = _uow.TrainingPlanRepo;

                // Get source plan
                var sourcePlan = await planRepo.GetQueryable()
                                .Include(p => p.Exercises)
                                .FirstOrDefaultAsync(p => p.UserId == userId && p.DayOfWeek == request.SourceDayOfWeek, cancellationToken);

                if (sourcePlan == null || sourcePlan.Exercises == null || !sourcePlan.Exercises.Any())
                {
                    throw new BadRequestException([$"No plan found for user {userId} on day {request.SourceDayOfWeek}."]);
                }

                var sourceOrderedExercises = sourcePlan.Exercises.OrderBy(e => e.Order).ToList();
                if (request.SourceOrder > sourceOrderedExercises.Count)
                {
                    throw new BadRequestException([$"SourceOrder {request.SourceOrder} is out of range. There are only {sourceOrderedExercises.Count} exercises in the source plan."]);
                }

                var exerciseToMove = sourceOrderedExercises[request.SourceOrder - 1];
                if (exerciseToMove.ExercisesSelfId != request.ExerciseId)
                {
                    throw new BadRequestException([$"Exercise at source order {request.SourceOrder} does not match the provided ExerciseId {request.ExerciseId}."]);
                }

                // Get or create target plan
                var targetPlan = await planRepo.GetQueryable()
                                .Include(p => p.Exercises)
                                .FirstOrDefaultAsync(p => p.UserId == userId && p.DayOfWeek == request.TargetDayOfWeek, cancellationToken);

                if (targetPlan == null)
                {
                    targetPlan = new TrainingPlan
                    {
                        UserId = userId,
                        DayOfWeek = request.TargetDayOfWeek,
                        Exercises = []
                    };
                    await planRepo.Create(targetPlan);
                    await _uow.CommitAsync();
                }

                var targetOrderedExercises = targetPlan.Exercises?.OrderBy(e => e.Order).ToList() ?? [];
                if (request.TargetOrder > targetOrderedExercises.Count + 1)
                {
                    throw new BadRequestException([$"TargetOrder {request.TargetOrder} is out of range. There are only {targetOrderedExercises.Count} exercises in the target plan."]);
                }

                // Remove exercise from source plan
                sourcePlan.Exercises.Remove(exerciseToMove);

                // Reorder remaining exercises in source plan
                var remainingSourceExercises = sourcePlan.Exercises.OrderBy(e => e.Order).ToList();
                for (int i = 0; i < remainingSourceExercises.Count; i++)
                {
                    remainingSourceExercises[i].Order = i + 1;
                }

                // If source plan is empty, delete it
                if (!sourcePlan.Exercises.Any())
                {
                    await planRepo.HardDelete(sourcePlan);
                }
                else
                {
                    await planRepo.Update(sourcePlan);
                }

                // Add exercise to target plan with proper reordering
                exerciseToMove.TrainingPlansId = targetPlan.Id;

                // Handle reordering logic
                var exercisesToReorder = targetOrderedExercises.ToList();
                
                if (request.SourceDayOfWeek == request.TargetDayOfWeek)
                {
                    // Same day reordering
                    // Remove the exercise from its current position
                    exercisesToReorder.RemoveAt(request.SourceOrder - 1);
                    
                    // Build the final list step by step
                    var finalList = new List<ExerciseTrainingPlan>();
                    
                    // Add exercises before the target position
                    for (int i = 0; i < request.TargetOrder - 1 && i < exercisesToReorder.Count; i++)
                    {
                        finalList.Add(exercisesToReorder[i]);
                    }
                    
                    // Add the moved exercise at target position
                    finalList.Add(exerciseToMove);
                    
                    // Add remaining exercises after the target position
                    for (int i = request.TargetOrder - 1; i < exercisesToReorder.Count; i++)
                    {
                        finalList.Add(exercisesToReorder[i]);
                    }
                    
                    exercisesToReorder = finalList;
                }
                else
                {
                    // Cross-day movement
                    // Insert the exercise at the target position
                    int insertPosition = Math.Min(request.TargetOrder - 1, exercisesToReorder.Count);
                    exercisesToReorder.Insert(insertPosition, exerciseToMove);
                }

                // Assign proper order numbers
                for (int i = 0; i < exercisesToReorder.Count; i++)
                {
                    exercisesToReorder[i].Order = i + 1;
                }

                targetPlan.Exercises = exercisesToReorder;
                await planRepo.Update(targetPlan);

                await _uow.CommitAsync();

                var moveDescription = request.SourceDayOfWeek == request.TargetDayOfWeek 
                    ? $"Exercise reordered from position {request.SourceOrder} to {request.TargetOrder}"
                    : $"Exercise moved from day {request.SourceDayOfWeek} position {request.SourceOrder} to day {request.TargetDayOfWeek} position {request.TargetOrder}";

                return LiftNetRes.SuccessResponse(moveDescription);
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while moving exercise");
                return LiftNetRes.ErrorResponse("An error occurred while moving the exercise.");
            }
        }
    }
} 