using System.Threading.Tasks;

namespace codeRR.Server.App.Modules.Triggers.Domain.Actions
{
    [TriggerActionName("Email")]
    internal class SendEmailTask : ITriggerAction
    {
        public Task ExecuteAsync(ActionExecutionContext context)
        {
            //TODO: Create a generic emailer with symbols as the default
            // notification system is currently much better than this notification thingy.
            //i.e. it do not add any value currently.
            return Task.FromResult<object>(null);
        }
    }
}