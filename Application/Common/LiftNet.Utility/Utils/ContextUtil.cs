using LiftNet.Domain.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Utils
{
    public class ContextUtil
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private static string? GetClaimValue(string claimType)
        {
            if (_httpContextAccessor?.HttpContext?.User != null)
            {
                return _httpContextAccessor.HttpContext.User.FindFirst(claimType)?.Value;
            }
            return null;
        }

        public static string? UId()
        {
            return GetClaimValue(LiftNetClaimType.UId);
        }

        public static string? UEmail()
        {
            return GetClaimValue(LiftNetClaimType.UEmail);
        }

        public static string? Username()
        {
            return GetClaimValue(LiftNetClaimType.Username);
        }
    }
}
