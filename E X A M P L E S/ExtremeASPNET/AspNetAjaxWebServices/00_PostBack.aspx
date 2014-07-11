<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

    protected void Button1_Click(object sender, EventArgs e)
    {
        Label1.Text = (new Random(Environment.TickCount)).Next(0, 120).ToString();
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        
    }
</script>

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <a href="Default.aspx">Back home</a>
        <hr />
        Enter symbol:<br />
        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox><br />
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Lookup" /><br />
        <asp:Label ID="Label1" runat="server"></asp:Label></div>
    </form>
</body>
</html>
