using System;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Accounts.Queries;
using Coderr.Server.Domain.Core.Account;
using DotNetCqs;
using Coderr.Server.ReportAnalyzer.Abstractions;

namespace Coderr.Server.App.Core.Accounts.Queries
{
    /// <summary>
    ///     Handler for <see cref="GetAccountQueryHandler" />
    /// </summary>
    public class GetAccountQueryHandler : IQueryHandler<GetAccountById, AccountDTO>
    {
        private readonly IAccountRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="GetAccountQueryHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        public GetAccountQueryHandler(IAccountRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        /// <summary>
        ///     Method used to execute the query
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>
        ///     Task which will contain the result once completed.
        /// </returns>
        public async Task<AccountDTO> HandleAsync(IMessageContext context, GetAccountById query)
        {
            if (query == null) throw new ArgumentNullException("query");

            var account = await _repository.GetByIdAsync((int) query.AccountId);
            if (account == null)
                return null;

            var dto = new AccountDTO
            {
                CreatedAtUtc = account.CreatedAtUtc,
                Email = account.Email,
                Id = account.Id,
                LastLoginAtUtc = account.LastLoginAtUtc,
                State =
                    (Api.Core.Accounts.Queries.AccountState)
                        Enum.Parse(typeof(Api.Core.Accounts.Queries.AccountState), account.AccountState.ToString()),
                UpdatedAtUtc = account.UpdatedAtUtc,
                UserName = account.UserName
            };

            return dto;
        }
    }
}