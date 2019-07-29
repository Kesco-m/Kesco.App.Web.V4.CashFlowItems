//Глобальные переменные должны быть инициализированы во время загрузки страницы, обычно в методе Page_Load()
var callbackUrl;
var control;
var multiReturn;

var mvc = 0;
var domain = "";

$(document).ready(function() {
    $(function() {
        /* Установить способность изменять расположение inline элементов при отображении страницы в диалоге IE */
        v4_setResizableInDialog();

        /* Умещаем дерево в пределах окна при готовности страницы */
        v4_treeViewHandleResize("tvCashFlowItem");
        $(window).resize(function() {
            /* Умещаем дерево в пределах окна при изменении размеров окна */
            v4_treeViewHandleResize("tvCashFlowItem");

            setIframeHeight();
        });
    });
});

function AddCashFlowItem(id) {
    cashFlowItem_dialogShow(cashFlowItem.AddFormTitle, idp, "0", id);
}

function EditCashFlowItem(id) {
    cashFlowItem_dialogShow(cashFlowItem.EditFormTitle, idp, id);
}

function setIframeHeight() {
    $("#ifr").height($("#divCashFlowItemAdd").height());
};

var _pageId, _Id, _parent;
// ------------------------------------------------------------------------------------------------------------
cashFlowItem_dialogShow.form = null;
var _ifrIsLoaded = false;

function cashFlowItem_dialogShow(titleForm, pageId, recId, parent) {
    if (titleForm && titleForm != "") title = titleForm;

    if (pageId && pageId != "" && recId && recId != "") {
        _pageId = pageId;
        _Id = recId;
    } else {
        _pageId = "";
        _Id = 0;
    }

    _parent = parent;

    var idContainer = "divCashFlowItemAdd";
    var width = 451;
    var height = 255;

    if (null == cashFlowItem_dialogShow.form) {
        var onOpen = function() {
            if (!_ifrIsLoaded) {
                $("#ifr").attr("src", "CashFlowItemForm.aspx?idpp=" + _pageId + "&id=" + _Id + "&parentid=" + _parent);
                _ifrIsLoaded = true;
            }
        };
        var onClose = function() { CloseDialog(cashFlowItem_dialogShow.form, null, 0); };
        var buttons = null;

        cashFlowItem_dialogShow.form =
            v4_dialog(idContainer, $("#" + idContainer), title, width, height, onOpen, onClose, buttons);
    }

    $("#" + idContainer).dialog("option", "title", title);
    cashFlowItem_dialogShow.form.dialog("open");
}

function RefreshTreeView(id, refreshParent) {
    if (refreshParent) {
        v4_reloadParentNode("tvCashFlowItem", id);
    } else {
        v4_reloadNode("tvCashFlowItem", id);
    }
}