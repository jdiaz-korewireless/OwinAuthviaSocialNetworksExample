if (!ExtAuthDialog) {
    var ExtAuthDialog = {}
}

ExtAuthDialog.Tab = {
    open: function (url) {

        window.open("ExtAuthRequest#url=" + url,
            "External Provider Authentication",
            "top=" + (0.25 * ExtAuthDialog.page.height()) + "px," +
            "left=" + (0.35 * ExtAuthDialog.page.width()) + "px," +
            "scrollbars=yes," +
            "resizable=yes," +
            "width=750px," +
            "height=550px");
    },
    close: function (extAuthWindow)
    {
        extAuthWindow.close();
    }
}

ExtAuthDialog.page = function () {
    return {
        top: function () { return document.body.scrollTop || document.documentElement.scrollTop },
        width: function () { return self.innerWidth || document.documentElement.clientWidth },
        height: function () { return self.innerHeight || document.documentElement.clientHeight },
        theight: function () {
            var d = document, b = d.body, e = d.documentElement;
            return Math.max(Math.max(b.scrollHeight, e.scrollHeight), Math.max(b.clientHeight, e.clientHeight))
        },
        twidth: function () {
            var d = document, b = d.body, e = d.documentElement;
            return Math.max(Math.max(b.scrollWidth, e.scrollWidth), Math.max(b.clientWidth, e.clientWidth))
        }
    }
}();