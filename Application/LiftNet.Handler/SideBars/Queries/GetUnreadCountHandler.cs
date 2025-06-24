using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Domain.Interfaces;
using LiftNet.Domain.Response;
using LiftNet.Handler.SideBars.Queries.Requests;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Handler.SideBars.Queries
{
    public class GetUnreadCountHandler : IRequestHandler<GetUnreadCountQuery, LiftNetRes<Dictionary<string, int>>>
    {
        private readonly ILiftLogger<GetUnreadCountQuery> _logger;
        private IFinderPostSeenStatusRepo _finderSeenRepo;
        private IAppointmentSeenStatusRepo _appointmentSeenRepo;
        private IChatSeenStatusRepo _chatSeenRepo;

        public GetUnreadCountHandler(ILiftLogger<GetUnreadCountQuery> logger, 
                                     IFinderPostSeenStatusRepo finderSeenRepo, 
                                     IAppointmentSeenStatusRepo appointmentSeenRepo, 
                                     IChatSeenStatusRepo chatSeenRepo)
        {
            _logger = logger;
            _finderSeenRepo = finderSeenRepo;
            _appointmentSeenRepo = appointmentSeenRepo;
            _chatSeenRepo = chatSeenRepo;
        }

        public async  Task<LiftNetRes<Dictionary<string, int>>> Handle(GetUnreadCountQuery request, CancellationToken cancellationToken)
        {
            try
            {
                Dictionary<string, int> unreadCounts = new Dictionary<string, int>()
                {
                    {"finder", 0 },
                    {"appointment", 0 },
                    {"chat", 0 }
                };

                unreadCounts["finder"] = await _finderSeenRepo.GetQueryable()
                                                       .Where(x => x.UserId == request.UserId &&
                                                                   x.NotiCount > 0)
                                                       .CountAsync();
                unreadCounts["appointment"] = await _appointmentSeenRepo.GetQueryable()
                                                       .Where(x => x.UserId == request.UserId &&
                                                                   x.NotiCount > 0)
                                                       .CountAsync();
                unreadCounts["chat"] = await _chatSeenRepo.GetQueryable()
                                                       .Where(x => x.UserId == request.UserId &&
                                                                   x.NotiCount > 0)
                                                       .CountAsync();
                return LiftNetRes<Dictionary<string, int>>.SuccessResponse(unreadCounts);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error in GetUnreadCountHandler");
                return LiftNetRes<Dictionary<string, int>>.ErrorResponse("An error occurred while fetching unread counts.");
            }
        }
    }
}
