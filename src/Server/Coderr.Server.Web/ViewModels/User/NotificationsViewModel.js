/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
var codeRR;
(function (codeRR) {
    var User;
    (function (User) {
        var cqs = Griffin.Cqs.CqsClient;
        var GetUserSettings = codeRR.Core.Users.Queries.GetUserSettings;
        var CqsClient = Griffin.Cqs.CqsClient;
        var NotificationsViewModel = /** @class */ (function () {
            function NotificationsViewModel() {
            }
            NotificationsViewModel.prototype.saveSettings_click = function (e) {
                e.isHandled = true;
                var dto = this.ctx.readForm("NotificationsView");
                var cmd = new codeRR.Core.Users.Commands.UpdateNotifications();
                cmd.NotifyOnNewIncidents = dto.NotifyOnNewIncidents;
                cmd.NotifyOnNewReport = dto.NotifyOnNewReport;
                cmd.NotifyOnPeaks = dto.NotifyOnPeaks;
                cmd.NotifyOnReOpenedIncident = dto.NotifyOnReOpenedIncident;
                cmd.NotifyOnUserFeedback = dto.NotifyOnUserFeedback;
                CqsClient.command(cmd);
                humane.log("Settings have been saved.");
            };
            NotificationsViewModel.prototype.getTitle = function () {
                return "Notifications settings";
            };
            NotificationsViewModel.prototype.activate = function (context) {
                var _this = this;
                codeRR.Applications.Navigation.breadcrumbs([{ href: "/settings/notifications", title: "Notification settings" }]);
                codeRR.Applications.Navigation.pageTitle = 'Notification settings';
                this.ctx = context;
                context.handle.click("#saveSettings", function (ev) { return _this.saveSettings_click(ev); });
                var query = new GetUserSettings();
                cqs.query(query)
                    .done(function (result) {
                    context.render(result.Notifications);
                    context.resolve();
                });
            };
            NotificationsViewModel.prototype.deactivate = function () {
            };
            return NotificationsViewModel;
        }());
        User.NotificationsViewModel = NotificationsViewModel;
    })(User = codeRR.User || (codeRR.User = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=NotificationsViewModel.js.map