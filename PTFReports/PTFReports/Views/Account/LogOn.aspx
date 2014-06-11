<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PTF.Reports.Models.LogOnModel>" %>

<asp:Content ID="loginTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Log On
</asp:Content>
<asp:Content ID="loginContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2 style="text-align: left; color: rgb(108, 108, 108);">
        Log On</h2>
    <p>
        Please enter your username and password.
    </p>
    <% using (Html.BeginForm("LogOn", "Account", FormMethod.Post, new { style = "width: 500px;" }))
       { %>
    <%= Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true, "Login was unsuccessful. Please correct the errors and try again.") %>
    <fieldset style="width: 400px;">
        <legend>Account Information</legend>
        <div class="editor-label">
            <%: Html.LabelFor(m => m.usr)%>
        </div>
        <div class="editor-field">
            <input id="usr" name="usr" type="text" maxlength="15" /><br />
            <%: Html.ValidationMessageFor(m => m.usr)%>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(m => m.pssw ) %>
        </div>
        <div class="editor-field">
            <input id="pssw" name="pssw" type="password" maxlength="16" /><br />
            <%: Html.ValidationMessageFor(m => m.pssw)%>
        </div>
        <div class="editor-label">
            <input id="rmbm" name="rmbm" type="checkbox" maxlength="1" />
            <%: Html.LabelFor(m => m.rmbm)%>
        </div>
        <p>
            <%: Html.ActionLink("I forgot my password", "ForgottenPassword") %>
            &nbsp;
            <%: Html.ActionLink("Change password", "ChangePassword") %>
        </p>
        <p>
            &nbsp;&nbsp;&nbsp;&nbsp;
            <input type="image" value="Login" title="Login" src="<%:Url.Content("~/Content/images/login3.png")%>" />
        </p>
    </fieldset>
    <% } %>
</asp:Content>
