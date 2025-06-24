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
    public class RemoveExerciseFromPlanHandler : IRequestHandler<RemoveExerciseFromPlanCommand, LiftNetRes>
    {
        private readonly ILiftLogger<RemoveExerciseFromPlanHandler> _logger;
        private readonly IUnitOfWork _uow;

        public RemoveExerciseFromPlanHandler(ILiftLogger<RemoveExerciseFromPlanHandler> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<LiftNetRes> Handle(RemoveExerciseFromPlanCommand request, CancellationToken cancellationToken)
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

                if (request.Order <= 0)
                {
                    throw new BadRequestException(["Order must be greater than 0."]);
                }

                _logger.Info($"Removing exercise at order {request.Order} from plan for user {userId} on day {request.DayOfWeek}");
                await _uow.BeginTransactionAsync();

                var planRepo = _uow.TrainingPlanRepo;
                var existingPlan = await planRepo.GetQueryable()
                                .Include(p => p.Exercises)
                                .FirstOrDefaultAsync(p => p.UserId == userId && p.DayOfWeek == request.DayOfWeek, cancellationToken);

                if (existingPlan == null || existingPlan.Exercises == null || !existingPlan.Exercises.Any())
                {
                    throw new BadRequestException([$"No plan found for user {userId} on day {request.DayOfWeek}."]);
                }

                var orderedExercises = existingPlan.Exercises.OrderBy(e => e.Order).ToList();
                if (request.Order > orderedExercises.Count)
                {
                    throw new BadRequestException([$"Order {request.Order} is out of range. There are only {orderedExercises.Count} exercises in the plan."]);
                }

                var exerciseToRemove = orderedExercises[request.Order - 1];
                existingPlan.Exercises.Remove(exerciseToRemove);

                if (!existingPlan.Exercises.Any())
                {
                    await planRepo.HardDelete(existingPlan);
                }
                else
                {
                    await planRepo.Update(existingPlan);
                }

                await _uow.CommitAsync();
                return LiftNetRes.SuccessResponse("Exercise removed successfully from the plan.");
            }
            catch (Exception e)
            {
                _logger.Error(e, "Error occurred while removing exercise from plan");
                return LiftNetRes.ErrorResponse("An error occurred while removing the exercise from the plan.");
            }
        }
    }
} 