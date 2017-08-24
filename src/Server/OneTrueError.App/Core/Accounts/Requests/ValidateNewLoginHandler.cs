using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Accounts.Requests;

namespace OneTrueError.App.Core.Accounts.Requests
{
    /// <summary>
    ///     Handler for <see cref="ValidateNewLogin" />.
    /// </summary>
    [Component]
    public class ValidateNewLoginHandler : IRequestHandler<ValidateNewLogin, ValidateNewLoginReply>
    {
        private readonly IAccountRepository _repository;

        /// <summary>
        ///     Creates a new instance of <see cref="ValidateNewLoginHandler" />.
        /// </summary>
        /// <param name="repository">repos</param>
        /// <exception cref="ArgumentNullException">repository</exception>
        public ValidateNewLoginHandler(IAccountRepository repository)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            _repository = repository;
        }

        /// <summary>
        ///     Execute the request and generate a reply.
        /// </summary>
        /// <param name="request">Request to execute</param>
        /// <returns>
        ///     Task which will contain the reply once completed.
        /// </returns>
        public async Task<ValidateNewLoginReply> ExecuteAsync(ValidateNewLogin request)
        {
            var reply = new ValidateNewLoginReply();
            if (!string.IsNullOrEmpty(request.Email))
                reply.EmailIsTaken = await _repository.IsEmailAddressTakenAsync(request.Email);

            if (!string.IsNullOrEmpty(request.UserName))
                reply.UserNameIsTaken = await _repository.FindByUserNameAsync(request.UserName) != null;

            return reply;
        }
    }
}