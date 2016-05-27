/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />

module OneTrueError.User {
    import ClickEventArgs = Griffin.WebApp.ClickEventArgs;
    var cqs = Griffin.Cqs.CqsClient;
    import Yo = Griffin.Yo;
    import UserSettingsResult = OneTrueError.Core.Users.Queries.GetUserSettingsResult;
    import GetUserSettings = OneTrueError.Core.Users.Queries.GetUserSettings;
    import CqsClient = Griffin.Cqs.CqsClient;

    export class NotificationsViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private notifyNewIncident: boolean;
        private notifyNewReport: boolean;
        private notifyReOpenedIncident: boolean;
        private notifyPeaks: boolean;

        constructor() {
        }

        public saveSettings_click(e: any) {
            e.isHandled = true;
            var dto = this.ctx.readForm('NotificationsView');
            var cmd = new OneTrueError.Core.Users.Commands.UpdateNotifications();
            cmd.NotifyOnNewIncidents = dto.NotifyOnNewIncidents;
            cmd.NotifyOnNewReport = dto.NotifyOnNewReport;
            cmd.NotifyOnPeaks = dto.NotifyOnPeaks;
            cmd.NotifyOnReOpenedIncident = dto.NotifyOnReOpenedIncident;
            cmd.NotifyOnUserFeedback = dto.NotifyOnUserFeedback;
            CqsClient.command(cmd);
            humane.log("Settings have been saved.");
        }

        getTitle(): string { return "Notifications settings"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            this.ctx = context;

            context.handle.click("#saveSettings", ev => this.saveSettings_click(ev));
            var query = new GetUserSettings();
            cqs.query<UserSettingsResult>(query).done(result => {
                context.render(result.Notifications);
                context.resolve();
            });

        }

        deactivate() {
            
        }

        private ctx: Griffin.Yo.Spa.ViewModels.IActivationContext;
    }
} 