    window.onresize = function()
    {   
         if(_isFull)
            ChangeControlSize();  
    } 
    
    var clientwidth;

    function ChangeControlSize()
    {
        $(".re_header").hide();
        var width = document.documentElement.clientWidth - 5;
        clientwidth = width;
        var height = document.documentElement.clientHeight- 5;
        $(".re_container").width(width+"px");
        $(".re_func_toolbar").width(width+"px");
        $(".re_content").width(width+"px");       
        var thumbWidth = $("#_plcImgsThumbs").outerWidth(true);
        var docviewerWidth;
        if(_isStretch)
            docviewerWidth = width-thumbWidth;
        else
            docviewerWidth = width;
        $("#_plcBigImg").width(docviewerWidth+"px");
        $("#re_canvas").width(docviewerWidth+"px");  
        $("#loading").width(width+"px");     
        _docWidth = docviewerWidth-17;
		
		$("#_footerImg").css("padding","10px 0px 10px "+width*0.5+"px");
		$("#_footerImg").width(width*0.5+"px");
		
		var toolbarHeight = $(".re_func_toolbar").height()+10;
		var footerHeight = $("#_footerImg").height()+20;
        $(".re_footer").hide();
		var ModiHeight = height-toolbarHeight-footerHeight;

		_docHeight = ModiHeight;
		$("#_plcImgsThumbs").height(ModiHeight - 40);
		$("#_plcBigImg").height(ModiHeight)
		$("#re_canvas").height(ModiHeight);
		$("#loading").height(ModiHeight);
		
		_fileDocument[curFileId].ChangeState();
		var curPg = parseInt(_curCorrectPageId);
        var pageIndex = _fileDocument[curFileId].GetPageIndex(curPg);
        var viewer = _fileDocument[curFileId].Viewers[pageIndex];
		ChangeViewerPosition(pageIndex);
		resizeDocViewerByCSS3("imgBig",pageIndex);
		drawImageViewerAnnotations(pageIndex);
		showIndex = _fileDocument[curFileId].GetShowIndex(pageIndex);	
		addShowPageIds(showIndex);
		changePosition();
    } 
    
    function RestoreSize()
    {
        $(".re_header").show();
        var width = origiWidth;
        $(".re_container").width(width+"px");
        $(".re_func_toolbar").width(width+"px");
        $(".re_content").width(width+"px");
        var thumbWidth = $("#_plcImgsThumbs").outerWidth(true);
        var docviewerWidth;
        if(_isStretch)
            docviewerWidth = width - thumbWidth;
        else
            docviewerWidth = width;
        $("#_plcBigImg").width(docviewerWidth+"px");
        $("#re_canvas").width(docviewerWidth+"px");
        $("#loading").width(width+"px");
        _docWidth = docviewerWidth - 17;
		
		$("#_footerImg").css("padding","10px 0px 10px "+width*0.5+"px");
		$("#_footerImg").width(width*0.5+"px");
		
		_docHeight = _height;
		$("#_plcImgsThumbs").height(_height-80);
		$("#_plcBigImg").height(_height - 40)
		$("#re_canvas").height(_height - 40);
		$("#loading").height(_height);
		$(".re_footer").show(); 
		
		_fileDocument[curFileId].ChangeState();
		var curPg = parseInt(_curCorrectPageId);
        var pageIndex = _fileDocument[curFileId].GetPageIndex(curPg);
		ChangeViewerPosition(pageIndex);
		resizeDocViewerByCSS3("imgBig",pageIndex);   
		drawImageViewerAnnotations(pageIndex);
		changePosition();  
    }
    
    function ThumbShrink()
    {
        $("#_plcImgsThumbs").hide();
        var width;
        if(_isFull)
            width = clientwidth - 5;
        else
            width = 1024;
        $("#_plcBigImg").width(width+"px");
	    $("#re_canvas").width(width+"px");
		_docWidth = width - 17;
		_fileDocument[curFileId].ChangeState();
        var curPg = parseInt(_curCorrectPageId);
        var pageIndex = _fileDocument[curFileId].GetPageIndex(curPg);
		ChangeViewerPosition(pageIndex);
		resizeDocViewerByCSS3("imgBig",pageIndex);
		drawImageViewerAnnotations(pageIndex);
		var _projectName = getProjectName();
        var icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_navi_expansion.png";
        $("#re_func_exSh").css("background","url('" + icoUrl1 + "') no-repeat 0px 0px");
        changePosition();
    }
    
    function ThumbExpansion()
    {
        $("#_plcImgsThumbs").show();
        var width;
        if(_isFull)
        {
            var thumbWidth = $("#_plcImgsThumbs").outerWidth(true);
           // width = document.documentElement.clientWidth -5 - thumbWidth;
           width = clientwidth - thumbWidth;
        }
        else
        {
            width = _width + 15;
        }
        _docWidth = width-17;
        $("#_plcBigImg").width(width+"px");
        $("#re_canvas").width(width+"px");
        _fileDocument[curFileId].ChangeState();
        var curPg = parseInt(_curCorrectPageId);
        var pageIndex = _fileDocument[curFileId].GetPageIndex(curPg);
		ChangeViewerPosition(pageIndex);
		resizeDocViewerByCSS3("imgBig",pageIndex); 
		drawImageViewerAnnotations(pageIndex);
		var _projectName = getProjectName();
        var icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_navi_shrink.png";
        $("#re_func_exSh").css("background","url('" + icoUrl1 + "') no-repeat 0px 0px");
        changePosition();
    }
    
    function changePosition()
    {
        if($("#zoomList").val() == "Fit Page")
        {
            var pageIndex = parseInt(_curCorrectPageId);	
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
            var viewer = _fileDocument[curFileId].Viewers[pageIndex];

            var percent = (_docHeight/viewer.OneToOne_Height*100).toFixed(2);
            _zoomListIndex = setPercentPosition(percent); 
        } 
        else if($("#zoomList").val() == "Fit Width")
        {
            var pageIndex = parseInt(_curCorrectPageId);	
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
            var viewer = _fileDocument[curFileId].Viewers[pageIndex];

            var percent = (_docWidth/viewer.OneToOne_Width*100).toFixed(2);
            _zoomListIndex = setPercentPosition(percent);
        }
    }
    

