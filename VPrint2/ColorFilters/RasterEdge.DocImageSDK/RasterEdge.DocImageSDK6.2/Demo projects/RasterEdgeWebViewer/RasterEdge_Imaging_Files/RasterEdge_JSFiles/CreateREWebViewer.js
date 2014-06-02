function CreateREWebViewer(parentId)
{
    CreateREThumbViewer(parentId);
    CreateREDocumentViewer(parentId);
    CreateREFooterViewer(parentId);
}

function CreateREDocumentViewer(viewerId)
{
    var docPanel = "<div id='docPanel1'style='float:left;'></div>";
    $(viewerId).append(docPanel);
    
    var docWidth = _docWidth + 17 - 2;
    var docHeight = _docHeight - 40;

    var docInnerPanel = "<div id='_plcBigImg' style='background:#ababab;overflow:hidden;position:relative;width:" + docWidth + "px;height:" + docHeight + "px;'></div>";
    $("#docPanel1").append(docInnerPanel);
    
    var canvasPanel = "<div id='re_canvas' style='background:#ababab;position:relative;width:" + docWidth + "px;height:" + docHeight + "px;overflow:auto;'></div>";
    $("#_plcBigImg").append(canvasPanel);
    
    var docViewer = "<div id='draw_canvas' class='RE_DocumentViewer' style='position: relative;width:" + (docWidth-17) + "px;height:" + docHeight + "px;'></div>";
    $("#re_canvas").append(docViewer);
    
    CreateDocImage("#draw_canvas");
}

function CreateREThumbViewer(viewerId)
{
    var thumbPanel = "<div id='thumbPanel1' style='float:left;'></div>";
    $(viewerId).append(thumbPanel);
    
    var thumbWidth = 1024 - (_docWidth + 17) - 20;
    var thumbHeight = _docHeight - 80;
    
    var thumbViewer = "<div id='_plcImgsThumbs' class='RE_ThumbViewer'style='text-align:center;padding:20px 10px 20px 10px;background:#ababab;width:" + thumbWidth + "px;height:" + thumbHeight + "px;overflow-x:scroll;'></div>";
    $("#thumbPanel1").append(thumbViewer);
    
    GetTotalPages();
    for(var i=0;i<_totalPageCount;i++)
    {
        var lbl = "<div class='thumbnail' id='lbl_" + i + "' onclick='ChangePg(" + i + ")'></div>";
		$("#_plcImgsThumbs").append(lbl);
		
		var thumbDiv = "<div class='thumbDiv' id='thumbDiv_" + i + "' style='height:" + _thumbHeight + "px;width:" + _thumbWidth + "px;'></div>";
		$("#lbl_" + i).append(thumbDiv);

		CreateThumbImage("#thumbDiv_"+i,i);

		var thumbPageIndex = "<div id='thumbPgId_" + i + "' class='thumbPgId'>" + (i + 1) + "</div>";
        $("#thumbDiv_" + i).after(thumbPageIndex);
    }
}

function CreateREFooterViewer(viewerId)
{
    var navPanel = "<div id='navPanel' style='float:left;'></div>";
    $(viewerId).append(navPanel);
    
    var footerWidth = 1024 - 403;
    var footerHeight = 20;
    var pageFooter = "<div id='_footerImg' style='text-align:center;padding:10px 0px 10px 403px;width:" + footerWidth + "px;height:" + footerHeight + "px;background:#ffffff;'></div>";
    $("#navPanel").append(pageFooter);
    
    var pagePart = "<div id='navi_pagePart'></div>";
    $("#_footerImg").append(pagePart);
    CreatePagePart("#navi_pagePart");
    
    CreateZoomPart("#_footerImg");
}

function CreatePagePart(footerId)
{   
    var firstPage = "<div id='navi_first'></div>";
    $(footerId).append(firstPage);
    var prePage = "<div id='navi_pre'></div>";
    $(footerId).append(prePage); 
      
    var pageNavi = "<select class='navi_page' style='width:100px;height:20px;' id='pageIdList'></select>";
    $(footerId).append(pageNavi); 
    
    var nextPage = "<div id='navi_ne'></div>";
    $(footerId).append(nextPage);
    var lastPage = "<div id='navi_last'></div>";
    $(footerId).append(lastPage);
}

function CreateZoomPart(footerId)
{
    var zoomPart = "<div id='navi_zoomPart'></div>";
    $(footerId).append(zoomPart);
    
    var zoomItems = "<select class='navi_zoom' style='width:100px;height:20px;' id='zoomList'></select>";
    $("#navi_zoomPart").append(zoomItems);
    InitZoomItems("#zoomList");
    
    var zoomOut = "<div id='navi_zoomOut' onclick='btnZoomOut()'></div>";
    $("#navi_zoomPart").append(zoomOut);
    var zoomIn = "<div id='navi_zoomIn' onclick='btnZoomIn()'></div>";
    $("#navi_zoomPart").append(zoomIn);
}

function CreateThumbImage(divId,index)
{
    var src = "";
	src += getRootPath();
	src += "/RasterEdge_Imaging_Files/thumb_upload.gif";

	var image = "<img id='thumbnail_" + index + "' src='" + src + "' />"
	$(divId).append(image);
}

function CreateDocImage(divId)
{
    var src = "";
	src += getRootPath();
	src += "/RasterEdge_Imaging_Files/upload.gif";

	var image = "<img id='imgBig' class='showByDrag' src='" + src + "' />"
	$(divId).append(image);
}

function InitZoomItems(divId)
{
    for(var i=0; i<_zoomItems.length;i++)
    {
        if(i==16)
        {
            var option = "<option disabled='disabled'>"+_zoomItems[i]+"</option>"
            $(divId).append(option);
        }
        else
            $(divId).append( $('<option></option>').html(_zoomItems[i])); 
    }
    $(divId).val("Fit Page").attr("selected",true);
}

function CreateThumbLab()
{   
	GetTotalPages();
	for(var i=0; i<_totalPageCount; i++)
	{
		var lbl = "<div class='thumbnail' id='lbl_" + i + "' onclick='ChangePg(" + i + ")'></div>";
		$("#_plcImgsThumbs").append(lbl);
		
		var thumbDiv = "<div class='thumbDiv' id='thumbDiv_" + i + "' style='height:" + _thumbHeight + "px;width:" + _thumbWidth + "px;'></div>";
		$("#lbl_" + i).append(thumbDiv);

		CreateThumbImage("#thumbDiv_" + i, i);

		var thumbPageIndex = "<div id='thumbPgId_" + i + "' class='thumbPgId'>" + (i + 1) + "</div>";
		$("#thumbDiv_" + i).after(thumbPageIndex);
	}
}