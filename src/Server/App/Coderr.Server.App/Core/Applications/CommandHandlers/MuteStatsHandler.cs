using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Commands;
using Coderr.Server.Domain.Core.Applications;
using DotNetCqs;

namespace Coderr.Server.App.Core.Applications.CommandHandlers
{
    class MuteStatisticsQuestionHandler : IMessageHandler<MuteStatisticsQuestion>
    {
        private readonly IApplicationRepository _applicationRepository;

        public MuteStatisticsQuestionHandler(IApplicationRepository applicationRepository)
        {
            _applicationRepository = applicationRepository ?? throw new ArgumentNullException(nameof(applicationRepository));
        }

        public async Task HandleAsync(IMessageContext context, MuteStatisticsQuestion message)
        {
            var app = await _applicationRepository.GetByIdAsync(message.ApplicationId);
            app.MuteStatisticsQuestion = true;
            await _applicationRepository.UpdateAsync(app);
        }
    }
}
