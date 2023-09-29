using System.Collections.Generic;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Queries;
using DotNetCqs;
using Griffin.Data;

namespace Coderr.Server.SqlServer.Core.Applications.Queries
{
    internal class GetApplicationGroupsHandler : IQueryHandler<GetApplicationGroups, GetApplicationGroupsResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetApplicationGroupsHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetApplicationGroupsResult> HandleAsync(IMessageContext context, GetApplicationGroups query)
        {
            using (var cmd = _unitOfWork.CreateDbCommand())
            {
                cmd.CommandText = "SELECT Id,Name FROM ApplicationGroups";
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    var items = new List<GetApplicationGroupsResultItem>();
                    while (await reader.ReadAsync())
                    {
                        var entry = new GetApplicationGroupsResultItem
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1)
                        };
                        items.Add(entry);
                    }

                    return new GetApplicationGroupsResult
                    {
                        Items = items.ToArray()
                    };
                }
            }
        }
    }
}