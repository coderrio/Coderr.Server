"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Dropdown = void 0;
var Dropdown = /** @class */ (function () {
    function Dropdown(selectorOrElement) {
        var _this = this;
        this.visible = false;
        if (typeof selectorOrElement === "string") {
            this.menu = document.querySelector(selectorOrElement);
        }
        else if (selectorOrElement) {
            this.menu = selectorOrElement;
        }
        else {
            throw new Error("Not an element nor selector.");
        }
        window.addEventListener('resize', function () {
            _this.reposition();
        });
        window.addEventListener('click', function (e) {
            if (e.target === _this.menuTrigger) {
                return;
            }
            if (!_this.isDescendant(_this.menu, e.target)) {
                _this.hide();
            }
        });
    }
    Dropdown.prototype.bindClick = function (selectorOrElement) {
        var _this = this;
        if (typeof selectorOrElement === "string") {
            this.menuTrigger = document.querySelector(selectorOrElement);
        }
        else if (selectorOrElement) {
            this.menuTrigger = selectorOrElement;
        }
        else {
            throw new Error("Not an element nor selector.");
        }
        this.menuTrigger.addEventListener('click', function (e) {
            e.preventDefault();
            if (_this.visible) {
                _this.hide();
            }
            else {
                _this.show();
            }
            _this.visible = !_this.visible;
        });
    };
    Dropdown.prototype.hide = function () {
        this.menu.classList.remove('shown');
    };
    Dropdown.prototype.show = function () {
        this.reposition();
        this.menu.classList.add('shown');
    };
    Dropdown.prototype.reposition = function () {
        var triggerRect = this.menuTrigger.getBoundingClientRect();
        var menuRect = this.menu.getBoundingClientRect();
        if (triggerRect.left + menuRect.width + 10 > window.innerWidth) {
            this.menu.style.left = (triggerRect.right - menuRect.width) + "px";
            this.menu.style.top = triggerRect.bottom + 5 + "px";
        }
        else {
            this.menu.style.left = triggerRect.left + "px";
            this.menu.style.top = triggerRect.bottom + 5 + "px";
        }
    };
    Dropdown.prototype.isDescendant = function (parent, child) {
        var node = child.parentNode;
        while (node != null) {
            if (node === parent) {
                return true;
            }
            node = node.parentNode;
        }
        return false;
    };
    return Dropdown;
}());
exports.Dropdown = Dropdown;
//# sourceMappingURL=Dropdown.js.map