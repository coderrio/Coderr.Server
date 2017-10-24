//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using DotNetCqs;
//using Griffin.ApplicationServices;
//using Griffin.Container;
//using Griffin.Data;
//using codeRR.Core.Api.Reports;
//using codeRR.Core.Api.Reports.Queries;
//using codeRR.Core.IncidentTagging.Data;
//using codeRR.Data;

//namespace codeRR.Core.IncidentTagging
//{
//    [Component(RegisterAsSelf = true)]
//    public class DoInitialRun : IBackgroundJobAsync
//    {
//        private static bool _hasRun;
//        private IAdoNetUnitOfWork _unitOfWork;
//        private IQueryBus _queryBus;
//        private ITagsRepository _repository;

//        public DoInitialRun(IAdoNetUnitOfWork unitOfWork, IQueryBus queryBus, ITagsRepository repository)
//        {
//            _unitOfWork = unitOfWork;
//            _queryBus = queryBus;
//            _repository = repository;
//        }

//        public async Task HandleAsync(IMessageContext context, )
//        {
//            if (_hasRun)
//                return;
//            _hasRun = true;

//            using (var cmd = _unitOfWork.CreateCommand())
//            {
//                cmd.CommandText = "SELECT Value FROM Settings WHERE Name = 'TagRun'";
//                if (cmd.ExecuteScalar() != null)
//                    return;
//            }

//            using (var cmd = _unitOfWork.CreateCommand())
//            {
//                cmd.CommandText = "INSERT INTO Settings (Name, Value) VALUES('TagRun', @date)";
//                cmd.AddParameter("date", DateTime.UtcNow.ToString());
//                cmd.ExecuteNonQuery();
//            }

//            using (var cmd = _unitOfWork.CreateCommand())
//            {
//                cmd.CommandText = @"WITH summary AS (
//    SELECT p.id as reportid, 
//           p.incidentid, 
//		   p.createdatutc,
//           ROW_NUMBER() OVER(PARTITION BY p.incidentid 
//                                 ORDER BY p.createdatutc asc) AS rk
//      FROM errorreports p)
//SELECT s.*
//  FROM summary s
//  left join incidenttags t on (t.incidentid=s.incidentid)
// WHERE s.rk = 1
// AND t.incidentid is null";
//                using (var reader = cmd.ExecuteReader())
//                {
//                    while (reader.Read())
//                    {
//                        var reportId = (int) reader["reportid"];
//                        var incidentId = (int)reader["incidentid"];
//                        var q = new FindReport() {ReportId = reportId};
//                        var report = await _queryBus.QueryAsync(q);

//                        var dto = new NewReportDTO
//                        {
//                            ContextCollections = report.ContextCollections,
//                            CreatedAtUtc = report.CreatedAtUtc,
//                            Exception = report.Exception,
//                            RemoteAddress = report.RemoteAddress,
//                            ReportVersion = report.ReportVersion
//                        };
//                        var ctx = new TagIdentifierContext(dto);
//                        var identiferProvider = new IdentifierProvider();
//                        var identifiers = identiferProvider.GetIdentifiers(ctx);
//                        foreach (var identifier in identifiers)
//                        {
//                            identifier.Identify(ctx);
//                        }

//                        _repository.Add(incidentId, ctx.Tags.ToArray());
//                    }
//                }
//            }
//        }
//    }
//}

