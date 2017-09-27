namespace codeRR.Server.App.Modules.Messaging
{
    /// <summary>
    ///     Generates HTML from a text template.
    /// </summary>
    public interface ITemplateParser
    {
        /// <summary>
        ///     Run all transformation steps.
        /// </summary>
        /// <param name="messageTemplate">Template to transform</param>
        /// <param name="viewModel">View model</param>
        /// <returns>Generated HTML</returns>
        string RunAll(Template messageTemplate, object viewModel);

        /// <summary>
        ///     Format keywords only. i.e. no HTML transformation etc.
        /// </summary>
        /// <param name="messageTemplate">Template to transform</param>
        /// <param name="viewModel">View model</param>
        /// <returns>Generated HTML</returns>
        string RunFormatterOnly(Template messageTemplate, object viewModel);
    }
}