<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ASP.NET AJAX Web Services: Complex Web Service Sample Page</title>
    <script type="text/javascript">    
    function OnMarry()
    {           
      var p1 = new MsdnMagazine.SampleTypes.Person();      
      var p2 = new MsdnMagazine.SampleTypes.Person();

      p1.FirstName = document.getElementById("_firstName1TextBox").value;
      p1.LastName  = document.getElementById("_lastName1TextBox").value;
      p1.Age       = document.getElementById("_age1TextBox").value;
      p1.Married   = document.getElementById("_married1CheckBox").checked;
      
      p2.FirstName = document.getElementById("_firstName2TextBox").value;
      p2.LastName  = document.getElementById("_lastName2TextBox").value;
      p2.Age       = document.getElementById("_age2TextBox").value;
      p2.Married   = document.getElementById("_married2CheckBox").checked;

      MsdnMagazine.MarriageService.Marry([p1,p2], OnMarryComplete);
    }
    
    function OnMarryComplete(result)
    {
      var resultLabel = $get("_resultLabel");
      var ret = new Sys.StringBuilder();
      ret.append("Congratulations to the new happy couple ");
      ret.append(result[0].FirstName);
      ret.append(" ")
      ret.append(result[0].LastName);
      ret.append(" and ");
      ret.append(result[1].FirstName);
      ret.append(" ");
      ret.append(result[1].LastName);
      ret.append("!!<br />");
      resultLabel.innerHTML += ret.toString();
   } 
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="_scriptManager" runat="server">
     <Services>
        <asp:ServiceReference Path="services/MarriageService.asmx" />
     </Services>
    </asp:ScriptManager>
    <div>
    <a href="Default.aspx">Back home</a>
    <hr />
    
    <h1>ASP.NET AJAX Extensions: Complex Web Service Sample Page</h1>
     Person1 to marry: <br />
        First: <asp:TextBox runat="server" id="_firstName1TextBox" Text="Alice" /><br />
        Last: <asp:TextBox runat="server" id="_lastName1TextBox" Text="Jones" /><br />
        Age: <asp:TextBox runat="server" id="_age1TextBox" Text="33" /><br />
        Married? <asp:CheckBox runat="server" ID="_married1CheckBox" /><br />
        <br />
     Person2 to marry: <br />
        First: <asp:TextBox runat="server" id="_firstName2TextBox" Text="Bob" /><br />
        Last: <asp:TextBox runat="server" id="_lastName2TextBox" Text="Smith" /><br />
        Age: <asp:TextBox runat="server" id="_age2TextBox" Text="32" /><br />
        Married? <asp:CheckBox runat="server" ID="_married2CheckBox" /><br />
        <br />
        <input onclick="OnMarry();" id="_marryButton" type="button" value="Marry!" />
        <br />
        <asp:Label runat="server" id="_resultLabel" />
    </div>
    </form>
</body>
</html>
