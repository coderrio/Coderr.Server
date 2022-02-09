using System.Threading.Tasks;
using Coderr.Server.Abstractions.Boot;
using Coderr.Server.ReportAnalyzer.Inbound;
using Griffin.Data;

namespace Coderr.Server.SqlServer.ReportAnalyzer.Environments
{
    /// <summary>
    ///     Checks if the specified environment is configured to delete incidents.
    /// </summary>
    [ContainerService]
    internal class EnvironmentFilter : IReportFilter
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public EnvironmentFilter(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<FilterResult> Filter(FilterContext context)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = @"SELECT DeleteIncidents 
                                    FROM ApplicationEnvironments
                                    JOIN Environments ON (EnvironmentId=Environments.Id)
                                    WHERE Name = @name";
                cmd.AddParameter("name", context.ErrorReport.EnvironmentName);
                var deleteIncidents = await cmd.ExecuteScalarAsync();
                return deleteIncidents?.Equals(true) == true ? FilterResult.DiscardReport : FilterResult.FullAnalyzis;
            }
        }
    }
}