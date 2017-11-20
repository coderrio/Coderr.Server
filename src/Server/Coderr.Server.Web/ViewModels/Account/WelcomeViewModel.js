var codeRR;
(function (codeRR) {
    var Account;
    (function (Account) {
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
        Account.WelcomeViewModel = WelcomeViewModel;
    })(Account = codeRR.Account || (codeRR.Account = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=WelcomeViewModel.js.map