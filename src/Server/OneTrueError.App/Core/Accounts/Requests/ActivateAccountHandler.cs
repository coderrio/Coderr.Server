using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Accounts.Events;
using OneTrueError.Api.Core.Accounts.Requests;
using OneTrueError.Api.Core.Applications.Queries;
using OneTrueError.Infrastructure.Security;

namespace OneTrueError.App.Core.Accounts.Requests
{
    /// <summary>
    ///     Handler for <see cref="ActivateAccount" />.
    /// </summary>
    [Component]
    public class ActivateAccountHandler : IRequestHandler<ActivateAccount, ActivateAccountReply>
    {
        private readonly IEventBus _eventBus;
        private readonly IQueryBus _queryBus;
        private readonly IAccountRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="ActivateAccountHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <param name="eventBus">used to publish <see cref="AccountActivated" />.</param>
        /// <param name="queryBus"> </param>
        public ActivateAccountHandler(IAccountRepository repository, IEventBus eventBus, IQueryBus queryBus)
        {
            _repository = repository;
            _eventBus = eventBus;
            _queryBus = queryBus;
        }

        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="request">Request to execute</param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        public async Task<ActivateAccountReply> ExecuteAsync(ActivateAccount request)
        {
            var account = await _repository.FindByActivationKeyAsync(request.ActivationKey);
            if (account == null)
                throw new ArgumentOutOfRangeException("ActivationKey", request.ActivationKey,
                    "Key was not found.");

            account.Activate();
            await _repository.UpdateAsync(account);

            var query = new GetApplicationList();
            var apps = await _queryBus.QueryAsync(query);
            var claims =
                apps.Select(x => new Claim(OneTrueClaims.Application, x.Id.ToString(), ClaimValueTypes.Integer32))
                    .ToArray();

            if (ClaimsPrincipal.Current.IsAccount(account.Id))
            {
                var context = new PrincipalFactoryContext(account.Id, account.UserName, new string[0]) {Claims = claims};
                var identity = await PrincipalFactory.CreateAsync(context);
                identity.AddUpdateCredentialClaim();
                Thread.CurrentPrincipal = identity;
            }

            var evt = new AccountActivated(account.Id, account.UserName)
            {
                EmailAddress = account.Email
            };
            await _eventBus.PublishAsync(evt);

            return new ActivateAccountReply(account.Id, account.UserName);
        }
    }
}