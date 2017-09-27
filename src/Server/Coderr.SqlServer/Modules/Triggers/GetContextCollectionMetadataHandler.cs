﻿using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using Griffin.Data;
using Griffin.Data.Mapper;
using codeRR.Api.Modules.Triggers.Queries;

namespace codeRR.SqlServer.Modules.Triggers
{
    [Component]
    internal class GetContextCollectionMetadataHandler :
        IQueryHandler<GetContextCollectionMetadata, GetContextCollectionMetadataItem[]>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;

        public GetContextCollectionMetadataHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GetContextCollectionMetadataItem[]> ExecuteAsync(GetContextCollectionMetadata query)
        {
            using (var cmd = (DbCommand) _unitOfWork.CreateCommand())
            {
                cmd.CommandText =
                    "SELECT * FROM CollectionMetadata WHERE ApplicationId = @id";

                cmd.AddParameter("id", query.ApplicationId);
                var items = await cmd.ToListAsync(new CollectionMetadataMapper());
                return items.Select(x => new GetContextCollectionMetadataItem
                {
                    Name = x.Name,
                    Properties = x.Properties.ToArray()
                }).ToArray();
            }
        }
    }
}