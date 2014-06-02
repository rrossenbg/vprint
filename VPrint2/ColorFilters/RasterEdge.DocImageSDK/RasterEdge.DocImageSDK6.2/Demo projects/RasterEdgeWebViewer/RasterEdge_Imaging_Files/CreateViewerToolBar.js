function CreateWebToolbar(parentId)
{
	var loadingStr = "<div id='loading'><div>Loading</div></div>";
	$(parentId).prepend(loadingStr);  
    
    CreateExpansionbar(parentId);
	CreateFileToolbar(parentId);
	//CreatePageToolbar(parentId);
	CreateProcessingToolbar(parentId);
	CreateAnnotationToolbar(parentId);
	CreateFullToolbar(parentId);
}

function CreateFullToolbar(btnId)
{
    var btnFullStr = "<div class='re_func_btn_full'>";
    var fullStr = new ToolbarItem({CssId:"re_func_full",Event:'btnFull();ChangeIconStyle("re_func_full")'});
    btnFullStr += fullStr.CreateHtml();
    btnFullStr += "</div>";
    $(btnId).append(btnFullStr);
}

function CreateExpansionbar(btnId)
{
    var expanToolbarStr = "<div class='re_func_btn_expansion'>";
    expanToolbarStr +=  "<div class='re_func_title'>Stretch</div>";
    var expanStr = new ToolbarItem({CssId:"re_func_exSh",Event:'ThumbStretch();ChangeIconStyle("re_func_exSh")'});
    expanToolbarStr += expanStr.CreateHtml();
    expanToolbarStr += "</div>"
    $(btnId).append(expanToolbarStr);
}

function CreateFileToolbar(btnId)
{
	var fileToolbarStr = "<div class='re_func_btn_file'>";
	fileToolbarStr += "<div class='re_func_title'>File</div>";
	
	fileToolbarStr += "<div class='re_func_btns' id='re_func_upload' title='Upload File'><input id='fileToUpload' type='file' name='fileToUpload' onchange='ajaxFileUpload()'/></div>";
//	var toolbarOpenFile = new ToolbarItem({CssId: "re_func_upload", Title: "Open File", Event: 'OpenFile();ChangeIconStyle("re_func_upload")'});
//	fileToolbarStr += toolbarOpenFile.CreateHtml();
	var toolbarPrintFile = new ToolbarItem({CssId: "re_func_print", Title: "Print File", Event: 'PrintFile();ChangeIconStyle("re_func_print")'});
	fileToolbarStr += toolbarPrintFile.CreateHtml();
	var toolbarSaveFile = new ToolbarItem({CssId: "re_func_save", Title: "Save File", Event: 'SaveFile();ChangeIconStyle("re_func_save")'});
	fileToolbarStr += toolbarSaveFile.CreateHtml();
	var toolbarDownloadFile = new ToolbarItem({CssId: "re_func_download", Title: "Download File", Event: 'DownloadFile();ChangeIconStyle("re_func_download")'});
	fileToolbarStr += toolbarDownloadFile.CreateHtml();
	
	fileToolbarStr += "</div>";
	$(btnId).append(fileToolbarStr); 
}

