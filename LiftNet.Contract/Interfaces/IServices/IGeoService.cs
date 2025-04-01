using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices
{
    public interface IGeoService
    {
        Task<List<ProvinceDto>> SearchProvincesAsync(string q);
        Task<List<DistrictDto>> SearchDistrictsAsync(int provinceCode, string q);
        Task<List<WardDto>> SearchWardsAsync(int provinceCode, int districtCode, string q);
    }
}
