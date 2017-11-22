/// <reference path="../../../Scripts/CqsClient.ts" />
var CqsClient = Griffin.Cqs.CqsClient;
var Yo = Griffin.Yo;
var Pager = Griffin.WebApp.Pager;
var IncidentOrder = codeRR.Core.Incidents.IncidentOrder;
var IncidentTableViewModel = (function () {
    function IncidentTableViewModel(ctx) {
        this.sortType = IncidentOrder.Newest;
        this.incidentType = "new";
        this.sortAscending = false;
        this.ctx = ctx;
    }
    IncidentTableViewModel.prototype.load = function (applicationId, applicationVersion, callback) {
        var _this = this;
        var query = new codeRR.Core.Incidents.Queries.FindIncidents();
        query.PageNumber = 1;
        query.ItemsPerPage = 20;
        query.IsNew = true;
        if (applicationId != null) {
            query.ApplicationId = applicationId;
            this.applicationId = applicationId;
        }
        if (applicationVersion != null) {
            query.Version = applicationVersion;
        }
        CqsClient.query(query)
            .done(function (response) {
            var itemsElem = _this.ctx.viewContainer.querySelector("#incidentTable");
            _this.renderTable(itemsElem, response);
            _this.pager = new Griffin.WebApp.Pager(response.PageNumber, 20, response.TotalCount);
            _this.pager.subscribe(_this);
            _this.pager.draw(_this.ctx.select.one("#pager"));
            if (callback) {
                callback();
            }
        });
        this.ctx.handle.click("#btnClosed", function (e) { return _this.onBtnClosed(e); });
        this.ctx.handle.click("#btnNew", function (e) { return _this.onBtnNew(e); });
        this.ctx.handle.click("#btnActive", function (e) { return _this.onBtnActive(e); });
        this.ctx.handle.click("#btnIgnored", function (e) { return _this.onBtnIgnored(e); });
        this.ctx.handle.click("#LastReportCol", function (e) { return _this.onLastReportCol(e); });
        this.ctx.handle.click("#CountCol", function (e) { return _this.onCountCol(e); });
        this.ctx.handle.keyUp('[data-name="freeText"]', function (e) { return _this.onFreeText(e); });
    };
    IncidentTableViewModel.prototype.onFreeText = function (e) {
        var el = (e.target);
        if (el.value.length >= 3) {
            this.freeText = el.value;
            this.loadItems(this.pager.currentPage);
        }
        else if (el.value === '') {
            this.freeText = el.value;
            this.loadItems(this.pager.currentPage);
        }
        else {
            this.freeText = el.value;
        }
    };
    IncidentTableViewModel.prototype.onPager = function (pager) {
        this.loadItems(pager.currentPage);
    };
    IncidentTableViewModel.prototype.onBtnClosed = function (e) {
        e.preventDefault();
        this.incidentType = "closed";
        this.pager.reset();
        $(e.target).parent().find('label').removeClass('active');
        $(e.target).addClass('active');
        this.loadItems();
    };
    IncidentTableViewModel.prototype.onBtnActive = function (e) {
        e.preventDefault();
        this.incidentType = "active";
        this.pager.reset();
        $(e.target).parent().find('label').removeClass('active');
        $(e.target).addClass('active');
        this.loadItems();
    };
    IncidentTableViewModel.prototype.onBtnNew = function (e) {
        e.preventDefault();
        this.incidentType = "new";
        this.pager.reset();
        $(e.target).parent().find('label').removeClass('active');
        $(e.target).addClass('active');
        this.loadItems();
    };
    IncidentTableViewModel.prototype.onBtnIgnored = function (e) {
        e.preventDefault();
        this.incidentType = "ignored";
        this.pager.reset();
        $(e.target).parent().find('label').removeClass('active');
        $(e.target).addClass('active');
        this.loadItems();
    };
    IncidentTableViewModel.prototype.onCountCol = function (args) {
        if (this.sortType !== IncidentOrder.MostReports) {
            this.sortType = IncidentOrder.MostReports;
            this.sortAscending = true; //will be changed below
        }
        this.updateSorting(args.target);
        this.loadItems();
    };
    IncidentTableViewModel.prototype.onLastReportCol = function (args) {
        if (this.sortType !== IncidentOrder.Newest) {
            this.sortType = IncidentOrder.Newest;
            this.sortAscending = false; //will be changed below
        }
        if (this.sortAscending) {
            //TODO: SORT
        }
        else {
        }
        this.updateSorting(args.target);
        this.loadItems();
    };
    IncidentTableViewModel.prototype.updateSorting = function (parentElement) {
        this.sortAscending = !this.sortAscending;
        var icon = IncidentTableViewModel.UP;
        if (!this.sortAscending) {
            icon = IncidentTableViewModel.DOWN;
        }
        $("#IndexView thead th span")
            .removeClass("fa-chevron-down")
            .addClass("glyphicon " + icon)
            .css("visibility", "hidden");
        $("span", parentElement)
            .attr("class", "glyphicon " + icon)
            .css("visibility", "inherit");
    };
    IncidentTableViewModel.prototype.renderTable = function (target, data) {
        var directives = {
            Name: {
                href: function (params, dto) {
                    return "#/application/" + dto.ApplicationId + "/incident/" + dto.Id + "/";
                },
                text: function (value) {
                    return value;
                }
            },
            ApplicationName: {
                href: function (params, dto) {
                    return "#/application/" + dto.ApplicationId + "/";
                },
                text: function (value) {
                    return value;
                }
            },
            LastReportReceivedAtUtc: {
                text: function (value) {
                    return momentsAgo(value);
                }
            }
        };
        if (this.applicationId == null) {
            delete directives.ApplicationName;
        }
        Yo.G.render(target, data.Items, directives);
    };
    IncidentTableViewModel.prototype.findYPosition = function (element) {
        if (element.offsetParent) {
            var curtop = 0;
            do {
                curtop += element.offsetTop;
                element = (element.offsetParent);
            } while (element);
            return curtop;
        }
    };
    IncidentTableViewModel.prototype.loadItems = function (pageNumber) {
        var _this = this;
        if (pageNumber === void 0) { pageNumber = 0; }
        var query = new codeRR.Core.Incidents.Queries.FindIncidents();
        if (this.applicationId != null) {
            query.ApplicationId = this.applicationId;
        }
        query.SortType = this.sortType;
        query.SortAscending = this.sortAscending;
        if (this.incidentType === "closed") {
            query.IsClosed = true;
        }
        else if (this.incidentType === 'ignored') {
            query.IsIgnored = true;
        }
        else if (this.incidentType === 'new') {
            query.IsNew = true;
        }
        else {
            query.IsAssigned = true;
        }
        if (pageNumber === 0) {
            query.PageNumber = this.pager.currentPage;
        }
        else {
            query.PageNumber = pageNumber;
        }
        var searchBox = this.ctx.select.one('freeText');
        if (searchBox.value.length >= 3)
            query.FreeText = searchBox.value;
        query.ItemsPerPage = 20;
        CqsClient.query(query)
            .done(function (response) {
            var table = document.getElementById("incidentTable");
            _this.renderTable(table, response);
            var yPos = _this.findYPosition(table);
            window.scrollTo(0, yPos);
        });
    };
    return IncidentTableViewModel;
}());
IncidentTableViewModel.UP = "fa fa-chevron-up";
IncidentTableViewModel.DOWN = "fa fa-chevron-down";
//# sourceMappingURL=IncidentTable.js.map