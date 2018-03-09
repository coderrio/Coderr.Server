using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using codeRR.Server.ReportAnalyzer;
using codeRR.Server.ReportAnalyzer.Domain.Reports;

namespace codeRR.Server.SqlServer.Analysis.Jobs
{
    internal class Importer
    {
        private readonly AnalysisDbContext _dbContext;
        private readonly DataTable _dataTable = new DataTable();

        public Importer(AnalysisDbContext dbContext)
        {
            _dataTable.Columns.Add("ReportId", typeof(int));
            _dataTable.Columns.Add("Name");
            _dataTable.Columns.Add("PropertyName");
            _dataTable.Columns.Add("Value");
            _dbContext = dbContext;
        }

        public void AddContextCollections(int reportId, ErrorReportContext[] contexts)
        {
            foreach (var context in contexts)
            {
                foreach (var property in context.Properties)
                {
                    if (property.Value == null)
                        continue;
                    var row = CreateDataTableRow(_dataTable, reportId, context, property);
                    _dataTable.Rows.Add(row);
                }
            }
        }

        public async Task Execute()
        {
            //TODO: Remove once all processing is in a seperate library.
            var uow = (OurUnitOfWork) _dbContext.UnitOfWork;
            using (var bulkCopy = new SqlBulkCopy((SqlConnection)_dbContext.Connection, SqlBulkCopyOptions.Default, (SqlTransaction)uow.Transaction))
            {
                bulkCopy.DestinationTableName = "ErrorReportCollectionProperties";
                bulkCopy.ColumnMappings.Add("ReportId", "ReportId");
                bulkCopy.ColumnMappings.Add("Name", "Name");
                bulkCopy.ColumnMappings.Add("PropertyName", "PropertyName");
                bulkCopy.ColumnMappings.Add("Value", "Value");
                await bulkCopy.WriteToServerAsync(_dataTable);
            }
        }

        private static DataRow CreateDataTableRow(DataTable dataTable, int reportId,
            ErrorReportContext context,
            KeyValuePair<string, string> property)
        {
            var contextName = context.Name.Length > 50
                ? context.Name.Substring(0, 47) + "..."
                : context.Name;
            var propertyName = property.Key.Length > 50
                ? property.Key.Substring(0, 47) + "..."
                : property.Key;

            var row = dataTable.NewRow();
            row["ReportId"] = reportId;
            row["Name"] = contextName;
            row["PropertyName"] = propertyName;
            row["Value"] = property.Value;
            return row;
        }

        public void Clear()
        {
            _dataTable.Clear();
        }
    }
}