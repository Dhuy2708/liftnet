using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Mappers
{
    public static class AddressMapper
    {
        public static AddressDto ToDto(this AddressView addressView)
        {
            if (addressView == null) return null;

            return new AddressDto
            {
                Province = addressView.Province?.ToDto(),
                District = addressView.District?.ToDto(),
                Ward = addressView.Ward?.ToDto(),
                Address = addressView.Address
            };
        }

        public static AddressView ToView(this AddressDto addressDto)
        {
            if (addressDto == null) return null;

            return new AddressView
            {
                Province = addressDto.Province?.ToView(),
                District = addressDto.District?.ToView(),
                Ward = addressDto.Ward?.ToView(),
                Address = addressDto.Address
            };
        }

        public static ProvinceDto ToDto(this ProvinceView provinceView)
        {
            if (provinceView == null) return null;

            return new ProvinceDto
            {
                Code = provinceView.Code,
                Name = provinceView.Name,
                PhoneCode = provinceView.PhoneCode
            };
        }

        public static ProvinceView ToView(this ProvinceDto provinceDto)
        {
            if (provinceDto == null) return null;

            return new ProvinceView
            {
                Code = provinceDto.Code,
                Name = provinceDto.Name,
                PhoneCode = provinceDto.PhoneCode
            };
        }

        public static DistrictDto ToDto(this DistrictView districtView)
        {
            if (districtView == null) return null;

            return new DistrictDto
            {
                Code = districtView.Code,
                Name = districtView.Name
            };
        }

        public static DistrictView ToView(this DistrictDto districtDto)
        {
            if (districtDto == null) return null;

            return new DistrictView
            {
                Code = districtDto.Code,
                Name = districtDto.Name
            };
        }

        public static WardDto ToDto(this WardView wardView)
        {
            if (wardView == null) return null;

            return new WardDto
            {
                Code = wardView.Code,
                Name = wardView.Name
            };
        }

        public static WardView ToView(this WardDto wardDto)
        {
            if (wardDto == null) return null;

            return new WardView
            {
                Code = wardDto.Code,
                Name = wardDto.Name
            };
        }
    }
}
