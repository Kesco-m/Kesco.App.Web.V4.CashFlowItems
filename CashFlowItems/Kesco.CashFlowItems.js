function CloseDialog(form, ifrIdp, addFocus) {

    if (null == form) return;
    if (ifrIdp == null) {
        var _idp = $("#ifr")[0].contentWindow.idp;
        v4_closeIFrameSrc("ifr", _idp);
    }

    form.dialog("close");
    form = null;
    _ifrIsLoaded = false;

    if (addFocus != "") {
        $("#" + addFocus).focus();
    }
}