using System;
using System.IO;
using System.Text;
using codeRR.Server.App.Modules.Messaging.Templating.Formatting;
using ColorCode;
using Griffin.Container;

namespace codeRR.Server.App.Modules.Messaging.Templating
{
    /// <summary>
    ///     Used to parse texts (convert markdown and extras to HTML)
    /// </summary>
    [ContainerService(ContainerLifetime.Transient)]
    public class TemplateParser : ITemplateParser
    {
        /// <summary>
        ///     Run all formatters (replace text tokens, run markdown and finalize with a syntax highlighter)
        /// </summary>
        /// <param name="messageTemplate">template to parse</param>
        /// <param name="viewModel">viewModel used to fill the template.</param>
        /// <returns>HTML</returns>
        public string RunAll(Template messageTemplate, object viewModel)
        {
            if (messageTemplate == null) throw new ArgumentNullException("messageTemplate");

            var sr = new StreamReader(messageTemplate.Content);
            var text = sr.ReadToEnd();
            var formatter = new StringFormatter();
            text = formatter.Format(text, viewModel);
            text = Markdown(text);
            return ColorizeCode(text);
        }

        /// <summary>
        ///     Run only the viewModel replacer.
        /// </summary>
        /// <param name="messageTemplate">template to parse</param>
        /// <param name="viewModel">viewModel used to fill the template.</param>
        /// <returns>HTML</returns>
        public string RunFormatterOnly(Template messageTemplate, object viewModel)
        {
            if (messageTemplate == null) throw new ArgumentNullException("messageTemplate");

            var sr = new StreamReader(messageTemplate.Content);
            var text = sr.ReadToEnd();
            //return Smart.Format(text, viewModel);
            var formatter = new StringFormatter();
            return formatter.Format(text, viewModel);
        }

        private static string ColorizeCode(string html)
        {
            var colorizer = new CodeColorizer();
            var sb = new StringBuilder();
            var pos = html.IndexOf("<pre><code>", StringComparison.Ordinal);
            if (pos > -1)
            {
                var lastPos = 0;
                sb.Append(html.Substring(0, pos));
                while (pos > -1)
                {
                    lastPos = html.IndexOf("</code></pre>", pos, StringComparison.Ordinal);

                    var snippet = html.Substring(pos + 11, lastPos - pos - 11);
                    var colorizedSnippet = colorizer.Colorize(snippet, Languages.CSharp);
                    sb.Append(colorizedSnippet);

                    pos = html.IndexOf("<pre><code>", lastPos, StringComparison.Ordinal);
                }

                sb.Append(html.Substring(lastPos + 13));
                html = sb.ToString();
            }

            return html;
        }

        private static string Markdown(string template)
        {
            return Markdig.Markdown.ToHtml(template);
        }
    }
}