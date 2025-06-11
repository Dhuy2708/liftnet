using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Plannings;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Plannings.Queries.Requests;
using LiftNet.Utility.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace LiftNet.Handler.Plannings.Queries
{
    public class ListTrainingPlansHandler : IRequestHandler<ListTrainingPlansQuery, PaginatedLiftNetRes<TrainingPlanView>>
    {
        private readonly ITrainingPlanRepo _trainingPlanRepo;
        private readonly ILiftLogger<ListTrainingPlansHandler> _logger;

        public ListTrainingPlansHandler(ITrainingPlanRepo trainingPlanRepo, ILiftLogger<ListTrainingPlansHandler> logger)
        {
            _trainingPlanRepo = trainingPlanRepo;
            _logger = logger;
        }

        public async Task<PaginatedLiftNetRes<TrainingPlanView>> Handle(ListTrainingPlansQuery request, CancellationToken cancellationToken)
        {
            if (request?.UserId == null)
            {
                return PaginatedLiftNetRes<TrainingPlanView>.ErrorResponse("UserId is required");
            }

            var queryable = _trainingPlanRepo.GetQueryable();
            var trainingPlans = await queryable
                .Include(x => x.Exercises)
                    .ThenInclude(etp => etp.Exercise)
                .Where(x => x.UserId == request.UserId)
                .OrderBy(x => x.DayOfWeek)
                .ToListAsync();

            var views = trainingPlans.Select(x => new TrainingPlanView
            {
                Id = x.Id,
                DayOfWeek = x.DayOfWeek,
                Exercises = x.Exercises?
                    .Where(e => e?.Exercise != null)
                    .OrderBy(e => e.Order) 
                    .Select((e, index) => new ExerciseView
                    {
                        Id = e.Exercise.SelfId ?? string.Empty,
                        BodyPart = e.Exercise.BodyPart!,
                        Equipment = e.Exercise.Equipment!,
                        GifUrl = e.Exercise.GifUrl!,
                        Name = e.Exercise.Name!,
                        Target = e.Exercise.Target!,
                        SecondaryMuscles = !string.IsNullOrEmpty(e.Exercise.SecondaryMuscles) ?
                            JsonSerializer.Deserialize<List<string>>(e.Exercise.SecondaryMuscles ?? "[]")! : [],
                        Instructions = !string.IsNullOrEmpty(e.Exercise.Instructions) ?
                            JsonSerializer.Deserialize<List<string>>(e.Exercise.Instructions ?? "[]")! : [],
                        Order = index + 1 
                    })
                    .ToList() ?? []
            }).ToList();

            return PaginatedLiftNetRes<TrainingPlanView>.SuccessResponse(views);
        }
    }
} 