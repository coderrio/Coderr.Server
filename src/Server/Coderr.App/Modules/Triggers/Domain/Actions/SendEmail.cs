using System.Threading.Tasks;

namespace codeRR.App.Modules.Triggers.Domain.Actions
{
    [TriggerActionName("Email")]
    internal class SendEmailTask : ITriggerAction
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task ExecuteAsync(ActionExecutionContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            //TODO: Create a generic emailer with symbols as the default
            // notification system is currently much better than this notification thingy.
            //i.e. it do not add any value currently.
        }
    }
}