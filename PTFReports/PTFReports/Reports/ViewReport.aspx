<%@ Page Language="C#" AutoEventWireup="true" EnableViewState="true" CodeBehind="ViewReport.aspx.cs"
    Inherits="PTF.Reports.ViewReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>View Report</title>
    <style type="text/css">
        html
        {
            height: 100%;
        }
        body
        {
            height: 100%;
            font-size: .75em;
            font-family: Verdana, Helvetica, Sans-Serif;
            margin: 0;
            padding: 0;
            background-color: White;
            color: #696969;
        }
        
        #txtMessage
        {
            color: Red;
            font-size: medium;
            width: 100%;
            border: 0px solid black;
            background-color: White;
        }
        
        div#pnlMain
        {
            width: 100%;
            height: 100%;
        }
		
		div#ReportViewer1
		{
		    margin-left: auto;
            margin-right: auto;
			border: 1px solid Gray;
		}
		
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
    </asp:ScriptManager>
    <asp:UpdatePanel ID="pnlMain" runat="server">
        <ContentTemplate>
            <asp:TextBox ID="txtMessage" runat="server" ReadOnly="true" Visible="false" />
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Width="800px" Height="900px"
                InteractivityPostBackMode="AlwaysSynchronous" AsyncRendering="true" ShowBackButton="true"
                ShowParameterPrompts="true">
            </rsweb:ReportViewer>
        </ContentTemplate>
    </asp:UpdatePanel>
    </form>
</body>
</html>
