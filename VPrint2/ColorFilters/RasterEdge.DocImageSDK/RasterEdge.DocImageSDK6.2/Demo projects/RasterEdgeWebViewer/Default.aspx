<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" EnableViewState="False" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
<title></title>

<script src="RasterEdge_Imaging_Files/RasterEdge.js" type="text/javascript"></script>
<script src="RasterEdge_Imaging_Files/CreateViewerToolBar.js" type="text/javascript"></script>

<script type="text/javascript">   
    _serverUrl = "<%=ServerUrl%>"; 
    _openFileUrl = "<%=OpenFileUrl%>";     
    _docWidth = "<%=DocWidth%>" - 17; 
    _docHeight = "<%=DocHeight%>"; 
    _thumbWidth = "<%=ThumbWidth%>"; 
    _thumbHeight = "<%=ThumbHeight%>"; 
    _rnd = "<%=SessionId%>"; 
    _pageSizeType = "<%=PageSizeType%>"
    
    TextAnnoStyle = new AnnoStyle({FillColor: "White", ShowedText: "double click to edit", TextColor: "Black", TextFont: "Arial", TextSize: 12, TextStyle :"normal"});
    FreehandAnnoStyle = new AnnoStyle({OutLineColor: "Red", OutLineWidth: 3.0});
    HighlightAnnoStyle = new AnnoStyle({FillColor: "Yellow"});
    RectangleAnnoStyle = new AnnoStyle({OutLineColor: "Black", OutLineWidth: 3.0});
    FilledRectangleAnnoStyle = new AnnoStyle({OutLineColor: "Black", OutLineWidth: 3.0, FillColor: "Black", Transparency: 1});
    EllipseAnnoStyle = new AnnoStyle({FillColor: "Orange"});
    RubberStampAnnoStyle = new AnnoStyle({OutLineColor: "Tomato", OutLineWidth: 3.0, FillColor: "Red", ShowedText: "Good", TextColor: "Black", TextFont: "Arial", TextSize: 20, TextStyle: "Italic"});
    PolygonLinesAnnoStyle = new AnnoStyle({OutLineColor: "Red", OutLineWidth: 3.0});
    PolygonAnnoStyle = new AnnoStyle({OutLineColor: "OrangeRed", OutLineWidth: 3.0, FillColor: "OrangeRed"});
    LineAnnoStyle = new AnnoStyle({OutLineColor: "Red", OutLineWidth: 3.0});   
         
</script>



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
          
	<div class="re_func_toolbar"></div>
	    
    <div id="_tblImgs" class="re_content" style="width:100%" >           
        <div class = "RE_WebViewer" id="REWebViewer1"></div>
    </div>  
    
    <div class="re_footer" >
		RasterEdge.com is professional provider of document, content and imaging solutions, available for ASP.NET AJAX, Windows Forms. We are dedicated to provide powerful & profession imaging controls and components for capturing, viewing, processing, converting, compressing and storing images, documents and more. 
		<span>&copy;2000-2014 Raster Edge.com</span>
    </div>
        	
</div>

    </form>  
</body> 
</html>
