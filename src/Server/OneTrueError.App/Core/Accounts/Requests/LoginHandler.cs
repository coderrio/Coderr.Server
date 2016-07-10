using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using log4net;
using OneTrueError.Api.Core.Accounts.Events;
using OneTrueError.Api.Core.Accounts.Requests;

namespace OneTrueError.App.Core.Accounts.Requests
{
    /// <summary>
    ///     Handler for <see cref="Login" />.
    /// </summary>
    [Component(Lifetime = Lifetime.Scoped)]
    public class LoginHandler : IRequestHandler<Login, LoginReply>
    {
        private readonly IEventBus _eventBus;
        private readonly ILog _logger = LogManager.GetLogger(typeof (LoginHandler));
        private readonly IAccountRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="LoginHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <param name="eventBus">used to publish <see cref="LoginFailed" />.</param>
        /// <exception cref="ArgumentNullException">repository; eventBus</exception>
        public LoginHandler(IAccountRepository repository, IEventBus eventBus)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (eventBus == null) throw new ArgumentNullException("eventBus");
            _repository = repository;
            _eventBus = eventBus;
        }

        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="request">Request to execute</param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        public async Task<LoginReply> ExecuteAsync(Login request)
        {
            if (request == null) throw new ArgumentNullException("request");

            var account = await _repository.FindByUserNameAsync(request.UserName);

            try
            {
                if (account == null || !account.Login(request.Password))
                {
                    _logger.Debug("Logging in " + request.UserName);
                    await _eventBus.PublishAsync(new LoginFailed(request.UserName) {InvalidLogin = true});
                    if (account != null)
                        await _repository.UpdateAsync(account);
                    return new LoginReply {Result = LoginResult.IncorrectLogin};
                }
            }
            catch (AuthenticationException ex)
            {
                _logger.Debug("Logging failed for " + request.UserName, ex);
                _eventBus.PublishAsync(new LoginFailed(request.UserName) {IsLocked = true}).Start();
                return new LoginReply {Result = LoginResult.Locked};
            }

            await _repository.UpdateAsync(account);
            return new LoginReply
            {
                Result = LoginResult.Successful,
                UserName = account.UserName,
                AccountId = account.Id
            };
        }
    }
}