using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Notifications;

namespace OneTrueError.App.Core.Notifications.Commands
{
    /// <summary>
    /// Handler for <see cref="AddNotification"/>.
    /// </summary>
    [Component]
    public class AddNotificationHandler : ICommandHandler<AddNotification>
    {
        /// <summary>
        /// Not implemented yet.
        /// </summary>
        /// <param name="command">cmd</param>
        /// <returns>task</returns>
        public Task ExecuteAsync(AddNotification command)
        {
            //TODO: Implement
            return Task.FromResult<object>(null);
        }
    }
}
