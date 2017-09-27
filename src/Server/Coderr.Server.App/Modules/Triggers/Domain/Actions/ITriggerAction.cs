using System.Threading.Tasks;

namespace codeRR.Server.App.Modules.Triggers.Domain.Actions
{
    /// <summary>
    ///     Represents trigger actions, i.e. the work that should be done once all rules have accepted the report.
    /// </summary>
    public interface ITriggerAction
    {
        /// <summary>
        ///     Execute the action.
        /// </summary>
        /// <param name="context">action context</param>
        Task ExecuteAsync(ActionExecutionContext context);
    }
}