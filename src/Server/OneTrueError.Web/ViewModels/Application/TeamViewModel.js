/// <reference path="../../Scripts/Griffin.Yo.d.ts" />
var OneTrueError;
(function (OneTrueError) {
    var Application;
    (function (Application) {
        var CqsClient = Griffin.Cqs.CqsClient;
        var TeamViewModel = (function () {
            function TeamViewModel() {
            }
            TeamViewModel.prototype.getTitle = function () {
                return "Team members";
            };
            TeamViewModel.prototype.activate = function (context) {
                var _this = this;
                this.context = context;
                this.applicationId = parseInt(context.routeData["applicationId"], 10);
                context.render({ Members: [], Invited: [] });
                var query = new OneTrueError.Core.Applications.Queries.GetApplicationTeam(this.applicationId);
                CqsClient.query(query)
                    .done(function (result) {
                    context.render(result);
                    _this.data = result;
                    context.resolve();
                    context.handle.click("#InviteUserBtn", function (e) { return _this.onInviteUser(e); });
                });
            };
            TeamViewModel.prototype.deactivate = function () { };
            TeamViewModel.prototype.onInviteUser = function (mouseEvent) {
                mouseEvent.preventDefault();
                var inputElement = this.context.select.one("emailAddress");
                var email = inputElement.value;
                var el = this.context.select.one("reason");
                var reason = el.value;
                var cmd = new OneTrueError.Core.Invitations.Commands.InviteUser(this.applicationId, email);
                cmd.Text = reason;
                CqsClient.command(cmd);
                var newItem = new OneTrueError.Core.Applications.Queries.GetApplicationTeamResultInvitation();
                newItem.EmailAddress = email;
                this.data.Invited.push(newItem);
                this.context.render(this.data);
                inputElement.value = "";
            };
            return TeamViewModel;
        }());
        Application.TeamViewModel = TeamViewModel;
    })(Application = OneTrueError.Application || (OneTrueError.Application = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=TeamViewModel.js.map