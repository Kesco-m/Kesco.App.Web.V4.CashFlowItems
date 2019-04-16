<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CashFlowTypeForm.aspx.cs" Inherits="Kesco.App.Web.CashFlowItems.CashFlowTypeForm" %>
<%@ Register TagPrefix="v4control" Namespace="Kesco.Lib.Web.Controls.V4" Assembly="Controls.V4" %>

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
        <div id="IDPanel" class="predicate_block">
            <div class="label"><%=Resx.GetString("Cfi_lblCode")%>:</div>
            <v4control:Number ID="efId" runat="server" Width="50px" IsRequired="True" CSSClass="aligned_control" NextControl="efId" />
        </div>
            
        <div id="NamePanel" class="predicate_block">
            <div class="label"><%=Resx.GetString("Cfi_lblName")%>:</div>
            <v4control:TextBox ID="efName" runat="server" Width="370px" IsRequired="True" CSSClass="aligned_control" NextControl="efName1C"/>
        </div>
        
        <div id="Name1CPanel" class="predicate_block">
            <div class="label"><%=Resx.GetString("Cfi_lblNameIn1C")%>:</div>
            <v4control:TextBox ID="efName1C" runat="server" Width="370px" IsRequired="True" CSSClass="aligned_control" NextControl="efName"/>
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
