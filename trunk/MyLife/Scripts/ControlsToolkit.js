// Common functions
function getClientBounds() {
    var clientWidth; var clientHeight;
    if ($.browser.msie) {
        clientWidth = document.documentElement.clientWidth;
        clientHeight = document.documentElement.clientHeight;
    }
    else if ($.browser.safari) {
        clientWidth = window.innerWidth;
        clientHeight = window.innerHeight;
    }
    else if ($.browser.opera) {
        clientWidth = Math.min(window.innerWidth, document.body.clientWidth);
        clientHeight = Math.min(window.innerHeight, document.body.clientHeight);
    } else {
        clientWidth = Math.min(window.innerWidth, document.documentElement.clientWidth);
        clientHeight = Math.min(window.innerHeight, document.documentElement.clientHeight);
    }

    return {
        width: clientWidth,
        height: clientHeight
    };
};

// Always Visible Control
AlwaysVisibleControl = function(el, options) {
    if (el.jquery == null) {
        el = $(document.getElementById(el));
    }
    this.element = el;
    if (!options) {
        options = {};
    }
    this.options = $.extend({}, AlwaysVisibleControl.Options, options);
    this.options.animate = false;
    this.initialize();
};
AlwaysVisibleControl.prototype.initialize = function() {
    if ($.browser.msie && $.browser.version < 7) {
        this.options.animate = true;
    }

    if (this.options.animate) {
        this.element.css("position", "absolute");
    } else {
        this.element.css("position", "fixed");
    }

    $(window).bind("resize", this, this.repositionHandler);
    if (this.options.animate) {
        $(window).bind("scroll", this, this.repositionHandler);
    }

    var e = jQuery.Event("none");
    e.data = this;
    this.repositionHandler(e);
};
AlwaysVisibleControl.prototype.dispose = function() {
    if (this.options.animate) {
        $(window).unbind("scroll", this.repositionHandler);
    }
    $(window).unbind("resize", this.repositionHandler);
};
AlwaysVisibleControl.prototype.repositionHandler = function(eventObject) {
    var x = 0; var y = 0;
    if (eventObject.data.options.animate) {
        if (document.documentElement && document.documentElement.scrollTop) {
            x = document.documentElement.scrollLeft;
            y = document.documentElement.scrollTop;
        } else {
            x = document.body.scrollLeft;
            y = document.body.scrollTop;
        }
    }

    var clientBounds = getClientBounds();
    var width = clientBounds.width;
    var height = clientBounds.height;
    switch (eventObject.data.options.horizontalSide) {
        case AlwaysVisibleControl.HorizontalSide.Center:
            x = Math.max(0, Math.floor(x + width / 2.0 - eventObject.data.element.width() / 2.0 - eventObject.data.options.horizontalOffset));
            break;
        case AlwaysVisibleControl.HorizontalSide.Right:
            x = Math.max(0, x + width - eventObject.data.element.width() - eventObject.data.options.horizontalOffset);
            break;
        case AlwaysVisibleControl.HorizontalSide.Left:
        default:
            x += eventObject.data.options.horizontalOffset;
            break;
    }

    switch (eventObject.data.options.verticalSide) {
        case AlwaysVisibleControl.VerticalSide.Middle:
            y = Math.max(0, Math.floor(y + height / 2.0 - eventObject.data.element.height() / 2.0 - eventObject.data.options.verticalOffset));
            break;
        case AlwaysVisibleControl.VerticalSide.Bottom:
            y = Math.max(0, y + height - eventObject.data.element.height() - eventObject.data.options.verticalOffset);
            break;
        case AlwaysVisibleControl.VerticalSide.Top:
        default:
            y += eventObject.data.options.verticalOffset;
            break;
    }

    eventObject.data.element.css("left", x + "px").css("top", y + "px");
    if (eventObject.data.options.repositionHandler) {
        eventObject.data.options.repositionHandler();
    }
};
AlwaysVisibleControl.HorizontalSide = {
    Left: 0,
    Center: 1,
    Right: 2
};
AlwaysVisibleControl.VerticalSide = {
    Top: 0,
    Middle: 1,
    Bottom: 2
};
AlwaysVisibleControl.Options = {
    horizontalSide: AlwaysVisibleControl.HorizontalSide.Center,
    verticalSide: AlwaysVisibleControl.VerticalSide.Middle,
    horizontalOffset: 0,
    verticalOffset: 0,
    repositionHandler: null
};

// Ajax Loading
AjaxLoading = function(el) {
    if (el.jquery == null) {
        el = $(document.getElementById(el));
    }
    this.element = el;
    this.element.addClass("ajaxLoading");
    this.element.hide();
    this.alwaysVisibleControl = null;
    var self = this;
    $(this).ajaxStart(function() {
        self.show();
    });

    $(this).ajaxStop(function() {
        self.hide();
    });
};
AjaxLoading.prototype.show = function() {
    this.element.show();
    this.alwaysVisibleControl = new AlwaysVisibleControl(this.element, { verticalOffset: 100 });
};
AjaxLoading.prototype.hide = function() {
    if (this.alwaysVisibleControl) {
        this.alwaysVisibleControl.dispose();
    }
    this.element.hide();
};

// Message Box
MessageBox = function(el) {
    if (el.jquery == undefined) {
        el = $(document.getElementById(el));
    }
    this.element = el;
    el.hide();
    el.removeClass();
};
MessageBox.prototype.showInfo = function(text) {
    this.element.removeClass();
    this.element.addClass("message-box message-box-info");
    this.element.text(text);
    this.element.show();
};
MessageBox.prototype.showError = function(text) {
    this.element.removeClass();
    this.element.addClass("message-box message-box-error");
    this.element.text(text);
    this.element.show();
};
MessageBox.prototype.showWait = function(text) {
    this.element.removeClass();
    this.element.addClass("message-box message-box-wait");
    this.element.text(text);
    this.element.show();
};
MessageBox.prototype.hide = function(delay) {
    if (delay) {
        var self = this;
        setTimeout(function() {
            self.element.hide();
        }, delay);
    } else {
        this.element.hide();
    }
};