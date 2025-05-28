using LiftNet.Domain.Constants;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Hub.Provider
{
    public class LiftNetUserIdProvider : IUserIdProvider
    {
        public string? GetUserId(HubConnectionContext connection)
        {
            return connection.User?.FindFirst(LiftNetClaimType.UId)?.Value;
        }
    }
}
