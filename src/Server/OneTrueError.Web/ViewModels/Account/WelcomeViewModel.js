var OneTrueError;
(function (OneTrueError) {
    var Account;
    (function (Account) {
        var WelcomeViewModel = (function () {
            function WelcomeViewModel() {
            }
            WelcomeViewModel.prototype.getTitle = function () { return "Welcome"; };
            WelcomeViewModel.prototype.activate = function (context) {
                context.resolve();
            };
            WelcomeViewModel.prototype.deactivate = function () { };
            return WelcomeViewModel;
        }());
        Account.WelcomeViewModel = WelcomeViewModel;
    })(Account = OneTrueError.Account || (OneTrueError.Account = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=WelcomeViewModel.js.map