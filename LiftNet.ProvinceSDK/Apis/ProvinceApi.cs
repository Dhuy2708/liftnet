using LiftNet.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.ProvinceSDK.Apis
{
    public class ProvinceApi : BaseGeoApi
    {
        private readonly ILiftLogger<ProvinceApi> _logger;

        public ProvinceApi(ILiftLogger<ProvinceApi> logger)
            : base()
        {
            _logger = logger;
        }
    }
}
