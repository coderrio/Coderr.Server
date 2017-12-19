var codeRR;
(function (codeRR) {
    var Home;
    (function (Home) {
        var WelcomeSaasViewModel = /** @class */ (function () {
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
    })(Home = codeRR.Home || (codeRR.Home = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=WelcomeSaasViewModel.js.map