function CreatePageToolbar(btnId)
{
	var pageToolbarStr = "<div class='re_func_btn_paging'>";
	pageToolbarStr += "<div class='re_func_title'>Page View</div>";

	var toolbarUpPage = new ToolbarItem({CssId: "re_func_upPage", Title: "Up Page", Event: 'ChangeIconStyle("re_func_upPage")'});
	pageToolbarStr += toolbarUpPage.CreateHtml();
	//pageToolbarStr += "<div class='re_func_btns' id='re_func_upPage' title='Up Page' ></div>";
	pageToolbarStr += "<textarea id='currentPageIndex' class='re_page1' >0</textarea><div class='re_page2'>/</div><div id='totalPageIndex' class='re_page2' >0</div>";
	//pageToolbarStr += "<div class='re_func_btns' id='re_func_downPage' title='Down Page' ></div>";
	
	var toolbarDownPage = new ToolbarItem({CssId: "re_func_downPage", Title: "Down Page", Event: 'ChangeIconStyle("re_func_downPage")'});
	pageToolbarStr += toolbarDownPage.CreateHtml();
	var toolbarZoomIn = new ToolbarItem({CssId: "re_func_zoomIn", Title: "ZoomIn", Event: 'btnZoomIn();ChangeIconStyle("re_func_zoomIn")'});
	pageToolbarStr += toolbarZoomIn.CreateHtml();
	var toolbarZoomOut = new ToolbarItem({CssId: "re_func_zoomOut", Title: "ZoomOut", Event: 'btnZoomOut();ChangeIconStyle("re_func_zoomOut")'});
	pageToolbarStr += toolbarZoomOut.CreateHtml();
	var toolbarFitWidth = new ToolbarItem({CssId: "re_func_fitWidth", Title: "FitWidth", Event: 'btnFitWidth();ChangeIconStyle("re_func_fitWidth")'});
	pageToolbarStr += toolbarFitWidth.CreateHtml();
	var toolbarBestFit = new ToolbarItem({CssId: "re_func_bestFit", Title: "BestFit", Event: 'btnBestFit();ChangeIconStyle("re_func_bestFit")'});
	pageToolbarStr += toolbarBestFit.CreateHtml();
	var toolbarOneToOne = new ToolbarItem({CssId: "re_func_oneToOne", Title: "OneToOne", Event: 'btnOneToOne();ChangeIconStyle("re_func_oneToOne")'});
	pageToolbarStr += toolbarOneToOne.CreateHtml();

	pageToolbarStr += "</div>";
	$(btnId).append(pageToolbarStr); 
}

function CreateProcessingToolbar(btnId)
{
	var proToolbarStr = "<div class='re_func_btn_processing'>";
	proToolbarStr += "<div class='re_func_title'>Processing</div>";

	var toolbarAddPage = new ToolbarItem({CssId: "re_func_addPage", Title: "Add Page", Event: 'AddPage();ChangeIconStyle("re_func_addPage")'});
	proToolbarStr += toolbarAddPage.CreateHtml();
	var toolbarDeletePage = new ToolbarItem({CssId: "re_func_deletePage", Title: "Delete Page", Event: 'DeletePage();ChangeIconStyle("re_func_deletePage")'});
	proToolbarStr += toolbarDeletePage.CreateHtml();
	var toolbarRotate90 = new ToolbarItem({CssId: "re_func_rotate90", Title: "Rotate 90", Event: 'btnRotate90();ChangeIconStyle("re_func_rotate90")'});
	proToolbarStr += toolbarRotate90.CreateHtml();
	var toolbarRotate270 = new ToolbarItem({CssId: "re_func_rotate270", Title: "Rotate -90", Event: 'btnRotate270();ChangeIconStyle("re_func_rotate270")'});
	proToolbarStr += toolbarRotate270.CreateHtml();
	var toolbarRotate180 = new ToolbarItem({CssId: "re_func_rotate180", Title: "Rotate 180", Event: 'btnRotate180();ChangeIconStyle("re_func_rotate180")'});
	proToolbarStr += toolbarRotate180.CreateHtml();

	proToolbarStr += "</div>";
	$(btnId).append(proToolbarStr); 
}

