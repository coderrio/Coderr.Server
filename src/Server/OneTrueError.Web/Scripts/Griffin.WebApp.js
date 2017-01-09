/// <reference path="typings/jquery/jquery.d.ts" />
/// <reference path="Promise.ts" />
var Griffin;
(function (Griffin) {
    var WebApp;
    (function (WebApp) {
        var ChangedEventArgs = (function () {
            function ChangedEventArgs(model, field, newValue) {
                this.model = model;
                this.field = field;
                this.newValue = newValue;
                this.isHandled = true;
            }
            return ChangedEventArgs;
        }());
        WebApp.ChangedEventArgs = ChangedEventArgs;
        var ClickEventArgs = (function () {
            function ClickEventArgs(model, field, newValue) {
                this.model = model;
                this.field = field;
                this.newValue = newValue;
                this.isHandled = true;
            }
            return ClickEventArgs;
        }());
        WebApp.ClickEventArgs = ClickEventArgs;
        var FieldModel = (function () {
            function FieldModel(propertyName) {
                this.propertyName = propertyName;
            }
            FieldModel.prototype.equalsName = function (name) {
                return this.propertyName.toLocaleLowerCase() === name.toLocaleLowerCase();
            };
            FieldModel.prototype.setContent = function (value) {
                var elm = this.elem;
                if (elm.value === "undefined") {
                    elm.value = value;
                }
                else {
                    this.elem.innerHTML = value;
                }
            };
            return FieldModel;
        }());
        var ViewModel = (function () {
            function ViewModel(modelId, viewModel) {
                this.jqueryMappings = new Array();
                this.instance = null;
                this.id = modelId;
                this.instance = viewModel;
                for (var fieldName in viewModel) {
                    var underscorePos = fieldName.indexOf("_");
                    if (underscorePos > 0 && typeof viewModel[fieldName] === "function") {
                        var propertyName = fieldName.substr(0, underscorePos);
                        var eventName = fieldName.substr(underscorePos + 1);
                        if (typeof this.jqueryMappings[propertyName] == "undefined") {
                            this.jqueryMappings[propertyName] = new FieldModel(propertyName);
                            var elem = this.elem(propertyName, this.id);
                            if (elem.length === 0) {
                                if (ViewModel.ENFORCE_MAPPINGS) {
                                    throw "Failed to find child element \"" + propertyName + "\" in model \"" + this.id + "\"";
                                }
                            }
                            else {
                                this.jqueryMappings[propertyName].elem = elem[0];
                            }
                        }
                        this.jqueryMappings[propertyName][eventName] = viewModel[fieldName];
                        this.mapEvent(propertyName, eventName, fieldName);
                    }
                    else if (typeof viewModel[fieldName] !== "function") {
                        if (fieldName[0] === "_") {
                            continue;
                        }
                        var elem = this.elem(fieldName, this.id);
                        if (elem.length === 0) {
                            if (ViewModel.ENFORCE_MAPPINGS) {
                                throw "Failed to find child element \"" + fieldName + "\" in model \"" + this.id + "\"";
                            }
                            continue;
                        }
                        if (typeof this.jqueryMappings[fieldName] == "undefined")
                            this.jqueryMappings[fieldName] = new FieldModel(fieldName);
                        this.jqueryMappings[fieldName].elem = elem[0];
                    }
                }
            }
            ViewModel.prototype.mapChanged = function (elementNameOrId) {
                var p = P.defer();
                var elem = this.elem(elementNameOrId, this.id);
                if (elem.length === 0)
                    throw "Failed to find child element \"" + elementNameOrId + "\" in model \"" + this.id + "\"";
                var model = this;
                elem.on("changed", function () {
                    p.resolve(new ChangedEventArgs(model, this, $(this).val()));
                });
                return p.promise();
            };
            ViewModel.prototype.bind = function () {
            };
            ViewModel.prototype.getField = function (name) {
                for (var fieldName in this.jqueryMappings) {
                    var field = this.jqueryMappings[fieldName];
                    if (field.equalsName(name))
                        return field;
                }
                return null;
            };
            ViewModel.prototype.update = function (json) {
                var model = this;
                for (var fieldName in json) {
                    var field = this.getField(fieldName);
                    if (field !== null) {
                        field.setContent(json[fieldName]);
                    }
                }
            };
            ViewModel.prototype.elem = function (name, parent) {
                if (parent === void 0) { parent = null; }
                var searchString = "#" + name + ",[data-name=\"" + name + "\"],[name=\"" + name + "\"]";
                if (parent == null)
                    return $(searchString);
                if (typeof parent === "string") {
                    return $(searchString, $("#" + parent));
                }
                return $(searchString, parent);
            };
            ViewModel.prototype.mapEvent = function (propertyName, eventName, fieldName) {
                var elem = this.elem(propertyName, this.id);
                if (elem.length === 0)
                    throw "Failed to find child element \"" + propertyName + "\" in model \"" + this.id + "\" for event \"" + eventName + "'.";
                var binder = this;
                if (eventName === "change" || eventName === "changed") {
                    elem.on("change", function (e) {
                        var args = new ChangedEventArgs(binder.instance, this, $(this).val());
                        binder.jqueryMappings[propertyName][eventName].apply(binder.instance, [args]);
                        if (args.isHandled) {
                            e.preventDefault();
                        }
                    });
                }
                else if (eventName === "click" || eventName === "clicked") {
                    elem.on("click", function (e) {
                        var value = "";
                        if (this.tagName == "A") {
                            value = this.getAttribute("href");
                        }
                        else {
                            value = $(this).val();
                        }
                        var args = new ClickEventArgs(binder.instance, this, value);
                        binder.jqueryMappings[propertyName][eventName].apply(binder.instance, [args]);
                        if (args.isHandled) {
                            e.preventDefault();
                        }
                    });
                }
            };
            ViewModel.ENFORCE_MAPPINGS = false;
            return ViewModel;
        }());
        WebApp.ViewModel = ViewModel;
        var Carburator = (function () {
            function Carburator() {
            }
            Carburator.mapRoute = function (elementNameOrId, viewModel) {
                return new ViewModel(elementNameOrId, viewModel);
            };
            return Carburator;
        }());
        WebApp.Carburator = Carburator;
        var PagerPage = (function () {
            function PagerPage(pageNumber, selected) {
                this.pageNumber = pageNumber;
                this.selected = selected;
            }
            PagerPage.prototype.select = function () {
                this.listItem.firstElementChild.setAttribute("class", "number " + Pager.BTN_ACTIVE_CLASS);
                this.selected = true;
            };
            PagerPage.prototype.deselect = function () {
                this.listItem.firstElementChild.setAttribute("class", "number " + Pager.BTN_CLASS);
                this.selected = true;
            };
            return PagerPage;
        }());
        WebApp.PagerPage = PagerPage;
        var Pager = (function () {
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
                for (i = 1; i <= this.pageCount; i++)
                    this.pages.push(new PagerPage(i, i === currentPage));
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
                if (this.currentPage < this.pageCount) {
                    this.pages[this.currentPage - 1].deselect();
                    this.currentPage += 1;
                    this.pages[this.currentPage - 1].select();
                    if (this.currentPage === this.pageCount) {
                        this.nextItem.style.display = "none";
                    }
                    this.prevItem.style.display = "";
                    this.notify();
                }
            };
            Pager.prototype.movePrevious = function () {
                if (this.currentPage > 1) {
                    this.pages[this.currentPage - 1].deselect();
                    this.currentPage -= 1;
                    this.pages[this.currentPage - 1].select();
                    if (this.currentPage === 1) {
                        this.prevItem.style.display = "none";
                    }
                    else {
                    }
                    this.nextItem.style.display = "";
                    this.notify();
                }
            };
            Pager.prototype.goto = function (pageNumber) {
                this.pages[this.currentPage - 1].deselect();
                this.currentPage = pageNumber;
                this.pages[this.currentPage - 1].select();
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
            Pager.prototype.createListItem = function (title, callback) {
                var btn = document.createElement("button");
                btn.innerHTML = title;
                btn.className = Pager.BTN_CLASS;
                btn.addEventListener("click", function (e) {
                    e.preventDefault();
                    callback();
                });
                var li = document.createElement("li");
                li.appendChild(btn);
                return li;
            };
            Pager.prototype.deactivateAll = function () {
                this.prevItem.firstElementChild.setAttribute("class", Pager.BTN_CLASS);
                this.nextItem.firstElementChild.setAttribute("class", Pager.BTN_CLASS);
                this.pages.forEach(function (item) {
                    item.selected = false;
                    item.listItem.firstElementChild.setAttribute("class", Pager.BTN_CLASS);
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
                var self = this;
                var ul = document.createElement("ul");
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
                        li.className = "active";
                        li.firstElementChild.setAttribute("class", "number " + Pager.BTN_ACTIVE_CLASS);
                    }
                });
                //next
                var li = this.createListItem("Next &gt;&gt;", function () {
                    self.moveNext();
                });
                ul.appendChild(li);
                this.nextItem = li;
                if (this.currentPage >= this.pageCount || this.pageCount <= 1) {
                    this.nextItem.style.display = "none";
                }
                else {
                    this.nextItem.style.display = "";
                }
                ul.className = "pager";
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
            Pager.BTN_ACTIVE_CLASS = "btn btn-success";
            Pager.BTN_CLASS = "btn";
            return Pager;
        }());
        WebApp.Pager = Pager;
    })(WebApp = Griffin.WebApp || (Griffin.WebApp = {}));
})(Griffin || (Griffin = {}));
//# sourceMappingURL=Griffin.WebApp.js.map