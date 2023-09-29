using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("Coderr.Server.App.Tests")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "*")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "Coderr.App.Modules.Tagging.Handlers.GetTagsForApplicationHandler.#.ctor(Coderr.App.Modules.Tagging.ITagsRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "Coderr.App.Core.Applications.QueryHandlers.GetApplicationTeamHandler.#.ctor(Coderr.App.Core.Applications.IApplicationRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Accounts.Account.#Id")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "Coderr.App.Core.Feedback.EventSubscribers.AttachFeedbackToIncident.#.ctor(Coderr.App.Core.Feedback.IFeedbackRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "Coderr.App.Core.Applications.CommandHandlers.CreateApplicationHandler.#.ctor(Coderr.App.Core.Applications.IApplicationRepository,Coderr.App.Core.Users.IUserRepository,DotNetCqs.IMessageBus)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "Coderr.App.Core.Applications.EventHandlers.CreateDefaultAppOnAccountActivated.#.ctor(DotNetCqs.IMessageBus)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "Coderr.App.Core.Users.EventHandlers.CreateOnNewAccount.#.ctor(Coderr.App.Core.Users.IUserRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Modules.Messaging.Templating.DateFormatter.#FormatAgo(System.DateTime)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "Coderr.App.Core.Users.WebApi.GetUserSettingsHandler.#.ctor(Coderr.App.Core.Notifications.INotificationsRepository,Coderr.App.Core.Users.IUserRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Incidents.Incident.#ApplicationId")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Incidents.Incident.#CreatedAtUtc")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Feedback.InvalidErrorReport.#Id")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Reports.Invalid.InvalidErrorReport.#Id")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "Coderr.App.Core.Invitations.CommandHandlers.InviteUserHandler.#.ctor(Coderr.App.Core.Invitations.Data.IInvitationRepository,DotNetCqs.IMessageBus,Coderr.App.Core.Users.IUserRepository,Coderr.App.Core.Applications.IApplicationRepository,DotNetCqs.IMessageBus)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Modules.Triggers.Domain.Actions.SendEmailTask.#.ctor(DotNetCqs.IMessageBus)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "Coderr.App.Core.Applications.EventHandlers.UpdateTeamOnInvitationAccepted.#.ctor(Coderr.App.Core.Applications.IApplicationRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Configuration.ErrorTracking")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Configuration.Messaging")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Accounts")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Accounts.CommandHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Accounts.Queries")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Applications")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Applications.QueryHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Feedback")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Feedback.EventSubscribers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Incidents")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Incidents.Commands")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Invitations")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Invitations.Data")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Invitations.EventHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Notifications")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Notifications.EventHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Notifications.Tasks")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Reports")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Reports.Invalid")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Reports.Queries")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Users")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Core.Users.WebApi")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Geolocation")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Geolocation.EventHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Messaging")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Messaging.Commands")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.ReportSpikes")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Similarities.Domain.Adapters.Normalizers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Similarities.Domain.Adapters.OperatingSystems")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Similarities.Domain.Adapters.Runner")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Similarities.EventHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Tagging.Domain")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Tagging.Handlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Triggers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Triggers.Commands")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Triggers.Domain.Actions.Tools")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Triggers.Domain.Rules")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Triggers.EventHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Triggers.Queries")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Re", Scope = "member",
        Target = "Coderr.App.Modules.Triggers.Domain.Trigger.#RunForReOpenedIncidents")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Re", Scope = "member",
        Target = "Coderr.App.Core.Notifications.UserNotificationSettings.#ReOpenedIncident")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ROLE", Scope = "member",
        Target = "Coderr.App.Core.Applications.Application.#ROLE_ADMIN")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ROLE", Scope = "member",
        Target = "Coderr.App.Core.Applications.Application.#ROLE_MEMBER")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SEQUENCE",
        Scope = "member", Target = "Coderr.App.Core.Accounts.Account.#SEQUENCE")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api",
        Scope = "namespace", Target = "Coderr.App.Core.Users.WebApi")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Username",
        Scope = "member",
        Target = "Coderr.App.Core.Accounts.IAccountRepository.#FindByUsernameAsync(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FilterCondition",
        Scope = "type", Target = "Coderr.App.Modules.Triggers.Domain.FilterCondition")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FilterCondition",
        Scope = "member",
        Target =
            "Coderr.App.Modules.Triggers.Queries.DomainToDtoConverters.#ConvertFilterCondition(Coderr.App.Modules.Triggers.Domain.FilterCondition)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "Coderr.App.Core.Invitations.CommandHandlers.AcceptInvitationHandler.#.ctor(Coderr.App.Core.Invitations.Data.IInvitationRepository,Coderr.App.Core.Accounts.IAccountRepository,DotNetCqs.IMessageBus)"
        )]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Scope = "member",
        Target =
            "Coderr.App.Modules.Similarities.Domain.Adapters.OperatingSystemAdapter.#Adapt(Coderr.App.Modules.Similarities.Domain.Adapters.Runner.ValueAdapterContext,System.Object)"
        )]
[assembly: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "Coderr.App.Modules.Tagging")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Feedback.FeedbackEntity.#CreatedAtUtc")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Feedback.FeedbackEntity.#Description")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Feedback.FeedbackEntity.#EmailAddress")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Feedback.FeedbackEntity.#ErrorId")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Incidents.Incident.#Id")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "Coderr.App.Core.Invitations.Invitation.#Id")]
