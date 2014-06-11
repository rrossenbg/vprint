<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Forgotten Password Failure
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Forgotten Password Failure</h2>
    <span style="color: Red;">
        <p>
            Can not find user by user name and email. Please try again or contact the system
            administrator.
        </p>
    </span>
    <div>
        <%: Html.ActionLink("Back", "Index", "Reports") %>
    </div>
</asp:Content>
