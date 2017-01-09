var OneTrueError;
(function (OneTrueError) {
    var Home;
    (function (Home) {
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
        Home.WelcomeViewModel = WelcomeViewModel;
    })(Home = OneTrueError.Home || (OneTrueError.Home = {}));
})(OneTrueError || (OneTrueError = {}));
//# sourceMappingURL=WelcomeViewModel.js.map