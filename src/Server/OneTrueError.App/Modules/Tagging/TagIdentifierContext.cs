using System;
using System.Collections.Generic;
using System.Linq;
using OneTrueError.Api.Core.Reports;
using OneTrueError.App.Modules.Tagging.Domain;

namespace OneTrueError.App.Modules.Tagging
{
    /// <summary>
    ///     Context used when trying to identify stackoverflow tags
    /// </summary>
    public class TagIdentifierContext
    {
        private readonly ReportDTO _reportToAnalyze;
        private readonly string[] _stacktrace;
        private readonly List<Tag> _tags = new List<Tag>();


        /// <summary>
        ///     Creates a new instance of <see cref="TagIdentifierContext" />.
        /// </summary>
        /// <param name="reportToAnalyze">rta</param>
        /// <exception cref="ArgumentNullException">reportToAnalyze</exception>
        public TagIdentifierContext(ReportDTO reportToAnalyze)
        {
            if (reportToAnalyze == null) throw new ArgumentNullException("reportToAnalyze");

            _reportToAnalyze = reportToAnalyze;
            var ex = reportToAnalyze.Exception;
            if (ex != null && ex.StackTrace != null)
                _stacktrace = ex.StackTrace.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
            else
                _stacktrace = new string[0];
        }

        /// <summary>
        ///     Exception to find tags for.
        /// </summary>
        /// <summary>
        ///     Tags identifying relevant tags which can be used to find information about why the exception happened. Like
        ///     "EntityFramework", "ASP.NET-MVC" etc.
        /// </summary>
        /// <remarks>
        ///     <para>These tags are used directly to search for possible solutions.</para>
        /// </remarks>
        public IReadOnlyList<Tag> Tags
        {
            get { return _tags; }
        }

        /// <summary>
        ///     Add tag if the specified text is found in the stack trace
        /// </summary>
        /// <param name="libraryToFind">text to find</param>
        /// <param name="tagToAdd">tag to add</param>
        /// <returns>index in stacktrace if found; otherwise -1</returns>
        public int AddIfFound(string libraryToFind, string tagToAdd)
        {
            if (libraryToFind == null) throw new ArgumentNullException("libraryToFind");
            if (tagToAdd == null) throw new ArgumentNullException("tagToAdd");

            for (var i = 0; i < _stacktrace.Length; i++)
            {
                if (_stacktrace[i].IndexOf(libraryToFind, StringComparison.OrdinalIgnoreCase) != -1)
                {
                    AddTag(tagToAdd, i);
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Add a stackoverflow tag
        /// </summary>
        /// <param name="tag">tag name</param>
        /// <param name="orderNumber">used to customize in which order the tags appear on the web page.</param>
        public void AddTag(string tag, int orderNumber)
        {
            if (_tags.Any(x => x.Name == tag))
                return;
            _tags.Add(new Tag(tag, orderNumber));
        }

        /// <summary>
        ///     Get a property value from a context collection
        /// </summary>
        /// <param name="collectionName">context collection</param>
        /// <param name="propertyName">property in the collection</param>
        /// <returns>value if found; otherwise <c>null</c>.</returns>
        public string GetPropertyValue(string collectionName, string propertyName)
        {
            if (collectionName == null) throw new ArgumentNullException("collectionName");
            if (propertyName == null) throw new ArgumentNullException("propertyName");

            var assemblies = _reportToAnalyze.ContextCollections.FirstOrDefault(x => x.Name == collectionName);
            if (assemblies == null)
                return null;

            string version;
            return assemblies.Properties.TryGetValue(propertyName, out version) ? version : null;
        }

        /// <summary>
        ///     Find a text in the stack trace
        /// </summary>
        /// <param name="libraryToFind">text to find</param>
        /// <returns><c>true</c> if found; otherwise <c>false</c>.</returns>
        public bool IsFound(string libraryToFind)
        {
            if (libraryToFind == null) throw new ArgumentNullException("libraryToFind");
            return _stacktrace.Any(t => t.IndexOf(libraryToFind, StringComparison.OrdinalIgnoreCase) != -1);
        }
    }
}