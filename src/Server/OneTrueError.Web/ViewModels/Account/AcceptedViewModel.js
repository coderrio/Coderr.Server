var OneTrueError;
(function (OneTrueError) {
    var Account;
    (function (Account) {
        var AcceptedViewModel = (function () {
            function AcceptedViewModel() {
            }
            AcceptedViewModel.prototype.getTitle = function () { return "Invitation accepted"; };
            AcceptedViewModel.prototype.activate = function (context) {
                context.resolve();
            };
            AcceptedViewModel.prototype.deactivate = function () { };
            return AcceptedViewModel;
        }());
        Account.AcceptedViewModel = AcceptedViewModel;
    })(Account = OneTrueError.Account || (OneTrueError.Account = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=AcceptedViewModel.js.map