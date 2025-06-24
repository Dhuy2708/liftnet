using LiftNet.Domain.Constants;
using LiftNet.Domain.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views.Users;
using LiftNet.Handler.Plannings.Commands.Requests;
using LiftNet.Handler.Plannings.Queries.Requests;
using LiftNet.Api.Requests.Plannings;
using LiftNet.Utility.Extensions;
using LiftNet.Contract.Views.Plannings;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace LiftNet.Api.Controllers
{
    public class PlanningController : LiftNetControllerBase
    {
        public PlanningController(IMediator mediator, IServiceProvider serviceProvider) : base(mediator, serviceProvider)
        {
        }

        [HttpPost("setPhysicalStat")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetPhysicalStats([FromBody] SetPhysicalStatsReq req)
        {
            var command = new SetPhysicalStatsCommand
            {
                UserId = UserId,
                Age = req.Age,
                Gender = req.Gender,
                Height = req.Height,
                Mass = req.Mass,
                Bdf = req.Bdf,
                ActivityLevel = (int?)req.ActivityLevel,
                Goal = (int?)req.Goal
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("getPhysicalStat")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<UserPhysicalStatView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetPhysicalStats()
        {
            var query = new GetPhysicalStatsQuery
            {
                UserId = UserId
            };

            var result = await _mediator.Send(query);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("list")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(PaginatedLiftNetRes<TrainingPlanView>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListTrainingPlans()
        {
            var query = new ListTrainingPlansQuery
            {
                UserId = UserId
            };

            var result = await _mediator.Send(query);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("setPlan")]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> SetPlan(SetPlanReq req)
        {
            var command = new SetPlanCommand();
            try
            {
                command.UserId = req.UserId;
                command.DayWithExerciseIds = req.Plan.ToDictionary(k => k.DayOfWeek, v => v.ExerciseIds);
            }
            catch
            {
                return BadRequest();
            }

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpGet("exercise/list")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes<List<ExerciseView>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListExercises([FromQuery]string search, [FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 20)
        {
            var query = new ListExercisesQuery
            {
                Search = search,
                PageNumber = pageNumber,    
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("updateSpecificDays")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateSpecificDays([FromBody] UpdateSpecificDaysPlanReq req)
        {
            if (req.UserId.IsNullOrEmpty())
            {
                return Unauthorized();
            }

            var command = new UpdateSpecificDaysPlanCommand
            {
                UserId = req.UserId,
                DaysOfWeek = req.DaysOfWeek,
                DayWithExerciseIds = req.Plan.ToDictionary(k => k.DayOfWeek, v => v.ExerciseIds)
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("exercise/add")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> AddExercise([FromBody] AddExerciseToPlanReq req)
        {
            var command = new AddExerciseToPlanCommand
            {
                UserId = UserId,
                DayOfWeek = req.DayOfWeek,
                ExerciseId = req.ExerciseId
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }

        [HttpPost("exercise/remove")]
        [Authorize(Policy = LiftNetPolicies.SeekerOrCoach)]
        [ProducesResponseType(typeof(LiftNetRes), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> RemoveExercise([FromBody] RemoveExerciseFromPlanReq req)
        {
            var command = new RemoveExerciseFromPlanCommand
            {
                UserId = UserId,
                DayOfWeek = req.DayOfWeek,
                Order = req.Order
            };

            var result = await _mediator.Send(command);
            if (result.Success)
            {
                return Ok(result);
            }
            return StatusCode(500, result);
        }
    }
}
