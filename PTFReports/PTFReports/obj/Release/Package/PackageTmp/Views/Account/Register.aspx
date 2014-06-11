<%@ Page Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<PTF.Reports.Models.RegisterModel>" %>

<asp:Content ID="registerTitle" ContentPlaceHolderID="TitleContent" runat="server">
    Register
</asp:Content>
<asp:Content ID="registerContent" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Create a New Account</h2>
    <p>
        Use the form below to create a new account.
    </p>
    <p>
        Passwords are required to be a minimum of
        <%: ViewData["PasswordLength"] %>
        characters in length.
    </p>
    <% using (Html.BeginForm())
       { %>
    <%= Html.AntiForgeryToken() %>
    <%: Html.ValidationSummary(true, "Account creation was unsuccessful. Please correct the errors and try again.") %>
    <div>
        <fieldset>
            <legend>Account Information</legend>
            <div class="editor-label">
                <%: Html.LabelFor(m => m.usr)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(m => m.usr)%><br />
                <%: Html.ValidationMessageFor(m => m.usr)%>
            </div>
            <div class="editor-label">
                <%: Html.LabelFor(m => m.eml)%>
            </div>
            <div class="editor-field">
                <%: Html.TextBoxFor(m => m.eml)%><br />
                <%: Html.ValidationMessageFor(m => m.eml)%>
            </div>
            <div class="editor-label">
                <%: Html.LabelFor(m => m.pssw)%>
            </div>
            <div class="editor-field">
                <%: Html.PasswordFor(m => m.pssw)%><br />
                <%: Html.ValidationMessageFor(m => m.pssw)%>
            </div>
            <div class="editor-label">
                <%: Html.LabelFor(m => m.cpssw)%>
            </div>
            <div class="editor-field">
                <%: Html.PasswordFor(m => m.cpssw)%><br />
                <%: Html.ValidationMessageFor(m => m.cpssw)%>
            </div>
            <p>
                <input type="submit" value="Register" />
            </p>
        </fieldset>
    </div>
    <% } %>
</asp:Content>
