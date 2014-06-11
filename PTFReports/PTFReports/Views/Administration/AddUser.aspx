<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Administration.Master"
    Inherits="System.Web.Mvc.ViewPage<PTF.Reports.Models.UserModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Add User
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <script type='text/javascript'>
        function dropdown_change(value) {
            //get hidden value
            $("#CurrentStep").val(value);
            $("#AddUserForm").submit();
        }

        function text_click(value) {
            //get hidden value
            $("#CurrentStep").val(value);
            $("#AddUserForm").submit();
        }

    </script>
    <h2>
        <%= Model.UserID == 0? "Add User": "Update User" %></h2>
    <% using (Html.BeginForm("AddUser", "Administration", FormMethod.Post, new { id = "AddUserForm" }))
       { %>
    <%: Html.ValidationSummary(true) %>
    <fieldset>
        <input type="hidden" id="CurrentStep" name="CurrentStep" value="<%= Html.Encode(Model.CurrentStep) %>" />
        <legend>Info</legend>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Login) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.Login) %><br />
            <%: Html.ValidationMessageFor(model => model.Login) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Name) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.Name) %><br />
            <%: Html.ValidationMessageFor(model => model.Name) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Surname) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.Surname) %><br />
            <%: Html.ValidationMessageFor(model => model.Surname) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Email) %>
        </div>
        <div class="editor-field">
            <%: Html.EditorFor(model => model.Email) %><br />
            <%: Html.ValidationMessageFor(model => model.Email) %>
        </div>
        <% var attrs = new Dictionary<string, object> { { "onchange", "dropdown_change(this.id)" } }; %>
        <% var attrs2 = new Dictionary<string, object> { { "onclick", "text_click(this.id)" }, { "readonly", "true" } }; %>
        <% var attrs3 = new Dictionary<string, object> { { "readonly", "true" } }; %>
        <% if (string.IsNullOrEmpty(Model.CurrentStep) || Model.CurrentStep == "FormStart")
           {%>
        <!-- STEP 1 -->
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Country) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownList("CountryList", ViewData["CountryList"] as SelectList, "<Select>", attrs)%><br />
            <%: Html.ValidationMessageFor(model => model.Country) %>
        </div>
        <% }
           else if (Model.CurrentStep == "CountryList")
           { %>
        <!-- STEP 2 -->
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Country) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBox("FormStart", Model.Country, attrs2)%>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Company) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownList("CompanyList", ViewData["CompanyList"] as SelectList, "<Select>", attrs)%><br />
            <%: Html.ValidationMessageFor(model => model.Company) %>
        </div>
        <% }
           else if (Model.CurrentStep == "CompanyList")
           { %>
        <!-- STEP 3 -->
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Country) %>
        </div>
        <div class="editor-field">
            <%: Html.Label(Model.Country) %>
        </div>
        <div class="editor-label">
            <%: Html.TextBox("CountryList", Model.Company, attrs2)%>
        </div>
        <div class="editor-field">
            <%: Html.Label(Model.Company) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Branch) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownList("BranchList", ViewData["BranchList"] as SelectList, "<Select>", attrs)%><br />
            <%: Html.ValidationMessageFor(model => model.Company) %>
        </div>
        <% }
           else if (Model.CurrentStep == "BranchList")
           { %>
        <!-- STEP 4 -->
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Country) %>
        </div>
        <div class="editor-field">
            <%: Html.Label(Model.Country) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Company) %>
        </div>
        <div class="editor-field">
            <%: Html.Label(Model.Company) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Branch)%>
        </div>
        <div class="editor-field">
            <%: Html.TextBox("CompanyList", Model.Branch, attrs2)%>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Language) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownList("LanguageList", ViewData["LanguageList"] as SelectList, "<Select>", null)%><br />
            <%: Html.ValidationMessageFor(model => model.Language)%>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.UserType) %>
        </div>
        <div class="editor-field">
            <%: Html.DropDownList("UserTypeList", ViewData["UserTypeList"] as SelectList, "<Select>", attrs)%>
        </div>
        <% }
           else
           { %>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Country) %>
        </div>
        <div class="editor-field">
            <%: Html.Label(Model.Country) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Company) %>
        </div>
        <div class="editor-field">
            <%: Html.Label(Model.Company) %>
        </div>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.Branch)%>
        </div>
        <div class="editor-field">
            <%: Html.Label(Model.Branch) %>
        </div>
        <div class="editor-field">
            Language:&nbsp;<%: Html.Label(Model.Language) %>
        </div>
        <div class="editor-label">
            Type:&nbsp;<%: Html.TextBox("BranchList", Model.UserType, attrs2)%>
        </div>
        <% if (Model.UserID == 0)
           {%>
        <div class="editor-label">
            <%: Html.LabelFor(model => model.DefaultPass) %>
        </div>
        <div class="editor-field">
            <%: Html.TextBoxFor(model => model.DefaultPass, attrs3)%>
        </div>
        <% } %>
        <p>
            <% if (Model.UserID == 0)
               {%>
            <input type="submit" value="Create" />
            <% }
               else
               { %>
            <input type="submit" value="Update" />
            <% } %>
        </p>
        <% } %>
    </fieldset>
    <% } %>
    <div>
        <%: Html.ActionLink("Back", "Index") %>
    </div>
</asp:Content>
