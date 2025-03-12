using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views;
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

        public static UserView ToView(this UserDto userDto)
        {
            return new UserView
            {
                Id = userDto.Id,
                Email = userDto.Email,
                Username = userDto.Username,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Avatar = userDto.Avatar,
                Address = userDto.Address?.ToView(),
                CreatedAt = userDto.CreatedAt,
                IsDeleted = userDto.IsDeleted,
                IsSuspended = userDto.IsSuspended
            };
        }

        public static UserDto ToDto(this UserView userView)
        {
            return new UserDto
            {
                Id = userView.Id,
                Email = userView.Email,
                Username = userView.Username,
                FirstName = userView.FirstName,
                LastName = userView.LastName,
                Avatar = userView.Avatar,
                Address = userView.Address?.ToDto(),
                CreatedAt = userView.CreatedAt,
                IsDeleted = userView.IsDeleted,
                IsSuspended = userView.IsSuspended
            };
        }
    }
}
