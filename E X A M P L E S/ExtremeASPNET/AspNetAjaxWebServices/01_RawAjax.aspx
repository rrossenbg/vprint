<%@ Page Language="C#" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <script language="javascript" type="text/javascript">
    // Browser-independent way to access XMLHttpRequest object
    if (!window.XMLHttpRequest)
    {
      window.XMLHttpRequest = function() {
        return new ActiveXObject("Microsoft.XMLHTTP");
      }
    }
    
    function OnLookup()
    {      
      debugger;
      var xr = new XMLHttpRequest();  
      var stb = document.getElementById("_symbolTextBox");  
      var res = document.getElementById("_result");
         
      xr.open("GET", "services/StockQuote.ashx?symbol=" + stb.value, true);
      xr.onreadystatechange = function() 
      {
        if (xr.readyState == 4) {
        
          if (xr.status == 200) // request succeeded   
          {         
            res.innerHTML = xr.responseText;
          }
          else
            res.innerHTML = "Error retrieving quote";
        }
      }
      xr.send(null);
    }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <a href="Default.aspx">Back home</a>
        <hr />
    Enter symbol: 
        <asp:TextBox runat="server" id="_symbolTextBox" />
        <br />
        <input onclick="OnLookup();" id="_lookupButton" type="button" value="Lookup" />
        <br />
        <span id="_result" />

    </div>
    </form>
</body>
</html>
