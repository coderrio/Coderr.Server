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
        /// <summary>
        /// Fetch all environments for a specific application.
        /// </summary>
        public int? ApplicationId { get; set; }
    }
}
