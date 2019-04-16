<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashFlowTypeList.aspx.cs" Inherits="Kesco.App.Web.CashFlowItems.CashFlowTypeList" %>
<%@ Register TagPrefix="csg" Namespace="Kesco.Lib.Web.Controls.V4.Grid" Assembly="Controls.V4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Виды Движения Денежных Средств</title>
    <link rel="stylesheet" type="text/css" href="Kesco.CashFlowItems.css" />
    <script src="Kesco.CashFlowTypeList.js" type="text/javascript"></script>
</head>
<body>
    <form id="mvcDialogResult" action="<%= Request["callbackUrl"] %>" method="post">
        <input type="hidden" name="escaped" value="0" />
		<input type="hidden" name="control" value="" />
        <input type="hidden" name="multiReturn" value="" />
		<input type="hidden" name="value" value="" />
    </form>
    <div id="divContainer">
        <csg:Grid runat="server" ID="GridCashFlowType" MarginBottom="50" ShowGroupPanel = "True" ExistServiceColumn="True"/>   
    </div>
    
<div id="divCashFlowTypeAdd" style="display: none; padding: 2px 0 0 0;">
    <div class="v4DivTable" id="divProgressBar" style="display: none; width: 100%; height: 100%; position: absolute">
        <div class="v4DivTableRow">
            <div class="v4DivTableCell">
                <img src="/styles/ProgressBar.gif" alt="wait"/><br/><%=Resx.GetString("lblWait")%>...
            </div>
        </div>
    </div>
    <div id="divFrame">
        <iframe id="ifr" style="width:100%;" onload="setIframeHeight();"></iframe>
    </div>
</div>    
<script type="text/javascript" language="javascript">
    $(document).ready(function () {
        window.v4_insert = function () {
            _add();
        };
    });
</script>
</body>
</html>
