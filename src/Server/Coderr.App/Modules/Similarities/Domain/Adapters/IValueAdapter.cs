using codeRR.Server.App.Modules.Similarities.Domain.Adapters.Runner;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters
{
    /// <summary>
    ///     Values might need to be normalized in some way before the similarities are calculated.
    /// </summary>
    /// <remarks>
    ///     For instance the amount of used memory needs to be rounded off into blocks.
    /// </remarks>
    public interface IValueAdapter
    {
        /// <summary>
        ///     Adapt the value specified in the context
        /// </summary>
        /// <param name="context">Context information</param>
        /// <param name="currentValue">Value which might have been adapted</param>
        /// <returns>The new value (or same as the current value if no modification has been made)</returns>
        object Adapt(ValueAdapterContext context, object currentValue);
    }
}