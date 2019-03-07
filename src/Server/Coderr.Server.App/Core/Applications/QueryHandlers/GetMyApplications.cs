using System;
using System.Linq;
using System.Threading.Tasks;
using Coderr.Server.Api.Core.Applications;
using Coderr.Server.Api.Core.Applications.Queries;
using Coderr.Server.Domain.Core.Account;
using Coderr.Server.Domain.Core.Applications;
using DotNetCqs;


namespace Coderr.Server.App.Core.Applications.QueryHandlers
{
    /// <summary>
    ///     Handler for <see cref="GetApplicationInfo" />.
    /// </summary>
    public class GetApplicationListHandler : IQueryHandler<GetApplicationList, ApplicationListItem[]>
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="GetApplicationInfoHandler" />.
        /// </summary>
        /// <param name="applicationRepository">repos</param>
        /// <param name="accountRepository">used to check if the user is sysadmin (can get all applications)</param>
        public GetApplicationListHandler(IApplicationRepository applicationRepository,
            IAccountRepository accountRepository)
        {
            if (applicationRepository == null) throw new ArgumentNullException("applicationRepository");
            _applicationRepository = applicationRepository;
            _accountRepository = accountRepository;
        }

        /// <summary>
        ///     Method used to execute the query
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <returns>
        ///     Task which will contain the result once completed.
        /// </returns>
        public async Task<ApplicationListItem[]> HandleAsync(IMessageContext context, GetApplicationList query)
        {
            if (query == null) throw new ArgumentNullException("query");
            ApplicationListItem[] result;

            var isSysAdmin = false;
            if (query.AccountId > 0)
            {
                var account = await _accountRepository.GetByIdAsync((int)query.AccountId);
                if (account.IsSysAdmin)
                {
                    query.AccountId = 0;
                    isSysAdmin = true;
                }

            }

            if (query.AccountId != 0)
            {
                var apps = await _applicationRepository.GetForUserAsync(query.AccountId);
                result = (
                    from x in apps
                    select new ApplicationListItem(x.ApplicationId, x.ApplicationName)
                    {
                        IsAdmin = x.IsAdmin,
                        NumberOfDevelopers = x.NumberOfDevelopers
                    }
                ).ToArray();
            }
            else
                result = (await _applicationRepository.GetAllAsync())
                    .Select(x => new ApplicationListItem(x.Id, x.Name) { IsAdmin = isSysAdmin })
                    .ToArray();

            return result;
        }
    }
}