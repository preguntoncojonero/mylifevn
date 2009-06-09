MyLifeController = function() {
    $.extend(this, new Controller());
};
MyLifeController.prototype.resetPassword = function() {
    var self = $("#fResetPassword");
    if (!self.validate().form()) {
        return false;
    }
    var action = self.attr("action");
    var data = self.serialize();
    this.post(action, data, function(result) {
    });
};
MyLifeController.prototype.changePassword = function() {
    var self = $("#fChangePassword");
    if (!self.valid()) {
        return false;
    }
    var action = self.attr("action");
    var data = self.serialize();
    this.post(action, data, function(result) {
    });
};
MyLifeController.prototype.updateProfile = function() {
    var self = $("#fProfile");
    if (!self.valid()) {
        return false;
    }
    var action = self.attr("action");
    var data = self.serialize();
    this.post(action, data, function(result) {
    });
};
MyLifeController.prototype.sendContact = function() {
    var self = $("#fContact");
    if (!self.valid()) {
        return false;
    }
    var action = self.attr("action");
    var data = self.serialize();
    this.post(action, data, function(result) { });
};