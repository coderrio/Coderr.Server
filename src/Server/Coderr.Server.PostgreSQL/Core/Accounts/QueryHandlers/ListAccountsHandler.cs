using System.Threading.Tasks;
using Coderr.Server.Api.Core.Accounts.Queries;
using DotNetCqs;
using Griffin.Data;
using Griffin.Data.Mapper;

namespace Coderr.Server.PostgreSQL.Core.Accounts.QueryHandlers
{
    public class ListAccountsHandler : IQueryHandler<ListAccounts, ListAccountsResult>
    {
        private readonly IAdoNetUnitOfWork _unitOfWork;
        private static readonly IEntityMapper<ListAccountsResultItem> _mapper = new MirrorMapper<ListAccountsResultItem>();

        public ListAccountsHandler(IAdoNetUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ListAccountsResult> HandleAsync(IMessageContext context, ListAccounts query)
        {
            var sql = "SELECT Id AccountId, UserName, CreatedAtUtc, Email FROM Accounts;";
            var users = await _unitOfWork.ToListAsync(_mapper, sql);
            return new ListAccountsResult() { Accounts = users.ToArray() };
        }
    }
}
