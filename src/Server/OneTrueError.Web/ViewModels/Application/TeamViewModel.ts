/// <reference path="../../Scripts/Griffin.Yo.d.ts" />

module OneTrueError.Application {
    import CqsClient = Griffin.Cqs.CqsClient;

    export class TeamViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private context: Griffin.Yo.Spa.ViewModels.IActivationContext;
        private applicationId: number;
        private data: Core.Applications.Queries.GetApplicationTeamResult;

        getTitle(): string {
            return "Team members";

        }

        activate(context: Griffin.Yo.Spa.ViewModels.IActivationContext): void {
            this.context = context;
            this.applicationId = parseInt(context.routeData["applicationId"], 10);
            context.render({ Members: [], Invited: [] });

            const query = new Core.Applications.Queries.GetApplicationTeam(this.applicationId);
            CqsClient.query<Core.Applications.Queries.GetApplicationTeamResult>(query)
                .done(result => {
                    context.render(result);
                    this.data = result;
                    context.resolve();
                    context.handle.click("#InviteUserBtn", e => this.onInviteUser(e));
                });
        }

        deactivate() {}

        onInviteUser(mouseEvent: MouseEvent) {
            mouseEvent.preventDefault();
            const inputElement = this.context.select.one("emailAddress") as HTMLInputElement;
            const email = inputElement.value;
            const el = this.context.select.one("reason") as HTMLTextAreaElement;
            const reason = el.value;

            const cmd = new Core.Invitations.Commands.InviteUser(this.applicationId, email);
            cmd.Text = reason;
            CqsClient.command(cmd);

            const newItem = new Core.Applications.Queries.GetApplicationTeamResultInvitation();
            newItem.EmailAddress = email;
            this.data.Invited.push(newItem);
            this.context.render(this.data);
            inputElement.value = "";
        }
    }
}