using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.Abstractions.Boot;

namespace Coderr.Server.ReportAnalyzer.ErrorReports
{
    /// <summary>
    ///     Used to generate hash codes for incoming error reports
    /// </summary>
    /// <remarks>
    ///     This new generator uses context information provided in exceptions to generate the hash code. For instance HTTP 404
    ///     exception
    ///     are based on the URI and not exception information
    /// </remarks>
    [ContainerService]
    public class HashCodeGenerator : IHashCodeGenerator
    {
        private readonly IHashCodeSubGenerator[] _generators;
        private static Regex _lineNumberRegEx = new Regex(RemoveLineNumbersRegEx, RegexOptions.Multiline);
        private static readonly Regex FirstWordRegEx = new Regex(@"^[ \t]*([^\s]+) ", RegexOptions.Multiline);
        private const string RemoveLineNumbersRegEx = @"^(.*)(:[\w]+ [\d]+)";
        static List<Regex> _cleanRegExes = new List<Regex>();
        private static List<Regex> _replaceRegExes;

        /// <summary>
        ///     Creates a new instance of <see cref="HashCodeGenerator" />.
        /// </summary>
        /// <param name="generators">Specialized generators. treated as singletons.</param>
        public HashCodeGenerator(IEnumerable<IHashCodeSubGenerator> generators)
        {
            _generators = generators.ToArray();
        }

        /// <summary>
        ///     Generate a new hash code
        /// </summary>
        /// <param name="entity">entity</param>
        /// <returns>hash code</returns>
        /// <exception cref="ArgumentNullException">entity</exception>
        public ErrorHashCode GenerateHashCode(ErrorReportEntity entity)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            foreach (var generator in _generators)
            {
                if (generator.CanGenerateFrom(entity))
                {
                    // forgiving ones so that we can get the report and process it with a default hash code instead.
                    var code = generator.GenerateHashCode(entity);
                    if (code != null)
                        return code;
                }
            }

            return DefaultCreateHashCode(entity);
        }

        /// <summary>
        ///     Method that will be invoked if no implementations of <see cref="IHashCodeSubGenerator" /> generates an hash code.
        /// </summary>
        /// <param name="report">received report</param>
        /// <returns>hash code</returns>
        /// <exception cref="ArgumentNullException">report</exception>
        protected virtual ErrorHashCode DefaultCreateHashCode(ErrorReportEntity report)
        {
            if (report == null) throw new ArgumentNullException("report");


            var hashSource = $"{report.Exception.FullName ?? report.Exception.Name}\r\n";
            var foundHashSource = false;

            // the client libraries can by themselves specify how we should identify
            // unique incidents. We then use that identifier in combination with the exception name.
            var collection = report.ContextCollections.FirstOrDefault(x => x.Name == "CoderrData");
            if (collection != null)
            {
                if (collection.Properties.TryGetValue("HashSource", out var reportHashSource))
                {
                    var url = GetUrl(report.ContextCollections);

                    //this is an workaround since our own Coderr Report vary url
                    if (url?.StartsWith("/receiver/report/") != true)
                    {
                        foundHashSource = true;
                        hashSource += reportHashSource;
                    }
                }
            }
            if (!foundHashSource)
            {
                // This identifier is determined by the developer when  the error is generated.
                foreach (var contextCollection in report.ContextCollections)
                {
                    if (!contextCollection.Properties.TryGetValue("ErrorHashSource", out var ourHashSource))
                        continue;

                    hashSource = ourHashSource;
                    foundHashSource = true;
                    break;
                }
            }

            var hashSourceForCompability = "";
            if (!foundHashSource)
            {
                hashSourceForCompability = hashSource + CleanStackTrace(report.Exception.StackTrace ?? "", false);
                hashSource += CleanStackTrace(report.Exception.StackTrace ?? "");

            }

            var hash = HashTheSource(hashSource);
            return new ErrorHashCode
            {
                CollisionIdentifier = report.GenerateHashCodeIdentifier(),
                HashCode = hash.ToString("X"),
                CompabilityHashSource = hashSourceForCompability == "" ? null : HashTheSource(hashSourceForCompability).ToString("X")
            };
        }

