using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.App.Core.Notifications;
using Coderr.Server.Domain.Modules.UserNotifications;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Notifications
{
    [ContainerService]
    public class NotificationRepository : INotificationsRepository, IUserNotificationsRepository
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public NotificationRepository(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<UserNotificationSettings> TryGetAsync(int accountId, int applicationId)
        {
            if (applicationId == 0)
                applicationId = -1;
            return await _unitOfWork.FirstOrDefaultAsync<UserNotificationSettings>(
                new {AccountId = accountId, ApplicationId = applicationId});
        }

        public async Task UpdateAsync(UserNotificationSettings notificationSettings)
        {
            if (notificationSettings.ApplicationId == 0)
                notificationSettings.ApplicationId = -1;
            await _unitOfWork.UpdateAsync(notificationSettings);
        }

        public async Task<IReadOnlyList<BrowserSubscription>> GetSubscriptions(int accountId)
        {
            return await _unitOfWork.ToListAsync<BrowserSubscription>(
                "SELECT * FROM NotificationsBrowser WHERE AccountId = @id",
                new {id = accountId});
        }

        public async Task Delete(BrowserSubscription subscription)
        {
            await _unitOfWork.DeleteAsync(subscription);
        }

        public async Task Save(BrowserSubscription message)
        {
            var existing =
                await _unitOfWork.FirstOrDefaultAsync<BrowserSubscription>(new {message.Endpoint, message.AccountId});

            if (existing != null)
            {
                existing.PublicKey = message.PublicKey;
                existing.AuthenticationSecret = message.AuthenticationSecret;
                existing.ExpiresAtUtc = message.ExpiresAtUtc;
                await _unitOfWork.UpdateAsync(existing);
            }
            else
            {
                await _unitOfWork.InsertAsync(message);
            }
        }

        public async Task DeleteBrowserSubscription(int accountId, string endpoint)
        {
            await _unitOfWork.DeleteAsync<BrowserSubscription>(new {AccountId = accountId, Endpoint = endpoint});
        }

        public async Task CreateAsync(UserNotificationSettings notificationSettings)
        {
            if (notificationSettings.ApplicationId == 0)
                notificationSettings.ApplicationId = -1;
            await _unitOfWork.InsertAsync(notificationSettings);
        }

        public async Task<IEnumerable<UserNotificationSettings>> GetAllAsync(int applicationId)
        {
            var sql =
                @"SELECT * 
                    FROM UserNotificationSettings 
                    WHERE ApplicationId = @1 
                        OR ApplicationId = -1 
                    ORDER By AccountId, ApplicationId DESC";
            var settings = await _unitOfWork.ToListAsync<UserNotificationSettings>(sql, applicationId);
            var dict = new Dictionary<int, UserNotificationSettings>();
            foreach (var setting in settings)
            {
                if (!dict.TryGetValue(setting.AccountId, out var appSettings))
                {
                    dict[setting.AccountId] = setting;
                    continue;
                }

                // We got the most specific settings thanks to the ASC sort.
                // now check if the app settings have "use global"

                if (appSettings.ApplicationSpike == NotificationState.UseGlobalSetting)
                    appSettings.ApplicationSpike = setting.ApplicationSpike;

                if (appSettings.NewIncident == NotificationState.UseGlobalSetting)
                    appSettings.NewIncident = setting.NewIncident;

                if (appSettings.ReopenedIncident == NotificationState.UseGlobalSetting)
                    appSettings.ReopenedIncident = setting.ReopenedIncident;

                if (appSettings.UserFeedback == NotificationState.UseGlobalSetting)
                    appSettings.UserFeedback = setting.UserFeedback;

                if (appSettings.WeeklySummary == NotificationState.UseGlobalSetting)
                    appSettings.WeeklySummary = setting.WeeklySummary;

                if (appSettings.ReopenedIncident == NotificationState.UseGlobalSetting)
                    appSettings.ReopenedIncident = setting.ReopenedIncident;
            }

            return dict.Values.ToList();
        }

        public async Task<bool> ExistsAsync(int accountId, int applicationId)
        {
            if (applicationId == 0)
                applicationId = -1;
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT TOP 1 AccountId FROM UserNotificationSettings WHERE AccountId = @id AND ApplicationId = @appId";
                cmd.AddParameter("id", accountId);
                cmd.AddParameter("appId", applicationId);
                return await cmd.ExecuteScalarAsync() != null;
            }
        }
    }
}