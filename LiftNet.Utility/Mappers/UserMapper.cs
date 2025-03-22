using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Views.Users;
using LiftNet.Domain.Entities;
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
                Address = JsonConvert.DeserializeObject<AddressDto>(user.Address ?? string.Empty) ,
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
                Address = JsonConvert.SerializeObject(userDto.Address),
                CreatedAt = userDto.CreatedAt,
                IsDeleted = userDto.IsDeleted,
                IsSuspended = userDto.IsSuspended
            };
        }

        public static UserOverView ToView(this UserDto userDto)
        {
            return new UserOverView
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

        public static UserDto ToDto(this UserOverView userView)
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

        public static UserOverview ToOverview(this User user)
        {
            return new UserOverview
            {
                Id = user.Id,
                Email = user.Email!,
                Username = user.UserName!,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Avatar = user.Avatar
            };
        }
    }
}
