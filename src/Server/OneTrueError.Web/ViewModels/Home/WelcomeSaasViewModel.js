var OneTrueError;
(function (OneTrueError) {
    var Home;
    (function (Home) {
        var WelcomeSaasViewModel = (function () {
            function WelcomeSaasViewModel() {
            }
            WelcomeSaasViewModel.prototype.getTitle = function () { return "Welcome"; };
            WelcomeSaasViewModel.prototype.activate = function (context) {
                context.resolve();
            };
            WelcomeSaasViewModel.prototype.deactivate = function () { };
            return WelcomeSaasViewModel;
        }());
        Home.WelcomeSaasViewModel = WelcomeSaasViewModel;
    })(Home = OneTrueError.Home || (OneTrueError.Home = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=WelcomeSaasViewModel.js.map