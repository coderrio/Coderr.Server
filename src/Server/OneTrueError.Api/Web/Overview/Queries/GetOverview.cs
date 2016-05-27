using DotNetCqs;

namespace OneTrueError.Api.Web.Overview.Queries
{
    /// <summary>
    ///     Get an OneTrueError summary (typically shown in the chart and right panel summary)
    /// </summary>
    public class GetOverview : Query<GetOverviewResult>
    {
        /// <summary>
        ///     Amount of time to look back (i.e. startdate = DateTime.Now.Substract(WindowSize))
        /// </summary>
        /// <remarks>
        ///     1 = switch to hours
        /// </remarks>
        public int NumberOfDays { get; set; }
    }
}