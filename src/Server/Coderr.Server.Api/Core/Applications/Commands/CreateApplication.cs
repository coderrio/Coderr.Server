using System;
using codeRR.Server.Api.Core.Accounts.Requests;
using DotNetCqs;

namespace codeRR.Server.Api.Core.Applications.Commands
{
    /// <summary>
    ///     Create a new application.
    /// </summary>
    [Message]
    public class CreateApplication
    {
        /// <summary>
        ///     Creates a new instance of <see cref="CreateApplication" />.
        /// </summary>
        /// <param name="name">Name of the application (as entered by the user)</param>
        /// <param name="typeOfApplication">Application type</param>
        public CreateApplication(string name, TypeOfApplication typeOfApplication)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (!Enum.IsDefined(typeof(TypeOfApplication), typeOfApplication))

                throw new ArgumentOutOfRangeException("typeOfApplication");
            Name = name;
            TypeOfApplication = typeOfApplication;
        }

        /// <summary>
        ///     Generated application key
        /// </summary>
        public string ApplicationKey { get; set; }

        /// <summary>
        ///     User specified name
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        ///     Application type
        /// </summary>
        public TypeOfApplication TypeOfApplication { get; set; }


        /// <summary>
        ///     Account id for the user that sent the command
        /// </summary>
        [IgnoreField]
        public int UserId { get; set; }
    }
}