using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Plannings;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Plannings.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiftNet.Handler.Plannings.Queries
{
    public class ListExercisesHandler : IRequestHandler<ListExercisesQuery, LiftNetRes<List<ExerciseView>>>
    {
        private readonly ILiftLogger<ListExercisesHandler> _logger;
        private readonly IExerciseRepo _exerciseRepo;

        public ListExercisesHandler(ILiftLogger<ListExercisesHandler> logger, IExerciseRepo exerciseRepo)
        {
            _logger = logger;
            _exerciseRepo = exerciseRepo;
        }

        public async Task<LiftNetRes<List<ExerciseView>>> Handle(ListExercisesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var search = request.Search;

                var exercises = await _exerciseRepo.GetQueryable()
                                                   .Where(x => (x.Name!.Contains(search) || x.Description!.Contains(search)))
                                                   .Take(request.PageSize)
                                                   .Skip((request.PageNumber - 1) * request.PageSize)
                                                   .ToListAsync();
                var exerciseViews = exercises.Select(e => new ExerciseView
                {
                    Id = e.SelfId ?? string.Empty,
                    BodyPart = e.BodyPart!,
                    Equipment = e.Equipment!,
                    GifUrl = e.GifUrl!,
                    Name = e.Name!,
                    Target = e.Target!,
                    SecondaryMuscles = !string.IsNullOrEmpty(e.SecondaryMuscles) ?
                            JsonSerializer.Deserialize<List<string>>(e.SecondaryMuscles ?? "[]")! : [],
                    Instructions = !string.IsNullOrEmpty(e.Instructions) ?
                            JsonSerializer.Deserialize<List<string>>(e.Instructions ?? "[]")! : [],
                    Category = e.Category,
                    Difficulty = e.Difficulty,
                    Description = e.Description
                }).ToList();
                return LiftNetRes<List<ExerciseView>>.SuccessResponse(exerciseViews);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error occurred while listing exercises");
                return LiftNetRes<List<ExerciseView>>.ErrorResponse("An error occurred while processing your request.");
            }
        }
    }
}
