/**
    Module P: Generic Promises for TypeScript

    Project, documentation, and license: https://github.com/pragmatrix/Promise
*/
var P;
(function (P) {
    /**
        Returns a new "Deferred" value that may be resolved or rejected.
    */
    function defer() {
        return new DeferredI();
    }
    P.defer = defer;
    /**
        Converts a value to a resolved promise.
    */
    function resolve(v) {
        return defer().resolve(v).promise();
    }
    P.resolve = resolve;
    /**
        Returns a rejected promise.
    */
    function reject(err) {
        return defer().reject(err).promise();
    }
    P.reject = reject;
    /**
        http://en.wikipedia.org/wiki/Anamorphism

        Given a seed value, unfold calls the unspool function, waits for the returned promise to be resolved, and then
        calls it again if a next seed value was returned.

        All the values of all promise results are collected into the resulting promise which is resolved as soon
        the last generated element value is resolved.
    */
    function unfold(unspool, seed) {
        var d = defer();
        var elements = new Array();
        unfoldCore(elements, d, unspool, seed);
        return d.promise();
    }
    P.unfold = unfold;
    function unfoldCore(elements, deferred, unspool, seed) {
        var result = unspool(seed);
        if (!result) {
            deferred.resolve(elements);
            return;
        }
        // fastpath: don't waste stack space if promise resolves immediately.
        while (result.next && result.promise.status == Status.Resolved) {
            elements.push(result.promise.result);
            result = unspool(result.next);
            if (!result) {
                deferred.resolve(elements);
                return;
            }
        }
        result.promise
            .done(function (v) {
            elements.push(v);
            if (!result.next)
                deferred.resolve(elements);
            else
                unfoldCore(elements, deferred, unspool, result.next);
        })
            .fail(function (e) {
            deferred.reject(e);
        });
    }
    /**
        The status of a Promise. Initially a Promise is Unfulfilled and may
        change to Rejected or Resolved.
     
        Once a promise is either Rejected or Resolved, it can not change its
        status anymore.
    */
    (function (Status) {
        Status[Status["Unfulfilled"] = 0] = "Unfulfilled";
        Status[Status["Rejected"] = 1] = "Rejected";
        Status[Status["Resolved"] = 2] = "Resolved";
    })(P.Status || (P.Status = {}));
    var Status = P.Status;
    /**
        Creates a promise that gets resolved when all the promises in the argument list get resolved.
        As soon one of the arguments gets rejected, the resulting promise gets rejected.
        If no promises were provided, the resulting promise is immediately resolved.
    */
    function when() {
        var promises = [];
        for (var _i = 0; _i < arguments.length; _i++) {
            promises[_i - 0] = arguments[_i];
        }
        var allDone = defer();
        if (!promises.length) {
            allDone.resolve([]);
            return allDone.promise();
        }
        var resolved = 0;
        var results = [];
        promises.forEach(function (p, i) {
            p
                .done(function (v) {
                results[i] = v;
                ++resolved;
                if (resolved === promises.length && allDone.status !== Status.Rejected)
                    allDone.resolve(results);
            })
                .fail(function (e) {
                if (allDone.status !== Status.Rejected)
                    allDone.reject(new Error("when: one or more promises were rejected"));
            });
        });
        return allDone.promise();
    }
    P.when = when;
    /**
        Implementation of a promise.

        The Promise<Value> instance is a proxy to the Deferred<Value> instance.
    */
    var PromiseI = (function () {
        function PromiseI(deferred) {
            this.deferred = deferred;
        }
        Object.defineProperty(PromiseI.prototype, "status", {
            get: function () { return this.deferred.status; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PromiseI.prototype, "result", {
            get: function () { return this.deferred.result; },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(PromiseI.prototype, "error", {
            get: function () { return this.deferred.error; },
            enumerable: true,
            configurable: true
        });
        PromiseI.prototype.done = function (f) {
            this.deferred.done(f);
            return this;
        };
        PromiseI.prototype.fail = function (f) {
            this.deferred.fail(f);
            return this;
        };
        PromiseI.prototype.always = function (f) {
            this.deferred.always(f);
            return this;
        };
        PromiseI.prototype.then = function (f) {
            return this.deferred.then(f);
        };
        return PromiseI;
    }());
    /**
        Implementation of a deferred.
    */
    var DeferredI = (function () {
        function DeferredI() {
            this._resolved = function (_) { };
            this._rejected = function (_) { };
            this._status = Status.Unfulfilled;
            this._error = { message: "" };
            this._promise = new PromiseI(this);
        }
        DeferredI.prototype.promise = function () {
            return this._promise;
        };
        Object.defineProperty(DeferredI.prototype, "status", {
            get: function () {
                return this._status;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(DeferredI.prototype, "result", {
            get: function () {
                if (this._status != Status.Resolved)
                    throw new Error("Promise: result not available");
                return this._result;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(DeferredI.prototype, "error", {
            get: function () {
                if (this._status != Status.Rejected)
                    throw new Error("Promise: rejection reason not available");
                return this._error;
            },
            enumerable: true,
            configurable: true
        });
        DeferredI.prototype.then = function (f) {
            var d = defer();
            this
                .done(function (v) {
                var promiseOrValue = f(v);
                // todo: need to find another way to check if r is really of interface
                // type Promise<any>, otherwise we would not support other 
                // implementations here.
                if (promiseOrValue instanceof PromiseI) {
                    var p = promiseOrValue;
                    p.done(function (v2) { return d.resolve(v2); })
                        .fail(function (err) { return d.reject(err); });
                    return p;
                }
                d.resolve(promiseOrValue);
            })
                .fail(function (err) { return d.reject(err); });
            return d.promise();
        };
        DeferredI.prototype.done = function (f) {
            if (this.status === Status.Resolved) {
                f(this._result);
                return this;
            }
            if (this.status !== Status.Unfulfilled)
                return this;
            var prev = this._resolved;
            this._resolved = function (v) {
                prev(v);
                f(v);
            };
            return this;
        };
        DeferredI.prototype.fail = function (f) {
            if (this.status === Status.Rejected) {
                f(this._error);
                return this;
            }
            if (this.status !== Status.Unfulfilled)
                return this;
            var prev = this._rejected;
            this._rejected = function (e) {
                prev(e);
                f(e);
            };
            return this;
        };
        DeferredI.prototype.always = function (f) {
            this
                .done(function (v) { return f(v); })
                .fail(function (err) { return f(null, err); });
            return this;
        };
        DeferredI.prototype.resolve = function (result) {
            if (this._status !== Status.Unfulfilled)
                throw new Error("tried to resolve a fulfilled promise");
            this._result = result;
            this._status = Status.Resolved;
            this._resolved(result);
            this.detach();
            return this;
        };
        DeferredI.prototype.reject = function (err) {
            if (this._status !== Status.Unfulfilled)
                throw new Error("tried to reject a fulfilled promise");
            this._error = err;
            this._status = Status.Rejected;
            this._rejected(err);
            this.detach();
            return this;
        };
        DeferredI.prototype.detach = function () {
            this._resolved = function (_) { };
            this._rejected = function (_) { };
        };
        return DeferredI;
    }());
    function generator(g) {
        return function () { return iterator(g()); };
    }
    P.generator = generator;
    ;
    function iterator(f) {
        return new IteratorI(f);
    }
    P.iterator = iterator;
    var IteratorI = (function () {
        function IteratorI(f) {
            this.f = f;
            this.current = undefined;
        }
        IteratorI.prototype.advance = function () {
            var _this = this;
            var res = this.f();
            return res.then(function (value) {
                if (isUndefined(value))
                    return false;
                _this.current = value;
                return true;
            });
        };
        return IteratorI;
    }());
    /**
        Iterator functions.
    */
    function each(gen, f) {
        var d = defer();
        eachCore(d, gen(), f);
        return d.promise();
    }
    P.each = each;
    function eachCore(fin, it, f) {
        it.advance()
            .done(function (hasValue) {
            if (!hasValue) {
                fin.resolve({});
                return;
            }
            f(it.current);
            eachCore(fin, it, f);
        })
            .fail(function (err) { return fin.reject(err); });
    }
    /**
        std
    */
    function isUndefined(v) {
        return typeof v === "undefined";
    }
    P.isUndefined = isUndefined;
})(P || (P = {}));
//# sourceMappingURL=Promise.js.map