using LiftNet.Api.Requests;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;
using LiftNet.Utility.Mappers;

namespace LiftNet.Api.ToDto
{
    public static class AppointmentReqToDto
    {
        public static AppointmentDto ToDto(this BookAppointmentReq req, string bookerId)
        {
            return new AppointmentDto
            {
                Id = Guid.NewGuid().ToString(),
                Booker = new UserDto()
                {
                    Id = bookerId
                },
                Participants = req.ParticipantIds.Select(x => new UserDto()
                {
                    Id = x
                }).Concat([new UserDto()
                        {
                            Id = bookerId
                        }]).ToList(),
                Name = req.Name,
                Description = req.Description,
                StartTime = req.StartTime,
                EndTime = req.EndTime,
                Address = req.Address.ToDto(),
                Status = AppointmentStatus.Pending,
                RepeatingType = req.RepeatingType
            };
        }
    }
}
