function AddCashFlowType() {
    cashFlow_RecordsAdd(cashflowtype.addaction + " " + cashflowtype.title, idp, "0");
}

function EditCashFlowType(id) {
    cashFlow_RecordsAdd(cashflowtype.editaction + " " + cashflowtype.title, idp, id);
}

function DeleteCashFlowType(id) {
    cmd("cmd", "DeleteCashFlowType", "Id", id);
}

function setIframeHeight() {
    $("#ifr").height($("#divCashFlowTypeAdd").height());
};

function returnValue(id, name) {
    v4_returnValue(id, name);
} 

$(window).resize(function() {
    setIframeHeight();
});

var _pageId, _Id;
//------------------------------------------------------------------------------------------------------------
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
    var width = 531;
    var height = 200;

    if (null == cashFlow_RecordsAdd.form) {
        var onOpen = function() {
            if (!_ifrIsLoaded) {
                $("#ifr").attr("src", "CashFlowTypeForm.aspx?idpp=" + _pageId + "&id=" + _Id);
                _ifrIsLoaded = true;
            }
        };
        var onClose = function() { CloseDialog(cashFlow_RecordsAdd.form, null, 0); };
        var buttons = null;

        cashFlow_RecordsAdd.form =
            v4_dialog(idContainer, $("#" + idContainer), title, width, height, onOpen, onClose, buttons);
    }

    $("#" + idContainer).dialog("option", "title", title);
    cashFlow_RecordsAdd.form.dialog("open");
}

function RefreshData(isAddNew) {
    cmd("cmd", "RefreshDataGrid", "IsAddNew", isAddNew);
}