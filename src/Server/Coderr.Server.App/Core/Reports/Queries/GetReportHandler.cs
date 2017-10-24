using System;
using System.Linq;
using System.Threading.Tasks;
using codeRR.Server.Api.Core.Reports;
using codeRR.Server.Api.Core.Reports.Queries;
using DotNetCqs;
using Griffin.Container;

namespace codeRR.Server.App.Core.Reports.Queries
{
    /// <summary>
    ///     Get report.
    /// </summary>
    [Component]
    public class GetReportHandler : IQueryHandler<GetReport, GetReportResult>
    {
        private readonly IReportsRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="GetReportHandler" />.
        /// </summary>
        /// <param name="repository">repository</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public GetReportHandler(IReportsRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        /// <summary>Method used to execute the query</summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>Task which will contain the result once completed.</returns>
        public async Task<GetReportResult> HandleAsync(IMessageContext context, GetReport query)
        {
            var report = await _repository.GetAsync(query.ReportId);
            var collections = Enumerable.ToList((
                from x in report.ContextCollections
                where x.Properties.Count > 0
                let properties = Enumerable.Select(x.Properties, y => new KeyValuePair(y.Key, y.Value))
                select new GetReportResultContextCollection(x.Name, Enumerable.ToArray(properties))
            ));

            //TODO: Fix feedback
            //var feedbackQuery = new GetReportFeedback(query.ReportId, query.);//TODO: Fix customerId
            //var feedback = await _queryBus.QueryAsync(feedbackQuery);
            //if (feedback != null)
            //{
            //    collections.Add(new GetReportResultContextCollection("UserFeedback", new[]
            //    {
            //        new KeyValuePair("EmailAddress", feedback.EmailAddress),
            //        new KeyValuePair("Description", feedback.Description)
            //    })
            //        );
            //}

            return new GetReportResult
            {
                ContextCollections = collections.ToArray(),
                CreatedAtUtc = report.CreatedAtUtc,
                ErrorId = report.ReportId,
                Exception = ConvertException(report.Exception),
                Id = report.Id.ToString(),
                IncidentId = report.IncidentId.ToString(),
                Message = report.Exception.Message,
                StackTrace = report.Exception.StackTrace
                //UserFeedback = feedback != null ? feedback.Description : "",
                //EmailAddress = feedback != null ? feedback.EmailAddress : ""
                //                ReportHashCode = report.
            };
        }

        private GetReportException ConvertException(ReportExeptionDTO exception)
        {
            var ex = new GetReportException
            {
                AssemblyName = exception.AssemblyName,
                BaseClasses = exception.BaseClasses,
                FullName = exception.FullName,
                Everything = exception.Everything,
                Name = exception.Name,
                Namespace = exception.Namespace,
                Message = exception.Message,
                StackTrace = exception.StackTrace
            };
            if (exception.InnerException != null)
                ex.InnerException = ConvertException(exception.InnerException);
            return ex;
        }
    }
}