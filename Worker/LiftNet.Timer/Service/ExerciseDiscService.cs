using LiftNet.Contract.Enums.Job;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Constants;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.ExerciseSDK.Core;
using LiftNet.ExerciseSDK.Res;
using LiftNet.Timer.Service.Common;
using LiftNet.Utility.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Timer.Service
{
    public class ExerciseDiscService : BaseSystemJob
    {
        private ILiftLogger<ExerciseDiscService> _logger => _provider.GetRequiredService<ILiftLogger<ExerciseDiscService>>();
        private ExerciseApiClient exerciseSDK => _provider.GetRequiredService<ExerciseApiClient>();
        private IUnitOfWork uow => _provider.GetRequiredService<IUnitOfWork>();
        private HttpClient httpClient;

        public ExerciseDiscService(IServiceProvider provider) : base(JobType.ExerciseDisc, provider, TimeSpan.FromHours(JobIntervalHour.EXERCISE_DISC - 1))
        {
            httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(60)
            };
        }

        protected override async Task<JobStatus> KickOffJobServiceAsync()
        {
            try
            {
                _logger.Info("begin exercise disc service");
                await uow.BeginTransactionAsync();

                var allExercises = await exerciseSDK.GetAllExercisesAsync();
                if (allExercises.IsNullOrEmpty())
                {
                    return JobStatus.Finished;
                }

                var existingExercises = await uow.ExerciseRepo.GetQueryable().ToListAsync();
                var existingDict = existingExercises.ToDictionary(e => e.SelfId);

                var entitiesToUpdate = new List<Exercise>();
                var entitiesToCreate = new List<Exercise>();

                foreach (var dto in allExercises)
                {
                    var entity = ToEntity(dto);
                    if (existingDict.TryGetValue(entity.SelfId, out var existingEntity))
                    {
                        existingEntity.Name = entity.Name;
                        existingEntity.Description = entity.Description;
                        existingEntity.Instructions = entity.Instructions;
                        entitiesToUpdate.Add(existingEntity);
                    }
                    else
                    {
                        entitiesToCreate.Add(entity);
                    }
                }

                if (entitiesToUpdate.Any())
                {
                    await uow.ExerciseRepo.UpdateRange(entitiesToUpdate);
                }

                if (entitiesToCreate.Any())
                {
                    await uow.ExerciseRepo.CreateRange(entitiesToCreate);
                }

                var result = await uow.CommitAsync();
                if (result > 0)
                {
                    if (entitiesToCreate.Any())
                    {
                        await InserChromaDB(entitiesToCreate);
                    }
                    return JobStatus.Finished;
                }
                return JobStatus.Failed;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error while discover provinces");
                await uow.RollbackAsync();
                return JobStatus.Failed;
            }
        }

        private async Task InserChromaDB(List<Exercise> exercises)
        {
            var response = await httpClient.PostAsJsonAsync("http://127.0.0.1:5000/api/exercises/insert", exercises);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
            }
        }

        private Exercise ToEntity(ExerciseRes apiRes)
        {
            return new Exercise
            {
                SelfId = apiRes.Id,
                BodyPart = apiRes.BodyPart,
                Equipment = apiRes.Equipment,
                GifUrl = apiRes.GifUrl,
                Name = apiRes.Name,
                Target = apiRes.Target,
                SecondaryMuscles = JsonConvert.SerializeObject(apiRes.SecondaryMuscles),
                Instructions = JsonConvert.SerializeObject(apiRes.Instructions),
                Category = apiRes.Category,
                Description = apiRes.Description,
                Difficulty = apiRes.Difficulty
            };
        }
    }
}
