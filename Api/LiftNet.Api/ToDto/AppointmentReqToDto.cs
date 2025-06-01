using LiftNet.Api.Requests.Appointments;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Dtos.Appointment;
using LiftNet.Contract.Enums.Appointment;
using LiftNet.Utility.Mappers;

namespace LiftNet.Api.ToDto
{
    public static class AppointmentReqToDto
    {
        public static AppointmentDto ToDto(this BookAppointmentReq req, string bookerId, PlaceDetailDto? placeDetail = null)
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
                Price = req.Price,
                Description = req.Description,
                StartTime = req.StartTime,
                EndTime = req.EndTime,
                PlaceDetail = placeDetail,
                RepeatingType = req.RepeatingType
            };
        }
    }
}
