/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
var OneTrueError;
(function (OneTrueError) {
    var User;
    (function (User) {
        var cqs = Griffin.Cqs.CqsClient;
        var CqsClient = Griffin.Cqs.CqsClient;
        var SettingsViewModel = (function () {
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
                var cmd = new OneTrueError.Core.Accounts.Requests.ChangePassword(dto.CurrentPassword, dto.NewPassword);
                CqsClient.request(cmd)
                    .done(function (result) {
                    if (result.Success) {
                        _this.context.render({ NewPassword: "", NewPassword2: "", CurrentPassword: "" });
                        humane.log("Password have been changed.");
                    }
                    else {
                        humane.error("Password could not be changed. Did you enter your current password correctly?");
                    }
                });
            };
            SettingsViewModel.prototype.saveSettings_click = function (e) {
                e.isHandled = true;
                var dto = this.context.readForm("PersonalSettings");
                var cmd = new OneTrueError.Core.Users.Commands.UpdatePersonalSettings();
                cmd.FirstName = dto.FirstName;
                cmd.LastName = dto.LastName;
                cmd.MobileNumber = dto.MobileNumber;
                CqsClient.command(cmd);
                humane.log("Settings have been saved.");
            };
            SettingsViewModel.prototype.getTitle = function () { return "Personal settings"; };
            SettingsViewModel.prototype.activate = function (context) {
                var _this = this;
                this.context = context;
                context.handle.click("[name=\"saveSettings\"]", function (ev) { return _this.saveSettings_click(ev); });
                context.handle.click("[name='changePassword']", function (ev) { return _this.changePassword_click(ev); });
                var query = new OneTrueError.Core.Users.Queries.GetUserSettings();
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
    })(User = OneTrueError.User || (OneTrueError.User = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=SettingsViewModel.js.map