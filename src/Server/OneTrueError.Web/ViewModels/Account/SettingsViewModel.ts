/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Promise.ts"/>

module OneTrueError.Account {
    import CqsClient = Griffin.Cqs.CqsClient;
    import GetUserSettings = OneTrueError.Core.Users.Queries.GetUserSettings;
    import UserSettingsResult = OneTrueError.Core.Users.Queries.GetUserSettingsResult;
    import ChangePassword = OneTrueError.Core.Accounts.Requests.ChangePassword;

    export class AccountSettings {
        public firstName: string;
        public lastName: string;
        public notifyNewIncident: boolean = true;
        public notifyNewReport: boolean;
        public notifyReOpenedIncident: boolean;
        public notifyPeaks: boolean;
    }

    export class SettingsViewModel {
        public firstName: string;
        public lastName: string;
        public notifyNewIncident: boolean;
        public notifyNewReport: boolean;
        public notifyReOpenedIncident: boolean;
        public notifyPeaks: boolean;
        public weeklySummary: boolean;

        public static load(): P.Promise<SettingsViewModel> {
            var def = P.defer<SettingsViewModel>();
            var query = new GetUserSettings();
            CqsClient.query< UserSettingsResult>(query).
                done(response => {
                    var vm = new SettingsViewModel();
                    
                    def.resolve(vm);
                });

            return def.promise();
        }

        public saveSettings(): void {
            var mapping = {
                'ignore': ["saveSettings"]
            }
           

        }

        public changePassword(): void {
            var json = {
                currentPassword: $('#CurrentPassword').val(),
                newPassword: $('#NewPassword').val()
            };
            if (json.newPassword != $('#NewPassword2').val()) {
                alert("The new password fields do not match.");
                return;
            }

            var newPw = $('#NewPassword2').val();
            var oldPw = $('#CurrentPassword').val();
            var pw = new ChangePassword(newPw, oldPw);
            CqsClient.command(pw)
                .done(value => {
                }).fail(err => {
                });
            

        }
    }
} 