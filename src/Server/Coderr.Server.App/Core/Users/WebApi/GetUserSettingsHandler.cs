using System.Threading.Tasks;
using Coderr.Server.Api;
using Coderr.Server.Api.Core.Users;
using Coderr.Server.Api.Core.Users.Queries;
using Coderr.Server.App.Core.Notifications;
using Coderr.Server.Domain.Core.User;
using Coderr.Server.Domain.Modules.UserNotifications;
using DotNetCqs;
using Griffin.Container;
using NotificationState = Coderr.Server.Api.Core.Users.NotificationState;

namespace Coderr.Server.App.Core.Users.WebApi
{
    internal class GetUserSettingsHandler : IQueryHandler<GetUserSettings, GetUserSettingsResult>
    {
        private readonly INotificationsRepository _repository;
        private readonly IUserRepository _userRepository;

        public GetUserSettingsHandler(INotificationsRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<GetUserSettingsResult> HandleAsync(IMessageContext context, GetUserSettings query)
        {
            var settings = await _repository.TryGetAsync(query.UserId, query.ApplicationId)
                           ?? new UserNotificationSettings(query.UserId, query.ApplicationId);
            var user = await _userRepository.GetUserAsync(query.UserId);
            return new GetUserSettingsResult
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                MobileNumber = user.MobileNumber,
                EmailAddress = user.EmailAddress,
                Notifications = new NotificationSettings
                {
                    NotifyOnReOpenedIncident = settings.ReopenedIncident.ConvertEnum<NotificationState>(),
                    NotifyOnUserFeedback = settings.UserFeedback.ConvertEnum<NotificationState>(),
                    NotifyOnPeaks = settings.ApplicationSpike.ConvertEnum<NotificationState>(),
                    NotifyOnNewReport = settings.NewReport.ConvertEnum<NotificationState>(),
                    NotifyOnNewIncidents = settings.NewIncident.ConvertEnum<NotificationState>()
                }
            };
        }
    }
}