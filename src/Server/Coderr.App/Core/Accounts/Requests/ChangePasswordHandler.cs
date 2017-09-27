using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using codeRR.Api.Core.Accounts.Requests;

namespace codeRR.App.Core.Accounts.Requests
{
    /// <summary>
    ///     Handler for <see cref="ChangePassword" />.
    /// </summary>
    [Component]
    public class ChangePasswordHandler : IRequestHandler<ChangePassword, ChangePasswordReply>
    {
        private readonly IAccountRepository _repository;

        /// <summary>
        ///     Create a new instance of <see cref="ChangePasswordHandler" />.
        /// </summary>
        /// <param name="repository">Used to load/update the account.</param>
        public ChangePasswordHandler(IAccountRepository repository)
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
        public async Task<ChangePasswordReply> ExecuteAsync(ChangePassword request)
        {
            if (request == null) throw new ArgumentNullException("request");

            var user = await _repository.GetByIdAsync(request.UserId);
            if (!user.ValidatePassword(request.CurrentPassword))
                return new ChangePasswordReply {Success = false};

            user.ChangePassword(request.NewPassword);
            await _repository.UpdateAsync(user);
            return new ChangePasswordReply {Success = true};
        }
    }
}