//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Linq;
//using System.Threading.Tasks;
//using Griffin.Container;
//using Griffin.Data;
//using Griffin.Data.Mapper;
//using codeRR.Core;
//using codeRR.ReportsAnalytics.Reports;
//using codeRR.UnitOfWork;

//namespace codeRR.ReportAnalytics.Data.SqlServer
//{

//    //    public interface IReportsRepository
//    //    {
//    //        Task Create(InvalidErrorReport invalidReport);
//    //        /// <summary>
//    //        ///     Customer specific id.
//    //        /// </summary>
//    //        /// <param name="errorId"></param>
//    //        /// <returns></returns>
//    //        //Task<ErrorReportEntity> FindByErrorIdAsync(string errorId);
//    //        Task<PagedReports> GetForIncidentAsync(int incidentId, int pageNumber, int pageSize);

//    //        /// <summary>
//    //        /// Finds the by error identifier asynchronous.
//    //        /// </summary>
//    //        /// <param name="errorId">Customer generated id (from the client library).</param>
//    //        /// <returns></returns>
//    //        Task<ReportDTO> FindByErrorIdAsync(string errorId);
//    //    }


//    [Component]
//    internal class ErrorReportRepository// : IReportsRepository
//    {
//        private IAdoNetUnitOfWork _uow;

//        public ErrorReportRepository(CustomerUnitOfWork uow)
//        {
//            if (uow == null) throw new ArgumentNullException("uow");

//            _uow = uow;
//        }

//        public void Create(ErrorReportEntity entity)
//        {
//            using (var cmd = _uow.CreateCommand())
//            {
//                cmd.CommandText = "INSERT INTO ErrorReports (Id, ErrorId, ApplicationId, Title, Exception, ReportHashCode, HashCodeIdentifier, IncidentId, CreatedAtUtc, ContextInfo)"
//                                  +
//                                  " VALUES (@Id, @ErrorId, @ApplicationId, @Title, @Exception, @ReportHashCode, @HashCodeIdentifier, @IncidentId, @CreatedAtUtc, @ContextInfo)";

//                var ex = JsonSerializer.Serialize(entity.Exception);
//                var contexts = JsonSerializer.Serialize(entity.ContextInfo);
//                cmd.AddParameter("Id", entity.Id);
//                cmd.AddParameter("ErrorId", entity.ClientReportId);
//                cmd.AddParameter("ApplicationId", entity.ApplicationId);
//                cmd.AddParameter("Exception", ex);
//                cmd.AddParameter("ReportHashCode", entity.ReportHashCode);
//                cmd.AddParameter("HashCodeIdentifier", entity.HashCodeIdentifier);
//                cmd.AddParameter("IncidentId", entity.IncidentId);
//                cmd.AddParameter("CreatedAtUtc", entity.CreatedAtUtc);
//                cmd.AddParameter("ContextInfo", contexts);

//                if (entity.Exception != null)
//                {
//                    var pos = entity.Exception.Message.IndexOfAny(new[] { '\r', '\n' });
//                    cmd.AddParameter("Title", pos == -1
//                        ? entity.Exception.Message
//                        : entity.Exception.Message.Substring(0, pos));
//                }
//                else
//                    cmd.AddParameter("Title", "");

//                cmd.ExecuteNonQuery();
//            }
//        }


//        public void Update(ErrorReportEntity entity)
//        {
//            var ex = JsonSerializer.Serialize(entity.Exception);
//            var contexts = JsonSerializer.Serialize(entity.ContextInfo);


//            using (var cmd = _uow.CreateCommand())
//            {
//                cmd.CommandText = @"UPDATE ErrorReports SET ErrorId = @ErrorId,
//ApplicationId = @ApplicationId,
//Exception = @Exception,
//ReportHashCode = @ReportHashCode,
//HashCodeIdentifier = @HashCodeIdentifier,
//IncidentId = @incidentId,
//CreatedAtUtc = @CreatedAtUtc,
//ContextInfo = @ContextInfo
//WHERE Id = @id";

