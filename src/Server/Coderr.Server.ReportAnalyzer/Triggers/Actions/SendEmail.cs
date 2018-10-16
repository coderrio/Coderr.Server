using System.Threading.Tasks;

namespace Coderr.Server.ReportAnalyzer.Triggers.Actions
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