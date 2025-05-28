using LiftNet.Contract.Dtos;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Domain.Interfaces;
using LiftNet.Utility.Mappers;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Service.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly ILiftLogger<AppointmentService> _logger;
        private readonly IUnitOfWork _uow;

        public AppointmentService(ILiftLogger<AppointmentService> logger, IUnitOfWork uow)
        {
            _logger = logger;
            _uow = uow;
        }

        public async Task<List<AppointmentDto>> ListUserAppointments(string userId)
        {
            var queryable = _uow.AppointmentRepo.GetQueryable();
            var appointments = await queryable.Where(x => x.Participants.Any(x => x.UserId == userId)).ToListAsync();
            return appointments.ToDtos();
        }
    }
}
