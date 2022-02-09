"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Guide = void 0;
var Guide = /** @class */ (function () {
    function Guide() {
        var _this = this;
        this.button = null;
        this.guideContainer = null;
        this.dimScreenElement = null;
        this.closed = function () { };
        this.showNext = function () { };
        this.dimScreenElement = document.createElement("div");
        this.dimScreenElement.className = "dimScreen";
        this.guideContainer = document.getElementById("note");
        if (!this.guideContainer) {
            throw new Error("Failed to find an element with id 'note'.");
        }
        var button = document.getElementById("guideCloseBtn");
        button.addEventListener("click", function () {
            _this.reset();
            if (_this.closed) {
                _this.closed();
            }
        });
        this.nextButton = document.getElementById("guideNextBtn");
        this.nextButton.addEventListener('click', function (x) { return _this.onClickNext(); });
    }
    /**
     * Show a guide
     * @param elementToHighlight Element which the guide is for.
     * @param title Title in the guide
     * @param body Guide body (i.e. the explanation).
     * @param hasMore There are more guides available on this page.
     */
    Guide.prototype.show = function (elementToHighlight, title, body, hasMore) {
        if (!elementToHighlight) {
            throw new Error("Element must be specified.");
        }
        if (!title) {
            throw new Error("title must be specified.");
        }
        if (!body) {
            throw new Error("body must be specified.");
        }
        if (this.elementToHighlight) {
            this.reset();
        }
        if (typeof elementToHighlight === "string") {
            if (elementToHighlight.substr(0, 1) === "#") {
                elementToHighlight = elementToHighlight.substr(1);
            }
            this.elementToHighlight = document.getElementById(elementToHighlight);
            if (!this.elementToHighlight) {
                throw new Error("Failed to find element " + elementToHighlight);
            }
        }
        else {
            this.elementToHighlight = elementToHighlight;
        }
        if (hasMore) {
            this.nextButton.style.display = '';
        }
        else {
            this.nextButton.style.display = 'none';
        }
        document.body.appendChild(this.dimScreenElement);
        this.dimScreenElement.style.display = "block";
        this.guideContainer.style.display = "block";
        var elementRect = this.elementToHighlight.getBoundingClientRect();
        this.guideContainer.children[0].innerHTML = title;
        var textStr = "";
        if ((typeof body === "string" && body.substr(0, 1) === '#')) {
            textStr = document.getElementById(body.substr(1)).innerHTML;
        }
        else if (typeof body !== "string") {
            textStr = body.innerHTML;
        }
        else {
            textStr = body.replace(/(?:\r\n|\r|\n)/g, "<br>");
        }
        this.guideContainer.children[1].innerHTML = textStr;
        var note = this.guideContainer;
        var noteRect = note.getBoundingClientRect();
        var bodyRect = document.body.getBoundingClientRect();
        if (noteRect.width + elementRect.right > bodyRect.right) {
            note.style.left = (elementRect.right - noteRect.width) + "px";
        }
        else {
            note.style.left = elementRect.left + "px";
        }
        if (noteRect.height + elementRect.bottom > bodyRect.bottom) {
            note.style.top = (elementRect.top - noteRect.height - 10) + "px";
        }
        else {
            note.style.top = (elementRect.bottom + 10) + "px";
        }
        this.highlight();
    };
    Guide.prototype.highlight = function () {
        this.elementToHighlight.classList.add("guide-highlight");
    };
    Guide.prototype.reset = function () {
        this.elementToHighlight.classList.remove("guide-highlight");
        this.elementToHighlight = null;
        document.body.removeChild(this.dimScreenElement);
        this.guideContainer.style.display = "none";
        this.guideContainer.style.top = "";
        this.guideContainer.style.bottom = "";
        this.guideContainer.style.left = "";
        this.guideContainer.style.right = "";
        this.dimScreenElement.style.display = "none";
    };
    Guide.prototype.onClickNext = function () {
        this.showNext();
    };
    return Guide;
}());
exports.Guide = Guide;
//# sourceMappingURL=Guide.js.map