using LiftNet.Api.Requests;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;
using LiftNet.Utility.Mappers;

namespace LiftNet.Api.ToDto
{
    public static class AppointmentReqToDto
    {
        public static AppointmentDto ToDto(this BookAppointmentReq req)
        {
            return new AppointmentDto
            {
                Id = Guid.NewGuid().ToString(),
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
