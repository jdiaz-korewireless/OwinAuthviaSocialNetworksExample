if (!ExtAuthDialog) {
    var ExtAuthDialog = {}
}

ExtAuthDialog.Tab = {
    open: function (url) {

        document.getElementById('extAuthDialog_iframe').src = "../../Content/ExtAuthRequest.htm#url=" + url;

        //Adjust locations
        document.getElementById('extAuthDialog_mask').style.height = ExtAuthDialog.page.theight() + "px";
        document.getElementById('extAuthDialog_mask').style.width = ExtAuthDialog.page.twidth() + "px";
        document.getElementById('extAuthDialog_wrapper').style.top = (0.10 * ExtAuthDialog.page.height()) + "px";

        //Show mask
        document.getElementById('extAuthDialog_overlay').style.display = 'block';
    },
    close: function () {
        document.getElementById('extAuthDialog_overlay').style.display = 'none';
        document.getElementById('extAuthDialog_iframe').src = "about:blank;";
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