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
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Timer.Service
{
    public class ExerciseDiscService : BaseSystemJob
    {
        private ILiftLogger<ExerciseDiscService> _logger => _provider.GetRequiredService<ILiftLogger<ExerciseDiscService>>();
        private ExerciseApiClient exerciseSDK => _provider.GetRequiredService<ExerciseApiClient>();
        private IUnitOfWork uow => _provider.GetRequiredService<IUnitOfWork>();

        public ExerciseDiscService(IServiceProvider provider) : base(JobType.ExerciseDisc, provider, TimeSpan.FromHours(JobIntervalHour.EXERCISE_DISC - 1))
        {
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
                if (existingExercises.IsNotNullOrEmpty())
                {
                    await uow.ExerciseRepo.HardDeleteRange(existingExercises);
                }

                await uow.ExerciseRepo.CreateRange(allExercises.Select(ToEntity));

                var result = await uow.CommitAsync();
                if (result > 0)
                {
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
                Instructions = JsonConvert.SerializeObject(apiRes.Instructions)
            };
        }
    }
}
