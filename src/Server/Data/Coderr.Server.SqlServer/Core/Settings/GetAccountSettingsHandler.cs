using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Settings.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Settings
{
    class GetAccountSettingsHandler : IQueryHandler<GetAccountSettings, GetAccountSettingsResult>
    {
        private readonly IAdoNetUnitOfWork _uow;

        public GetAccountSettingsHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GetAccountSettingsResult> HandleAsync(IMessageContext context, GetAccountSettings query)
        {
            var items = await _uow.ToListAsync<AccountSetting>("AccountId = @AccountId", new { AccountId = query.AccountId });
            return new GetAccountSettingsResult { Settings = items.ToDictionary(x => x.Name, x => x.Value) };
        }
    }
}
