using LiftNet.Contract.Dtos;
using LiftNet.Hub.Constant;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Hub.Core
{
    [Authorize]
    public class NotiHub : BaseHub<NotiMes>
    {
        public NotiHub(ConnectionPool connPool) : base(connPool, HubNames.noti)
        {
        }
    }
}
