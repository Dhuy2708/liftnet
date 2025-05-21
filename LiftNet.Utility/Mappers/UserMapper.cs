using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Mappers
{
    public static class UserMapper
    {
        public static UserDto ToDto(this User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
                Username = user.UserName ?? string.Empty,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Avatar = user.Avatar,
                Address = user.Province != null ? new AddressDto
                {
                    Province = user.Province.ToDto(),
                    District = user.District.ToDto(),
                    Ward = user.Ward.ToDto(),
                    Location = user.Location
                } : null,
                Location = user.Location,
                CreatedAt = user.CreatedAt,
                IsDeleted = user.IsDeleted,
                IsSuspended = user.IsSuspended
            };
        }

        public static User ToEntity(this UserDto userDto)
        {
            return new User
            {
                Id = userDto.Id,
                Email = userDto.Email,
                UserName = userDto.Username,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Avatar = userDto.Avatar,
                Province = userDto.Address?.Province?.ToEntity(),
                District = userDto.Address?.District?.ToEntity(),
                Ward = userDto.Address?.Ward?.ToEntity(),
                Location = userDto.Location,
                CreatedAt = userDto.CreatedAt,
                IsDeleted = userDto.IsDeleted,
                IsSuspended = userDto.IsSuspended,
            };
        }

        public static UserOverview ToView(this UserDto userDto)
        {
            return new UserOverview
            {
                Id = userDto.Id,
                Email = userDto.Email,
                Username = userDto.Username,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Avatar = userDto.Avatar,
                IsDeleted = userDto.IsDeleted,
                IsSuspended = userDto.IsSuspended
            };
        }

        public static UserDto ToDto(this UserOverview userView)
        {
            return new UserDto
            {
                Id = userView.Id,
                Email = userView.Email,
                Username = userView.Username,
                FirstName = userView.FirstName,
                LastName = userView.LastName,
                Avatar = userView.Avatar,
                IsDeleted = userView.IsDeleted,
                IsSuspended = userView.IsSuspended
            };
        }

        public static UserOverview ToOverview(this User user, Dictionary<string, LiftNetRoleEnum>? userRoleMapping = null)
        {
            var result = new UserOverview
            {
                Id = user.Id,
                Email = user.Email!,
                Username = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Avatar = user.Avatar,
                IsDeleted = user.IsDeleted,
                IsSuspended = user.IsSuspended
            };
            if (userRoleMapping != null)
            {
                result.Role = userRoleMapping[user.Id];
            }
            return result;
        }

        public static List<UserOverview> ToOverviews(this List<User> users, Dictionary<string, LiftNetRoleEnum>? userRoleMapping = null)
        {
            var result = new List<UserOverview>();
            foreach (var user in users)
            {
                var userView = user.ToOverview(userRoleMapping);
                result.Add(userView);
            }
            return result;
        }
    }
}
