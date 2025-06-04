using DocumentFormat.OpenXml.Office2010.ExcelAc;
using DocumentFormat.OpenXml.Wordprocessing;
using i4optioncore.DBModelsUser;
using i4optioncore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace i4optioncore.Repositories
{
    public class NotificationService : INotificationService
    {
        private readonly I4optionUserDbContext dbUser;
        private readonly ICommonBL commonBL;
        public NotificationService(I4optionUserDbContext dbUser, ICommonBL commonBL)
        {
            this.dbUser = dbUser;
            this.commonBL = commonBL;
        }

        public async Task Save(NotificationModel.NotificationRequest request)
        {
            var value = dbUser.Notifications.FirstOrDefault(n => n.Id == request.Id) ?? new Notification();
            value.Status = request.Status;
            value.Schedule = request.Schedule;
            value.Date = request.Date;
            value.UpdatedOn = DateTime.Now;
            value.Campaign = request.Campaign;
            value.Condition = request.Condition;
            value.Days = request.Days;
            value.Template = request.Template;
            if (value.Id == 0)
            {
                value.CreatedOn = DateTime.Now;
                dbUser.Notifications.Add(value);
            }
            await dbUser.SaveChangesAsync();
        }
        public async Task<List<Notification>> GetAll()
        {
            return await dbUser.Notifications.ToListAsync();
        }
        public async Task Delete(int id)
        {
            var res = dbUser.Notifications.FirstOrDefault(x => x.Id == id);
            if (res != null)
            {
                dbUser.Notifications.Remove(res);
                await dbUser.SaveChangesAsync();
            }
        }
        public async Task SendExpiryNotifications()
        {
            var dayToday = DateTime.Now.DayOfWeek.ToString();
            var expiredYesterday = dbUser.Notifications.Where(x => x.Condition == "Expired Yesterday" && x.Days.Contains(dayToday)).FirstOrDefault();
            var expired7Days = dbUser.Notifications.Where(x => x.Condition == "Expired 7 Days" && x.Days.Contains(dayToday)).FirstOrDefault();
            var expired30Days = dbUser.Notifications.Where(x => x.Condition == "Expired 30 Days" && x.Days.Contains(dayToday)).FirstOrDefault();
            var registeredToday = dbUser.Notifications.Where(x => x.Condition == "Registered Today" && x.Days.Contains(dayToday)).FirstOrDefault();
            var expiringTomorrow = dbUser.Notifications.Where(x => x.Condition == "Expiring Tomorrow" && x.Days.Contains(dayToday)).FirstOrDefault();



            //Expired Yesterday
           await dbUser.Users
                  .Where(x => x.PlanExpireDate.HasValue && x.PlanExpireDate.Value.Date == DateTime.Now.AddDays(-1).Date)
                 .ForEachAsync(u =>
                  {
                      commonBL.SendWhatsapp($"+91{u.Mobile}", expiredYesterday.Template, $"{u.FirstName} {u.LastName}", u.PlanExpireDate.HasValue ? u.PlanExpireDate.Value.ToString("dd-MM-yyyy") : null, null, u.Email, null);
                  });

            //Expired 7 Days
           await dbUser.Users
                  .Where(x => x.PlanExpireDate.HasValue && x.PlanExpireDate.Value.Date == DateTime.Now.AddDays(-7).Date)
                  .ForEachAsync(u =>
                  {
                      commonBL.SendWhatsapp($"+91{u.Mobile}", expired7Days.Template, $"{u.FirstName} {u.LastName}", u.PlanExpireDate.HasValue ? u.PlanExpireDate.Value.ToString("dd-MM-yyyy") : null, null, u.Email, null);
                  });

            //Registered Today
            await dbUser.Users
                   .Where(x => x.CreatedOnUtc.HasValue && x.CreatedOnUtc.Value.Date == DateTime.Now.Date)
                   .ForEachAsync(u =>
                   {
                       commonBL.SendWhatsapp($"+91{u.Mobile}", registeredToday.Template, $"{u.FirstName} {u.LastName}", u.PlanExpireDate.HasValue ? u.PlanExpireDate.Value.ToString("dd-MM-yyyy") : null, null, u.Email, null);
                   });


            //Expring Tomorrow
            await dbUser.Users
                  .Where(x => x.PlanExpireDate.HasValue && x.PlanExpireDate.Value.Date == DateTime.Now.AddDays(1).Date)
                 .ForEachAsync(u =>
                 {
                     commonBL.SendWhatsapp($"+91{u.Mobile}", expiringTomorrow.Template, $"{u.FirstName} {u.LastName}", u.PlanExpireDate.HasValue ? u.PlanExpireDate.Value.ToString("dd-MM-yyyy") : null, null, u.Email, null);
                 });

            //Expired 30 Days
            await dbUser.Users
                  .Where(x => x.PlanExpireDate.HasValue && x.PlanExpireDate.Value.Date == DateTime.Now.AddDays(-30).Date)
                  .ForEachAsync(u =>
                  {
                      commonBL.SendWhatsapp($"+91{u.Mobile}", expired30Days.Template, $"{u.FirstName} {u.LastName}", u.PlanExpireDate.HasValue ? u.PlanExpireDate.Value.ToString("dd-MM-yyyy") : null, null, u.Email, null);
                  });

        }

        public class NotificationsBody
        {
            public string TemplateName { get; set; }
            public string Body { get; set; }
        }
    }
}
