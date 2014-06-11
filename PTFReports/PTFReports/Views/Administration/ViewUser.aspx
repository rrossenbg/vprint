<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Administration.Master"
    Inherits="System.Web.Mvc.ViewPage<PTF.Reports.Models.UserModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    View User
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        View User</h2>
    <table>
        <tr>
            <th>
                &nbsp;
            </th>
            <th>
                &nbsp;
            </th>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Login</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Login %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Name</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Name %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Surname</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Surname %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Email</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Email %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Country</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Country %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Company</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Company %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Branch</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Branch %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Active</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Active.ToYesNo() %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    UserType</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.UserType %></div>
            </td>
        </tr>
    </table>
    <p>
        <%: Html.ActionLink("Delete", "DeleteUser", new {  id=Model.UserID })%>
        |
        <%: Html.ActionLink("Back", "Index") %>
    </p>
</asp:Content>
