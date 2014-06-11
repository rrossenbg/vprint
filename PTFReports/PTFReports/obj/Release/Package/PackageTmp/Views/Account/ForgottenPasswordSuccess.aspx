<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ForgottenPasswordSuccess
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Forgotten Password Success</h2>
    <p>
        Your password has been reset successfully. You will recieve email conformation soon.
    </p>
    <div>
        <%: Html.ActionLink("Back", "Index", "Reports") %>
    </div>
</asp:Content>
