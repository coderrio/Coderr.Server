namespace codeRR.Server.App.Modules.Triggers.Domain
{
    /// <summary>
    ///     Decides if an error report can be passed on
    /// </summary>
    public interface ITriggerRule
    {
        /// <summary>
        ///     Validate report
        /// </summary>
        /// <param name="context">Context info</param>
        /// <returns>Recommendation</returns>
        FilterResult Validate(FilterContext context);
    }
}