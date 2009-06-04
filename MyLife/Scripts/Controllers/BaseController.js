View = function(control, options) {
    if (control.jquery == null) {
        control = $(document.getElementById(control));
    }
    this.control = control;
    if (!options) {
        options = {};
    }
    this.options = $.extend({}, View.Options, options);
    this.showing = this.control.css('display') == 'block';
};
View.Options = {
    alwaysShow: false,
    onShowing: function() { },
    onShow: function() { },
    onHiding: function() { },
    onHide: function() { }
};
View.prototype.show = function() {
    this.options.onShowing();
    this.showing = true;
    this.control.show();
    this.options.onShow();
};
View.prototype.hide = function() {
    this.options.onHiding();
    this.showing = false;
    this.control.hide();
    this.options.onHide();
};
View.prototype.toString = function() {
    return this.control[0].id;
};

Controller = function() {
    this.views = [];
    this.lastView = null;
};
Controller.prototype.addView = function(view) {
    this.views.push(view);
};
Controller.prototype.hideAllViews = function() {
    var self = this;
    $(this.views).each(function(i, view) {
        if (!view.options.alwaysShow && view.showing) {
            self.lastView = view;
        }
        if (!view.options.alwaysShow) {
            view.hide();
        }
    });
};
Controller.prototype.showViewById = function(id, hideOtherViews) {
    $(this.views).each(function(i, view) {
        if (hideOtherViews || true) {
            if (view.toString() != id && !view.options.alwaysShow && view.showing) {
                view.hide();
            }
        }
        if (view.toString() == id) {
            view.show();
        }
    });
};
Controller.prototype.showLastView = function() {
    if (this.lastView) {
        $(this.views).each(function(i, view) {
            if (!view.options.alwaysShow) {
                view.hide();
            }
        });
        this.lastView.show();
    }
};
Controller.prototype.hideViewById = function(id) {
    $(this.views).each(function(i, view) {
        if (view.toString() == id) {
            view.hide();
            return false;
        }
    });
};
Controller.prototype.getViewById = function(id) {
    var retval = null;
    $(this.views).each(function(i, view) {
        if (view.toString() == id) {
            retval = view;
            return false;
        }
    });
    return retval;
};
Controller.prototype.post = function(url, data, callback, type) {
    var self = this;
    var contentType = "application/x-www-form-urlencoded";
    if (type == "json") {
        contentType += "; application/json";
    }

    $.ajax({
        url: url,
        data: data,
        type: "POST",
        dataType: type || "json",
        contentType: contentType,
        success: function(data, textStatus) {
            if (this.dataType == "json") {
                self.onCallback(data);
            }
            callback(data);
        }
    });
};
Controller.prototype.onCallback = function(result) {
    if (result.Message != null) {
        alert(result.Message);
    }

    if (result.RedirectUrl != null) {
        window.location = result.RedirectUrl;
    }
};