using LiftNet.Contract.Dtos;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Appointments;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.Appointments.Queries.Requests;
using LiftNet.Ioc;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Mappers;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.Appointments.Queries
{
    public class ListAppointmentHandler : IRequestHandler<ListAppointmentsQuery, PaginatedLiftNetRes<AppointmentOverview>>
    {
        private readonly IAppointmentRepo _appointmentRepo;
        private readonly ILiftLogger<ListAppointmentHandler> _logger;

        public ListAppointmentHandler(IAppointmentRepo appointmentRepo, ILiftLogger<ListAppointmentHandler> logger)
        {
            _appointmentRepo = appointmentRepo;
            _logger = logger;
        }

        public async Task<PaginatedLiftNetRes<AppointmentOverview>> Handle(ListAppointmentsQuery request, CancellationToken cancellationToken)
        {
            _logger.Info("begin list appointment overviews");
            var queryable = _appointmentRepo.GetQueryable();
            var conditions = request.Conditions;

            if (conditions == null)
            {
                conditions = new QueryCondition();
            }
            queryable = queryable.Include(x => x.Booker)
                                 .Include(x => x.Participants)
                                 .ThenInclude(x => x.User);
            var query = queryable.Where(x => x.BookerId == request.UserId || (x.Participants.Select(p => p.UserId).Contains(request.UserId)));
            query = BuildQuery(query, conditions, request.UserId);

            var count = await query.CountAsync();
            query = BuildSort(query, conditions.Sort);
            query = query.Skip((conditions.PageNumber - 1) * conditions.PageSize)
                         .Take(conditions.PageSize);

            var appointments = await query.ToListAsync();

            var appointmentDtos = appointments.Select(x => x.ToOverview(GetCurrentUserStatusFromAppointment(x, request.UserId))).ToList();

            _logger.Info("list appointment overviews successfully");
            return PaginatedLiftNetRes<AppointmentOverview>.SuccessResponse(appointmentDtos, conditions.PageNumber, conditions.PageSize, count);
        }

        private AppointmentParticipantStatus GetCurrentUserStatusFromAppointment(Appointment appointment, string userId)
        {
            var participant = appointment?.Participants?.FirstOrDefault(x => x.UserId == userId);
            if (participant != null)
            {
                return (AppointmentParticipantStatus)participant.Status;
            }
            return AppointmentParticipantStatus.None;
        }

        private IQueryable<Appointment> BuildQuery(IQueryable<Appointment> queryable, QueryCondition conditions, string userId)
        {
            if (conditions == null)
            {
                return queryable;
            }

            var nameCond = conditions.FindCondition("name");
            if (nameCond != null)
            {
                queryable = queryable.Where(x => x.Name.Contains(nameCond.Values.FirstOrDefault() ?? string.Empty));
            }

            var statusInt = conditions.GetValue<int>("status");
            if (statusInt.HasValue)
            {
                queryable = queryable.Where(x => x.Participants.Any(p => p.UserId == userId && p.Status == statusInt));
            }

            var appointmentStatusInt = conditions.GetValue<int>("appointmentStatus");
            if (appointmentStatusInt.HasValue)
            {
                switch (appointmentStatusInt.Value)
                {
                    case (int)AppointmentStatus.Upcomming:
                        queryable = queryable.Where(x => x.StartTime > DateTime.UtcNow);
                        break;
                    case (int)AppointmentStatus.InProgress:
                        queryable = queryable.Where(x => x.StartTime <= DateTime.UtcNow && x.EndTime >= DateTime.UtcNow);
                        break;
                    case (int)AppointmentStatus.Expired:
                        queryable = queryable.Where(x => x.EndTime < DateTime.UtcNow);
                        break;
                }
            }

            var startTime = conditions.GetValue<DateTime>("starttime");
            if (startTime.HasValue)
            {
                queryable = queryable.Where(x => x.StartTime > startTime || x.RepeatingType != (int)RepeatingType.None);
            }

            var endTime = conditions.GetValue<DateTime>("endtime");
            if (endTime.HasValue)
            {
                queryable = queryable.Where(x => x.EndTime < endTime || x.RepeatingType != (int)RepeatingType.None);
            }

            return queryable;
        }

        private IQueryable<Appointment> BuildSort(IQueryable<Appointment> queryable, SortCondition? sortCond)
        {
            if (sortCond == null)
            {
                return queryable;
            }
            switch (sortCond.Type)
            {
                case SortType.Asc:
                    if (sortCond.Name.Eq("starttime"))
                    {
                        return queryable.OrderBy(x => x.StartTime);
                    }
                    if (sortCond.Name.Eq("endtime"))
                    {
                        return queryable.OrderBy(x => x.EndTime);
                    }
                    break;
                case SortType.Desc:
                    if (sortCond.Name.Eq("starttime"))
                    {
                        return queryable.OrderByDescending(x => x.StartTime);
                    }
                    if (sortCond.Name.Eq("endtime"))
                    {
                        return queryable.OrderByDescending(x => x.EndTime);
                    }
                    break;
                default:
                    if (sortCond.Name.Eq("starttime"))
                    {
                        return queryable.OrderByDescending(x => x.StartTime);
                    }
                    if (sortCond.Name.Eq("endtime"))
                    {
                        return queryable.OrderByDescending(x => x.EndTime);
                    }
                    break;
            }
            return queryable;
        }
    }
}
