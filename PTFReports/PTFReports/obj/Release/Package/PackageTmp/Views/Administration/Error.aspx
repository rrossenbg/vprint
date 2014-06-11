<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Administration.Master" 
Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Error
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Error</h2>
    <div class="editor-label">
        <%: Html.Encode(Convert.ToString(ViewData[Strings.ERR]))%>
    </div>
    <div>
        <%: Html.ActionLink("Back", "Index", "Administration") %>
    </div>
</asp:Content>
