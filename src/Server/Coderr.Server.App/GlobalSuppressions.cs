using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

[assembly:InternalsVisibleTo("codeRR.Server.App.Tests")]
[assembly: SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "*")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "codeRR.App.Modules.Tagging.Handlers.GetTagsForApplicationHandler.#.ctor(codeRR.App.Modules.Tagging.ITagsRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "codeRR.App.Core.Applications.QueryHandlers.GetApplicationTeamHandler.#.ctor(codeRR.App.Core.Applications.IApplicationRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Accounts.Account.#Id")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "codeRR.App.Core.Feedback.EventSubscribers.AttachFeedbackToIncident.#.ctor(codeRR.App.Core.Feedback.IFeedbackRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "codeRR.App.Core.Applications.CommandHandlers.CreateApplicationHandler.#.ctor(codeRR.App.Core.Applications.IApplicationRepository,codeRR.App.Core.Users.IUserRepository,DotNetCqs.IMessageBus)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "codeRR.App.Core.Applications.EventHandlers.CreateDefaultAppOnAccountActivated.#.ctor(DotNetCqs.IMessageBus)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "codeRR.App.Core.Users.EventHandlers.CreateOnNewAccount.#.ctor(codeRR.App.Core.Users.IUserRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Modules.Messaging.Templating.DateFormatter.#FormatAgo(System.DateTime)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "codeRR.App.Core.Users.WebApi.GetUserSettingsHandler.#.ctor(codeRR.App.Core.Notifications.INotificationsRepository,codeRR.App.Core.Users.IUserRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Incidents.Incident.#ApplicationId")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Incidents.Incident.#CreatedAtUtc")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Feedback.InvalidErrorReport.#Id")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Reports.Invalid.InvalidErrorReport.#Id")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "codeRR.App.Core.Invitations.CommandHandlers.InviteUserHandler.#.ctor(codeRR.App.Core.Invitations.Data.IInvitationRepository,DotNetCqs.IMessageBus,codeRR.App.Core.Users.IUserRepository,codeRR.App.Core.Applications.IApplicationRepository,DotNetCqs.IMessageBus)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Modules.Triggers.Domain.Actions.SendEmailTask.#.ctor(DotNetCqs.IMessageBus)")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "codeRR.App.Core.Applications.EventHandlers.UpdateTeamOnInvitationAccepted.#.ctor(codeRR.App.Core.Applications.IApplicationRepository)"
        )]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Configuration.ErrorTracking")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Configuration.Messaging")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Accounts")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Accounts.CommandHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Accounts.Queries")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Applications")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Applications.QueryHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Feedback")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Feedback.EventSubscribers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Incidents")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Incidents.Commands")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Invitations")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Invitations.Data")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Invitations.EventHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Notifications")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Notifications.EventHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Notifications.Tasks")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Reports")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Reports.Invalid")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Reports.Queries")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Users")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Core.Users.WebApi")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Geolocation")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Geolocation.EventHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Messaging")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Messaging.Commands")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.ReportSpikes")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Similarities.Domain.Adapters.Normalizers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Similarities.Domain.Adapters.OperatingSystems")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Similarities.Domain.Adapters.Runner")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Similarities.EventHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Tagging.Domain")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Tagging.Handlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Triggers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Triggers.Commands")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Triggers.Domain.Actions.Tools")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Triggers.Domain.Rules")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Triggers.EventHandlers")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Triggers.Queries")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Re", Scope = "member",
        Target = "codeRR.App.Modules.Triggers.Domain.Trigger.#RunForReOpenedIncidents")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Re", Scope = "member",
        Target = "codeRR.App.Core.Notifications.UserNotificationSettings.#ReOpenedIncident")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ROLE", Scope = "member",
        Target = "codeRR.App.Core.Applications.Application.#ROLE_ADMIN")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "ROLE", Scope = "member",
        Target = "codeRR.App.Core.Applications.Application.#ROLE_MEMBER")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "SEQUENCE",
        Scope = "member", Target = "codeRR.App.Core.Accounts.Account.#SEQUENCE")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Api",
        Scope = "namespace", Target = "codeRR.App.Core.Users.WebApi")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "Username",
        Scope = "member",
        Target = "codeRR.App.Core.Accounts.IAccountRepository.#FindByUsernameAsync(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FilterCondition",
        Scope = "type", Target = "codeRR.App.Modules.Triggers.Domain.FilterCondition")]
[assembly:
    SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "FilterCondition",
        Scope = "member",
        Target =
            "codeRR.App.Modules.Triggers.Queries.DomainToDtoConverters.#ConvertFilterCondition(codeRR.App.Modules.Triggers.Domain.FilterCondition)"
        )]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target =
            "codeRR.App.Core.Invitations.CommandHandlers.AcceptInvitationHandler.#.ctor(codeRR.App.Core.Invitations.Data.IInvitationRepository,codeRR.App.Core.Accounts.IAccountRepository,DotNetCqs.IMessageBus)"
        )]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Scope = "member",
        Target =
            "codeRR.App.Modules.Similarities.Domain.Adapters.OperatingSystemAdapter.#Adapt(codeRR.App.Modules.Similarities.Domain.Adapters.Runner.ValueAdapterContext,System.Object)"
        )]
[assembly: SuppressMessage("Microsoft.Design", "CA1014:MarkAssembliesWithClsCompliant")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace",
        Target = "codeRR.App.Modules.Tagging")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Feedback.FeedbackEntity.#CreatedAtUtc")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Feedback.FeedbackEntity.#Description")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Feedback.FeedbackEntity.#EmailAddress")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Feedback.FeedbackEntity.#ErrorId")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Incidents.Incident.#Id")]
[assembly:
    SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member",
        Target = "codeRR.App.Core.Invitations.Invitation.#Id")]