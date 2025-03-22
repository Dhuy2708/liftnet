using LiftNet.Contract.Enums;
using LiftNet.Contract.Views;
using System.Diagnostics.CodeAnalysis;

namespace LiftNet.Api.Requests
{
    public class BookAppointmentReq
    {
        public List<string> ParticipantIds
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        [AllowNull]
        public string Description
        {
            get; set;
        }
        public AddressView Address
        {
            get; set;
        }
        public DateTime StartTime
        {
            get; set;
        }
        public DateTime EndTime
        {
            get; set;
        }
        public RepeatingType RepeatingType
        {
            get; set;
        }
    }
}