function CreateAnnotationToolbar(btnId)
{
	var annoToolbarStr = "<div class='re_func_btn_annotations'>";
	annoToolbarStr += "<div class='re_func_title'>Annotations</div>";
	
	var toolbarText = new ToolbarItem({CssId: "re_func_text", Title: "Text", Event: 'draw_annotation("Text");ChangeIconStyle("re_func_text")'});
	annoToolbarStr += toolbarText.CreateHtml();
	var toolbarFreehand = new ToolbarItem({CssId: "re_func_freeHand", Title: "Freehand", Event: 'draw_annotation("Freehand");ChangeIconStyle("re_func_freeHand")'});
	annoToolbarStr += toolbarFreehand.CreateHtml();
	var toolbarLine = new ToolbarItem({CssId: "re_func_line", Title: "Line", Event: 'draw_annotation("Line");ChangeIconStyle("re_func_line")'});
	annoToolbarStr += toolbarLine.CreateHtml();
	var toolbarPolygonLines = new ToolbarItem({CssId: "re_func_polygonLines", Title: "Polygon Lines", Event: 'draw_annotation("PolygonLines");ChangeIconStyle("re_func_polygonLines")'});
	annoToolbarStr += toolbarPolygonLines.CreateHtml();
	var toolbarFilledRectangle = new ToolbarItem({CssId: "re_func_filledRectangle", Title: "Filled rectangle", Event: 'draw_annotation("FilledRectangle");ChangeIconStyle("re_func_filledRectangle")'});
	annoToolbarStr += toolbarFilledRectangle.CreateHtml();
	var toolbarRectangle = new ToolbarItem({CssId: "re_func_rectangle", Title: "Rectangle", Event: 'draw_annotation("Rectangle");ChangeIconStyle("re_func_rectangle")'});
	annoToolbarStr += toolbarRectangle.CreateHtml();
	var toolbarHighlight = new ToolbarItem({CssId: "re_func_highlight", Title: "Highlight", Event: 'draw_annotation("Highlight");ChangeIconStyle("re_func_highlight")'});
	annoToolbarStr += toolbarHighlight.CreateHtml();
	var toolbarEllipse = new ToolbarItem({CssId: "re_func_ellipse", Title: "Ellipse", Event: 'draw_annotation("Ellipse");ChangeIconStyle("re_func_ellipse")'});
	annoToolbarStr += toolbarEllipse.CreateHtml();
	var toolbarPolygon = new ToolbarItem({CssId: "re_func_polygon", Title: "Polygon", Event: 'draw_annotation("Polygon");ChangeIconStyle("re_func_polygon")'});
	annoToolbarStr += toolbarPolygon.CreateHtml();
	var toolbarRubberStamp = new ToolbarItem({CssId: "re_func_rubberStamp", Title: "RubberStamp", Event: 'draw_annotation("RubberStamp");ChangeIconStyle("re_func_rubberStamp")'});
	annoToolbarStr += toolbarRubberStamp.CreateHtml();
	var toolbarRubberStamp = new ToolbarItem({CssId: "re_func_burnAnnotation", Title: "Burn", Event: 'burnAnnotationToImage();ChangeIconStyle("re_func_burnAnnotation")'});
	annoToolbarStr += toolbarRubberStamp.CreateHtml();
	var toolbarRubberStamp = new ToolbarItem({CssId: "re_func_deleteAnnotation", Title: "Delete", Event: 'DeleteAnnotation();ChangeIconStyle("re_func_deleteAnnotation")'});
	annoToolbarStr += toolbarRubberStamp.CreateHtml();

	annoToolbarStr += "</div>";
	$(btnId).append(annoToolbarStr); 
}

function ChangeIconStyle(divId)
{
	$(".re_func_btns").removeClass("iconBorder");
	$("#"+divId).addClass("iconBorder");
}



var ToolbarItem = Class.create();
ToolbarItem.prototype = 
{

	initialize: function(options) 
	{
		this.SetOptions(options);

		this.CssClass = this.options.CssClass;
		this.CssId = this.options.CssId;
		this.Title = this.options.Title;
		this.Event = this.options.Event;	
	},
		
	SetOptions: function(options) 
	{
		this.options = 
		{
			CssClass:		"re_func_btns",
			CssId:			"",			
			Title:			"",
			Event:	        "",
		};
		Extend(this.options, options || {});
    },
	
	CreateHtml: function()
	{			
		var html = "<div ";
		html += "class='" + this.CssClass + "' ";
		html += "id='" + this.CssId + "' ";
		html += "title='" + this.Title + "' ";
		html += "onclick='" + this.Event + "' ";
		html += "></div>";

		return html;
	},
};
