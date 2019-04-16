function _edit(id) {
    cashFlow_RecordsAdd(cashflowtype.editaction + ' ' + cashflowtype.title, idp, id);
}

function _add() {
    cashFlow_RecordsAdd(cashflowtype.addaction + ' ' + cashflowtype.title, idp, "0");
}

function _delete(id) {
    cmd("cmd", "Delete", "Id", id);
}

function _return(id, name) {
    v4_returnValue(id, name);
}


function setIframeHeight() {
    $('#ifr').height($('#divCashFlowTypeAdd').height());
};

$(window).resize(function () {
    setIframeHeight();
});

var _pageId, _Id;
// ------------------------------------------------------------------------------------------------------------
cashFlow_RecordsAdd.form = null;
var _ifrIsLoaded = false;
function cashFlow_RecordsAdd(titleForm, pageId, recId) {
    if (titleForm && titleForm != "") title = titleForm;

    if (pageId && pageId != "" && recId && recId != "") {
        _pageId = pageId;
        _Id = recId;
    } else {
        _pageId = "";
        _Id = 0;
    }

    var idContainer = "divCashFlowTypeAdd";
    var width = 531; var height = 200;

    if (null == cashFlow_RecordsAdd.form) {
        var onOpen = function () {
            if (!_ifrIsLoaded) {
                $("#ifr").attr('src', "CashFlowTypeForm.aspx?idpp=" + _pageId + "&id=" + _Id);
                _ifrIsLoaded = true;
            }
        };
        var onClose = function () { Records_Close(null, 0); };
        var buttons = null;

        cashFlow_RecordsAdd.form = v4_dialog(idContainer, $("#" + idContainer), title, width, height, onOpen, onClose, buttons);
    }

    $("#" + idContainer).dialog("option", "title", title);
    cashFlow_RecordsAdd.form.dialog("open");
}

function Records_Close(ifrIdp, addFocus) {

    if (null == cashFlow_RecordsAdd.form) return;
    if (ifrIdp == null) {
        var _idp = $("#ifr")[0].contentWindow.idp;
        v4_closeIFrameSrc("ifr", _idp);
    }

    cashFlow_RecordsAdd.form.dialog("close");
    cashFlow_RecordsAdd.form = null;
    _ifrIsLoaded = false;

    if (addFocus != "") {
        $('#' + addFocus).focus();
    }
}

function Records_Save(ctrlFocus, reload, isnew) {
    cmd("cmd", "RefreshData", "ctrlFocus", ctrlFocus, "ReloadForm", reload, "IsNew", isnew);
}

