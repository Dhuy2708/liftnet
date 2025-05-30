using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Contract.Views.Appointments;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
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
        public static AppointmentDetailView ToDetailView(this Appointment entity, bool editable = false, AppointmentParticipantStatus status = AppointmentParticipantStatus.None)
        {
            var bookerPart = entity.Participants.FirstOrDefault(x => x.IsBooker);
            if (bookerPart != null)
            {
                entity.Participants.Remove(bookerPart);
            }
            var booker = bookerPart!.User;
            return new AppointmentDetailView
            {
                Id = entity.Id,
                Booker = new UserViewInAppointmentDetail
                {
                    Id = booker!.Id,
                    Email = booker!.Email ?? string.Empty,
                    Username = booker!.UserName ?? string.Empty,
                    // Role = (LiftNetRoleEnum)booker!.Role,
                    IsDeleted = booker!.IsDeleted,
                    IsSuspended = booker!.IsSuspended,
                    Avatar = booker!.Avatar,
                    FirstName = booker!.FirstName,
                    LastName = booker!.LastName,
                    Status = (AppointmentParticipantStatus)bookerPart!.Status
                },
                OtherParticipants = entity.Participants?
                        .Select(x =>
                        {
                            var overViewUser = x.User!.ToDto().ToView();
                            return new UserViewInAppointmentDetail
                            {
                                Id = overViewUser.Id,
                                Email = overViewUser.Email,
                                Username = overViewUser.Username,
                                // Role = overViewUser.Role,
                                IsDeleted = overViewUser.IsDeleted,
                                IsSuspended = overViewUser.IsSuspended,
                                Avatar = overViewUser.Avatar,
                                FirstName = overViewUser.FirstName,
                                LastName = overViewUser.LastName,
                                Status = (AppointmentParticipantStatus)x.Status
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
                Price = entity.Price    
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
                Price = entity.Price
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
                Price = dto.Price,
            };
        }

        public static AppointmentOverview ToOverview(this AppointmentDto dto, AppointmentParticipantStatus status = AppointmentParticipantStatus.None)
        {
            var startTime = new DateTimeOffset(dto.StartTime, TimeSpan.Zero);
            var endTime = new DateTimeOffset(dto.EndTime, TimeSpan.Zero);

            if (dto.RepeatingType is not RepeatingType.None)
            {
                var now = DateTimeOffset.UtcNow;
                var duration = endTime - startTime;

                if (startTime <= now)
                {
                    var steps = dto.RepeatingType switch
                    {
                        RepeatingType.Daily => Math.Ceiling((now - startTime).TotalDays),
                        RepeatingType.Weekly => Math.Ceiling((now - startTime).TotalDays / 7),
                        RepeatingType.Monthly => (now.Year - startTime.Year) * 12 + now.Month - startTime.Month + (now.Day > startTime.Day ? 1 : 0),
                        RepeatingType.Yearly => (now.Year - startTime.Year) + (now.DayOfYear > startTime.DayOfYear ? 1 : 0),
                        _ => 0
                    };

                    startTime = dto.RepeatingType switch
                    {
                        RepeatingType.Daily => startTime.AddDays(steps),
                        RepeatingType.Weekly => startTime.AddDays(7 * steps),
                        RepeatingType.Monthly => startTime.AddMonths((int)steps),
                        RepeatingType.Yearly => startTime.AddYears((int)steps),
                        _ => startTime
                    };

                    endTime = startTime + duration;
                }
            }

            var appointmentStatus = AppointmentStatus.None;
            if (startTime > DateTime.UtcNow)
            {
                appointmentStatus = AppointmentStatus.Upcomming;
            }
            if (startTime <= DateTime.UtcNow && endTime >= DateTime.UtcNow)
            {
                appointmentStatus = AppointmentStatus.InProgress;
            }
            if (endTime < DateTime.UtcNow)
            {
                appointmentStatus = AppointmentStatus.Expired;
            }

            return new AppointmentOverview
            {
                Id = dto.Id,
                Booker = dto.Booker?.ToView()!,
                ParticipantCount = dto.Participants.Count,
                Name = dto.Name,
                Description = dto.Description,
                Location = dto.PlaceDetail,
                Price = dto.Price,
                StartTime = startTime,
                EndTime = endTime,
                Status =  status,
                AppointmentStatus = appointmentStatus,
                RepeatingType = dto.RepeatingType,
                Created = new DateTimeOffset(dto.Created, TimeSpan.Zero),
                Modified = new DateTimeOffset(dto.Modified, TimeSpan.Zero),
            };
        }

        public static AppointmentOverview ToOverview(this Appointment entity, AppointmentParticipantStatus status = AppointmentParticipantStatus.None)
        {
            var view = entity.ToDto().ToOverview(status);
            return view;
        }
    }
}
