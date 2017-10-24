//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using DotNetCqs;
//using Griffin.Container;
//using log4net;
//using codeRR.Core.Api.Reports;
//using codeRR.Core.Api.Reports.Queries;
//using codeRR.Core.Reports;

//namespace codeRR.Core.ErrorReports.Queries
//{
//    [Component]
//    internal class FindReportHandler : IQueryHandler<GetReport, GetReportResult>
//    {
//        private readonly IReportsRepository _repository;
//        private ILog _logger = LogManager.GetLogger(typeof (FindReportHandler));

//        public FindReportHandler(IReportsRepository repository)
//        {
//            _repository = repository;
//        }

//        public async Task<GetReportResult> ExecuteAsync(IMessageContext context, GetReport query)
//        {
//            ErrorReportEntity entity = null;

//            if (!string.IsNullOrEmpty(query.ErrorId))
//            {
//                entity = await _repository.FindByErrorIdAsync(query.ErrorId);
//            }
//            if (entity == null && query.ReportId != 0)
//            {
//                entity = await _repository.GetAsync(query.ReportId);
//            }


//            if (entity == null)
//            {
//                _logger.ErrorFormat("Failed to find report for {0} / {1}", query.ErrorId, query.ReportId);
//                return null;
//            }


//            return new ReportDTO
//            {
//                ApplicationId = entity.ApplicationId,
//                ContextCollections = entity.ContextInfo.Select(ConvertCollection).ToArray(),
//                CreatedAtUtc = entity.CreatedAtUtc,
//                Exception = ConvertException(entity.Exception),
//                Id = entity.Id.ToString(),
//                IncidentId = entity.IncidentId,
//                RemoteAddress = entity.RemoteAddress,
//                ReportVersion = "1"
//            };
//        }

//        private NewReportException ConvertException(ErrorReportException exception)
//        {
//            return new NewReportException
//            {
//                AssemblyName = exception.AssemblyName,
//                BaseClasses = exception.BaseClasses,
//                Everything = exception.Everything,
//                FullName = exception.FullName,
//                InnerException = exception.InnerException == null ? null : ConvertException(exception.InnerException),
//                Message = exception.Message,
//                Name = exception.Name,
//                Namespace = exception.Namespace,
//                StackTrace = exception.StackTrace
//            };
//        }

//        private NewReportContextInfo ConvertCollection(ErrorReportContext arg)
//        {
//            return new NewReportContextInfo(arg.Name, arg.Properties);
//        }
//    }
//}

