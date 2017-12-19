/// <reference path="../../Scripts/CqsClient.ts" />
var codeRR;
(function (codeRR) {
    var Account;
    (function (Account) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var GetUserSettings = codeRR.Core.Users.Queries.GetUserSettings;
        var ChangePassword = codeRR.Core.Accounts.Requests.ChangePassword;
        var AccountSettings = /** @class */ (function () {
            function AccountSettings() {
                this.notifyNewIncident = true;
            }
            return AccountSettings;
        }());
        Account.AccountSettings = AccountSettings;
        var SettingsViewModel = /** @class */ (function () {
            function SettingsViewModel() {
            }
            SettingsViewModel.load = function () {
                var def = P.defer();
                var query = new GetUserSettings();
                CqsClient.query(query)
                    .done(function (response) {
                    var vm = new SettingsViewModel();
                    def.resolve(vm);
                });
                return def.promise();
            };
            SettingsViewModel.prototype.saveSettings = function () {
                var mapping = {
                    'ignore': ["saveSettings"]
                };
            };
            SettingsViewModel.prototype.changePassword = function () {
                var json = {
                    currentPassword: $("#CurrentPassword").val(),
                    newPassword: $("#NewPassword").val()
                };
                if (json.newPassword != $("#NewPassword2").val()) {
                    alert("The new password fields do not match.");
                    return;
                }
                var newPw = $("#NewPassword2").val();
                var oldPw = $("#CurrentPassword").val();
                var pw = new ChangePassword(newPw, oldPw);
                CqsClient.command(pw)
                    .done(function (value) {
                })
                    .fail(function (err) {
                });
            };
            return SettingsViewModel;
        }());
        Account.SettingsViewModel = SettingsViewModel;
    })(Account = codeRR.Account || (codeRR.Account = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=SettingsViewModel.js.map