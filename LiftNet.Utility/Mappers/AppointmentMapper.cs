using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Views.Appointments;
using LiftNet.Domain.Entities;
using LiftNet.Utility.Extensions;
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
        public static AppointmentDetailView ToDetailView(this Appointment entity)
        {
            return new AppointmentDetailView
            {
                Id = entity.Id,
                Booker = entity.Booker.ToDto().ToView(),
                Participants = entity.Participants?.Select(x => x.User!.ToDto().ToView())?.ToList() ?? [],
                Name = entity.Name,
                Description = entity.Description,
                Address = JsonConvert.DeserializeObject<AddressDto>(entity.Address).ToView(),
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Status = (AppointmentStatus)entity.Status,
                RepeatingType = (RepeatingType)entity.RepeatingType,
                Created = entity.Created,
                Modified = entity.Modified,
            };
        }

        public static AppointmentDto ToDto(this Appointment entity)
        {
            return new AppointmentDto
            {
                Id = entity.Id,
                Booker = entity.Booker?.ToDto(),
                Participants = entity.Participants?.Select(x => x.User.ToDto()).ToList() ?? [],
                Name = entity.Name,
                Description = entity.Description,
                Address = JsonConvert.DeserializeObject<AddressDto>(entity.Address),
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                Status = (AppointmentStatus)entity.Status,
                RepeatingType = (RepeatingType)entity.RepeatingType,
                Created = entity.Created,
                Modified = entity.Modified,
            };
        }

        public static Appointment ToEntity(this AppointmentDto dto)
        {
            return new Appointment
            {
                Id = dto.Id,
                BookerId = dto.Booker?.Id,
                Participants = dto.Participants?.Select(x => new AppointmentParticipant()
                {
                    UserId = x.Id,
                    AppointmentId = dto.Id,
                    IsBooker = x.Id.Eq(dto.Booker.Id) ? true : false,
                }).ToList(),
                Name = dto.Name,
                Description = dto.Description,
                Address = JsonConvert.SerializeObject(dto.Address),
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = (int)dto.Status,
                RepeatingType = (int)dto.RepeatingType,
            };
        }

        public static AppointmentOverview ToOverview(this AppointmentDto dto)
        {
            return new AppointmentOverview
            {
                Id = dto.Id,
                Booker = dto.Booker?.ToView(),
                ParticipantCount = dto.Participants.Count,
                Name = dto.Name,
                Description = dto.Description,
                Address = dto.Address?.ToView(),
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Status = dto.Status,
                RepeatingType = dto.RepeatingType,
                Created = dto.Created,
                Modified = dto.Modified,
            };
        }

        public static AppointmentOverview ToOverview(this Appointment entity)
        {
            var view = entity.ToDto().ToOverview();
            return view;
        }
    }
}
