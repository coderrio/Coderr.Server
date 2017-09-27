using System;
using System.Collections.Generic;
using System.Linq;
using codeRR.Server.Api.Core.Reports;
using codeRR.Server.App.Modules.Tagging.Domain;

namespace codeRR.Server.App.Modules.Tagging
{
    /// <summary>
    ///     Context used when trying to identify StackOverflow.com tags
    /// </summary>
    public class TagIdentifierContext
    {
        private readonly IReadOnlyList<Tag> _existingTags;
        private readonly List<Tag> _newTags = new List<Tag>();
        private readonly ReportDTO _reportToAnalyze;
        private readonly string[] _stacktrace;


        /// <summary>
        ///     Creates a new instance of <see cref="TagIdentifierContext" />.
        /// </summary>
        /// <param name="reportToAnalyze">rta</param>
        /// <param name="tags"></param>
        /// <exception cref="ArgumentNullException">reportToAnalyze;tags</exception>
        public TagIdentifierContext(ReportDTO reportToAnalyze, IReadOnlyList<Tag> tags)
        {
            _reportToAnalyze = reportToAnalyze ?? throw new ArgumentNullException(nameof(reportToAnalyze));
            _existingTags = tags ?? throw new ArgumentNullException(nameof(tags));
            var ex = reportToAnalyze.Exception;
            _stacktrace = ex?
                              .StackTrace?
                              .Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries)
                          ?? new string[0];
        }

        /// <summary>
        ///     Application that the report is for.
        /// </summary>
        public int ApplicationId => _reportToAnalyze.ApplicationId;

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
        public IReadOnlyList<Tag> NewTags => _newTags;

        /// <summary>
        ///     Add tag if the specified text is found in the stack trace
        /// </summary>
        /// <param name="libraryToFind">text to find</param>
        /// <param name="tagToAdd">tag to add</param>
        /// <returns>index in stacktrace if found; otherwise -1</returns>
        /// <exception cref="ArgumentNullException">libraryToFind;tagToAdd</exception>
        public int AddIfFound(string libraryToFind, string tagToAdd)
        {
            if (libraryToFind == null) throw new ArgumentNullException(nameof(libraryToFind));
            if (tagToAdd == null) throw new ArgumentNullException(nameof(tagToAdd));

            for (var i = 0; i < _stacktrace.Length; i++)
            {
                if (_stacktrace[i].IndexOf(libraryToFind, StringComparison.OrdinalIgnoreCase) == -1)
                    continue;

                AddTag(tagToAdd, i);
                return i;
            }

            return -1;
        }

        /// <summary>
        ///     Add an incident tag
        /// </summary>
        /// <param name="tag">tag name</param>
        /// <param name="orderNumber">used to customize in which order the tags appear on the web page. 1 = first</param>
        public void AddTag(string tag, int orderNumber)
        {
            if (_newTags.Any(x => x.Name == tag))
                return;
            if (_existingTags.Any(x => x.Name == tag))
                return;

            _newTags.Add(new Tag(tag, orderNumber));
        }

        /// <summary>
        ///     Get a property value from a context collection
        /// </summary>
        /// <param name="collectionName">context collection</param>
        /// <param name="propertyName">property in the collection</param>
        /// <exception cref="ArgumentNullException">collectionName; propertyName</exception>
        /// <returns>value if found; otherwise <c>null</c>.</returns>
        public string GetPropertyValue(string collectionName, string propertyName)
        {
            if (collectionName == null) throw new ArgumentNullException(nameof(collectionName));
            if (propertyName == null) throw new ArgumentNullException(nameof(propertyName));

            var assemblies = Enumerable.FirstOrDefault(_reportToAnalyze.ContextCollections, x => x.Name == collectionName);
            if (assemblies == null)
                return null;

            return assemblies.Properties.TryGetValue(propertyName, out string version) ? version : null;
        }

        /// <summary>
        ///     Find a text in the stack trace
        /// </summary>
        /// <param name="libraryToFind">text to find</param>
        /// <exception cref="ArgumentNullException">libraryToFind</exception>
        /// <returns><c>true</c> if found; otherwise <c>false</c>.</returns>
        public bool IsFound(string libraryToFind)
        {
            if (libraryToFind == null) throw new ArgumentNullException(nameof(libraryToFind));
            return _stacktrace.Any(t => t.IndexOf(libraryToFind, StringComparison.OrdinalIgnoreCase) != -1);
        }
    }
}