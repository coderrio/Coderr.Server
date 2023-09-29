using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Applications.Queries
{
    internal class GetApplicationGroupMapHandler : IQueryHandler<GetApplicationGroupMap, GetApplicationGroupMapResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetApplicationGroupMapHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetApplicationGroupMapResult> HandleAsync(IMessageContext context,
            GetApplicationGroupMap query)
        {
            if (query.ApplicationId != null)
            {
                var item = await _unitOfWork.FirstAsync<ApplicationGroupMap>(new { query.ApplicationId });
                return new GetApplicationGroupMapResult
                {
                    Items = new[]
                    {
                        new GetApplicationGroupMapResultItem
                        {
                            ApplicationId = item.ApplicationId,
                            GroupId = item.ApplicationGroupId
                        }
                    }
                };
            }

            using (var cmd = _unitOfWork.CreateCommand())
            {
                cmd.CommandText = @"declare @defaultId int;
                                    select top(1) @defaultId = id from ApplicationGroups order by id;

                                    SELECT agm.Id, a.Id ApplicationId, case when agm.ApplicationGroupId is null then @defaultId else agm.ApplicationGroupId end ApplicationGroupId
                                    FROM Applications a
                                    LEFT JOIN ApplicationGroupMap agm ON (agm.ApplicationId=a.Id)";

                var items = await cmd.ToListAsync<ApplicationGroupMap>();
                return new GetApplicationGroupMapResult
                {
                    Items = items.Select(x => new GetApplicationGroupMapResultItem
                    {
                        ApplicationId = x.ApplicationId,
                        GroupId = x.ApplicationGroupId
                    }).ToArray()
                };

            }

        }
    }
}