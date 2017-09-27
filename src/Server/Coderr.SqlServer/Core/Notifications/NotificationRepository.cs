using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.App.Core.Notifications;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace codeRR.Server.SqlServer.Core.Notifications
{
    [Component]
    public class NotificationRepository : INotificationsRepository
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
                new
                {
                    AccountId = accountId,
                    ApplicationId = applicationId
                });
        }

        public async Task UpdateAsync(UserNotificationSettings notificationSettings)
        {
            if (notificationSettings.ApplicationId == 0)
                notificationSettings.ApplicationId = -1;
            await _unitOfWork.UpdateAsync(notificationSettings);
        }

        public async Task CreateAsync(UserNotificationSettings notificationSettings)
        {
            if (notificationSettings.ApplicationId == 0)
                notificationSettings.ApplicationId = -1;
            await _unitOfWork.InsertAsync(notificationSettings);
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
                if (dict.ContainsKey(setting.AccountId))
                    continue;
                dict[setting.AccountId] = setting;
            }

            return dict.Values.ToList();
        }
    }
}