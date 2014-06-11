<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PTF.Reports.Models.ChangePasswordModel>" %>

<asp:Content ID="changePasswordTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Change Password
</asp:Content>
<asp:Content ID="changePasswordContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Change Password</h2>
    <p>
        Use the form below to change your password.
    </p>
    <p>
        New passwords are required to be a minimum of
        <%: ViewData["PasswordLength"] %>
        characters in length.
    </p>
    <% using (Html.BeginForm("ChangePassword", "Account", FormMethod.Post, new { style = "width: 500px;" }))
       { %>
    <%= Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true, "Password change was unsuccessful. Please correct the errors and try again.") %>
    <fieldset style="width: 400px;">
        <legend>Account Information</legend>
        <div class="editor-label">
            <%: Html.LabelFor(m => m.usr)%>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(m => m.usr)%><br />
            <%: Html.ValidationMessageFor(m => m.usr)%>
        </div>
        <%if (Model == null || string.IsNullOrWhiteSpace(Model.opssw))
          { %>
        <div class="editor-label">
            <%: Html.LabelFor(m => m.opssw)%>
        </div>
        <div class="editor-field">
            <%: Html.PasswordFor(m => m.opssw)%><br />
            <%: Html.ValidationMessageFor(m => m.opssw)%>
        </div>
        <%}%>
        <div class="editor-label">
            <%: Html.LabelFor(m => m.npssw)%>
        </div>
        <div class="editor-field">
            <%: Html.PasswordFor(m => m.npssw)%><br />
            <%: Html.ValidationMessageFor(m => m.npssw)%>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(m => m.cpssw)%>
        </div>
        <div class="editor-field">
            <%: Html.PasswordFor(m => m.cpssw)%><br />
            <%: Html.ValidationMessageFor(m => m.cpssw)%>
        </div>
        <p>
            <input type="submit" value="Change Password" />
        </p>
    </fieldset>
    <% } %>
</asp:Content>
