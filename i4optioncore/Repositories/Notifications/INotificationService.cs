using i4optioncore.DBModelsUser;
using i4optioncore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public interface INotificationService
    {
        Task Delete(int id);
        Task<List<Notification>> GetAll();
        Task Save(NotificationModel.NotificationRequest request);
        Task SendExpiryNotifications();
    }
}