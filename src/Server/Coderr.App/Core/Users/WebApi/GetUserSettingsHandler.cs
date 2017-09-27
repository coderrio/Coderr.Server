using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using codeRR.Api.Core;
using codeRR.Api.Core.Users;
using codeRR.Api.Core.Users.Queries;
using codeRR.App.Core.Notifications;
using NotificationState = codeRR.Api.Core.Users.NotificationState;

namespace codeRR.App.Core.Users.WebApi
{
    [Component]
    internal class GetUserSettingsHandler : IQueryHandler<GetUserSettings, GetUserSettingsResult>
    {
        private readonly INotificationsRepository _repository;
        private readonly IUserRepository _userRepository;

        public GetUserSettingsHandler(INotificationsRepository repository, IUserRepository userRepository)
        {
            _repository = repository;
            _userRepository = userRepository;
        }

        public async Task<GetUserSettingsResult> ExecuteAsync(GetUserSettings query)
        {
            var settings = await _repository.TryGetAsync(query.UserId, query.ApplicationId)
                           ?? new UserNotificationSettings(query.UserId, query.ApplicationId);
            var user = await _userRepository.GetUserAsync(query.UserId);
            return new GetUserSettingsResult
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                MobileNumber = user.MobileNumber,
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