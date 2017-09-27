/// <reference path="../../Scripts/Models/AllModels.ts" />
/// <reference path="../../Scripts/Promise.ts" />
/// <reference path="../../Scripts/CqsClient.ts" />
/// <reference path="../ChartViewModel.ts" />
var codeRR;
(function (codeRR) {
    var Incident;
    (function (Incident) {
        var OriginsViewModel = (function () {
            function OriginsViewModel() {
            }
            OriginsViewModel.prototype.getTitle = function () {
                return "Close incident";
            };
            OriginsViewModel.prototype.activate = function (context) {
                this.context = context;
            };
            OriginsViewModel.prototype.deactivate = function () { };
            return OriginsViewModel;
        }());
        Incident.OriginsViewModel = OriginsViewModel;
    })(Incident = codeRR.Incident || (codeRR.Incident = {}));
})(codeRR || (codeRR = {}));
