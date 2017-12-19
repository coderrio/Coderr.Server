/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="Promise.ts" />
var Griffin;
(function (Griffin) {
    var WebApp;
    (function (WebApp) {
        var PagerPage = /** @class */ (function () {
            function PagerPage(pageNumber, selected) {
                this.pageNumber = pageNumber;
                this.selected = selected;
            }
            PagerPage.prototype.select = function () {
                this.listItem.classList.add(Pager.LI_ACTIVE_CLASS);
                this.selected = true;
            };
            PagerPage.prototype.deselect = function () {
                this.listItem.classList.remove(Pager.LI_ACTIVE_CLASS);
                this.selected = true;
            };
            return PagerPage;
        }());
        WebApp.PagerPage = PagerPage;
        var Pager = /** @class */ (function () {
            function Pager(currentPage, pageSize, totalNumberOfItems) {
                this.currentPage = currentPage;
                this.pageSize = pageSize;
                this.totalNumberOfItems = totalNumberOfItems;
                this.pageCount = 0;
                this.pages = new Array();
                this._subscribers = new Array();
                this.update(currentPage, pageSize, totalNumberOfItems);
            }
            Pager.prototype.update = function (currentPage, pageSize, totalNumberOfItems) {
                if (totalNumberOfItems < pageSize || pageSize === 0 || totalNumberOfItems === 0) {
                    this.pageCount = 0;
                    this.pages = [];
                    if (this.parent) {
                        this.draw(this.parent);
                    }
                    return;
                }
                var isFirstUpdate = this.totalNumberOfItems === 0;
                this.pageCount = Math.ceil(totalNumberOfItems / pageSize);
                this.currentPage = currentPage;
                this.totalNumberOfItems = totalNumberOfItems;
                this.pageSize = pageSize;
                var i = 1;
                this.pages = new Array();
                if (this.pageCount > 10) {
                    this.partialPages = true;
                    this.generateDynamicPaging();
                }
                else {
                    for (i = 1; i <= this.pageCount; i++)
                        this.pages.push(new PagerPage(i, i === currentPage));
                }
                if (this.parent) {
                    this.draw(this.parent);
                }
                if (!isFirstUpdate) {
                    this.notify();
                }
            };
            Pager.prototype.subscribe = function (subscriber) {
                this._subscribers.push(subscriber);
            };
            Pager.prototype.moveNext = function () {
                if (this.currentPage >= this.pageCount) {
                    return;
                }
                if (this.partialPages) {
                    this.currentPage += 1;
                    this.generateDynamicPaging();
                    this.draw(this.parent);
                }
                else {
                    this.pages[this.currentPage - 1].deselect();
                    this.currentPage += 1;
                    this.pages[this.currentPage - 1].select();
                }
                if (this.currentPage === this.pageCount) {
                    this.nextItem.style.display = "none";
                }
                this.prevItem.style.display = "";
                this.notify();
            };
            Pager.prototype.movePrevious = function () {
                if (this.currentPage <= 1) {
                    return;
                }
                if (this.partialPages) {
                    this.currentPage += 1;
                    this.generateDynamicPaging();
                    this.draw(this.parent);
                }
                else {
                    this.pages[this.currentPage - 1].deselect();
                    this.currentPage -= 1;
                    this.pages[this.currentPage - 1].select();
                }
                if (this.currentPage === 1) {
                    this.prevItem.style.display = "none";
                }
                this.nextItem.style.display = "";
                this.notify();
            };
            Pager.prototype.goto = function (pageNumber) {
                console.log(pageNumber, this.partialPages);
                if (this.partialPages) {
                    this.currentPage = pageNumber;
                    this.generateDynamicPaging();
                    this.draw(this.parent);
                }
                else {
                    this.pages[this.currentPage - 1].deselect();
                    this.currentPage = pageNumber;
                    this.pages[this.currentPage - 1].select();
                }
                if (this.currentPage === 1 || this.pageCount === 1) {
                    this.prevItem.style.display = "none";
                }
                else {
                    this.prevItem.style.display = "";
                }
                if (this.currentPage === this.pageCount || this.pageCount === 1) {
                    this.nextItem.style.display = "none";
                }
                else {
                    this.nextItem.style.display = "";
                }
                this.notify();
            };
            Pager.prototype.generateDynamicPaging = function () {
                this.pages = [];
                var firstPage = this.currentPage - 4;
                if (firstPage < 1)
                    firstPage = 1;
                else
                    this.pages.push(new PagerPage(1, false));
                console.log(firstPage);
                var lastPage = firstPage + 9;
                if (lastPage > this.pageCount)
                    lastPage = this.pageCount;
                var i;
                for (i = firstPage; i <= lastPage; i++)
                    this.pages.push(new PagerPage(i, i === this.currentPage));
                if (lastPage < this.pageCount)
                    this.pages.push(new PagerPage(this.pageCount, false));
            };
            Pager.prototype.createListItem = function (title, callback) {
                var btn = document.createElement("a");
                btn.innerHTML = title;
                btn.className = Pager.LINK_CLASS;
                btn.addEventListener("click", function (e) {
                    e.preventDefault();
                    callback();
                });
                var li = document.createElement("li");
                li.className = Pager.LI_CLASS;
                li.appendChild(btn);
                return li;
            };
            Pager.prototype.deactivateAll = function () {
                this.prevItem.classList.remove(Pager.LI_ACTIVE_CLASS);
                this.nextItem.classList.remove(Pager.LI_ACTIVE_CLASS);
                this.pages.forEach(function (item) {
                    item.selected = false;
                    item.listItem.classList.remove(Pager.LI_ACTIVE_CLASS);
                });
            };
            Pager.prototype.draw = function (containerIdOrElement) {
                var _this = this;
                if (typeof containerIdOrElement === "string") {
                    this.parent = document.getElementById(containerIdOrElement);
                    if (!this.parent)
                        throw new Error("Failed to find '" + containerIdOrElement + "'");
                }
                else {
                    if (!containerIdOrElement)
                        throw new Error("No element was specified");
                    this.parent = containerIdOrElement;
                }
                while (this.parent.childElementCount > 0) {
                    this.parent.removeChild(this.parent.firstElementChild);
                }
                var self = this;
                var ul = document.createElement("ul");
                ul.className = Pager.UL_CLASS;
                //prev
                var li = this.createListItem("&lt;&lt; Previous", function () {
                    self.movePrevious();
                });
                ul.appendChild(li);
                this.prevItem = li;
                if (this.currentPage <= 1 || this.pageCount <= 1) {
                    this.prevItem.style.display = "none";
                }
                else {
                    this.prevItem.style.display = "";
                }
                //pages
                this.pages.forEach(function (item) {
                    var li = _this.createListItem(item.pageNumber.toString(), function () {
                        self.goto(item.pageNumber);
                    });
                    item.listItem = li;
                    ul.appendChild(li);
                    if (item.selected) {
                        li.classList.add(Pager.LI_ACTIVE_CLASS);
                    }
                });
                //next
                var li = this.createListItem("Next &gt;&gt;", function () {
                    self.moveNext();
                });
                li.className = Pager.LI_CLASS;
                ul.appendChild(li);
                this.nextItem = li;
                if (this.currentPage >= this.pageCount || this.pageCount <= 1) {
                    this.nextItem.style.display = "none";
                }
                else {
                    this.nextItem.style.display = "";
                }
                while (this.parent.firstChild) {
                    this.parent.removeChild(this.parent.firstChild);
                }
                this.parent.appendChild(ul);
            };
            Pager.prototype.notify = function () {
                var self = this;
                this._subscribers.forEach(function (item) {
                    item.onPager.apply(item, [self]);
                });
            };
            Pager.prototype.reset = function () {
                this.pageCount = 0;
                this.pages = [];
                if (this.parent) {
                    this.draw(this.parent);
                }
            };
            Pager.LI_ACTIVE_CLASS = "active";
            Pager.UL_CLASS = "pagination";
            Pager.LI_CLASS = "page-item";
            Pager.LINK_CLASS = "page-link";
            return Pager;
        }());
        WebApp.Pager = Pager;
    })(WebApp = Griffin.WebApp || (Griffin.WebApp = {}));
})(Griffin || (Griffin = {}));
//# sourceMappingURL=Griffin.WebApp.js.map