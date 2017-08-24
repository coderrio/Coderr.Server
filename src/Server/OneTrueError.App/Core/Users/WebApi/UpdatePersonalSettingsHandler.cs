using System;
using System.Threading.Tasks;
using DotNetCqs;
using Griffin.Container;
using OneTrueError.Api.Core.Users.Commands;

namespace OneTrueError.App.Core.Users.WebApi
{
    /// <summary>
    ///     Handler for <see cref="UpdatePersonalSettings" />.
    /// </summary>
    [Component]
    public class UpdatePersonalSettingsHandler : ICommandHandler<UpdatePersonalSettings>
    {
        private readonly IUserRepository _userRepository;

        /// <summary>
        ///     Creates a new instance of <see cref="UpdatePersonalSettingsHandler" />.
        /// </summary>
        /// <param name="userRepository">repos</param>
        /// <exception cref="ArgumentNullException">userRepository</exception>
        public UpdatePersonalSettingsHandler(IUserRepository userRepository)
        {
            if (userRepository == null) throw new ArgumentNullException("userRepository");
            _userRepository = userRepository;
        }

        /// <summary>
        ///     Execute a command asynchronously.
        /// </summary>
        /// <param name="command">Command to execute.</param>
        /// <returns>
        ///     Task which will be completed once the command has been executed.
        /// </returns>
        public async Task ExecuteAsync(UpdatePersonalSettings command)
        {
            if (command == null) throw new ArgumentNullException("command");

            var user = await _userRepository.GetUserAsync(command.UserId);
            user.FirstName = command.FirstName;
            user.LastName = command.LastName;
            user.MobileNumber = command.MobileNumber;
            await _userRepository.UpdateAsync(user);
        }
    }
}