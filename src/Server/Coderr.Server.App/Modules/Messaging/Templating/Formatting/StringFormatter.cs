using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace codeRR.Server.App.Modules.Messaging.Templating.Formatting
{
    /// <summary>
    ///     Converts a string
    /// </summary>
    public class StringFormatter
    {
        /// <summary>
        ///     Format a string
        /// </summary>
        /// <param name="source">string to format</param>
        /// <param name="arguments">arguments to replace</param>
        /// <returns>formatted string</returns>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public string Format(string source, params object[] arguments)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (arguments == null) throw new ArgumentNullException("arguments");
            if (arguments.Length == 0)
                throw new ArgumentException("Must provide at least one argument.");

            var tokenizer = new Tokenizer();
            var tokens = tokenizer.Parse(source);

            //Console.WriteLine(string.Join("+", tokens.Select(x => x.Name + x.Value)));
            var converter = new ObjectToDictionaryConverter();
            var model = converter.Convert(arguments[0]);

            if (arguments.Length != 1)
            {
                return "";
            }

            var includeEnd = false;
            var sb = new StringBuilder();
            foreach (var token in tokens)
            {
                if (includeEnd)
                {
                    sb.Append('}');
                    includeEnd = false;
                }
                else if (token.Name != null && token.Name.IndexOfAny(new[] {' ', ',', '-', '+', ':'}) > -1)
                {
                    // vars can't contain spaces.
                    sb.Append('{');
                    sb.Append(token.Name);
                    includeEnd = true;
                }
                else if (token.Name == null)
                    sb.Append(token.Value);
                else
                {
                    object value;
                    if (!model.TryGetValue(token.Name, out value))
                        throw new FormatException("Failed to find '" + token.Name + "' in model");
                    sb.Append(value);
                }
            }
            return sb.ToString();
        }
    }
}