using LiftNet.Contract.Dtos;
using LiftNet.Contract.Dtos.Query;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Views.Appointments;
using LiftNet.Domain.Entities;
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

        public ListAppointmentHandler(IAppointmentRepo appointmentRepo)
        {
            _appointmentRepo = appointmentRepo;
        }

        public async Task<PaginatedLiftNetRes<AppointmentOverview>> Handle(ListAppointmentsQuery request, CancellationToken cancellationToken)
        {
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
            query = BuildQuery(query, conditions);
            query.Take(conditions.PageSize);
            query.Skip((conditions.PageNumber - 1) * conditions.PageSize);
            query = BuildSort(query, conditions.Sort);

            var appointments = await query.ToListAsync();

            var count = await _appointmentRepo.GetCount();
            var appointmentDtos = appointments.Select(x => x.ToOverview()).ToList();
            return PaginatedLiftNetRes<AppointmentOverview>.SuccessResponse(appointmentDtos, conditions.PageNumber, conditions.PageSize, count);
        }

        public IQueryable<Appointment> BuildQuery(IQueryable<Appointment> queryable, QueryCondition conditions)
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
