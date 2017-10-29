/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
module codeRR.User {
    var cqs = Griffin.Cqs.CqsClient;
    import CqsClient = Griffin.Cqs.CqsClient;

    export class SettingsViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private notifyNewIncident: boolean;
        private notifyNewReport: boolean;
        private notifyReOpenedIncident: boolean;
        private notifyPeaks: boolean;
        private context: Griffin.Yo.Spa.ViewModels.IActivationContext;

        constructor() {
        }

        changePassword_click(e: any) {
            e.preventDefault();
            const dto = this.context.readForm("PasswordView");
            if (dto.NewPassword !== dto.NewPassword2) {
                humane.error("New passwords do not match.");
                return;
            }
            if (!dto.CurrentPassword) {
                humane.error("You must enter the current password.");
                return;
            }

            const cmd = new Core.Accounts.Requests.ChangePassword(dto.CurrentPassword, dto.NewPassword);
            CqsClient.command(cmd)
                .done(result => {
                    this.context.render({ NewPassword: "", NewPassword2: "", CurrentPassword: "" });
                    humane.log("Password have been changed.");
                });

        }

        saveSettings_click(e: any) {
            e.isHandled = true;
            const dto = this.context.readForm("PersonalSettings");
            const cmd = new Core.Users.Commands.UpdatePersonalSettings();
            cmd.FirstName = dto.FirstName;
            cmd.LastName = dto.LastName;
            cmd.MobileNumber = dto.MobileNumber;
            cmd.EmailAddress = dto.EmailAddress;
            CqsClient.command(cmd);
            humane.log("Settings have been saved.");
        }

        getTitle(): string { return "Personal settings"; }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            Applications.Navigation.breadcrumbs([{ href: "/settings/personal", title: "Account settings" }]);
            Applications.Navigation.pageTitle = 'Account settings';

            this.context = context;

            context.handle.click("[name=\"saveSettings\"]", ev => this.saveSettings_click(ev));
            context.handle.click("[name='changePassword']", ev => this.changePassword_click(ev));
            const query = new Core.Users.Queries.GetUserSettings();
            cqs.query<Core.Users.Queries.GetUserSettingsResult>(query)
                .done(result => {
                    context.render(result);
                    context.resolve();
                });

        }

        deactivate() {

        }

    }
}