using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Views;
using LiftNet.Domain.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Mappers
{
    public static class AppointmentMapper
    {
        public static AppointmentDto ToDto(this Appointment entity)
        {
            return new AppointmentDto
            {
                Id = entity.Id,
                Client = entity.Client?.ToDto(),
                Coach = entity.Coach?.ToDto(),
                Name = entity.Name,
                Description = entity.Description,
                Address = JsonConvert.DeserializeObject<AddressDto>(entity.Address),
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Status = (AppointmentStatus)entity.Status
            };
        }

        public static Appointment ToEntity(this AppointmentDto entity)
        {
            return new Appointment
            {
                Id = entity.Id,
                Client = entity.Client?.ToEntity(),
                Coach = entity.Coach?.ToEntity(),
                Name = entity.Name,
                Description = entity.Description,
                Address = JsonConvert.SerializeObject(entity.Address),
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Status = (int)entity.Status
            };
        }

        public static AppointmentView ToView(this AppointmentDto dto)
        {
            return new AppointmentView
            {
                Id = dto.Id,
                Client = dto.Client?.ToView(),
                Coach = dto.Coach?.ToView(),
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address?.ToView(),
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = dto.Status
            };
        }

        public static AppointmentDto ToDto(this AppointmentView view)
        {
            return new AppointmentDto
            {
                Id = view.Id,
                Client = view.Client?.ToDto(),
                Coach = view.Coach?.ToDto(),
                Name = view.Name,
                Description = view.Description,
                Address = view.Address?.ToDto(),
                StartTime = view.StartTime,
                EndTime = view.EndTime,
                Status = (AppointmentStatus)view.Status
            };
        }
    }
}
