//Глобальные переменные должны быть инициализированы во время загрузки страницы, обычно в методе Page_Load()
var callbackUrl;
var control;
var multiReturn;

var mvc = 0;
var domain = '';

function cashFlowItem_edit(id) {
    cashFlowItem_dialogShow(cashflowitem.editaction + ' ' + cashflowitem.title, idp, id);
}

function cashFlowItem_add(parent) {
    cashFlowItem_dialogShow(cashflowitem.addaction + ' ' + cashflowitem.title, idp, "0", parent);
}

function cashFlowItem_delete(id) {
    cmd("cmd", "DeleteCashFlowItem", "Id", id);
}

function cashFlowItem_return(id, name) {
    v4_returnValue(id, name);
}

function setIframeHeight() {
    $('#ifr').height($('#divCashFlowItemAdd').height());
};

$(window).resize(function () {
    setIframeHeight();
});

var _pageId, _Id, _parent;
// ------------------------------------------------------------------------------------------------------------
cashFlowItem_dialogShow.form = null;
var _ifrIsLoaded = false;
function cashFlowItem_dialogShow(titleForm, pageId, recId, parent) {
    if (titleForm && titleForm != "") title = titleForm;

    if (pageId && pageId != "" && recId && recId != "") {
        _pageId = pageId;
        _Id = recId;
        _parent = parent;
    } else {
        _pageId = "";
        _Id = 0;
        _parent = parent;
    }

    var idContainer = "divCashFlowItemAdd";
    var width = 451; var height = 255;

    if (null == cashFlowItem_dialogShow.form) {
        var onOpen = function () {
            if (!_ifrIsLoaded) {
                $("#ifr").attr('src', "CashFlowItemForm.aspx?idpp=" + _pageId + "&id=" + _Id + "&parentid=" + _parent);
                _ifrIsLoaded = true;
            }
        };
        var onClose = function () { Records_Close(null, 0); };
        var buttons = null;

        cashFlowItem_dialogShow.form = v4_dialog(idContainer, $("#" + idContainer), title, width, height, onOpen, onClose, buttons);
    }

    $("#" + idContainer).dialog("option", "title", title);
    cashFlowItem_dialogShow.form.dialog("open");
}

function Records_Close(ifrIdp, addFocus) {

    if (null == cashFlowItem_dialogShow.form) return;
    if (ifrIdp == null) {
        var _idp = $("#ifr")[0].contentWindow.idp;
        v4_closeIFrameSrc("ifr", _idp);
    }

    cashFlowItem_dialogShow.form.dialog("close");
    cashFlowItem_dialogShow.form = null;
    _ifrIsLoaded = false;

    if (addFocus != "") {
        $('#' + addFocus).focus();
    }
}

function Records_Save(ctrlFocus, reload, isnew) {
    cmd("cmd", "RefreshData", "ctrlFocus", ctrlFocus, "ReloadForm", reload, "IsNew", isnew);
}


