using Coderr.Server.Api;
using DotNetCqs;

namespace Coderr.Server.Common.AzureDevOps.Api.Connection.Queries
{
    /// <summary>
    /// We need to get the type of work item that should be created for our incidents. Typically "Bug" or "Issue".
    /// </summary>
    [Message]
    public class GetWorkItemTypes : Query<GetWorkItemTypesResult>
    {
        /// <summary>
        /// Token used to authenticate.
        /// </summary>
        /// <remarks>
        ///<para>
        ///Should have "Read &amp; Write" access for "Work items".
        /// </para>
        /// </remarks>
        public string PersonalAccessToken { get; set; }

        /// <summary>
        /// Url to azure dev ops (including organization)
        /// </summary>
        /// <remarks>
        ///<para>
        ///For Azure (cloud) the url should be <c>https://dev.azure.com/yourOrganizationName</c>
        /// </para>
        /// </remarks>
        public string Url { get; set; }

        /// <summary>
        /// Name or id of the project that we should connect to.
        /// </summary>
        public string ProjectNameOrId { get; set; }
    }
}
