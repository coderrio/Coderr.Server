using System;
using System.Collections.Generic;
using System.Text;

namespace Coderr.Server.Api.Core.Incidents.Commands
{
    /// <summary>
    /// Notify all users that have subscribed on an incident.
    /// </summary>
    [Command]
    public class NotifySubscribers
    {
        public NotifySubscribers(int incidentId)
        {
            if (incidentId <= 0) throw new ArgumentOutOfRangeException(nameof(incidentId));
            IncidentId = incidentId;
        }

        protected NotifySubscribers()
        {

        }
           
        /// <summary>
        /// Incident id
        /// </summary>
        public int IncidentId { get; private set; }
        /// <summary>
        ///     Text to send as email body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        ///     Title of outbound notification.
        /// </summary>
        public string Title { get; set; }

    }
}
