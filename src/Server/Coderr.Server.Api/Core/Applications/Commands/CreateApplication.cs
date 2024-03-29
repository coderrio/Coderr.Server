﻿using System;

namespace Coderr.Server.Api.Core.Applications.Commands
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

        /// <summary>
        /// Estimated number of errors
        /// </summary>
        public int? NumberOfErrors { get; set; }

        /// <summary>
        /// Number of developers that work full time with this application.
        /// </summary>
        public decimal? NumberOfDevelopers { get; set; }

        /// <summary>
        /// Number of days to keep new incidents.
        /// </summary>
        public int? RetentionDays { get; set; }

        /// <summary>
        /// Application group that this application should be part of.
        /// </summary>
        public int? GroupId { get; set; }
    }
}