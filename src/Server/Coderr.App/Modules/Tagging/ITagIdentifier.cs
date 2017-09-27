namespace codeRR.Server.App.Modules.Tagging
{
    /// <summary>
    ///     Purpose of this interface is to provide a way to be able to search through the error report context to be able to
    ///     identify StackOverflow.com tags.
    /// </summary>
    public interface ITagIdentifier
    {
        /// <summary>
        ///     Check if the wanted tag is supported.
        /// </summary>
        /// <param name="context">Error context providing information to search through</param>
        void Identify(TagIdentifierContext context);
    }
}