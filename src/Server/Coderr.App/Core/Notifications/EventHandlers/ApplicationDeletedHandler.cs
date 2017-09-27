using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using codeRR.Api.Core.Applications.Events;

namespace codeRR.App.Core.Notifications.EventHandlers
{
    /// <summary>
    ///     Will delete all reports for the given application
    /// </summary>
    [Component(RegisterAsSelf = true)]
    public class ApplicationDeletedHandler : IApplicationEventSubscriber<ApplicationDeleted>
    {
        private IAdoNetUnitOfWork _uow;

        /// <summary>
        /// Creates a new instance of <see cref="ApplicationDeletedHandler"/>.
        /// </summary>
        public ApplicationDeletedHandler(IAdoNetUnitOfWork uow)
        {
            if (uow == null) throw new ArgumentNullException("uow");
            _uow = uow;
        }

        /// <inheritdoc />
        public Task HandleAsync(ApplicationDeleted e)
        {
            _uow.ExecuteNonQuery("DELETE FROM UserNotificationSettings WHERE ApplicationId = @id", new { id = e.ApplicationId });
            return Task.FromResult<object>(null);
        }
    }
}