<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PTF.Reports.Models.ForgottenPassword>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    ForgottenPassword
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Forgotten Password</h2>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.min.js") %>" type="text/javascript"></script>
    <script src="<%: Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js") %>"
        type="text/javascript"></script>
    <% using (Html.BeginForm())
       { %>
    <%= Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>Forgotten Password</legend>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.usr)%>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.usr)%><br />
            <%: Html.ValidationMessageFor(model => model.usr)%>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.eml)%>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.eml)%><br />
            <%: Html.ValidationMessageFor(model => model.eml)%>
        </div>
        <p>
            <input type="submit" value="Send" />
        </p>
    </fieldset>
    <% } %>
</asp:Content>
