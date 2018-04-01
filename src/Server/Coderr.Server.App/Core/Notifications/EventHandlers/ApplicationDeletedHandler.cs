using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Events;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;
using Griffin.Data;

namespace Coderr.Server.App.Core.Notifications.EventHandlers
{
    /// <summary>
    ///     Will delete all reports for the given application
    /// </summary>
    public class ApplicationDeletedHandler : IMessageHandler<ApplicationDeleted>
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
        public Task HandleAsync(IMessageContext context, ApplicationDeleted e)
        {
            _uow.ExecuteNonQuery("DELETE FROM UserNotificationSettings WHERE ApplicationId = @id", new { id = e.ApplicationId });
            return Task.FromResult<object>(null);
        }
    }
}