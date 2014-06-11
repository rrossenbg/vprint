<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Administration.Master"
    Inherits="System.Web.Mvc.ViewPage<PTF.Reports.Models.BranchModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Show Branch
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Show Branch</h2>
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
                    Description</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Description %></div>
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
                    Email1</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Email1 %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Email2</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Email2 %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Line1</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Line1 %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Line2</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Line2 %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Line3</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Line3 %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Line4</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Line4 %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    County</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.County %></div>
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
                    Phone1</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Phone1 %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Contact</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Contact %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Contact1</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Contact1 %></div>
            </td>
        </tr>
        <tr>
            <td>
                <div class="display-label">
                    Contact2</div>
            </td>
            <td>
                <div class="display-field">
                    <%: Model.Contact2 %></div>
            </td>
        </tr>
    </table>
    <p>
        <%: Html.ActionLink("Back", "Index") %>
    </p>
</asp:Content>
