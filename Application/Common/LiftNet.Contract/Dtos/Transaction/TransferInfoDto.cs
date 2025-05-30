using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos.Transaction
{
    public class TransferInfoDto
    {
        public string? AppointmentId
        {
            get; set;
        }
        public string? FromUserId
        {
            get; set;
        }
        public string? ToUserId
        {
            get; set;
        }
    }
}
