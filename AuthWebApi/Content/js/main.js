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
function extAuth(url) {
    $('#info').hide();
    ExtAuthDialog.Tab.open(url);
}

function extAuthCallback(extAuthWindow, tokenType, accessToken) {
    ExtAuthDialog.Tab.close(extAuthWindow);

    $.ajax({
        type: "GET",
        url: "api/account/user",
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", tokenType + " " + accessToken);
        },
        success: function (data) { showUserInfo(tokenType, accessToken, data); },
        error: function (error) { alert(JSON.stringify(error)); }
    });
}

/* Register a user authenticated by an external provider */
function registerExternal(tokenType, accessToken) {
    $.ajax({
        type: "POST",
        url: "api/account/registerExternal",
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", tokenType + " " + accessToken);
        },
        success: function (data) { registerExternalCallback(data); },
        error: function (error) { executeActionFailCallback(error); }
    });
}

function registerExternalCallback(data)
{
    if (!checkStatus(data))
        return;

    var accessToken = data["accessToken"];
    showUserInfo(accessToken["type"], accessToken["value"], data["user"]);
}

/* Verify a registered user by sending him an email wiht a confirmation code */
function verify(tokenType, accessToken) {
    $.ajax({
        type: "POST",
        url: "api/account/verify",
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", tokenType + " " + accessToken);
        },
        success: function (data) { showUserInfo(tokenType, accessToken, data); },
        error: function (error) { executeActionFailCallback(error); }
    });
}

/* Delete a registered user and all his dependencies */
function deleteUser(tokenType, accessToken) {
    $.ajax({
        type: "DELETE",
        url: "api/account/user",
        dataType: 'json',
        async: true,
        beforeSend: function (xhr) {
            xhr.setRequestHeader("Authorization", tokenType + " " + accessToken);
        },
        success: function (result) { deleteUserCallback(result); },
        error: function (error) { executeActionFailCallback(error); }
    });
}

function deleteUserCallback(result)
{
    if (!checkStatus(result))
        return;

    showMessage("Account has been successfully deleted.<br/>" +
        "A confirmation letter has been sent to your email.");
}

function showMessage(message)
{
    var messageHtml = "<div class=\"message\"><div>"
        + message
        + "</div></div>";

    $('#info').html(messageHtml);
}

/* Generate a table with the user's information and available actions */
function showUserInfo(tokenType, accessToken, userInfo)
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
        info += generateActionLink("Delete", "deleting", "deleteUser", tokenType, accessToken, "margin-right:20px");

        //  if not verified -> verify by sending an email with a confirmation code
        if (userInfo["verified"] == true)
        {
            info += "<span>Verified</span>";
        }
        else
        {
            info += generateActionLink("Verify", "verifying", "verify", tokenType, accessToken);
        }
    }
    else
    {
        //Register new account
        info += generateActionLink("Register", "registering", "registerExternal", tokenType, accessToken);
    }

    userInfoDiv.html(info);
    userInfoDiv.fadeIn(1000);
}

function generateActionLink(text, activeText, actionName, tokenType, accessToken, style)
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
                + actionName + ",'"
                + tokenType + "', '"
                + accessToken + "');\">"+ text + "</span>"
            + "<span id=\"progress-" + actionName + "\" class=\"progress\" "
            + "style=\"" + styleAttrProgressValue + "\">"
            + activeText + "...</span>";
}

function executeAction(id, action, tokenType, accessToken)
{
    $(".href").hide();
    $("#progress-" + id).show();

    action(tokenType, accessToken);
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