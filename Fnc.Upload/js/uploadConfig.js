var uploadConfig = function () {
    this.getOrigin = function () {
        var result;
        if (!window.location.origin) {
            result = window.location.protocol + "//" + window.location.hostname + (window.location.port ? ':' + window.location.port : '');
        } else {
            result = window.location.origin;
        }
        return result;
    };
};

uploadConfig.prototype = (function () {
    return {
        origin: function () {
            return this.getOrigin();
        }
    };
}());