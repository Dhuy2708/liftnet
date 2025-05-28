using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Mappers
{
    public static class AddressMapper
    {
        #region province
        public static Province ToEntity(this ProvinceDto dto)
        {
            if (dto == null) return null;
            return new Province
            {
                Code = dto.Code,
                Codename = dto.CodeName,
                DivisionType = dto.DivisionType,
                Name = dto.Name,
                PhoneCode = dto.PhoneCode
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

        public static ProvinceDto ToDto(this Province entity)
        {
            if (entity == null) return null;

            return new ProvinceDto
            {
                Code = entity.Code,
                CodeName = entity.Codename,
                DivisionType = entity.DivisionType,
                Name = entity.Name,
                PhoneCode = entity.PhoneCode,
            };
        }
        #endregion

        #region district
        public static District ToEntity(this DistrictDto dto)
        {
            if (dto == null) return null;
            return new District
            {
                Code = dto.Code,
                Codename = dto.CodeName,
                DivisionType = dto.DivisionType,
                Name = dto.Name
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

        public static DistrictDto ToDto(this District entity)
        {
            if (entity == null) return null;

            return new DistrictDto
            {
                Code = entity.Code,
                CodeName = entity.Codename,
                DivisionType = entity.DivisionType,
                Name = entity.Name,
            };
        }
        #endregion

        #region ward
        public static Ward ToEntity(this WardDto dto)
        {
            if (dto == null) return null;
            return new Ward
            {
                Code = dto.Code,
                Codename = dto.CodeName,
                DivisionType = dto.DivisionType,
                Name = dto.Name,
                DistrictCode = dto.DistrictCode
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
        public static WardDto ToDto(this Ward entity)
        {
            if (entity == null) return null;

            return new WardDto
            {
                Code = entity.Code,
                CodeName = entity.Codename,
                DivisionType = entity.DivisionType,
                Name = entity.Name,
                DistrictCode = entity.DistrictCode,
            };
        }

        #endregion

        #region index
        public static LocationIndexData? ToLocationIndexData(this PlaceDetailDto placeDetail)
        {
            if (placeDetail == null) return null;
            return new LocationIndexData
            {
                PlaceId = placeDetail.PlaceId,
                PlaceName = placeDetail.PlaceName,
                Latitude = placeDetail.Latitude,
                Longitude = placeDetail.Longitude,
                FormattedAddress = placeDetail.FormattedAddress
            };
            #endregion
        }
    }
}
