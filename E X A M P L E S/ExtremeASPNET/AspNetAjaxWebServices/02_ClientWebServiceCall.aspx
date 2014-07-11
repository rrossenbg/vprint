<%@ Page Language="C#" AutoEventWireup="true"  %>
<%@ Import Namespace="System.Web.Services" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>ASP.NET AJAX Web Services: Web Service Sample Page</title>
    <script runat="server">
        private static Random _rand = new Random(Environment.TickCount);
        
        [WebMethod]
        public static float GetStockQuoteFromPage(string symbol)
        {
            return _rand.Next(0, 120);
        }
    </script>
    
    <script type="text/javascript"> 
    function OnLookup()
    {           
      // Set default callbacks for stock quote service
      MsdnMagazine.StockQuoteService.set_defaultSucceededCallback(OnLookupComplete);
      MsdnMagazine.StockQuoteService.set_defaultFailedCallback(OnError);
      
      var stb = document.getElementById("_symbolTextBox");  
      // PageMethods.GetStockQuoteFromPage(stb.value, OnLookupComplete);
           
      MsdnMagazine.StockQuoteService.GetStockQuote(stb.value, OnLookupComplete, OnError, stb.value);
      
      // MsdnMagazine.StockQuoteService.GetStockQuote(stb.value);
    }
    
    function OnLookupComplete(result)//, userContext)
    {
      // userContext contains symbol passed into method
      //
      //alert("Value for " + userContext + " : " + result);
      var res = document.getElementById("_resultLabel");
      res.innerHTML = "<b>" + result + "</b>";
    }
    
    function OnError(result)
    {
      alert("Error: " + result.get_message());
    }
    </script>
    
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="_scriptManager" runat="server">
      <Services>
        <asp:ServiceReference Path="services/StockQuoteService.asmx" />
      </Services>
    </asp:ScriptManager>
    <div>
    <a href="Default.aspx">Back home</a>
    <hr />
    
    <h1>ASP.NET AJAX Web Services: Web Service Sample Page</h1>
     Enter symbol: 
        <asp:TextBox runat="server" id="_symbolTextBox" Text="A" />
        <br />
        <input onclick="OnLookup();" id="_lookupButton" type="button" value="Lookup" />
        <br />
        <asp:Label runat="server" id="_resultLabel" />
    </div>
    </form>
</body>
</html>
