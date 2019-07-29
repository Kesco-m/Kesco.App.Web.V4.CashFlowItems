<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="Kesco.App.Web.CashFlowItems.Search" %>
<%@ Register TagPrefix="cstv" Namespace="Kesco.Lib.Web.Controls.V4.TreeView" Assembly="Controls.V4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title><%= Resx.GetString("Cfi_lblCashFlowItems") %></title>
    <link rel="stylesheet" type="text/css" href="Kesco.CashFlowItems.css"/>
    <script src="Kesco.Search.js" type="text/javascript"></script>
    <script src="Kesco.CashFlowItems.js" type="text/javascript"></script>
</head>
<body>
<form id="mvcDialogResult" method="post">
    <input type="hidden" name="escaped" value="0"/>
    <input type="hidden" name="control" value=""/>
    <input type="hidden" name="multiReturn" value=""/>
    <input type="hidden" name="value" value=""/>
</form>

<div style="display: none; font-weight: bold; margin-left: 11px; margin-top: 10px;"><%= Resx.GetString("Cfi_lblCashFlowItems") %></div>
<div id="divContainer">
    <div id="divMyTreeContainer" class="ui-widget-content">
        <cstv:TreeView runat="server" ID="tvCashFlowItem"/>
    </div>
</div>

<div id="divCashFlowItemAdd" style="display: none; padding: 2px 0 0 0;">
    <div class="v4DivTable" id="divProgressBar" style="display: none; height: 100%; position: absolute; width: 100%;">
        <div class="v4DivTableRow">
            <div class="v4DivTableCell">
                <img src="/styles/ProgressBar.gif" alt="wait"/><br/><%= Resx.GetString("lblWait") %>...
            </div>
        </div>
    </div>
    <div id="divFrame">
        <iframe id="ifr" style="width: 100%;" onload="setIframeHeight();"></iframe>
    </div>
</div>
</body>

<script type="text/javascript">
    $(document).ready(function () {
        if (window.dialogWidth && window.dialogHeight) {
            window.dialogWidth = "650px";
            window.dialogHeight = "600px";
        }

        v4_setResizableInDialog();

        /* Умещаем дерево в пределах окна, в зависимости от высоты других элементов (второй параметр), при готовности страницы и при изменении размеров окна */
        v4_treeViewHandleResize('tvCashFlowItem');
        $(window).resize(function() {
            v4_treeViewHandleResize('tvCashFlowItem');
        });

    });

</script>

</html>