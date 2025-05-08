using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums.Appointment;
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
        public static AppointmentDetailView ToDetailView(this Appointment entity, bool editable = false, AppointmentStatus status = AppointmentStatus.None)
        {
            var booker = entity.Participants.FirstOrDefault(x => x.IsBooker);
            if (booker != null)
            {
                entity.Participants.Remove(booker);
            }
            return new AppointmentDetailView
            {
                Id = entity.Id,
                Booker = entity.Booker.ToDto().ToView(),
                OtherParticipants = entity.Participants?
                        .Select(x =>
                        {
                            var overViewUser = x.User!.ToDto().ToView();
                            return new UserViewInAppointmentDetail
                            {
                                Id = overViewUser.Id,
                                Email = overViewUser.Email,
                                Username = overViewUser.Username,
                                Role = overViewUser.Role,
                                IsDeleted = overViewUser.IsDeleted,
                                IsSuspended = overViewUser.IsSuspended,
                                Avatar = overViewUser.Avatar,
                                FirstName = overViewUser.FirstName,
                                LastName = overViewUser.LastName,
                                Status = (AppointmentStatus)x.Status
                            };
                        })
                        .ToList() ?? [],
                Name = entity.Name,
                Editable = editable,
                Description = entity.Description,
                Location = JsonConvert.DeserializeObject<PlaceDetailDto>(entity.PlaceDetail),
                StartTime = new DateTimeOffset(entity.StartTime, TimeSpan.Zero),
                EndTime = new DateTimeOffset(entity.EndTime, TimeSpan.Zero),
                Status = status,
                RepeatingType = (RepeatingType)entity.RepeatingType,
                Created = new DateTimeOffset(entity.Created, TimeSpan.Zero),
                Modified = new DateTimeOffset(entity.Modified, TimeSpan.Zero),
            };
        }

        public static AppointmentDto ToDto(this Appointment entity)
        {
            return new AppointmentDto
            {
                Id = entity.Id,
                Booker = entity.Booker?.ToDto()!,
                Participants = entity.Participants?.Select(x => x.User.ToDto()).ToList() ?? [],
                Name = entity.Name,
                Description = entity.Description,
                PlaceDetail = JsonConvert.DeserializeObject<PlaceDetailDto>(entity.PlaceDetail),
                StartTime = entity.StartTime,
                EndTime = entity.EndTime,
                RepeatingType = (RepeatingType)entity.RepeatingType,
                Created = entity.Created,
                Modified = entity.Modified,
            };
        }

        public static List<AppointmentDto> ToDtos(this List<Appointment> entities)
        {
            return entities.Select(x => x.ToDto()).ToList();
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
                    IsBooker = x.Id.Equals(dto.Booker?.Id),
                }).ToList() ?? [],
                Name = dto.Name,
                Description = dto.Description,
                PlaceDetail = JsonConvert.SerializeObject(dto.PlaceDetail),
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                RepeatingType = (int)dto.RepeatingType,
            };
        }

        public static AppointmentOverview ToOverview(this AppointmentDto dto, AppointmentStatus status = AppointmentStatus.None)
        {
            return new AppointmentOverview
            {
                Id = dto.Id,
                Booker = dto.Booker?.ToView()!,
                ParticipantCount = dto.Participants.Count,
                Name = dto.Name,
                Description = dto.Description,
                Location = dto.PlaceDetail,
                StartTime = new DateTimeOffset(dto.StartTime, TimeSpan.Zero),
                EndTime = new DateTimeOffset(dto.EndTime, TimeSpan.Zero),
                Status = status,
                RepeatingType = dto.RepeatingType,
                Created = new DateTimeOffset(dto.Created, TimeSpan.Zero),
                Modified = new DateTimeOffset(dto.Modified, TimeSpan.Zero),
            };
        }

        public static AppointmentOverview ToOverview(this Appointment entity, AppointmentStatus status = AppointmentStatus.None)
        {
            var view = entity.ToDto().ToOverview(status);
            return view;
        }
    }
}
