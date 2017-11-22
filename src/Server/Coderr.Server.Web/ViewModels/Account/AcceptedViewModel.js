var codeRR;
(function (codeRR) {
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
    })(Account = codeRR.Account || (codeRR.Account = {}));
})(codeRR || (codeRR = {}));
//# sourceMappingURL=AcceptedViewModel.js.map