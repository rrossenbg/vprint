<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Administration.Master"
    Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
    Index
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2>
        Please select</h2>
    <div>
        <!--Html.ActionLink("Test", "Test2", "Administration")-->
        <table>
            <tr>
                <td style="width: 400px;">
                    <div>
                        <span><b>Settings</b></span>
                        <p>
                            <%: Html.ActionLink("Folders", "Folders", "Administration")%>
                            to view, add, edit or remove Report Folder.
                            <br />
                        </p>
                        <p>
                            <%: Html.ActionLink("Reports", "Reports", "Administration")%>
                            to view, add, edit or remove Report.
                            <br />
                            (Country Report, Company Report, Retailer Report)
                        </p>
                    </div>
                </td>
            </tr>
        </table>
        <br />
        <table>
            <tr>
                <td style="width: 400px;">
                    <div>
                        <span><b>Administration</b></span>
                        <p>
                            <%: Html.ActionLink("Countries", "Countries", "Administration")%>
                            to view Countries.
                        </p>
                        <p>
                            <%: Html.ActionLink("Companies", "Companies", "Administration")%>
                            to view Companies.
                        </p>
                        <p>
                            <%: Html.ActionLink("Branches", "Branches", "Administration")%>
                            to view Branches.
                        </p>
                    </div>
                </td>
            </tr>
        </table>
        <br />
        <table>
            <tr>
                <td style="width: 400px;">
                    <div>
                        <span><b>Security</b></span>
                        <p>
                            <%: Html.ActionLink("Users", "Users", "Administration")%>
                            to view Users.
                        </p>
                        <p>
                            <%: Html.ActionLink("Sessions", "Sessions", "Administration")%>
                            to view Sessions.
                        </p>
                         <p>
                            <%: Html.ActionLink("Open Sessions", "OpenSessions", "Administration")%>
                            to view open Sessions.
                        </p>
                        <p>
                            <%: Html.ActionLink("IPs", "IPs", "Administration")%>
                            to view all blocked ips.
                        </p>
                        <p>
                            <%: Html.ActionLink("Generate UnblockIP Url", "UnblockIPScript", "Administration")%>
                            generate unblock blocked ip script.
                        </p>
                    </div>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
