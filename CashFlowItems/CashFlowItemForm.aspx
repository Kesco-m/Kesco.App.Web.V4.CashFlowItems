<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashFlowItemForm.aspx.cs" Inherits="Kesco.App.Web.CashFlowItems.CashFlowItemForm" %>
<%@ Register TagPrefix="v4control" Namespace="Kesco.Lib.Web.Controls.V4" Assembly="Controls.V4" %>
<%@ Register TagPrefix="v4dbselect" Namespace="Kesco.Lib.Web.DBSelect.V4" Assembly="DBSelect.V4" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" type="text/css" href="Kesco.CashFlowItems.css" />
</head>
<body>
<div class="marginD"><%=RenderDocumentHeader()%></div>    
    <div class="v4formContainer"  >
        <div class="marginL">
            
        <v4control:Div runat="server" ID="divCashFlowItemPatch"></v4control:Div>

        <div class="predicate_block">
        <div class="label" style="width: 50px;"><%=Resx.GetString("Cfi_lblItem")%>:</div>
            <v4control:TextBox ID="tbCashFlowItemName" runat="server" Width="350px" IsRequired="True" NextControl="tbCashFlowItemType"></v4control:TextBox>
        </div>

        <div class="predicate_block">
        <div class="label" style="width: 50px;"><%=Resx.GetString("Cfi_lblType")%>:</div>
            <v4dbselect:DBSCashFlowType ID="tbCashFlowItemType" runat="server" Width="330px" IsRequired="True" IsAlwaysAdvancedSearch="True" AutoSetSingleValue="True" CSSClass="aligned_control" NextControl="tbCashFlowItemName"/>
        </div>

        <div class="footer">
            <v4control:Changed ID="efChanged" runat="server"/>
        </div>

     </div>
</div>
<script type="text/javascript" language="javascript">
$(document).ready(function () {
    window.v4_save = function () {
        $("#btnSave").focus();
         cmd('cmd', 'SaveData');
    };
});
</script>
</body>
</html>
