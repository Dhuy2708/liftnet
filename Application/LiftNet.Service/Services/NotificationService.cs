using LiftNet.Contract.Dtos;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Notis;
using LiftNet.Domain.Interfaces;
using LiftNet.Hub.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Service.Services
{
    public class NotificationService : INotificationService
    {
        private readonly NotiHub _notiHub;
        private readonly INotificationRepo _notiRepo;
        private readonly ILiftLogger<NotificationService> _logger;

        public NotificationService(NotiHub notiHub, 
                                   INotificationRepo notiRepo, 
                                   ILiftLogger<NotificationService> logger)
        {
            _notiHub = notiHub;
            _notiRepo = notiRepo;
            _logger = logger;
        }

        public async Task<int> GetNotiCount(string userId)
        {
            var result = await _notiRepo.GetQueryable()
                                        .Where(x => x.UserId.Equals(userId))
                                        .CountAsync();
            return result;
        }

        public async Task<List<NotificationView>> GetNotifications(string userId, int pageIndex, int pageSize)
        {
            var entites = await _notiRepo.GetQueryable()
                             .Where(x => x.UserId.Equals(userId))
                             .OrderByDescending(x => x.CreatedAt)
                             .Skip((pageIndex - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();
            var result = NotiConvertHelper.ConvertRange2VMs(entites);
            return result.OrderByDescending(x => x.CreatedAt).ToList();
        }

        public async Task<int> SaveNotification(NotiMessageDto dto)
        {
            var entity = NotiConvertHelper.Convert2Entity(dto);
            var result = await _notiRepo.Create(entity);

            if (result > 0)
            {
                return entity.Id;
            }
            return 0;
        }

        public async Task<int> DeleteNoti(int id)
        {
            var noti = await _notiRepo.GetById(id);

            if (noti != null)
            {
                return await _notiRepo.HardDelete(noti);
            }
            return 0;
        }

        public async Task Send(NotiMessageDto notiMessage, bool save = false)
        {
            try
            {

                notiMessage.Title = string.Format(notiTemplate?.Title ?? "", notiMessage.ObjectNames.ToArray());
                notiMessage.Body = string.Format(notiTemplate?.Body ?? "", notiMessage.ObjectNames.ToArray());

                await _notiHub.SendToUser(notiMessage.RecieverId!, ConvertToNotiMessage(notiMessage));
                if (save)
                {
                    await SaveNotification(notiMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("error when send noti message: {0}", ex.Message);
            }

        }

        private NotiMessage ConvertToNotiMessage(NotiMessageDto dto)
        {
            if (dto == null)
            {
                return null;
            }

            return new NotiMessage
            {
                Title = dto.Title,
                SenderUsername = dto.SenderUsername,
                SenderName = dto.SenderName,
                Body = dto.Body,
                CreatedAt = dto.CreatedAt,
                SenderType = dto.SenderType,
                RecieverType = dto.RecieverType,
                SenderAvatar = dto.SenderAvatar,
                ObjectNames = dto.ObjectNames,
                EventType = dto.EventType,
            };
        }
    }
}
