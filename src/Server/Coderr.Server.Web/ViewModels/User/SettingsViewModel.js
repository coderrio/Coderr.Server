/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
var codeRR;
(function (codeRR) {
    var User;
    (function (User) {
        var cqs = Griffin.Cqs.CqsClient;
        var CqsClient = Griffin.Cqs.CqsClient;
        var SettingsViewModel = /** @class */ (function () {
            function SettingsViewModel() {
            }
            SettingsViewModel.prototype.changePassword_click = function (e) {
                var _this = this;
                e.preventDefault();
                var dto = this.context.readForm("PasswordView");
                if (dto.NewPassword !== dto.NewPassword2) {
                    humane.error("New passwords do not match.");
                    return;
                }
                if (!dto.CurrentPassword) {
                    humane.error("You must enter the current password.");
                    return;
                }
                var cmd = new codeRR.Core.Accounts.Requests.ChangePassword(dto.CurrentPassword, dto.NewPassword);
                CqsClient.command(cmd)
                    .done(function (result) {
                    _this.context.render({ NewPassword: "", NewPassword2: "", CurrentPassword: "" });
                    humane.log("Password have been changed.");
                });
            };
            SettingsViewModel.prototype.saveSettings_click = function (e) {
                e.isHandled = true;
                var dto = this.context.readForm("PersonalSettings");
                var cmd = new codeRR.Core.Users.Commands.UpdatePersonalSettings();
                cmd.FirstName = dto.FirstName;
                cmd.LastName = dto.LastName;
                cmd.MobileNumber = dto.MobileNumber;
                cmd.EmailAddress = dto.EmailAddress;
                CqsClient.command(cmd);
                humane.log("Settings have been saved.");
            };
            SettingsViewModel.prototype.getTitle = function () { return "Personal settings"; };
            SettingsViewModel.prototype.activate = function (context) {
                var _this = this;
                codeRR.Applications.Navigation.breadcrumbs([{ href: "/settings/personal", title: "Account settings" }]);
                codeRR.Applications.Navigation.pageTitle = 'Account settings';
                this.context = context;
                context.handle.click("[name=\"saveSettings\"]", function (ev) { return _this.saveSettings_click(ev); });
                context.handle.click("[name='changePassword']", function (ev) { return _this.changePassword_click(ev); });
                var query = new codeRR.Core.Users.Queries.GetUserSettings();
                cqs.query(query)
                    .done(function (result) {
                    context.render(result);
                    context.resolve();
                });
            };
            SettingsViewModel.prototype.deactivate = function () {
            };
            return SettingsViewModel;
        }());
        User.SettingsViewModel = SettingsViewModel;
    })(User = codeRR.User || (codeRR.User = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=SettingsViewModel.js.map