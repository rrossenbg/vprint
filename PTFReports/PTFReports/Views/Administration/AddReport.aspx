<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Administration.Master" Inherits="System.Web.Mvc.ViewPage<PTF.Reports.Models.ReportModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    AddReport
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Add Report</h2>
    <% using (Html.BeginForm())
       {%>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <legend>Info</legend>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Name) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Name) %><br />
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Page) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Page) %><br />
            <%: Html.ValidationMessageFor(model => model.Page) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Description) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.Description) %><br />
            <%: Html.ValidationMessageFor(model => model.Description) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Folder) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownList("FolderList", ViewData["FolderList"] as SelectList)%>
            <%: Html.ValidationMessageFor(model => model.Folder)%>
        </div>
        <p>
            <input type="submit" value="Create" />
        </p>
    </fieldset>
    <% } %>
    <div>
        <%: Html.ActionLink("Back", "Index") %>
    </div>
</asp:Content>
