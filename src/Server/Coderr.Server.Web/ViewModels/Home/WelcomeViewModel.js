var codeRR;
(function (codeRR) {
    var Home;
    (function (Home) {
        var WelcomeViewModel = /** @class */ (function () {
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
    })(Home = codeRR.Home || (codeRR.Home = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=WelcomeViewModel.js.map