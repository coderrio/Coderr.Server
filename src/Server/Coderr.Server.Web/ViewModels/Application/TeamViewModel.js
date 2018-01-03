/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
var codeRR;
(function (codeRR) {
    var Application;
    (function (Application) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var RemoveTeamMember = codeRR.Core.Applications.Commands.RemoveTeamMember;
        var ApplicationService = codeRR.Applications.ApplicationService;
        var TeamViewModel = /** @class */ (function () {
            function TeamViewModel() {
            }
            TeamViewModel.prototype.getTitle = function () {
                var appId = this.context.routeData['applicationId'];
                var app = new ApplicationService();
                app.get(appId)
                    .then(function (result) {
                    var bc = [
                        { href: "/application/" + appId + "/", title: result.Name },
                        { href: "/application/" + appId + "/team", title: 'Team members' }
                    ];
                    codeRR.Applications.Navigation.breadcrumbs(bc);
                    codeRR.Applications.Navigation.pageTitle = 'Team members';
                });
                return "Team members";
            };
            TeamViewModel.prototype.activate = function (context) {
                var _this = this;
                this.context = context;
                this.applicationId = parseInt(context.routeData["applicationId"], 10);
                context.render({ Members: [], Invited: [] });
                var query = new codeRR.Core.Applications.Queries.GetApplicationTeam(this.applicationId);
                CqsClient.query(query)
                    .done(function (result) {
                    context.render(result);
                    _this.data = result;
                    context.resolve();
                    context.handle.click("#InviteUserBtn", function (e) { return _this.onInviteUser(e); });
                    context.handle.click('[data-name="RemoveUser"]', function (e) { return _this.onBtnRemoveUser(e); });
                });
            };
            TeamViewModel.prototype.deactivate = function () { };
            TeamViewModel.prototype.onBtnRemoveUser = function (e) {
                e.preventDefault();
                var node = e.target;
                var input = node.previousElementSibling;
                var accountId = parseInt(input.value, 10);
                if (accountId === 0)
                    throw new Error("Failed to find accountID");
                var cmd = new RemoveTeamMember(this.applicationId, accountId);
                CqsClient.command(cmd).done(function (v) {
                    var parent = node.parentElement;
                    while (parent.tagName != 'TR') {
                        parent = parent.parentElement;
                    }
                    parent.parentElement.removeChild(parent);
                    humane.log('User was removed');
                });
            };
            TeamViewModel.prototype.onInviteUser = function (mouseEvent) {
                mouseEvent.preventDefault();
                var inputElement = this.context.select.one("emailAddress");
                var email = inputElement.value;
                var el = this.context.select.one("reason");
                var reason = el.value;
                var cmd = new codeRR.Core.Invitations.Commands.InviteUser(this.applicationId, email);
                cmd.Text = reason;
                CqsClient.command(cmd);
                var newItem = new codeRR.Core.Applications.Queries.GetApplicationTeamResultInvitation();
                newItem.EmailAddress = email;
                this.data.Invited.push(newItem);
                this.context.render(this.data);
                inputElement.value = "";
            };
            return TeamViewModel;
        }());
        Application.TeamViewModel = TeamViewModel;
    })(Application = codeRR.Application || (codeRR.Application = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=TeamViewModel.js.map