using System;
using System.Collections.Generic;

namespace codeRR.Server.Api.Web.Feedback.Queries
{
    /// <summary>
    ///     Result for <see cref="GetIncidentFeedback" />.
    /// </summary>
    public class GetIncidentFeedbackResult
    {
        /// <summary>
        ///     Creates a new instance of <see cref="GetIncidentFeedbackResult" />.
        /// </summary>
        /// <param name="items">Feedback items</param>
        /// <param name="emails">Emails to all users that are waiting on status updates.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public GetIncidentFeedbackResult(IReadOnlyList<GetIncidentFeedbackResultItem> items, ICollection<string> emails)
        {
            if (items == null) throw new ArgumentNullException("items");
            if (emails == null) throw new ArgumentNullException("emails");
            Items = items;
            Emails = emails;
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected GetIncidentFeedbackResult()
        {
            Emails = new List<string>();
        }

        /// <summary>
        ///     Emails to all users that are waiting on status updates.
        /// </summary>
        public ICollection<string> Emails { get; set; }

        /// <summary>
        ///     Items
        /// </summary>
        public IReadOnlyList<GetIncidentFeedbackResultItem> Items { get; private set; }
    }
}