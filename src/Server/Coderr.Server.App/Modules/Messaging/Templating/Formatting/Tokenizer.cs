using System.Collections.Generic;

namespace codeRR.Server.App.Modules.Messaging.Templating.Formatting
{
    /// <summary>
    ///     Converts a string with named arguments to a string
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Supports escaping <c>"hello {{world}}!"</c> would produce only text tokens (i.e. no argument name was
    ///         detected)
    ///     </para>
    /// </remarks>
    /// <example>
    ///     <code>
    /// var tokenizer = new Tokenizer();
    /// var tokens = tokenizer.Parse("Hello {world}!");
    /// Console.WriteLine(tokens[0].Value); // --> "Hello "
    /// Console.WriteLine(tokens[1].Name); // --> "world"
    /// Console.WriteLine(tokens[2].Value); // --> "!"
    /// </code>
    /// </example>
    public class Tokenizer
    {
        private readonly List<Token> _tokens = new List<Token>();

        /// <summary>
        ///     Delimiters which makes switches from argument name to text when found inside of "{}".
        /// </summary>
        private readonly char[] NameDelimiters = {'{', '}', ' ', '-', '+', '/', '\t', ',', ':'};

        private bool _isInCurly;
        private int _offset;
        private int _textStartPos = -1;
        private string _value;

        private char Current
        {
            get { return _value[_offset]; }
        }

        private char Next
        {
            get { return _offset + 1 < _value.Length ? _value[_offset + 1] : char.MinValue; }
        }

        /// <summary>
        ///     Parse string
        /// </summary>
        /// <param name="text">Text to parse</param>
        /// <returns>Tokens found in the string</returns>
        public IList<Token> Parse(string text)
        {
            _value = text;
            while (_offset < _value.Length)
            {
                switch (Current)
                {
                    case '{':
                        ParseStartCurly();
                        break;
                    case '}':
                        ParseEndCurly();
                        break;
                    default:
                        ParseChar();
                        break;
                }

                ++_offset;
            }

            if (_textStartPos != -1)
            {
                _tokens.Add(new Token(_value.Substring(_textStartPos)));
            }

            return _tokens;
        }

        private void ParseChar()
        {
            if (_textStartPos == -1)
                _textStartPos = _offset;
        }

        private void ParseEndCurly()
        {
            if (_textStartPos != -1)
            {
                // not a valid argument name, just continue treating this single
                // end curly as text.
                if (Next != '}' && !_isInCurly)
                    return;

                if (_isInCurly)
                    _tokens.Add(new Token(_value.Substring(_textStartPos, _offset - _textStartPos), ""));
                else
                    _tokens.Add(new Token(_value.Substring(_textStartPos, _offset - _textStartPos)));

                _textStartPos = -1;
            }

            if (_isInCurly)
            {
                _isInCurly = false;
            }
            else
            {
                _tokens.Add(new Token("}"));
                if (Next == '}')
                    _offset++;
            }
        }

        private void ParseStartCurly()
        {
            if (Next == '{')
            {
                if (_textStartPos != -1)
                {
                    _tokens.Add(new Token(_value.Substring(_textStartPos, _offset - _textStartPos) + "{"));
                    _textStartPos = -1;
                }
                else
                    _tokens.Add(new Token("{"));

                _offset++;
                return;
            }

            // check if this really is for an argument
            var pos = _value.IndexOfAny(NameDelimiters, _offset + 1);
            if (pos != -1 && _value[pos] != '}')
            {
                if (_textStartPos == -1)
                    _textStartPos = _offset;

                //found another char, let's continue searching
                return;
            }

            if (_textStartPos != -1)
            {
                _tokens.Add(new Token(_value.Substring(_textStartPos, _offset - _textStartPos)));
                _textStartPos = -1;
            }

            _textStartPos = _offset + 1;
            _isInCurly = true;
        }
    }
}