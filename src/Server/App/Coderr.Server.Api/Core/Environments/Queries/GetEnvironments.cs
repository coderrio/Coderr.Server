using System;
using System.Collections.Generic;
using System.Text;
using DotNetCqs;

namespace Coderr.Server.Api.Core.Environments.Queries
{
    /// <summary>
    /// Get all environments that we've received error reports in
    /// </summary>
    [Message]
    public class GetEnvironments : Query<GetEnvironmentsResult>
    {
        public GetEnvironments(int applicationId)
        {
            if (applicationId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(applicationId));
            }

            ApplicationId = applicationId;
        }

        protected GetEnvironments()
        {

        }

        /// <summary>
        /// Fetch all environments for a specific application.
        /// </summary>
        public int ApplicationId { get; private set; }
    }
}
