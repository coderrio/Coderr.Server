﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace Coderr.Server.Domain.Core.Applications
{
    /// <summary>
    ///     An application which we can receive exceptions from.
    /// </summary>
    public class Application
    {
        private bool _muteStatisticsQuestion;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Application" /> class.
        /// </summary>
        /// <param name="createdById">Account id for the user that created this application.</param>
        /// <param name="name">Application name as defined by the user.</param>
        public Application(int createdById, string name)
        {
            if (createdById < 1) throw new ArgumentNullException("createdById");
            if (name == null) throw new ArgumentNullException("name");

            CreatedById = createdById;
            AppKey = Guid.NewGuid().ToString("N");
            Name = name;
            CreatedAtUtc = DateTime.UtcNow;
            SharedSecret = Guid.NewGuid().ToString("N");
        }

        /// <summary>
        ///     Serialization constructor
        /// </summary>
        protected Application()
        {
        }

        /// <summary>
        ///     Gets or sets ID used to identify this application
        /// </summary>
        /// <remarks>
        ///     The application id are used in the query string when reports are sent. It's then used to find the correct
        ///     application so that we can decrypt using the shared secret.
        /// </remarks>
        public string AppKey { get; set; }

        /// <summary>
        ///     Defines the type of application
        /// </summary>
        /// <remarks>
        ///     <para>Used to configure how the analysis should be made.</para>
        /// </remarks>
        public TypeOfApplication ApplicationType { get; set; }

        /// <summary>
        ///     When the application was created.
        /// </summary>
        public DateTime CreatedAtUtc { get; private set; }

        /// <summary>
        ///     Account id for the user that created the application.
        /// </summary>
        public int CreatedById { get; set; }

        /// <summary>
        ///     Gets db identifier used in relations.
        /// </summary>
        // ReSharper disable once UnusedAutoPropertyAccessor.Local, set by reflection
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode",
            Justification = "Set by reflection.")]
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public int Id { get; private set; }

        /// <summary>
        ///     Gets title
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the shared secret which is used to encrypt sensitive data between the reporter client and the server.
        /// </summary>
        /// <remarks>The user have to manually configure the secret.</remarks>
        public string SharedSecret { get; private set; }

        /// <summary>
        /// Entered when the user created the application
        /// </summary>
        public int? EstimatedNumberOfErrors { get; private set; }

        /// <summary>
        /// Number of full time developers
        /// </summary>
        public decimal? NumberOfFtes { get; private set; }

        /// <summary>
        /// UI group that the application should be displayed under.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// Number of days to keep incidents.
        /// </summary>
        public int RetentionDays { get; set; }

        public void AddStatsBase(decimal? fte, int? numberOfErrors)
        {
            EstimatedNumberOfErrors = numberOfErrors;
            NumberOfFtes = fte;
        }

        /// <summary>
        /// Don't ask for statistics information
        /// </summary>
        public bool MuteStatisticsQuestion
        {
            get => _muteStatisticsQuestion || NumberOfFtes > 0 || EstimatedNumberOfErrors > 0;
            set => _muteStatisticsQuestion = value;
        }
    }


}