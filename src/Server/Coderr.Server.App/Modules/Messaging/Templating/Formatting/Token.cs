using System;

namespace codeRR.Server.App.Modules.Messaging.Templating.Formatting
{
    /// <summary>
    ///     A token when parsing the template text
    /// </summary>
    public class Token
    {
        /// <summary>
        ///     Creates a new instance of <see cref="Token" />.
        /// </summary>
        /// <param name="value">value</param>
        /// <exception cref="ArgumentNullException">value</exception>
        public Token(string value)
        {
            if (value == null) throw new ArgumentNullException("value");
            Value = value;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="Token" />.
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <exception cref="ArgumentNullException">name; value</exception>
        public Token(string name, string value)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (value == null) throw new ArgumentNullException("value");
            Name = name;
            Value = value;
        }

        /// <summary>
        ///     Name (if this is an argument)
        /// </summary>
        public string Name { get; private set; }


        /// <summary>
        ///     Value/Text (text for non arguments, and the argument value for arguments)
        /// </summary>
        public string Value { get; private set; }
    }
}