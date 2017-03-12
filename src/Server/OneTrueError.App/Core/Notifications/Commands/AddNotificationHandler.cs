using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Notifications;

namespace OneTrueError.App.Core.Notifications.Commands
{
    [Component]
    class AddNotificationHandler : ICommandHandler<AddNotification>
    {
        public async Task ExecuteAsync(AddNotification command)
        {
            //TODO: Implement
        }
    }
}
