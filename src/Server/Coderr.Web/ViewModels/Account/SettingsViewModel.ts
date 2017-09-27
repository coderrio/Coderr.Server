/// <reference path="../../Scripts/CqsClient.ts" />
module codeRR.Account {
    import CqsClient = Griffin.Cqs.CqsClient;
    import GetUserSettings = Core.Users.Queries.GetUserSettings;
    import UserSettingsResult = Core.Users.Queries.GetUserSettingsResult;
    import ChangePassword = Core.Accounts.Requests.ChangePassword;

    export class AccountSettings {
        firstName: string;
        lastName: string;
        notifyNewIncident = true;
        notifyNewReport: boolean;
        notifyReOpenedIncident: boolean;
        notifyPeaks: boolean;
    }

    export class SettingsViewModel {
        firstName: string;
        lastName: string;
        notifyNewIncident: boolean;
        notifyNewReport: boolean;
        notifyReOpenedIncident: boolean;
        notifyPeaks: boolean;
        weeklySummary: boolean;

        static load(): P.Promise<SettingsViewModel> {
            var def = P.defer<SettingsViewModel>();
            const query = new GetUserSettings();
            CqsClient.query<UserSettingsResult>(query)
                .done(response => {
                    var vm = new SettingsViewModel();

                    def.resolve(vm);
                });

            return def.promise();
        }

        saveSettings(): void {
            const mapping = {
                'ignore': ["saveSettings"]
            };
        }

        changePassword(): void {
            const json = {
                currentPassword: $("#CurrentPassword").val(),
                newPassword: $("#NewPassword").val()
            };
            if (json.newPassword != $("#NewPassword2").val()) {
                alert("The new password fields do not match.");
                return;
            }

            const newPw = $("#NewPassword2").val();
            const oldPw = $("#CurrentPassword").val();
            const pw = new ChangePassword(newPw, oldPw);
            CqsClient.command(pw)
                .done(value => {
                })
                .fail(err => {
                });


        }
    }
}