using System;
using System.Collections.Generic;
using codeRR.Server.Api.Core.Reports;

namespace codeRR.Server.App.Modules.Similarities.Domain.Adapters.Runner
{
    /// <summary>
    ///     Context for <see cref="IValueAdapter" />.
    /// </summary>
    public class ValueAdapterContext
    {
        private readonly List<CustomField> _customFields = new List<CustomField>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValueAdapterContext" /> class.
        /// </summary>
        /// <param name="contextName">Context being processed.</param>
        /// <param name="propertyName">Name of the property that the value is for.</param>
        /// <param name="value">The value. Can be null as <c>null</c> can be specified during the collection.</param>
        /// <param name="report">Entire report which the context/property belongs to.</param>
        public ValueAdapterContext(string contextName, string propertyName, object value, ReportDTO report)
        {
            if (contextName == null) throw new ArgumentNullException("contextName");
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (report == null) throw new ArgumentNullException("report");

            Report = report;
            Value = value;
            PropertyName = propertyName;
            ContextName = contextName;

            //TODO: Load real application type.
            TypeOfApplication = "DesktopApplication";
        }

        /// <summary>
        ///     Context being processed.
        /// </summary>
        public string ContextName { get; private set; }

        /// <summary>
        ///     New fields created by the adapters
        /// </summary>
        public IEnumerable<CustomField> CustomFields
        {
            get { return _customFields; }
        }

        /// <summary>
        ///     Tells the analyzer that this specific property should not be added.
        /// </summary>
        public bool IgnoreProperty { get; set; }

        /// <summary>
        ///     Property name in the context collection
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        ///     Report that the context collection is for.
        /// </summary>
        public ReportDTO Report { get; private set; }

        /// <summary>
        ///     DesktopApplication, Mobile, Server. From the <see cref="TypeOfApplication" /> enum.
        /// </summary>
        public string TypeOfApplication { get; set; }

        /// <summary>
        ///     Property value
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        ///     Add a new custom field
        /// </summary>
        /// <param name="propertyName">Name of the new field</param>
        /// <param name="value">Property value for the new field</param>
        /// <remarks>
        ///     <para>
        ///         (used when the property value is aggregated information that should be split into multiple fields.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">propertyName; value</exception>
        public void AddCustomField(string propertyName, object value)
        {
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (value == null) throw new ArgumentNullException("value");

            _customFields.Add(new CustomField(ContextName, propertyName, value));
        }

        /// <summary>
        ///     Add a new custom field
        /// </summary>
        /// <param name="contextName">Associate the property with another context collection</param>
        /// <param name="propertyName">Name of the new field</param>
        /// <param name="value">Property value for the new field</param>
        /// <remarks>
        ///     <para>
        ///         (used when the property value is aggregated information that should be split into multiple fields.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException">contextName; propertyName; value</exception>
        public void AddCustomField(string contextName, string propertyName, object value)
        {
            if (contextName == null) throw new ArgumentNullException("contextName");
            if (propertyName == null) throw new ArgumentNullException("propertyName");
            if (value == null) throw new ArgumentNullException("value");

            _customFields.Add(new CustomField(contextName, propertyName, value));
        }
    }
}