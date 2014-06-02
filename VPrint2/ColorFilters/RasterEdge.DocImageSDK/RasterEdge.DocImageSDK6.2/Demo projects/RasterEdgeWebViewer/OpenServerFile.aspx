<%@ Page Language="C#" AutoEventWireup="true" CodeFile="OpenServerFile.aspx.cs" Inherits="OpenServerFile" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Untitled Page</title>
    <script src="RasterEdge_Imaging_Files/jquery.js" type="text/javascript"></script>
    <link rel="stylesheet" href="RasterEdge_Imaging_Files/RasterEdge.css" type="text/css"/>
    <link rel="SHORTCUT ICON" href="RasterEdge_Imaging_Files/re_ico.ico"/>   
    
   

</head>
<body>
    <form id="form1" runat="server">
    
    <div class="re_header">
	    <div class="re_header_content">
            &nbsp;<a class="re_header_logo" href="http://www.rasteredge.com/"><img src="RasterEdge_Imaging_Files/RE_logo2.jpg" title="www.rasteredge.com" width="206" height="38" border="0" alt="www.rasteredge.com"/></a>
		    <div class="re_header_title">HTML Document Viewer with Annotations</div>
		    <a class="re_header_download" href=""><img src="RasterEdge_Imaging_Files/RE_wt_download.jpg" width="206" height="60" alt="" border="0"/></a>
	    </div>
    </div>

    <div class="re_container">
           
        <asp:Panel ID="Panel1" runat="server">
        </asp:Panel>
        
        <div class="re_footer">
		    RasterEdge.com is professional provider of document, content and imaging solutions, available for ASP.NET AJAX, Windows Forms. We are dedicated to provide powerful & profession imaging controls and components for capturing, viewing, processing, converting, compressing and storing images, documents and more. 
		    <span>&copy;2000-2012 Raster Edge.com</span>
	    </div> 
    </div>
    </form>
</body>
</html>
