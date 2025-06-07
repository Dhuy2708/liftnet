using LiftNet.Contract.Dtos;
using LiftNet.Contract.Enums;
using LiftNet.Contract.Views.Notis;
using LiftNet.Domain.Entities;
using LiftNet.Utility.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Utility.Mappers
{
    public static class NotiMapper
    {
        public static Notification ToEntity(this NotiMessageDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new Notification
            {
                RecieverId = dto.RecieverId ?? throw new Exception("reciever id is null"),
                SenderType = (int)dto.SenderType,
                RecieverType = (int)dto.RecieverType,
                Title = dto.Title,
                Body = dto.Body,
                CreatedAt = dto.CreatedAt,
                ReferenceLocation = (int)dto.Location,
                EventType = (int)dto.EventType,
                SenderId = dto.SenderId,
            };
        }

        public static NotificationView ToView(this Notification entity)
        {
            if (entity == null)
            {
                return null;
            }
            return new NotificationView
            {
                Id = entity.Id,
                RecieverId = entity.RecieverId ?? throw new Exception("reciever id is null"),
                SenderName = entity.Sender?.FirstName + " " + entity.Sender?.LastName,
                SenderType = (int)entity.SenderType,
                SenderAvatar = entity.Sender?.Avatar ?? string.Empty,
                RecieverType = (int)entity.RecieverType,
                Title = entity.Title,
                Body = entity.Body,
                CreatedAt = entity.CreatedAt.ToOffSet(),
                EventType = (NotiEventType)entity.EventType,
                Location = (NotiRefernceLocationType)entity.ReferenceLocation
            };
        }
    }
}
