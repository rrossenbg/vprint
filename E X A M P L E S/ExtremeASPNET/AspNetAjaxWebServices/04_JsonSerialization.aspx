<%@ Page Language="C#" %>
<%@ Import Namespace="Microsoft.Web.Script.Serialization" %>
<%@ Import Namespace="MsdnMagazine.SampleTypes" %>
<%@ Import Namespace="System.Data" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">
protected void Page_Load(object sender, EventArgs e)
{
    Person p = new Person();
    p.FirstName = "Bob";
    p.LastName = "Smith";
    p.Age = 33;
    p.Married = true;

    Person p2 = new Person();
    p2.FirstName = "Alice";
    p2.LastName = "Chang";
    p2.Age = 29;
    p2.Married = true;   

    Company c = new Company();
    c.Name = "Humongous Insurance, Inc.";
    c.Employees.Add(p);
    c.Employees.Add(p2);

    DataSet ds = new DataSet();
    ds.ReadXml(Server.MapPath("~/App_Data/happenings.xml"));

    JavaScriptSerializer jss = new JavaScriptSerializer();
    _serializedPersonLabel.Text = jss.Serialize(p);
    _serializedCompanyLabel.Text = jss.Serialize(c);
    
    // NOTE - the DataSet serialization breaks with a circular reference
    // in this intermediate drop so this line is commented out until it's fixed
    //_serializedDataSetLabel.Text = jss.Serialize(ds);
}
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ASP.NET AJAX Web Services: Json Serialization</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    <a href="Default.aspx">Back home</a>
    <hr />

    <b>Instance of Person type:</b><br />
    <asp:Label runat="server" ID="_serializedPersonLabel" />
    <hr />
    <b>Instance of Company type:</b><br />
    <asp:Label runat="server" ID="_serializedCompanyLabel" />
    <hr />
    <b>Instance of DataSet:</b><br />
    <asp:Label ID="_serializedDataSetLabel" runat="server"></asp:Label>
    </div>
    </form>
</body>
</html>