        private string GetUrl(IReadOnlyList<ErrorReportContextCollection> contextCollections)
        {
            foreach (var collection in contextCollections)
            {
                if (collection.Properties.TryGetValue("Url", out var url))
                {
                    return url;
                }
            }

            return null;
        }

        private static int HashTheSource(string hashSource)
        {
            var hash = 23;
            foreach (var c in hashSource)
            {
                hash = hash * 31 + c;
            }

            return hash;
        }

        private static void EnsureRegExes()
        {
            if (_cleanRegExes.Count > 0)
                return;

            var linesToRemove = new[]
            {
                "---.*---\r?\n",
                @"[ ]*at System.Runtime.ExceptionServices.ExceptionDispatchInfo.Throw\(\)\r?\n",
                @"[ ]*at System.Runtime.CompilerServices.TaskAwaiter.*\r?\n",
                @"[ ]*at System.Threading.ExecutionContext.*\r?\n",
                @"[ ]*at NServiceBus.*\r?\n",
                @"[ ]*at RabbitMQ.*\r?\n",
                @"[ ]*at Coderr.Client.*\r?\n",
                @"[ ]*at .*d__+d+.MoveNext.*\r?\n",
                @"[ ]*at System.Threading.Tasks.*\r?\n",
                @"[ ]*at Microsoft.AspNetCore.Mvc.Internal.*\r?\n",
                @"[ ]*at sun.reflect.*\r?\n",
                @"[ ]*at java.lang.reflect.*\r?\n",
                @"\$\$Lambda\$[\d]+\/[\d]+", //Java Lambda,
                @" \[0x[a-f\d]+\] in \<[0-9a-z ]+\>:?\d*" // strange debug address for Java ;)
            };
            _cleanRegExes.Clear();
            foreach (var rex in linesToRemove)
            {
                if (rex.EndsWith("$") || rex.StartsWith("^"))
                    _cleanRegExes.Add(new Regex(rex, RegexOptions.Multiline));
                else
                    _cleanRegExes.Add(new Regex(rex));
            }
            
            var linesToReplace = new[]
            {
                @"(\w+):\d+\)\r?$",//@"\.\w+(:\d+)\)$", // java line numbers
                @"\<([a-zA-Z0-9_]+)\>d__[\d]+`?\d?.MoveNext",
                @"(sun.reflect.GeneratedMethodAccessor)\d+"
            };
            _replaceRegExes = new List<Regex>();
            foreach (var rex in linesToReplace)
            {
                if (rex.EndsWith("$") || rex.StartsWith("^"))
                    _replaceRegExes.Add(new Regex(rex, RegexOptions.Multiline));
                else
                    _replaceRegExes.Add(new Regex(rex));
            }
        }

        internal static string CleanStackTrace(string stacktrace, bool withLanguageCleaner = true)
        {
            EnsureRegExes();
            stacktrace = _lineNumberRegEx.Replace(stacktrace, "$1", 1000);
            var eol = stacktrace.IndexOf("\r\n") == -1 ? "\n" : "\r\n";

            foreach (var regEx in _cleanRegExes)
            {
                stacktrace = regEx.Replace(stacktrace, "", 100);
            }
            foreach (var regEx in _replaceRegExes)
            {
                stacktrace = regEx.Replace(stacktrace, "$1", 100);
                stacktrace = regEx.Replace(stacktrace, OnEvaluate);
            }

            if (withLanguageCleaner)
            {
                stacktrace = FirstWordRegEx.Replace(stacktrace, "", 100);
            }

            return stacktrace;
        }

        private static string OnEvaluate(Match match)
        {
            return match.Captures[0].Value;
        }
    }
}