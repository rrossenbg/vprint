        var _rnd;
        var _size;
        var _isInDocViewer = false;
        var timeout = false;
        var _showPageIds = new Array();
        var _fileDocument = new Array();
        var curFileId = 0;
        var _curCorrectPageId = 0;
        var _totalPageCount = 1;
        var _uploadFileName="";
        var _isImage = false;
        var mark = 1;
        var sign = 1;
        var sym ;
        var _zoomItems = new Array("6400%", "3200%", "1600%", "800%", "400%", "200%", "150%", "125%", 
                                                    "100%", "75%", "66.67%", "50%", "33.33%", "25%","12.5%","8.33%","----------","Actual Size","Fit Page","Fit Width");
        var _zoomListIndex = 18;
        var _isStretch = true;
        var _width;
        var _height;
        var _isFull = false;
        
        var _cookieFilePath = "RasterEdgeFilePath";
        var _cookieZoomIndex = "RasterEdgeZoomIndex";
        var _cookiePageIndex = "RasterEdgePageIndex";
        var _cookieIsFull = "RasterEdgeIsFull";
        var _cookieStretch = "RasterEdgeStretch";
        var _initPageIndex = 0;

	    $(document).ready(function(){
                   
		    $.ajaxSetup({
			    async: false
		    }); 
    		
		    _docWidth = parseInt(_docWidth);
		    _docHeight = parseInt(_docHeight);
		    _thumbWidth = parseInt(_thumbWidth);
		    _thumbHeight = parseInt(_thumbHeight);
		    _width = _docWidth;
		    _height = _docHeight;
    		
		    CreateWebToolbar(".re_func_toolbar");
            CreateREWebViewer("#REWebViewer1");
            
            CookieFileInfo();
            OnLoadReady();
            BindZoomEvent();
            CookieViewStyle();
        });
        
        function CookieFileInfo()
        {
            var str = getCookie(_cookiePageIndex);
            if(str != "" && str != null)
            {
                _initPageIndex = parseInt(str);
                _curCorrectPageId = _initPageIndex;
            }
            str = getCookie(_cookieZoomIndex);
            if(str != "" && str != null)
            {
                if(str == "Actual Size")
                {
                    $("#zoomList").val("Actual Size").attr("selected",true);
                    _zoomListIndex = 17;
                }
                else if(str == "Fit Page")
                {
                    $("#zoomList").val("Fit Page").attr("selected",true);
                    _zoomListIndex = 18;
                }
                else if(str == "Fit Width")
                {
                   $("#zoomList").val("Fit Width").attr("selected",true);
                   _zoomListIndex = 19;
                }
                else
                {
                    _pageSizeType = 2;
                    var array = str.split(";");
                    _zoomIndex = parseFloat(array[0]);
                    $("#zoomList").val(_zoomIndex * 10 + "%").attr("selected",true);
                    _zoomListIndex = parseInt(array[1]);
                }    
            }
        }
        
	    function OnLoadReady()
	    {		
            _defaultPageSizeType = 0;	
            var _isImage = false;
            var mark = 1;
            var sign = 1;
            
		    $("#imgBig").hide();
                    
            $("#currentPageIndex").val(_initPageIndex+1); 
		    GetTotalPages();
		    ChangeProcessStyle();
		    ChangeFooterStyle();
		    InitViewers();
		    ChangeThumbScrollBar(_initPageIndex,false);
            
            ChangePg(_initPageIndex);
           
            $(".RE_WebViewer").bind("contextmenu",function(e){ return false;});
                   
	        $(".showByDrag").bind("mousedown",dragImageMouseDown);
		    $(".showByDrag").bind("mousemove",dragImageMouseMove);
		    $(".showByDrag").bind("mouseup",dragImageMouseUp);
		    $(".showByDrag").addClass("mouseStyleMove");

            
		    $(".RE_ThumbViewer").sortable({stop: function(event, ui) { SortPage();}});
		    $(".RE_ThumbViewer").disableSelection();

		    $("#currentPageIndex").blur(function(){ CurrentPageIndexChanged(false);});
		    $("#currentPageIndex").keyup(function(event){
			    if(event.keyCode==13) 
			    {
				    CurrentPageIndexChanged(true);
			    }
		    });
    		
		    $("#re_canvas").hover(
			    function(){
				    _isInDocViewer = true;
			    },
			    function(){
				    _isInDocViewer = false;
			    }
		    );
    		
		    document.getElementById('imgBig').onload = function(){
			    $("#loading").hide();
			    $("#imgBig").show();
			    $("#draw_canvas").show();
			    };

		    $("#_plcImgsThumbs").scroll(function()
		    {
			    if (timeout)
			    {clearTimeout(timeout);}
			    timeout = setTimeout(function()
			    {
				    var sTop = $("#_plcImgsThumbs").scrollTop();
				    var divOuterHeight = $(".thumbnail").outerHeight(true);
				    var totalHeight =  $("#_plcImgsThumbs").height();
				    var showCount = parseInt(totalHeight/divOuterHeight) + 1;
				    var currentThumbTopId = parseInt(sTop/divOuterHeight);

				    var addIds = new Array();
				    for(var i=0; i<showCount+1; i++)
				    {
				        var showId = currentThumbTopId - i;
				        if(showId > -1)
				        {
				            if(!_showPageIds.in_array(showId))
				            {
				                _showPageIds.push(showId);
				                addIds.push(showId);
				            }
				        }
					    showId = currentThumbTopId + i;
					    if(showId > 0 && showId < _totalPageCount)
					    {
						    if(!_showPageIds.in_array(showId))
						    {
							    _showPageIds.push(showId);
							    addIds.push(showId);
						    }					
					    }
				    }							

				    refreshThumViewer(addIds);				
			    },100);
		    });
		    _zoomListIndex = parseInt($("#zoomList").get(0).selectedIndex);
		    map();
	    }
	    
	    function CookieViewStyle()
	    {
	        var str = getCookie(_cookieIsFull);
	        if(str == "true")
	        {
	            origiWidth = $(".re_content").width();
                ChangeControlSize();
		        _isFull = true;
	        }
	        str = getCookie(_cookieStretch);
	        if(str == "false")
	        {
	             ThumbShrink();
                _isStretch = false;
	        }
	    }
	
		function GetTotalPages()
		{
			$.post(getServerHandlerUrl(), { "action": "GetFilePageCount", "Rnd": _rnd }, function (result, status) {
                result = eval('(' + result + ')');
				if (result.state == "success")
				{
					var arr = result.msg;
					var flag = arr.indexOf(";");
					if(flag != -1)
					{
					    arr = 1;
					    _isImage = true;
					}
					else
					{
					    _isImage = false;
					}
					_totalPageCount = parseInt(arr);
					_maxTotalPageCount = parseInt(arr);
					$("#totalPageIndex").text(_totalPageCount);
				} 
				else
				{					
					alert("Can not get file count!");
				}
           });			
		}
		
		function ChangeProcessStyle()
		{
		    var _projectName = getProjectName();
		    if(_isImage && mark == 1)
		    {
		        var icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_wt_iconsC1.png";
				$("#re_func_addPage").css("background","url('" + icoUrl1 + "') no-repeat -137px 0px");
				$("#re_func_addPage").hover(function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -137px 0px");}, function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -137px 0px");});
				
				$("#re_func_deletePage").css("background","url('" + icoUrl1 + "') no-repeat -167px 0px");
				$("#re_func_deletePage").hover(function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -167px 0px");}, function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -167px 0px");});
				if( sign == 1)
				{
				    $("#re_func_addPage").removeAttr("onclick");
				    $("#re_func_deletePage").removeAttr("onclick");
				    sign = 0;
				}
				else
				{
				    $("#re_func_addPage").unbind("click");
				    $("#re_func_deletePage").unbind("click");   
				}				
				mark = -1;
		    }
		    else if (!_isImage && mark == -1)
		    {
		        var icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_wt_iconsB1.png";
				$("#re_func_addPage").css("background","url('" + icoUrl1 + "') no-repeat -137px 0px");
				$("#re_func_addPage").hover(function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -137px 0px");}, function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -137px 0px");});
				$("#re_func_addPage").bind("click",AddPage);
				
				$("#re_func_deletePage").css("background","url('" + icoUrl1 + "') no-repeat -167px 0px");
				$("#re_func_deletePage").hover(function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -167px 0px");}, function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -167px 0px");});
				$("#re_func_deletePage").bind("click",DeletePage);
				mark = 1;
		    }
		}
		
		        
		function InitViewers()
		{ 
			var fileDoc = new FileDocument({Index: curFileId});
			_fileDocument[curFileId] = fileDoc;
				
			for(var i=0; i<_totalPageCount; i++)
			{
				var viewer = new Viewer({PageIndex: i});

				_fileDocument[curFileId].Viewers[i] = viewer;
			}
		}
		
		function getCurrentPageSize(pageIndex)
		{
			$.post(getServerHandlerUrl(), { "action": "GetCurrentPageSize", "Rnd": _rnd, "PageIndex": pageIndex }, function (result, status) {
                result = eval('(' + result + ')');
				if (result.state == "success")
				{
					var arr = result.msg;
					_size = arr;
				} 
				else
				{					
				    alert("Failed to load document ");
				}
           });

		}
		
		function ChangePg(Pg) 
		{
            var pageIndex = parseInt(Pg);
            setCookie(_cookiePageIndex,pageIndex,3);
            _fileDocument[curFileId].DeleteBurnedAnnotations();
            if(_fileDocument[curFileId].Viewers[pageIndex].Init)
            {             
                var curPg =  _fileDocument[curFileId].GetPageIndex(_curCorrectPageId);
                if(pageIndex==curPg)
                {
                    drawImageViewerAnnotations(pageIndex);
                    return;
                }
            }
			showLoading();	
			
            ChangeViewerPosition(pageIndex);
			resizeDocViewerByCSS3("imgBig",pageIndex);
			
            document.getElementById('imgBig').src = _fileDocument[curFileId].Viewers[pageIndex].GetSrc();           
			           
			showIndex = _fileDocument[curFileId].GetShowIndex(pageIndex);	
			$("#currentPageIndex").val(showIndex+1);
			$("#pageIdList").val(showIndex+1);
			_curCorrectPageId = showIndex;

			addShowPageIds(showIndex);

            ChangeDivCssClass(Pg);
            changePageNavigationStyle();
           
			drawImageViewerAnnotations(pageIndex);
			if(_totalPageCount != 1)
			{
                ChangeBtnStyle();
            }
        }
        
                
		function ChangeViewerPosition(pageIndex)
		{
			_fileDocument[curFileId].Viewers[pageIndex].GetViewerLeftTop();
			$("#draw_canvas").css({"width":_fileDocument[curFileId].Viewers[pageIndex].BigWidth+"px","height":_fileDocument[curFileId].Viewers[pageIndex].BigHeight+"px"});
		}
        
        function resizeDocViewerByCSS3(target, pageIndex)
		{
			_fileDocument[curFileId].Viewers[pageIndex].GetFirstViewerLeftTop();
			$("#"+target).css({"width":_fileDocument[curFileId].Viewers[pageIndex].FirstActualWidth+"px","height":_fileDocument[curFileId].Viewers[pageIndex].FirstActualHeight+"px"}); 
			$("#"+target).css({"top":_fileDocument[curFileId].Viewers[pageIndex].FirstTop+"px","left":_fileDocument[curFileId].Viewers[pageIndex].FirstLeft+"px","position":"absolute"});
			
			target = document.getElementById(target);
			
			var degree = _fileDocument[curFileId].Viewers[pageIndex].Rotate * 90;
			var zoomValue = _zoomRatio*_zoomIndex;
			var translateX = (_fileDocument[curFileId].Viewers[pageIndex].BigWidth - _docWidth)/(2*zoomValue);
			var translateY = (_fileDocument[curFileId].Viewers[pageIndex].BigHeight - _docHeight)/(2*zoomValue);

			if(degree == 90)
			{
				newTranslateX = translateY;
				newTranslateY = -translateX;
				translateX = newTranslateX;
				translateY = newTranslateY;
			}
			else if(degree == 180)
			{
				newTranslateX = -translateX;
				newTranslateY = -translateY;
				translateX = newTranslateX;
				translateY = newTranslateY;
			}
			else if(degree == 270)
			{
				newTranslateX = -translateY;
				newTranslateY = translateX;
				translateX = newTranslateX;
				translateY = newTranslateY;
			}
			
			if (target.style.msTransform !== undefined) { // IE
               	target.style.msTransform = 'rotate(' + degree + 'deg) scale(' + zoomValue + ') translateX(' + translateX + 'px) translateY(' + translateY + 'px)';			
            } else if (target.style.MozTransform !== undefined) {  // Mozilla
                target.style.MozTransform = 'rotate(' + degree + 'deg) scale(' + zoomValue + ') translateX(' + translateX + 'px) translateY(' + translateY + 'px)';
            } else if (target.style.OTransform !== undefined) {   // Opera
                target.style.OTransform = 'rotate(' + degree + 'deg) scale(' + zoomValue + ') translateX(' + translateX + 'px) translateY(' + translateY + 'px)';
            } else if (target.style.webkitTransform !== undefined) { // Chrome Safari
                target.style.webkitTransform = 'rotate(' + degree + 'deg) scale(' + zoomValue + ') translateX(' + translateX + 'px) translateY(' + translateY + 'px)';
            } else {
                target.style.transform = 'rotate(' + degree + 'deg) scale(' + zoomValue + ') translateX(' + translateX + 'px) translateY(' + translateY + 'px)';
            }
		}
			
		function addShowPageIds(showIndex)
		{			
			var divOuterHeight = $(".thumbnail").outerHeight(true);
			var totalHeight =  $("#_plcImgsThumbs").height();
			var showCount = parseInt(totalHeight/divOuterHeight) + 1;

			var addIds = new Array();
			for(var i=0; i<showCount; i++)
			{
			    var showId = showIndex - i;
			    if(showId > -1)
			    {
			        if(!_showPageIds.in_array(showId))
			        {
			            _showPageIds.push(showId);
			            addIds.push(showId);
			        }
			    }
				showId = showIndex + i;
				if(showId > -1 && showId <= _totalPageCount)
				{
					if(!_showPageIds.in_array(showId))
					{
						_showPageIds.push(showId);
						addIds.push(showId);
					}					
				}
			}

			refreshThumViewer(addIds);
		}	
		
		function refreshThumViewer(refreshIds)
		{
			for(var index=0; index<refreshIds.length; index++)
			{
				var pageIndex = _fileDocument[curFileId].GetPageIndex(refreshIds[index]);				
				var viewer = _fileDocument[curFileId].Viewers[pageIndex];
				
				if(viewer == null || viewer.PageIndex == null)
				{continue;}

				$("#thumbnail_"+pageIndex).attr("src",viewer.GetThumbSrc());	
				
			}
		}
		
		function ChangeDivCssClass(Pg)
        {
            $("div").removeClass("thumb_select");
            
            selectId = "#lbl_" + Pg;
            $(selectId).addClass("thumb_select");
            
        }
        
        function changePageNavigationStyle()
		{
			var _projectName = getProjectName();
			var icoUrl = _projectName + "/RasterEdge_Imaging_Files/RE_wt_iconsB1.png";
			$("#re_func_upPage").css("background","url('" + icoUrl + "') no-repeat -73px 0px");
			$("#re_func_upPage").hover(function(){$(this).css("background","url('" + icoUrl + "') no-repeat -73px 0px #727272");}, function(){$(this).css("background","url('" + icoUrl + "') no-repeat -73px 0px");});
			$("#re_func_upPage").unbind("click");
			$("#re_func_upPage").bind("click",UpPage);
			$("#re_func_downPage").css("background","url('" + icoUrl + "') no-repeat -103px 0px");
			$("#re_func_downPage").hover(function(){$(this).css("background","url('" + icoUrl + "') no-repeat -103px 0px #727272");}, function(){$(this).css("background","url('" + icoUrl + "') no-repeat -103px 0px");});
			$("#re_func_downPage").unbind("click");
			$("#re_func_downPage").bind("click",DownPage);
						
			currentPg = parseInt(_curCorrectPageId);
			if(currentPg == 0)
			{
			    var icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_wt_iconsC1.png";
				$("#re_func_upPage").css("background","url('" + icoUrl1 + "') no-repeat -73px 0px");
				$("#re_func_upPage").hover(function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -73px 0px");}, function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -73px 0px");});
				$("#re_func_upPage").unbind("click");
				
			}
			if(currentPg == _totalPageCount - 1)
			{
				var icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_wt_iconsC1.png";
				$("#re_func_downPage").css("background","url('" + icoUrl1 + "') no-repeat -103px 0px");
				$("#re_func_downPage").hover(function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -103px 0px");}, function(){$(this).css("background","url('" + icoUrl1 + "') no-repeat -103px 0px");});
				$("#re_func_downPage").unbind("click");
			}
		}
		
		function resizeThumbViewerByCSS3(target, pageIndex)
		{
			target = document.getElementById(target);			
			var degree = _fileDocument[curFileId].Viewers[pageIndex].Rotate * 90;
			
			var translateX = 0;
			var translateY = 0;
			
			if(_thumbHeight < _thumbWidth)
			{
				if(degree == 90)
				{				
					translateX = -(_thumbHeight - _thumbWidth)/2;
					translateY = -(_thumbHeight - _thumbWidth)/2;
				}
				else if(degree == 270)
				{
					translateX = (_thumbHeight - _thumbWidth)/2;
					translateY = (_thumbHeight - _thumbWidth)/2;
				}
			}
			else
			{
				if(degree == 90)
				{
					translateX = -(_thumbHeight - _thumbWidth)/2;
					translateY = 0;
					
				}
				else if(degree == 270)
				{
					translateX = (_thumbHeight - _thumbWidth)/2;
					translateY = 0;
				}
			}
			
						
			if (target.style.msTransform !== undefined) { // IE
               	target.style.msTransform = 'rotate(' + degree + 'deg) translateX(' + translateX + 'px) translateY(' + translateY + 'px)';				
            } else if (target.style.MozTransform !== undefined) {  // Mozilla
                target.style.MozTransform = 'rotate(' + degree + 'deg) translateX(' + translateX + 'px) translateY(' + translateY + 'px)';				
            } else if (target.style.OTransform !== undefined) {   // Opera
                target.style.OTransform = 'rotate(' + degree + 'deg) translateX(' + translateX + 'px) translateY(' + translateY + 'px)';
            } else if (target.style.webkitTransform !== undefined) { // Chrome Safari
                target.style.webkitTransform = 'rotate(' + degree + 'deg) translateX(' + translateX + 'px) translateY(' + translateY + 'px)';
            } else {
                target.style.transform = 'rotate(' + degree + 'deg) translateX(' + translateX + 'px) translateY(' + translateY + 'px)';
            }
		}
		
		        
        function ImageViewerAnnotationResize(Pg)
        {                                         
            drawImageViewerAnnotations(Pg);                       
        }
        
		function showLoading()
		{
			$("#loading").show();
			$("#imgBig").hide();
			$("#draw_canvas").hide();
		}
		
		function ChangeThumbScrollBar(toShowId, isUp)
		{
			var divOuterHeight = $(".thumbnail").outerHeight(true);
			var sTop = $("#_plcImgsThumbs").scrollTop();
			var sHeight = $("#_plcImgsThumbs")[0].scrollHeight;
			var thumbHeight =  $("#_plcImgsThumbs").height();
			var newValue = toShowId * divOuterHeight;

			if(newValue >= sTop && newValue <= (sTop + thumbHeight - divOuterHeight))
			{}
			else 
			{
				if(newValue < 0)
				{
					$("#_plcImgsThumbs").scrollTop(0);
				}
				else if(newValue > sHeight)
				{
					$("#_plcImgsThumbs").scrollTop(sHeight);
				}
				else 
				{
					if(isUp)
					{
						$("#_plcImgsThumbs").scrollTop(newValue);
					}
					else 
					{
						if(newValue < (sTop + thumbHeight) && newValue > sTop)
						{
							newValue = $("#_plcImgsThumbs").scrollTop() + divOuterHeight;
							$("#_plcImgsThumbs").scrollTop(newValue);
						}
						else 
						{
							$("#_plcImgsThumbs").scrollTop(newValue);
						}
					}
				}
			}
		}
		
		function SortPage()
        {            
            var childrenDivs=$(".RE_ThumbViewer").children("div");
            var pageIndexOrder = new Array();
			var showIndexOrder = new Array();
            var newIndex = "";

            for(index in childrenDivs)
            {				
                var divId=childrenDivs.eq(index).attr("id");
                if(divId != null)
                {                    
                    idPosition=divId.indexOf("_");
                    var id=divId.substring(idPosition+1);
					pageIndexOrder[id] = index;
					showIndexOrder[index] = id;
                }
                
            }
         
			_fileDocument[curFileId].SortPage(pageIndexOrder);
        }
        
        function ThumbStretch()
        {
            if(_isStretch)
            {
                ThumbShrink();
                _isStretch = false;
            }
            else
            {
                ThumbExpansion();
                _isStretch = true;
            }
            setCookie(_cookieStretch,_isStretch,3);
        }
        
        function ajaxFileUpload()
		{
			$("#loading")
			.ajaxStart(function(){
				$(this).show();			
			})
			.ajaxComplete(function(){
				$(this).hide();			
			});

			$.ajaxFileUpload
			(	
				{			
				url:getRootPath() + '/RasterEdge_Imaging_Files/doUpload.aspx',
				secureuri:false,
				fileElementId:'fileToUpload',
				dataType: 'json',
				success: function (data, status)
				{			
					if(typeof(data.error) != 'undefined'){}
					_uploadFileName = data+"";
					if(_uploadFileName.startWith(" "))
					{
						_uploadFileName = _uploadFileName.substring(1);
					}
					if(_uploadFileName.endWith(" "))
					{
						_uploadFileName = _uploadFileName.substring(0,_uploadFileName.length-1);
					}
					setCookie(_cookieFilePath,_uploadFileName,3);
					changeServerFileName();
				},
				error: function (data, status, e)
				{
					alert(e);
				}
			}
			)	
			return false;
		}
		
		
		function changeServerFileName()
		{			
            var options = {
            type: "POST",
            url: getCurrentAspx(),
            async:false,
			data: {method:"OpenFile", uploadfile: _uploadFileName}, 
            success: function(response) { 				
            },
            error: function(XMLHttpRequest, textStatus, errorThrown) {  
            }  
            }
			
            $.ajax(options);				

			reloadViewer();
		}
        
        function reloadViewer()
		{		
			_imageViewers = new Array();
            _thumbViewers = new Array();
			_docAnnos = new Array();
			_thumbAnnos = new Array();
			_showPageIds = new Array();	
			_fileDocument = new Array();
			
			$("#_plcImgsThumbs").empty();
			CreateThumbLab();
			$("#navi_pagePart").empty();
			CreatePagePart("#navi_pagePart");
			
			$("#_plcImgsThumbs").scrollTop(0);
			_initPageIndex = 0;
			_curCorrectPageId = 0;
			
			OnLoadReady();
		}
		
		function OpenFile()
		{
			
			$("form").first().attr("action",_openFileUrl).submit(); 
		}

		function SaveFile()
		{		
		    $("#loading")
			.ajaxStart(function(){
				$(this).show();			
			})	            
			var str = {action: "SaveFile", Rnd: _rnd, jsonDoc: $.toJSON(_fileDocument[curFileId]),fileName:_uploadFileName};
           
			var options = {
            type: "POST",
            url: getServerHandlerUrl(),
            async:false,
            data: str, 
            success: function(response) {   
				_fileDocument[curFileId].DeleteBurnedAnnos();
            },
            error: function(err) {  
            }  
            }
 
            $.ajax(options);	
			
			$("#loading")	
			.ajaxComplete(function(){
				$(this).hide();			
			});
			
			var options1 = {
            type: "POST",
            url: getCurrentAspx(),
            async:false,
			data: {method:"SaveFile"}, 
            success: function(response) {                            
            },
            error: function(err) {  
            }  
            }
 
            $.ajax(options1);
		}

		function DownloadFile()
		{
		    $("#loading")
			.ajaxStart(function(){
				$(this).show();			
			})
			.ajaxComplete(function(){
				$(this).hide();			
			});
			
			var saveFile;
			
			var str = {action: "DownLoadFile", Rnd: _rnd, jsonDoc: $.toJSON(_fileDocument[curFileId]),fileName:_uploadFileName};
           
			var options = {
            type: "POST",
            url: getServerHandlerUrl(),
            async:false,
            data: str, 
            success: function(response) { 
				_fileDocument[curFileId].DeleteBurnedAnnos();
				var arr = eval('(' + response + ')');
				saveFile = getRootPath() + "/RasterEdge_Cache/" + arr.msg; 

				window.open(saveFile);
            },
            error: function(err) {  
            }  
            }
 
            $.ajax(options);			
		}

		function PrintFile()
		{
		    $("#loading")
			.ajaxStart(function(){
				$(this).show();			
			})
			.ajaxComplete(function(){
				$(this).hide();			
			});
			
			var saveFile;
			
			var str = {action: "DownLoadFile", Rnd: _rnd, jsonDoc: $.toJSON(_fileDocument[curFileId]),fileName:_uploadFileName};
           
			var options = {
            type: "POST",
            url: getServerHandlerUrl(),
            async:false,
            data: str, 
            success: function(response) { 
				_fileDocument[curFileId].DeleteBurnedAnnos();
				var arr = eval('(' + response + ')');
				saveFile = getRootPath() + "/RasterEdge_Cache/" + arr.msg; 
				if(document.all)
				{
					window.open(saveFile);
				}
				else
				{
					window.open(saveFile);
					window.onload = window.print();	
				}				
            },
            error: function(err) {  
            }  
            }
 
            $.ajax(options);       	
		}
		
		function AddPage()
        {
                        
            currentPg = parseInt(_curCorrectPageId); 
			thisPg = _fileDocument[curFileId].GetPageIndex(currentPg);	          
			 
			addPageIndex = _maxTotalPageCount;
			addDocument(addPageIndex);

		    _fileDocument[curFileId].AddPage(currentPg);
			
			addShowIndex = _fileDocument[curFileId].GetShowIndex(addPageIndex);                                                  
                                   
            addId = "thumbnail_" + addPageIndex;
            addString = "<div id='lbl_" + addPageIndex + "' class='thumbnail' onclick='ChangePg(" + addPageIndex + ")'>";
			addString += "<div id='thumbDiv_" + addPageIndex + "' class='thumbDiv' style='width:" + _thumbWidth + "px;height:" + _thumbHeight + "px;'>"
            addString += "<img id='thumbnail_" + addPageIndex + "' src='" + _fileDocument[curFileId].Viewers[addPageIndex].GetThumbSrc() + "' >";
            addString += "</div><div id='thumbPgId_" + addPageIndex + "' class='thumbPgId'>" + (addShowIndex + 1) + "</div></div>";
            
            beforeId = "#lbl_" + thisPg;
            
            $(beforeId).before(addString);  
            $("#pageIdList").empty();
            for(var i=1;i<=_totalPageCount;i++)
            {
                var text = i+" / "+_totalPageCount;
		        var value = i; 
		        $("#pageIdList").append( $('<option></option>').val(value).html(text));    
            }
			ChangePg(addPageIndex);			          
        }
        
        function DeletePage()
        {
			var deleteBox = window.confirm("Are you sure to delete? All deleted pages will not exist anymore.");
			if(deleteBox != true)
			{
				return false;
			}

			_totalPageCount--; 
			if(_totalPageCount<1)
			{
				_totalPageCount = 1;
				alert("Can not remove the only page from the file!");
				return false;
			}
			
            currentPg = parseInt(_curCorrectPageId);
			 
			pageIndex = _fileDocument[curFileId].GetPageIndex(currentPg);
                   
            deleteId = "lbl_" + pageIndex;
            
            $("#"+deleteId+"").remove();

            _fileDocument[curFileId].DeletePage(currentPg);			 
		
			deletePageIndex = _fileDocument[curFileId].GetPageIndex(currentPg);
			if(deletePageIndex==null)
			{
				deletePageIndex = _totalPageCount - 1;
			}
			  
			$("#totalPageIndex").text(_totalPageCount);

			deleteShowPageIds();
            $("#pageIdList").empty();
            for(var i=1;i<=_totalPageCount;i++)
            {
                var text = i+" / "+_totalPageCount;
		        var value = i; 
		        $("#pageIdList").append( $('<option></option>').val(value).html(text));
            }
			ChangePg(deletePageIndex);
        } 
        
		function deleteShowPageIds()
		{			
			_showPageIds.splice(_showPageIds.length-1,1);
		}
		
		function btnRotate90()
		{
			pageIndex = parseInt(_curCorrectPageId);	
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
			thumbId = "thumbnail_" + pageIndex;
			_fileDocument[curFileId].Viewers[pageIndex].ChangeRotate("1"); 
			
			ChangeViewerPosition(pageIndex);	
			
			resizeDocViewerByCSS3("imgBig",pageIndex);
			resizeThumbViewerByCSS3(thumbId, pageIndex);
			
			RotateAnnotations(pageIndex, _fileDocument[curFileId].Viewers[pageIndex].Rotate);
		}

		function btnRotate180()
		{
			pageIndex = parseInt(_curCorrectPageId);
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
			thumbId = "thumbnail_" + pageIndex;

			_fileDocument[curFileId].Viewers[pageIndex].ChangeRotate("2"); 
			
			ChangeViewerPosition(pageIndex);
			resizeDocViewerByCSS3("imgBig",pageIndex);
			resizeThumbViewerByCSS3(thumbId, pageIndex);		
			RotateAnnotations(pageIndex, _fileDocument[curFileId].Viewers[pageIndex].Rotate); 
		}

		function btnRotate270()
		{
			pageIndex = parseInt(_curCorrectPageId);
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
			thumbId = "thumbnail_" + pageIndex;

			_fileDocument[curFileId].Viewers[pageIndex].ChangeRotate("-1");

			ChangeViewerPosition(pageIndex);
			resizeDocViewerByCSS3("imgBig",pageIndex);
			resizeThumbViewerByCSS3(thumbId, pageIndex); 
			
			RotateAnnotations(pageIndex, _fileDocument[curFileId].Viewers[pageIndex].Rotate); 
		}
		
		function btnFull()
        {
            if(!_isFull)
            {
                origiWidth = $(".re_content").width();
                ChangeControlSize();
		        _isFull = true;
		    }
		    else
		    {
                RestoreSize();
		        _isFull = false;
		    }
		    setCookie(_cookieIsFull, _isFull ,3);
        }
        
        function ChangeFooterStyle()
		{
		    $("#pageIdList").empty();
            InitBtnCss();
		    for(var i=1;i<=_totalPageCount;i++)
		    {
		        var text = i+" / "+_totalPageCount;
		        var value = i; 
		        $("#pageIdList").append( $('<option></option>').val(value).html(text)); 
		    }
		    $("#pageIdList").bind("change",ChangeItem);
		}
		
		function InitBtnCss()
		{
		    sym = new Array(false,false);
            var _projectName = getProjectName();
            var icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_navi_iconsB1.png";
            $("#navi_first").css("background","url('" + icoUrl1 + "') no-repeat 0px 0px");
            $("#navi_pre").css("background","url('" + icoUrl1 + "') no-repeat -30px 0px");
            icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_navi_iconsB2.png";
            $("#navi_ne").css("background","url('" + icoUrl1 + "') no-repeat 0px 0px");
            $("#navi_last").css("background","url('" + icoUrl1 + "') no-repeat -25px 0px");
		}
		
		function ChangeItem()
		{
		    var changeValue = $(this).val()-1;
		    var curPg = _fileDocument[curFileId].GetPageIndex(changeValue);
		    var isUp = true;
		    if(changeValue > parseInt(_curCorrectPageId))
		    {
			    isUp = false;
		    }

		    ChangeThumbScrollBar(changeValue, isUp);
            ChangePg(curPg); 
		}
		
		function ChangeBtnStyle()
		{
		    var curPg = parseInt(_curCorrectPageId);
		    var _projectName = getProjectName();
		    if(curPg == 0)
		    {
		        if(sym[0])
		        {
		            var icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_navi_iconsB1.png";
		            $("#navi_first").css("background","url('" + icoUrl1 + "') no-repeat 0px 0px");
		            $("#navi_first").unbind("click");
		            $("#navi_pre").css("background","url('" + icoUrl1 + "') no-repeat -30px 0px");
		            $("#navi_pre").unbind("click");
		            sym[0]=false;
		        }
		        if(!sym[1])
		        {
		           icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_navi_iconsA2.png";
		            $("#navi_ne").css("background","url('" + icoUrl1 + "') no-repeat 0px 0px");
		            $("#navi_ne").bind("click",DownPage);
		            $("#navi_last").css("background","url('" + icoUrl1 + "') no-repeat -25px 0px");
		            $("#navi_last").bind("click",LastPage);
		            sym[1]=true; 
		        }
		    }
		    else if(curPg == _totalPageCount-1)
		    {
		        if(sym[1])
		        {
		            icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_navi_iconsB2.png";
		            $("#navi_ne").css("background","url('" + icoUrl1 + "') no-repeat 0px 0px");
		            $("#navi_ne").unbind("click");
		            $("#navi_last").css("background","url('" + icoUrl1 + "') no-repeat -25px 0px");
		            $("#navi_last").unbind("click");
		            sym[1]=false;
		        }
		        if(!sym[0])
		        {
		            var icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_navi_iconsA1.png";
		            $("#navi_first").css("background","url('" + icoUrl1 + "') no-repeat 0px 0px");
		            $("#navi_first").bind("click",FirstPage);
		            $("#navi_pre").css("background","url('" + icoUrl1 + "') no-repeat -30px 0px");
		            $("#navi_pre").bind("click",UpPage);
		            sym[0]=true; 
		        }
		    }
		    else
		    {
		        if(!sym[0])
		        {
		            var icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_navi_iconsA1.png";
		            $("#navi_first").css("background","url('" + icoUrl1 + "') no-repeat 0px 0px");
		            $("#navi_first").bind("click",FirstPage);
		            $("#navi_pre").css("background","url('" + icoUrl1 + "') no-repeat -30px 0px");
		            $("#navi_pre").bind("click",UpPage);
		            sym[0]=true;
		        }
		        if(!sym[1])
		        {
		            icoUrl1 = _projectName + "/RasterEdge_Imaging_Files/RE_navi_iconsA2.png";
		            $("#navi_ne").css("background","url('" + icoUrl1 + "') no-repeat 0px 0px");
		            $("#navi_ne").bind("click",DownPage);
		            $("#navi_last").css("background","url('" + icoUrl1 + "') no-repeat -25px 0px");
		            $("#navi_last").bind("click",LastPage);
		            sym[1]=true;
		        }
		    }
		}
		
		function FirstPage()
        {
            var curPg = _fileDocument[curFileId].GetPageIndex(0);
		    var isUp = true;
		    if(0 > parseInt(_curCorrectPageId))
		    {
			    isUp = false;
		    }

		    ChangeThumbScrollBar(0, isUp);
            ChangePg(curPg);
		}
		
		function LastPage()
		{
            var curPg = _fileDocument[curFileId].GetPageIndex(_totalPageCount-1);
		    var isUp = true;
		    if(_totalPageCount-1 > parseInt(_curCorrectPageId))
		    {
			    isUp = false;
		    }

		    ChangeThumbScrollBar(_totalPageCount-1, isUp);
            ChangePg(curPg);
		}
		
		 function UpPage()
        {
            var currentPg = parseInt(_curCorrectPageId);
            if(currentPg > 0)
            {                               
                to_page = currentPg - 1;
				ChangeThumbScrollBar(to_page, true);
				to_page = _fileDocument[curFileId].GetPageIndex(to_page);
                ChangePg(to_page);  				
            }
			
			return false;
        }
        
        function DownPage()
        {
            var currentPg = parseInt(_curCorrectPageId);           
            totalPages = _totalPageCount - 1;
            if(currentPg < totalPages)
            {                          
                to_page = currentPg + 1;
				ChangeThumbScrollBar(to_page, false);
				to_page = _fileDocument[curFileId].GetPageIndex(to_page);

                ChangePg(to_page); 
            }
			
            return false;
        }
        
        function BindZoomEvent()
        {
            $("#zoomList").bind("change",setZoomValue);
        }
        
        function setZoomValue()
        {
            _zoomListIndex = parseInt($(this).get(0).selectedIndex);
            var pageIndex = parseInt(_curCorrectPageId);	
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
			var viewer = _fileDocument[curFileId].Viewers[pageIndex];
			
            if(_zoomListIndex >=0 && _zoomListIndex<=15)
            {
                var length = _zoomItems[_zoomListIndex].length;
                var multiple = _zoomItems[_zoomListIndex].substring(0,length-1);
                _pageSizeType = 2;
                _zoomIndex = parseFloat(multiple)/10;
                var str = _zoomIndex + ";" + _zoomListIndex;
                setCookie(_cookieZoomIndex, str ,3);
                ChangeViewerPosition(pageIndex);
                    		    
			    resizeDocViewerByCSS3("imgBig",pageIndex);
			    ImageViewerAnnotationResize(pageIndex);
            }
            else
            {
                map();
            }
        }
        
        function map()
        {
            var pageIndex = parseInt(_curCorrectPageId);	
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
            var viewer = _fileDocument[curFileId].Viewers[pageIndex];
            switch(_zoomListIndex)
            {
                case 17:
                    btnOneToOne();
                    setCookie(_cookieZoomIndex,"Actual Size",3);
                    _zoomListIndex = 8;
                    break;
                case 18:
                    btnBestFit();
                    setCookie(_cookieZoomIndex,"Fit Page",3);
                    var percent = (_docHeight/viewer.OneToOne_Height*100).toFixed(2);
                    _zoomListIndex = setPercentPosition(percent);
                    break;
                case 19:
                    btnFitWidth();
                    setCookie(_cookieZoomIndex,"Fit Width",3);
                    var percent = (_docWidth/viewer.OneToOne_Width*100).toFixed(2);
                    _zoomListIndex = setPercentPosition(percent);
                    break;  
            } 
        }
        
        function setPercentPosition(percent)
        {
            var _comlength = _zoomItems.length;
            for(var i=0;i< _comlength - 3;i++)
            {
                var length = _zoomItems[i].length;
                var multiple = parseInt(_zoomItems[i].substring(0,length-1));
                if(percent>= parseFloat(multiple))
                    return i;
            }
            return _comlength - 4;
        }	

		function btnZoomIn()
		{			
			pageIndex = parseInt(_curCorrectPageId);	
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
            _pageSizeType = 2;
			ChangeZoomIndex("1");
				
			ChangeViewerPosition(pageIndex);
			
			resizeDocViewerByCSS3("imgBig",pageIndex);

			ImageViewerAnnotationResize(pageIndex);
		}

		function btnZoomOut()
		{
			pageIndex = parseInt(_curCorrectPageId);
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
            _pageSizeType = 2;
			ChangeZoomIndex("-1");
				
			ChangeViewerPosition(pageIndex);
			
			resizeDocViewerByCSS3("imgBig",pageIndex);

			ImageViewerAnnotationResize(pageIndex); 
		}
        
        function ChangeZoomIndex(changeValue)
		{
			if(changeValue == "-1")
			{
			    _zoomListIndex++;
			    if(_zoomListIndex > 15)
			        _zoomListIndex = 15;		
			}
			else if(changeValue == "1")
			{
				_zoomListIndex--;
				if(_zoomListIndex < 0)
				    _zoomListIndex = 0;
			}
			$("#zoomList").val(_zoomItems[_zoomListIndex]);	
			var length = _zoomItems[_zoomListIndex].length;
            var multiple = _zoomItems[_zoomListIndex].substring(0,length-1);
            _zoomIndex = parseFloat(multiple)/10;
            var str = _zoomIndex + ";" + _zoomListIndex;
            setCookie(_cookieZoomIndex, str ,3);
		}
		
		function btnFitWidth()
		{
			pageIndex = parseInt(_curCorrectPageId);
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);

			_pageSizeType = 0;
			_zoomIndex = _defaultZoonIndex;
				
			ChangeViewerPosition(pageIndex);
			resizeDocViewerByCSS3("imgBig",pageIndex);
			
			ImageViewerAnnotationResize(pageIndex);
		}

		function btnBestFit()
		{
			pageIndex = parseInt(_curCorrectPageId);
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);

			_pageSizeType = 1;
			_zoomIndex = _defaultZoonIndex;				
			
			ChangeViewerPosition(pageIndex);
			resizeDocViewerByCSS3("imgBig",pageIndex);
			
			ImageViewerAnnotationResize(pageIndex);
		}

		function btnOneToOne()
		{
			pageIndex = parseInt(_curCorrectPageId);
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);

			_pageSizeType = 2;
			_zoomIndex = _defaultZoonIndex;

			ChangeViewerPosition(pageIndex);
			resizeDocViewerByCSS3("imgBig",pageIndex);
			
			ImageViewerAnnotationResize(pageIndex);
		}
		
		function burnAnnotationToImage()
		{			
			DeleteAllAnnotation();
			_fileDocument[curFileId].BurnAnnos();
			
			pageIndex = parseInt(_curCorrectPageId);
			pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
			drawImageViewerAnnotations(pageIndex);
            //alert("Trial version does not support this feature!");
		}
		
		function DeleteAllAnnotation()
        {		        
            selectDiv = new Array();
        }
		
		function DeleteAnnotation()
        {
            for(var i = 0; i < selectDiv.length; i++)
            {
                deleteDivId=selectDiv[i];
				$("#div_"+deleteDivId).remove();
                $("#thumbAnnotation_"+deleteDivId).remove();
				_fileDocument[curFileId].DeleteAnno(deleteDivId);              
            }
            
        }
        
        var isCtrlDown=false;
        $(document).keydown(function(event)
        {
            if(event.ctrlKey)
            {
                isCtrlDown=true;            
            }
        });
        
        $(document).keyup(function()
        {
            isCtrlDown=false;
        });
        
        var scrollFunc=function(e)
        {
            e=e||window.event;
            if (e&&e.preventDefault){ 
                e.preventDefault();
                e.stopPropagation();
            }else{ 
                e.returnvalue=false;  
                return false;     
            }
	    };
    	
        String.prototype.startWith=function(str)
        {    
            var reg=new RegExp("^"+str);    
            return reg.test(this);       
        } 
     
        String.prototype.endWith=function(str)
        {    
            var reg=new RegExp(str+"$");    
            return reg.test(this);       
        }
        
                
		function getRootPath()
		{
			var curWwwPath = window.document.location.href;
			var pathName = window.document.location.pathname;
			var pos = curWwwPath.lastIndexOf(pathName);
			var localhostPath = curWwwPath.substring(0, pos);
			var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);

			return (localhostPath + projectName);
		}
		
		function getCurrentAspx()
		{
		    var curWwwPath = window.document.location.href;
					    
		    return curWwwPath;
		}

		function getServerHandlerUrl()
		{
			var curWwwPath = window.document.location.href;
			var projectName = getProjectName();
			
			var posi = curWwwPath.indexOf(projectName) + projectName.length;
			var handler = curWwwPath.substring(0, posi) + _serverUrl;

			return handler;
		}

		
		function getProjectName()
		{		    
			var pathName = window.document.location.pathname;
				
			var projectName = pathName.substring(0, pathName.substr(1).indexOf('/') + 1);

			return projectName;
		}
     