<%@ WebHandler Language="C#" Class="StockQuoteService" %>

using System;
using System.Web;

public class StockQuoteService : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        // Symbol is in query string (?symbol=xx)
        //
        string symbol = (string)(context.Request["symbol"] ?? "");
        // get quote with symbol here...
        
        context.Response.ContentType = "text/plain";
        context.Response.Write((new Random(Environment.TickCount)).Next(0, 120).ToString());
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}