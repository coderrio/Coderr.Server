using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Settings.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.SqlServer.Core.Settings
{
    class GetAccountSettingHandler : IQueryHandler<GetAccountSetting, GetAccountSettingResult>
    {
        private readonly IAdoNetUnitOfWork _uow;

        public GetAccountSettingHandler(IAdoNetUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<GetAccountSettingResult> HandleAsync(IMessageContext context, GetAccountSetting query)
        {
            var setting = await _uow.FirstOrDefaultAsync<AccountSetting>("AccountId = @AccountId AND Name = @Name", new { query.AccountId, query.Name });
            return new GetAccountSettingResult { Value = setting?.Value };
        }
    }
}
