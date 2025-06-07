using LiftNet.Contract.Constants;
using LiftNet.Contract.Dtos;
using LiftNet.Contract.Interfaces.IRepos;
using LiftNet.Contract.Interfaces.IServices;
using LiftNet.Contract.Views.Notis;
using LiftNet.Domain.Entities;
using LiftNet.Domain.Interfaces;
using LiftNet.Hub.Contract;
using LiftNet.Hub.Core;
using LiftNet.Utility.Extensions;
using LiftNet.Utility.Mappers;
using Microsoft.EntityFrameworkCore;
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
        private readonly IUserRepo _userRepo;
        private readonly ILiftLogger<NotificationService> _logger;

        public NotificationService(NotiHub notiHub, 
                                   INotificationRepo notiRepo,
                                   IUserRepo userRepo, 
                                   ILiftLogger<NotificationService> logger)
        {
            _notiHub = notiHub;
            _notiRepo = notiRepo;
            _userRepo = userRepo;
            _logger = logger;
        }

        public async Task<int> GetNotiCount(string userId)
        {
            var result = await _notiRepo.GetQueryable()
                                        .Where(x => x.RecieverId.Equals(userId))
                                        .CountAsync();
            return result;
        }

        public async Task<List<NotificationView>> GetNotifications(string userId, int pageIndex, int pageSize)
        {
            var entites = await _notiRepo.GetQueryable()
                             .Include(x => x.Sender)
                             .Include(x => x.Reciever)
                             .Where(x => x.RecieverId.Equals(userId))
                             .OrderByDescending(x => x.CreatedAt)
                             .Skip((pageIndex - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();
            var result = entites.Select(x => x.ToView());
            return result.OrderByDescending(x => x.CreatedAt).ToList();
        }

        public async Task<int> SaveNotification(NotiMessageDto dto)
        {
            var entity = dto.ToEntity();
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

        public async Task Send(NotiMessageDto notiMessage, bool save = true)
        {
            try
            {
                notiMessage.Title = NotiTitles.GetTitle(notiMessage.EventType);
                notiMessage.Body = NotiBodies.GetBody(notiMessage.EventType, notiMessage.ObjectNames.ToArray());

                var userQueryable = _userRepo.GetQueryable();

                var sender = await userQueryable
                                .FirstOrDefaultAsync(x => x.Id == notiMessage.SenderId);
                var receiver = await userQueryable
                                .FirstOrDefaultAsync(x => x.Id == notiMessage.RecieverId);

                await _notiHub.SendNotiToOneUser(ConvertToNotiMessage(notiMessage, sender, receiver));
                if (save)
                {
                    await SaveNotification(notiMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "error when send noti message: {0}");
            }

        }

        private NotiMessage ConvertToNotiMessage(NotiMessageDto dto, User? sender, User? reciever)
        {
            if (dto == null)
            {
                return null;
            }

            return new NotiMessage
            {
                TrackId = Guid.NewGuid().ToString(),
                Title = dto.Title,
                Body = dto.Body,
                SenderId = sender?.Id ?? string.Empty,
                SenderUsername = sender?.UserName ?? string.Empty,
                SenderName = sender?.FirstName + " " + sender?.LastName,
                SenderAvatar = sender?.Avatar ?? string.Empty,
                SenderType = dto.SenderType,
                RecieverId = reciever?.Id ?? string.Empty,
                RecieverType = dto.RecieverType,
                ObjectNames = dto.ObjectNames,
                EventType = dto.EventType,
                CreatedAt = dto.CreatedAt,
            };
        }
    }
}