//                cmd.AddParameter("ErrorId", entity.ClientReportId);
//                cmd.AddParameter("ApplicationId", entity.ApplicationId);
//                cmd.AddParameter("Exception", ex);
//                cmd.AddParameter("ReportHashCode", entity.ReportHashCode);
//                cmd.AddParameter("HashCodeIdentifier", entity.HashCodeIdentifier);
//                cmd.AddParameter("IncidentId", entity.IncidentId);
//                cmd.AddParameter("CreatedAtUtc", entity.CreatedAtUtc);
//                cmd.AddParameter("ContextInfo", contexts);
//                cmd.AddParameter("Id", entity.Id);
//                cmd.ExecuteNonQuery();
//            }
//        }


//        //public async Task Create(InvalidErrorReport entity)
//        //{
//        //    using (var cmd = (DbCommand)_uow.CreateCommand())
//        //    {
//        //        cmd.CommandText =
//        //            @"INSERT INTO InvalidErrorReports (Id, AddedAtUtc, ApplicationId, Body, Exception) VALUES(@Id, @AddedAtUtc, @OrganizationId, @ApplicationId, @Body, @Exception)";
//        //        cmd.AddParameter("Id", entity.Id);
//        //        cmd.AddParameter("AddedAtUtc", entity.AddedAtUtc);
//        //        cmd.AddParameter("ApplicationId", entity.ApplicationId);
//        //        cmd.AddParameter("Body", entity.Report);
//        //        cmd.AddParameter("Exception", entity.Exception);
//        //        await cmd.ExecuteNonQueryAsync();
//        //    }
//        //}


//        //public async Task<ReportDTO> FindByErrorIdAsync(string errorId)
//        //{
//        //    using (var cmd = (DbCommand)_uow.CreateCommand())
//        //    {
//        //        cmd.CommandText =
//        //            "SELECT * FROM ErrorReports WHERE ErrorId = @id";

//        //        cmd.AddParameter("id", errorId);
//        //        return await cmd.FirstOrDefaultAsync<ReportDTO>();
//        //    }
//        //}

//        //public async Task<PagedReports> GetForIncidentAsync(int incidentId, int pageNumber, int pageSize)
//        //{
//        //    using (var cmd = (DbCommand)_uow.CreateCommand())
//        //    {
//        //        cmd.AddParameter("incidentId", incidentId);
//        //        long totalRows = 0;
//        //        if (pageNumber > 0)
//        //        {
//        //            cmd.CommandText =
//        //"SELECT count(*) FROM ErrorReports WHERE IncidentId = @incidentId";
//        //            totalRows = (int)await cmd.ExecuteScalarAsync();
//        //        }

//        //        cmd.CommandText =
//        //            "SELECT * FROM ErrorReports WHERE IncidentId = @incidentId ORDER BY CreatedAtUtc DESC";
//        //        if (pageNumber > 0)
//        //        {
//        //            var offset = (pageNumber - 1)*pageSize;
//        //            cmd.CommandText += string.Format(@" OFFSET {0} ROWS FETCH NEXT {1} ROWS ONLY", offset, pageSize);
//        //        }

//        //        //cmd.AddParameter("incidentId", incidentId);
//        //        var list = await cmd.ToListAsync<ReportDTO>();
//        //        return new PagedReports()
//        //        {
//        //            TotalCount = (int) totalRows,
//        //            Reports = (IReadOnlyList<ReportDTO>)list
//        //        };
//        //    }
//        //}

//        public async Task<IEnumerable<ErrorReportEntity>> GetForIncidentAsync(int incidentId)
//        {
//            using (var cmd = (DbCommand)_uow.CreateCommand())
//            {
//                cmd.CommandText = "SELECT * FROM ErrorReports WHERE IncidentId = @id";
//                cmd.AddParameter("id", incidentId);
//                return await cmd.ToListAsync<ErrorReportEntity>();
//            }
//        }
//    }
//}

