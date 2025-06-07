using LiftNet.Contract.Dtos;
using LiftNet.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace LiftNet.Api.Requests.Auths
{
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [Range(1, 120)]
        public int? Age { get; set; }

        [Required]
        public LiftNetGender? Gender { get; set; }

        public AddressRequest? Address { get; set; }

        [Required]
        public LiftNetRoleEnum Role { get; set; }
    }

    public class AddressRequest
    {
        public string Location { get; set; }
        public int ProvinceCode { get; set; }
        public int DistrictCode { get; set; }
        public int WardCode { get; set; }
    }
}
