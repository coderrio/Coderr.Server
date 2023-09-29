namespace Coderr.Server.Api.Insights.Queries
{
    public class GetKeyMetricDefinitionsResultItem
    {
        /// <summary>
        ///     Value can be normalized (i.e. get average per developer).
        /// </summary>
        public bool CanBeNormalized { get; set; }

        public string Description { get; set; }
        public string DisplayName { get; set; }

        /// <summary>
        ///     To sort values, the highest have better quality.
        /// </summary>
        public bool? HigherValueIsBetter { get; set; }

        public int Id { get; set; }

        /// <summary>
        ///     The value name is the application name (otherwise the user name).
        /// </summary>
        public bool IsValueNameApplicationName { get; set; }

        public int Name { get; set; }

        /// <summary>
        ///     If the Value is an id (accountId/applicationId), this is the name of that entity.
        /// </summary>
        public string ValueName { get; set; }

        /// <summary>
        ///     Unit of value in pluralis ("user(s)")
        /// </summary>

        public string ValueUnit { get; set; }
    }
}