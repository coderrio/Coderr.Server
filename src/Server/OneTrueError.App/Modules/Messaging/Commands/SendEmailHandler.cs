using System;
using System.IO;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Newtonsoft.Json;
using OneTrueError.Api.Core.Messaging.Commands;

namespace OneTrueError.App.Modules.Messaging.Commands
{
    [Component]
    class SendEmailHandler : ICommandHandler<SendEmail>
    {
#pragma warning disable 1998
        public async Task ExecuteAsync(SendEmail command)
#pragma warning restore 1998
        {
            var json = JsonConvert.SerializeObject(command);
            File.WriteAllText(@"C:\temp\email_" + Guid.NewGuid().ToString("N") + ".json", json);

        }
    }
}
