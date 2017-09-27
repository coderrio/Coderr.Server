using System.Collections.Generic;
using System.Threading.Tasks;
using Griffin.Container;
using Griffin.Data;
using codeRR.App.Modules.Tagging;
using codeRR.App.Modules.Tagging.Domain;

namespace codeRR.SqlServer.Modules.Tagging
{
    [Component]
    public class TagsRepository : ITagsRepository
    {
        private readonly IAdoNetUnitOfWork _adoNetUnitOfWork;

        public TagsRepository(IAdoNetUnitOfWork adoNetUnitOfWork)
        {
            _adoNetUnitOfWork = adoNetUnitOfWork;
        }

        public async Task AddAsync(int incidentId, Tag[] tags)
        {
            foreach (var tag in tags)
            {
                using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
                {
                    cmd.CommandText =
                        "INSERT INTO IncidentTags (IncidentId, TagName, OrderNumber) VALUES(@incidentId, @name, @orderNumber)";
                    cmd.AddParameter("incidentId", incidentId);
                    cmd.AddParameter("name", tag.Name);
                    cmd.AddParameter("orderNumber", tag.OrderNumber);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<IReadOnlyList<Tag>> GetTagsAsync(int incidentId)
        {
            using (var cmd = _adoNetUnitOfWork.CreateDbCommand())
            {
                cmd.CommandText = "SELECT * FROM IncidentTags WHERE IncidentId = @id ORDER BY OrderNumber";
                cmd.AddParameter("id", incidentId);
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var tags = new List<Tag>();
                    while (await reader.ReadAsync())
                    {
                        var tag = new Tag((string) reader["TagName"], (int) reader["OrderNumber"]);
                        tags.Add(tag);
                    }
                    return tags;
                }
            }
        }
    }
}