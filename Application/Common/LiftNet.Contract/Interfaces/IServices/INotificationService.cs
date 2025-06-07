using LiftNet.Contract.Dtos;
using LiftNet.Contract.Views.Notis;
using LiftNet.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiftNet.Contract.Interfaces.IServices
{
    public interface INotificationService : IDependency
    {
        Task<List<NotificationView>> GetNotifications(string userId, int pageIndex, int pageSize);
        Task<int> GetNotiCount(string userId);
        Task<int> SaveNotification(NotiMessageDto dto);
        Task<int> DeleteNoti(int id);
        Task Send(NotiMessageDto notiMessage, bool save = true);
    }
}
