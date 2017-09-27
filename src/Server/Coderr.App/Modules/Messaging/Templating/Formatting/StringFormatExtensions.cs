namespace codeRR.Server.App.Modules.Messaging.Templating.Formatting
{
    /// <summary>
    ///     Extension methods for a string
    /// </summary>
    public static class StringFormatExtensions
    {
        /// <summary>
        ///     Format string
        /// </summary>
        /// <param name="format">string to format</param>
        /// <param name="arguments">arguments to replace with</param>
        /// <returns>formatted string</returns>
        public static string FormatWith(this string format, params object[] arguments)
        {
            var compiler = new StringFormatter();
            return compiler.Format(format, arguments);
        }
    }
}