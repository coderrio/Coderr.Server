using System.Threading.Tasks;
using Coderr.Client.Contracts;
using Coderr.Server.Domain.Core.ErrorReports;
using Coderr.Server.ReportAnalyzer.Incidents;

namespace Coderr.Server.ReportAnalyzer.Inbound
{
    public interface IFilterService
    {
        Task<FilterResult> CanProcess(ErrorReportEntity report, IncidentBeingAnalyzed incident);
    }
}