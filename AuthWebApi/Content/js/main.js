/* Get a list of external providers */
function getExternalProvidersList() {
    $.ajax({
        type: "GET",
        url: "api/account/externalLogins?returnUrl=%2F&generateState=true",
        dataType: 'json',
        async: true,
        success: function (data) { getExternalProvidersListCallback(data); },
        error: function (error) { alert(JSON.stringify(error)); }
    });
}

function getExternalProvidersListCallback(data) {
    var extProviders = $('#extProviders');

    $.each(data, function (key, value) {
        var img = jQuery('<img/>', {
            src: "../Content/img/" + value.name + ".png"
        });

        var div = jQuery('<div/>', {
            id: "extPr-" + value.name,
            title: value.name + ' authentication',
            onclick: "javascript:extAuth('" + value.url + "');",
        });

        img.appendTo(div);
        div.appendTo(extProviders);
    });
}

/* Authenticate a user via an external provider */
function extAuth(url)
{
    resetToken();
    $('#info').hide();

    window.location = "ExtAuthRequest#url=" + url;
}

function getUserInfo()
{
    $.ajax({
        type: "GET",
        url: "api/account/user",
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) { setRequestHeaders(xhr); },
        success: function (data) { showUserInfo(data); },
        error: function (error) { alert(JSON.stringify(error)); }
    });
}

/* Register a user authenticated by an external provider */
function registerExternal()
{
    $.ajax({
        type: "POST",
        url: "api/account/registerExternal",
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) { setRequestHeaders(xhr); },
        success: function (data) { registerExternalCallback(data); },
        error: function (error) { executeActionFailCallback(error); }
    });
}

function registerExternalCallback(data)
{
    if (!checkStatus(data))
        return;
    
    var accessToken = data["accessToken"];
    setToken(accessToken["type"], accessToken["value"]);

    showUserInfo(data["user"]);
}

/* Verify a registered user by sending him an email wiht a confirmation code */
function verify() {
    $.ajax({
        type: "POST",
        url: "api/account/verify",
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) { setRequestHeaders(xhr); },
        success: function (result) { verifyCallback(result); },
        error: function (error) { executeActionFailCallback(error); }
    });
}

function verifyCallback(result)
{
    if (!checkStatus(result))
        return;

    resetToken();

    showMessage("A confirmation email has been sent to<br/><strong>"
        + result["email"]
        + "</strong><br/><br/>"
        + "After the registration is confirmed, please, sign-in again.");
}

/* Delete a registered user and all his dependencies */
function deleteUser()
{
    $.ajax({
        type: "DELETE",
        url: "api/account/user",
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) { setRequestHeaders(xhr); },
        success: function (result) { deleteUserCallback(result); },
        error: function (error) { executeActionFailCallback(error); }
    });
}

function deleteUserCallback(result)
{
    if (!checkStatus(result))
        return;

    resetToken();

    showMessage("Account has been successfully deleted.<br/><br/>" +
        "A confirmation email has been sent to<br/><strong>"
        + result["email"]
        + "</strong>");
}

function showMessage(message)
{
    var messageHtml = "<div class=\"message\"><div>"
        + message
        + "</div></div>";

    $('#info').html(messageHtml);
}

/* Generate a table with the user's information and available actions */
function showUserInfo(userInfo)
{
    var userInfoDiv = $('#info');

    var info = "<div class=\"user\"><img src=\"" + userInfo["ava"] + "\" /></div>";
    info += "<p>" + "<strong>Email:</strong> " + userInfo["email"] + "</p>";
    info += "<p>" + "<strong>Name:</strong> " + userInfo["name"] + "</p>";
    info += "<p>" + "<strong>Provider:</strong> " + userInfo["provider"] + "</p>";

    if (userInfo["isReg"] == true)
    {
        //Registered user actions:

        //  delete account
        info += generateActionLink("Delete", "deleting", "deleteUser", "margin-right:20px");

        //  if not verified -> verify by sending an email with a confirmation code
        if (userInfo["verified"] == true)
        {
            info += "<span>Verified</span>";
        }
        else
        {
            info += generateActionLink("Verify", "verifying", "verify");
        }
    }
    else
    {
        //Register new account
        info += generateActionLink("Register", "registering", "registerExternal");
    }

    userInfoDiv.html(info);
    userInfoDiv.fadeIn(1000);
}

function generateActionLink(text, activeText, actionName, style)
{
    var styleAttr = "";
    var styleAttrProgressValue = "display:none;";

    if (style != null)
    {
        styleAttr = " style=\"" + style + "\" ";
        styleAttrProgressValue += style;
    }

    return "<span id=\"" + actionName + "\" class=\"href\"" + styleAttr
            + "onclick=\"javascript:executeAction('"
                + actionName + "',"
                + actionName + ");\">"+ text + "</span>"
            + "<span id=\"progress-" + actionName + "\" class=\"progress\" "
            + "style=\"" + styleAttrProgressValue + "\">"
            + activeText + "...</span>";
}

function executeAction(id, action)
{
    $(".href").hide();
    $("#progress-" + id).show();

    action();
}

function executeActionFailCallback(error)
{
    $(".progress").hide();
    $(".href").show();

    alert(JSON.stringify(error));
}

function checkStatus(result)
{
    if (result["status"] == "error")
    {
        executeActionFailCallback(result["statusmessage"]);
        return false;
    }

    return true;
}

/*============================= Token support functions ====================*/
var tokenTypeKey = "tokenType";
var tokenValueKey = "accessToken";

function isTokenSet()
{
    return sessionStorage.getItem(tokenTypeKey) != null
        && sessionStorage.getItem(tokenValueKey) != null;
}

function resetToken()
{
    sessionStorage.removeItem(tokenTypeKey);
    sessionStorage.removeItem(tokenValueKey);
}

function setToken(type, value)
{
    sessionStorage.setItem(tokenTypeKey, type);
    sessionStorage.setItem(tokenValueKey, value);
}

function setRequestHeaders(xhr)
{
    xhr.setRequestHeader("Authorization",
        sessionStorage.getItem(tokenTypeKey)
        + " "
        + sessionStorage.getItem(tokenValueKey));
}