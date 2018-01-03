/// <reference path="../../Scripts/Griffin.Yo.d.ts" />

module codeRR.Application {
    import CqsClient = Griffin.Cqs.CqsClient;
    import RemoveTeamMember = codeRR.Core.Applications.Commands.RemoveTeamMember;
    import ApplicationService = Applications.ApplicationService;

    export class TeamViewModel implements Griffin.Yo.Spa.ViewModels.IViewModel {
        private context: Griffin.Yo.Spa.ViewModels.IActivationContext;
        private applicationId: number;
        private data: Core.Applications.Queries.GetApplicationTeamResult;

        getTitle(): string {
            var appId = this.context.routeData['applicationId'];
            var app = new ApplicationService();
            app.get(appId)
                .then(result => {
                    var bc: Applications.IBreadcrumb[] = [
                        { href: `/application/${appId}/`, title: result.Name },
                        { href: `/application/${appId}/team`, title: 'Team members' }
                    ];
                    Applications.Navigation.breadcrumbs(bc);
                    Applications.Navigation.pageTitle = 'Team members';
                });

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
                    context.handle.click('[data-name="RemoveUser"]', e => this.onBtnRemoveUser(e));
                });
        }

        deactivate() { }

        private onBtnRemoveUser(e: Event): void {
            e.preventDefault();
            var node = <HTMLElement>e.target;
            var input = <HTMLInputElement>node.previousElementSibling;
            var accountId = parseInt(input.value, 10);
            if (accountId === 0)
                throw new Error("Failed to find accountID");
            var cmd = new RemoveTeamMember(this.applicationId, accountId);
            CqsClient.command(cmd).done(v => {
                var parent = node.parentElement;
                while (parent.tagName != 'TR') {
                    parent = parent.parentElement;
                }
                parent.parentElement.removeChild(parent);
                humane.log('User was removed');
            });
        }

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