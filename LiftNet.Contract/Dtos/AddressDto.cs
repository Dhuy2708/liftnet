using LiftNet.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Dtos
{
    public class AddressDto
    {
        public ProvinceDto Province
        {
            get; set;
        }
        public DistrictDto District
        {
            get; set;
        }
        public WardDto Ward
        {
            get; set;
        }
        public string Address
        {
            get; set;
        }
    }

    public class ProvinceDto
    {
        public int Code
        {
            get; set;
        }
        public string CodeName
        {
            get; set;
        }
        public string DivisionType
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public int PhoneCode
        {
            get; set;
        }
    }

    public class DistrictDto
    {
        public int Code
        {
            get; set;
        }
        public string CodeName
        {
            get; set;
        }
        public string DivisionType
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public int ProvinceCode
        {
            get; set;
        }
    }

    public class WardDto
    {
        public int Code
        {
            get; set;
        }
        public string CodeName
        {
            get; set;
        }
        public string DivisionType
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public int DistrictCode
        {
            get; set;
        }
    }
}
