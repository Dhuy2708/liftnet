using LiftNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ProvinceSDK.Apis
{
    public class ProvinceApi : BaseApi
    {
        private readonly ILiftLogger<ProvinceApi> _logger;

        public ProvinceApi(HttpClient httpClient, ILiftLogger<ProvinceApi> logger)
            : base(httpClient)
        {
            _logger = logger;
        }
    }
}
