var TextAnnoStyle = new AnnoStyle({FillColor: "White", ShowedText: "double click to edit", TextColor: "Black", TextFont: "Arial", TextSize: 12, TextStyle :"normal"});
var FreehandAnnoStyle = new AnnoStyle({OutLineColor: "Red", OutLineWidth: 3.0});
var HighlightAnnoStyle = new AnnoStyle({FillColor: "Yellow"});
var RectangleAnnoStyle = new AnnoStyle({OutLineColor: "Black", OutLineWidth: 3.0});
var FilledRectangleAnnoStyle = new AnnoStyle({OutLineColor: "Black", OutLineWidth: 3.0, FillColor: "Black", Transparency: 1});
var EllipseAnnoStyle = new AnnoStyle({FillColor: "Orange"});
var RubberStampAnnoStyle = new AnnoStyle({OutLineColor: "Tomato", OutLineWidth: 3.0, FillColor: "Red", ShowedText: "Stamp annotation can show text here", TextColor: "Black", TextFont: "Arial", TextSize: 12, TextStyle: "Italic"});
var PolygonLinesAnnoStyle = new AnnoStyle({OutLineColor: "Red", OutLineWidth: 3.0});
var PolygonAnnoStyle = new AnnoStyle({OutLineColor: "OrangeRed", OutLineWidth: 3.0, FillColor: "OrangeRed"});
var LineAnnoStyle = new AnnoStyle({OutLineColor: "Red", OutLineWidth: 3.0});

var _serverUrl=""; 
var _openFileUrl="";
var _docWidth; 
var _docHeight; 
var _thumbWidth; 
var _thumbHeight; 

var selectDiv = new Array();
var num=0;
var selectTextId;
var x_points = new Array();
var y_points = new Array();
var point_index=0;
var showIds = new Array();

//////////////////////////////////////////////////////////

var _zoomRatio = 0.1;
var _zoomIndex = 10; 
var _defaultZoonIndex = 10;
var _defaultPageSizeType=1;
var _pageSizeType=_defaultPageSizeType;
var _totalPageCount = 0;
var _maxTotalPageCount = 0;

Array.prototype.max = function(){   
 return Math.max.apply({},this); 
};

Array.prototype.min = function(){   
 return Math.min.apply({},this);
};

Array.prototype.S=String.fromCharCode(2);
Array.prototype.in_array=function(e){
    var r=new RegExp(this.S+e+this.S);
    return (r.test(this.S+this.join(this.S)+this.S));
};

var objDelete = {
	text: "Delete",
	func: function() {
		deleteAnno();
	}	
}, objBurn = {
	text: "Burn",
	func: function() {
		burnAnno();
		//alert("Trial version does not support this feature");
	}	
};
var annoMenuData = [
	[objDelete, objBurn]
];

function deleteAnno()
{	
	for(var i = 0; i < selectDiv.length; i++)
	{
		$("#div_"+selectDiv[i]).remove();
		$("#thumbAnnotation_"+selectDiv[i]).remove();
		_fileDocument[curFileId].DeleteAnno(selectDiv[i]);

		$("#smartMenu_"+selectDiv[i]).remove();
	}
}

function burnAnno()
{
	pageIndex = parseInt(_curCorrectPageId);	
	pageIndex = _fileDocument[curFileId].GetPageIndex(pageIndex);
	for(var i = 0; i < selectDiv.length; i++)
	{
		_fileDocument[curFileId].Viewers[pageIndex].BurnAnno(selectDiv[i]);
		
		$("#div_"+selectDiv[i]).remove();
		drawImageViewerBurnAnnotation(pageIndex)
		
		$("#smartMenu_"+selectDiv[i]).remove();
	}
	
}

//////////////////////////////
function loadThumbAnnotation()
{	

	for(index in _docAnnos)
    {   
		if(_docAnnos[index].Index == null)
		{
			continue;
		}
		
		thumbViewerWidth=GetThumbViewerByIndex(_docAnnos[index].PageIndex).Width;
		thumbViewerHeight=GetThumbViewerByIndex(_docAnnos[index].PageIndex).Height;
		

		$("#thumbDiv_"+_docAnnos[index].PageIndex).width(thumbViewerWidth).height(thumbViewerHeight);

		
        var thumbParentId = "thumbDiv_"+_docAnnos[index].Index;
        
		var thumbAnno = new Annotation({Type: _docAnnos[index].Type, Index: _docAnnos[index].Index, PageIndex: _docAnnos[index].PageIndex, ShowLeft: _docAnnos[index].Left, ShowTop: _docAnnos[index].Top, ShowWidth: _docAnnos[index].Width, ShowHeight: _docAnnos[index].Height, Rotate: _docAnnos[index].Rotate, ParentId: thumbParentId, Text: _docAnnos[index].Text, ShowPoints: _docAnnos[index].ShowPoints, IsThumb: true});
	    _thumbAnnos[_docAnnos[index].Index]=thumbAnno;	
		
    }

	for(index in _thumbAnnos)
	{		
		addThumbViewerAnnotation(_thumbAnnos[index]);
	}
}

$(function () {
	var AnnotationRightClick=function(e){
		e=e||window.event;
		selectId = (e.target.id);
		
		
		$(".rRightDown").css("visibility", "hidden");
		$(".rLeftDown").css("visibility", "hidden");
		$(".rLeftUp").css("visibility", "hidden");
		$(".rRightUp").css("visibility", "hidden");
		$(".rRight").css("visibility", "hidden");
		$(".rLeft").css("visibility", "hidden");
		$(".rUp").css("visibility", "hidden");
		$(".rDown").css("visibility", "hidden");

		if(_isInDocViewer)
		{
			idPosition = selectId.indexOf("_");

			if(idPosition == -1)
			{				
			}
			else 
			{	
				showResizeId = selectId.substring(idPosition + 1);

				if(isCtrlDown == false)
				{
					showIds = new Array();
				}
		
				showIds.push(showResizeId);

				for(var i = 0; i < showIds.length; i++)
				{
					$("#rRightDown_"+showIds[i]+"").css("visibility", "visible");
					$("#rLeftDown_"+showIds[i]+"").css("visibility", "visible");
					$("#rLeftUp_"+showIds[i]+"").css("visibility", "visible");
					$("#rRightUp_"+showIds[i]+"").css("visibility", "visible");
					$("#rRight_"+showIds[i]+"").css("visibility", "visible");
					$("#rLeft_"+showIds[i]+"").css("visibility", "visible");
					$("#rUp_"+showIds[i]+"").css("visibility", "visible");
					$("#rDown_"+showIds[i]+"").css("visibility", "visible");
				}

				if(e.button==2) 
				{	
					isAnnoBurned = _fileDocument[curFileId].IsAnnoBurned(showResizeId);
					
					if(!isAnnoBurned)
					{	
						$("#div_"+showResizeId).smartMenu(annoMenuData, {
							name: showResizeId							
						});
					}
					
				}
			}			
		}
		
	}

	$("body").bind("mouseup",AnnotationRightClick);
});

$(function () {
	var ShowAnnotationStyle=function(e){
		e=e||window.event;
		selectId = (e.target.id);

		$(".rRightDown").css("visibility", "hidden");
		$(".rLeftDown").css("visibility", "hidden");
		$(".rLeftUp").css("visibility", "hidden");
		$(".rRightUp").css("visibility", "hidden");
		$(".rRight").css("visibility", "hidden");
		$(".rLeft").css("visibility", "hidden");
		$(".rUp").css("visibility", "hidden");
		$(".rDown").css("visibility", "hidden");

		if(_isInDocViewer)
		{
			idPosition = selectId.indexOf("_");

			if(idPosition == -1)
			{				
			}
			else 
			{				
				showResizeId = selectId.substring(idPosition + 1);
		
				if(isCtrlDown == false)
				{
					showIds = new Array();
				}
		
				showIds.push(showResizeId);

				for(var i = 0; i < showIds.length; i++)
				{
					$("#rRightDown_"+showIds[i]+"").css("visibility", "visible");
					$("#rLeftDown_"+showIds[i]+"").css("visibility", "visible");
					$("#rLeftUp_"+showIds[i]+"").css("visibility", "visible");
					$("#rRightUp_"+showIds[i]+"").css("visibility", "visible");
					$("#rRight_"+showIds[i]+"").css("visibility", "visible");
					$("#rLeft_"+showIds[i]+"").css("visibility", "visible");
					$("#rUp_"+showIds[i]+"").css("visibility", "visible");
					$("#rDown_"+showIds[i]+"").css("visibility", "visible");
				}
				
			}

			currentTextDiv = "text_"+selectTextId;
			if(selectId != currentTextDiv)
			{
				$(".text_edit").css("visibility", "hidden");
				if(selectTextId != null)
				{
					text_shown_original = $("#text_"+selectTextId+"").val();
					text_shown = escape(text_shown_original).replace(/\+/g, '%2B').replace(/\"/g,'%22').replace(/\'/g, '%27').replace(/\//g,'%2F');
				
					curPg = parseInt(_curCorrectPageId);
					curPg = _fileDocument[curFileId].GetPageIndex(curPg);
					
					_fileDocument[curFileId].Viewers[curPg].Annotations[selectTextId].Text=text_shown_original;
					
					var annotation = _fileDocument[curFileId].Viewers[curPg].Annotations[selectTextId];

					src = createTextAnnoWidthCanvas(annotation.Text, {fontStyle: annotation.TextStyle, fontName: annotation.TextFont, fontSize: annotation.TextSize, fillColor: annotation.FillColor, textColor: annotation.TextColor, width: annotation.ShowWidth, height: annotation.ShowHeight, transparency: annotation.Transparency});
					$("#annotation_"+selectTextId+"").attr("src",src);

					selectTextId = null;
				}		
			}
		}
		
		
	}

	$("body").bind("click",ShowAnnotationStyle);
});

var isDragIn=false;
var divScrollLeft=0;
var divScrollTop=0;
var divScrollWidht = 0;
var divScrollHeight = 0;
var xDown=0,yDown=0;

	var dragImageMouseDown = function(e)
	{	
		isDragIn=true;
		divScrollLeft = $("#re_canvas")[0].scrollLeft;
		divScrollTop = $("#re_canvas")[0].scrollTop;
		divScrollWidht = $("#re_canvas")[0].scrollWidth;
		divScrollHeight = $("#re_canvas")[0].scrollHeight;
		e=e||window.event;
		xDown = e.pageX;
		yDown = e.pageY;
		
		return false;		
	};

	var dragImageMouseMove = function(e)
	{
		if(isDragIn)
		{
			e=e||window.event;
			xMove = e.pageX;
			yMove = e.pageY;
			move_x = xMove - xDown;
			move_y = yMove - yDown;
			moveLeft = divScrollLeft - (move_x);
			moveTop = divScrollTop - (move_y);
			if(moveLeft>0 && moveLeft<divScrollWidht)
			{
				$("#re_canvas")[0].scrollLeft = moveLeft;
			}
			if(moveTop>0 && moveTop<divScrollHeight);
			{
				$("#re_canvas")[0].scrollTop = moveTop;
			}
		}
		return false;		
	};

	var dragImageMouseUp = function(e)
	{
		isDragIn=false;
		return false;
	};


/////////////////////////////////////////////////////////

var _clickNum=0;
var _annotationLastType;

function draw_annotation(annotationType)
{
	_clickNum++;
	_annotationLastType=annotationType;	

	if(_clickNum == 1)
	{
		$(".showByDrag").unbind("mousedown",dragImageMouseDown);
		$(".showByDrag").unbind("mousemove",dragImageMouseMove);
		$(".showByDrag").unbind("mouseup",dragImageMouseUp);
		$(".showByDrag").removeClass("mouseStyleMove");
		$(".showByDrag").addClass("mouseStyleAnnotation");

		curPg = parseInt(_curCorrectPageId);
		curPg = _fileDocument[curFileId].GetPageIndex(curPg);

		if(_annotationLastType == "PolygonLines" || _annotationLastType == "Polygon")
		{
			draw_Polygon(curPg);
		}
		else if(_annotationLastType == "Line")
		{
			 draw_Line(curPg);
		}
		else
		{
			draw_rect(curPg);	
		}
		
		
	}	
}

function draw_Line(curPg)
{
	var parent_left=0;
	var parent_top=0;
	var nScrollLeft=0;
	var nScrollTop=0;
	
	var viewer = _fileDocument[curFileId].Viewers[curPg];

	var x=0,y=0;
	var down_flag=false;
	
	var MouseDown=function(e)
	{
		parent_left=$("#re_canvas").offset().left;
		parent_top=$("#re_canvas").offset().top;
		nScrollLeft=$("#re_canvas")[0].scrollLeft;
		nScrollTop=$("#re_canvas")[0].scrollTop;
		e=e||window.event;
		if(!down_flag)		
		{	
			x=e.pageX-parent_left+nScrollLeft;
			y=e.pageY-parent_top+nScrollTop;
			var content = "";
			x_points[point_index]=x;
			y_points[point_index]=y;
			point_index++;

						var x_min=x_points.min();
						var y_min=y_points.min();
						var x_max=x_points.max();
						var y_max=y_points.max();

						var width_max=x_max-x_min;
						var height_max=y_max-y_min;

						strPoints="";

						for(var i=0;i<x_points.length;i++)
						{
							strPoints+=x_points[i]+","+y_points[i]+";";
						}
						strPoints = strPoints.substring(0,strPoints.length-1);

var docAnno = new Annotation({Type: _annotationLastType, Index: num, ShowLeft: x_min, ShowTop: y_min, ShowPoints: strPoints, ViewerLeft: viewer.Left, ViewerTop: viewer.Top, ViewerWidth: viewer.ActualWidth, ViewerHeight: viewer.ActualHeight, ViewerScaleX: viewer.ScaleX, ViewerScaleY: viewer.ScaleY, ThumbViewerScaleX: viewer.ThumbScaleX, ThumbViewerScaleY: viewer.ThumbScaleY, ViewerOne2OneWidth: viewer.OneToOne_Width, ViewerOne2OneHeight: viewer.OneToOne_Height});

$("#div_"+num).remove();

			var content = "<div class='new_rect' id='div_"+num+"' style='left:"+x_min+"px;top:"+y_min+"px;"+"width:"+width_max+"px;height:"+height_max+"px'>";
				 content += drawAnnotationWithSVG(docAnno, "annotation", false);
				 content += "</div>"

			$("#draw_canvas").append(content);  

			down_flag = true;			
		}
		else 
		{
			$("#div_"+num).remove();
			
			addImageViewerAnnotation(num, 0, 0, 0, 0, _annotationLastType, nScrollLeft, nScrollTop);			
			
			x_points = new Array();
			y_points = new Array();
			point_index=0;
					
			addThumbViewerAnnotation(_fileDocument[curFileId].Viewers[curPg].Annotations[num], curPg);


			$("#div_"+num).draggable({containment:"parent",scroll:false});
			$("#div_"+num).mousedown(function(){
			$(this).addClass("mousedown");//add shadow
			});
			$("#div_"+num).mouseup(function(){
			$(this).removeClass("mousedown");//delete shadow
			});

			num++;

			$("#draw_canvas").unbind("mousedown",MouseDown);
			$("#draw_canvas").unbind("mousemove",MouseMove);
			
			_clickNum=0;

			$(".showByDrag").bind("mousedown",dragImageMouseDown);
			$(".showByDrag").bind("mousemove",dragImageMouseMove);
			$(".showByDrag").bind("mouseup",dragImageMouseUp);
			$(".showByDrag").removeClass("mouseStyleAnnotation");
			$(".showByDrag").addClass("mouseStyleMove");

			down_flag = false;
			
		}
	};	
    
	var MouseMove=function(e)
	{
		e=e||window.event;
		$("#div_"+num).remove();
		//if(e.button==2) 
		{
			x=e.pageX-parent_left+nScrollLeft;
			y=e.pageY-parent_top+nScrollTop;
			var content = "";
			x_points[point_index]=x;
			y_points[point_index]=y;
			

						var x_min=x_points.min();
						var y_min=y_points.min();
						var x_max=x_points.max();
						var y_max=y_points.max();

						var width_max=x_max-x_min;
						var height_max=y_max-y_min;

						strPoints="";

						for(var i=0;i<x_points.length;i++)
						{
							strPoints+=x_points[i]+","+y_points[i]+";";
						}
						strPoints = strPoints.substring(0,strPoints.length-1);

var docAnno = new Annotation({Type: _annotationLastType, Index: num, ShowLeft: x_min, ShowTop: y_min, ShowPoints: strPoints, ViewerLeft: viewer.Left, ViewerTop: viewer.Top, ViewerWidth: viewer.ActualWidth, ViewerHeight: viewer.ActualHeight, ViewerScaleX: viewer.ScaleX, ViewerScaleY: viewer.ScaleY, ThumbViewerScaleX: viewer.ThumbScaleX, ThumbViewerScaleY: viewer.ThumbScaleY, ViewerOne2OneWidth: viewer.OneToOne_Width, ViewerOne2OneHeight: viewer.OneToOne_Height});
var content = "<div class='new_rect' id='div_"+num+"' style='left:"+x_min+"px;top:"+y_min+"px;"+"width:"+width_max+"px;height:"+height_max+"px'>";
				 content += drawAnnotationWithSVG(docAnno, "annotation", false);
				 content += "</div>"

			$("#draw_canvas").append(content);  
		}
	};


	$("#draw_canvas").bind("mousedown",MouseDown);
	$("#draw_canvas").bind("mousemove",MouseMove);
}

function draw_Polygon(curPg)
{
	var parent_left=0;
	var parent_top=0;
	var nScrollLeft=0;
	var nScrollTop=0;
	
	var viewer = _fileDocument[curFileId].Viewers[curPg];

	var x=0,y=0;
	
	
	var MouseDown=function(e)
	{
		parent_left=$("#re_canvas").offset().left;
		parent_top=$("#re_canvas").offset().top;
		nScrollLeft=$("#re_canvas")[0].scrollLeft;
		nScrollTop=$("#re_canvas")[0].scrollTop;
		e=e||window.event;
		//if(!polygon_Working)//first press down
		{	
			x=e.pageX-parent_left+nScrollLeft;
			y=e.pageY-parent_top+nScrollTop;
			var content = "";
			x_points[point_index]=x;
			y_points[point_index]=y;
			point_index++;

						var x_min=x_points.min();
						var y_min=y_points.min();
						var x_max=x_points.max();
						var y_max=y_points.max();

						var width_max=x_max-x_min;
						var height_max=y_max-y_min;

						strPoints="";

						for(var i=0;i<x_points.length;i++)
						{
							strPoints+=x_points[i]+","+y_points[i]+";";
						}
						strPoints = strPoints.substring(0,strPoints.length-1);

var docAnno = new Annotation({Type: _annotationLastType, Index: num, ShowLeft: x_min, ShowTop: y_min, ShowPoints: strPoints, ViewerLeft: viewer.Left, ViewerTop: viewer.Top, ViewerWidth: viewer.ActualWidth, ViewerHeight: viewer.ActualHeight, ViewerScaleX: viewer.ScaleX, ViewerScaleY: viewer.ScaleY, ThumbViewerScaleX: viewer.ThumbScaleX, ThumbViewerScaleY: viewer.ThumbScaleY, ViewerOne2OneWidth: viewer.OneToOne_Width, ViewerOne2OneHeight: viewer.OneToOne_Height});

$("#div_"+num).remove();

			var content = "<div class='new_rect' id='div_"+num+"' style='left:"+x_min+"px;top:"+y_min+"px;"+"width:"+width_max+"px;height:"+height_max+"px'>";
				 content += drawAnnotationWithSVG(docAnno, "annotation", false);
				 content += "</div>"

			$("#draw_canvas").append(content);  
		}
	};	
    
	var MouseMove=function(e)
	{
		e=e||window.event;
		$("#div_"+num).remove();
		//if(e.button==2) 
		{
			x=e.pageX-parent_left+nScrollLeft;
			y=e.pageY-parent_top+nScrollTop;
			var content = "";
			x_points[point_index]=x;
			y_points[point_index]=y;
			

						var x_min=x_points.min();
						var y_min=y_points.min();
						var x_max=x_points.max();
						var y_max=y_points.max();

						var width_max=x_max-x_min;
						var height_max=y_max-y_min;

						strPoints="";

						for(var i=0;i<x_points.length;i++)
						{
							strPoints+=x_points[i]+","+y_points[i]+";";
						}
						strPoints = strPoints.substring(0,strPoints.length-1);

var docAnno = new Annotation({Type: _annotationLastType, Index: num, ShowLeft: x_min, ShowTop: y_min, ShowPoints: strPoints, ViewerLeft: viewer.Left, ViewerTop: viewer.Top, ViewerWidth: viewer.ActualWidth, ViewerHeight: viewer.ActualHeight, ViewerScaleX: viewer.ScaleX, ViewerScaleY: viewer.ScaleY, ThumbViewerScaleX: viewer.ThumbScaleX, ThumbViewerScaleY: viewer.ThumbScaleY, ViewerOne2OneWidth: viewer.OneToOne_Width, ViewerOne2OneHeight: viewer.OneToOne_Height});
var content = "<div class='new_rect' id='div_"+num+"' style='left:"+x_min+"px;top:"+y_min+"px;"+"width:"+width_max+"px;height:"+height_max+"px'>";
				 content += drawAnnotationWithSVG(docAnno, "annotation", false);
				 content += "</div>"

			$("#draw_canvas").append(content);  
		}
	};

	var MouseUp=function(e)
	{
		e=e||window.event;
		if(e.button==2) 
		{
			$("#div_"+num).remove();

			
			addImageViewerAnnotation(num, 0, 0, 0, 0, _annotationLastType, nScrollLeft, nScrollTop);			
			
			x_points = new Array();
			y_points = new Array();
			point_index=0;
					
			addThumbViewerAnnotation(_fileDocument[curFileId].Viewers[curPg].Annotations[num], curPg);


			$("#div_"+num).draggable({containment:"parent",scroll:false});
			$("#div_"+num).mousedown(function(){
			$(this).addClass("mousedown");//add shadow
			});
			$("#div_"+num).mouseup(function(){
			$(this).removeClass("mousedown");//delete shadow
			});

			num++;

			$("#draw_canvas").unbind("mousedown",MouseDown);
			$("#draw_canvas").unbind("mousemove",MouseMove);
			$("#draw_canvas").unbind("mouseup",MouseUp);

			_clickNum=0;

			$(".showByDrag").bind("mousedown",dragImageMouseDown);
			$(".showByDrag").bind("mousemove",dragImageMouseMove);
			$(".showByDrag").bind("mouseup",dragImageMouseUp);
			$(".showByDrag").removeClass("mouseStyleAnnotation");
			$(".showByDrag").addClass("mouseStyleMove");
		}
	};



	$("#draw_canvas").bind("mousedown",MouseDown);
	$("#draw_canvas").bind("mousemove",MouseMove);
	$("#draw_canvas").bind("mouseup",MouseUp);
}



function draw_rect(curPg){//'theid' used as a canvas layer


var parent_left=0;
var parent_top=0;
var nScrollLeft=0;
var nScrollTop=0;

var viewer = _fileDocument[curFileId].Viewers[curPg];

var x_down=0,y_down=0;
var new_width=0,new_height=0;
var x_original=0,y_original=0;
var original_flag=true,down_flag=false;
var x_point=0,y_point=0;
var append_string;
var MouseDown=function(e){
parent_left=$("#re_canvas").offset().left;
parent_top=$("#re_canvas").offset().top;
nScrollLeft=$("#re_canvas")[0].scrollLeft;
nScrollTop=$("#re_canvas")[0].scrollTop;
down_flag=true;
x_down=e.pageX;
y_down=e.pageY;//Recording the current coordinates of the mouse
if(original_flag){//If it is the first time you click, the coordinates of the starting point of the record to x_original and y_original
x_original=e.pageX-parent_left+nScrollLeft;
y_original=e.pageY-parent_top+nScrollTop;


original_flag=false;
}

return false;
};
var MouseMove=function(e){
if(down_flag){//Mouse movement
x_down=e.pageX-parent_left+nScrollLeft;
y_down=e.pageY-parent_top+nScrollTop;
x_point=x_original;
y_point=y_original;
new_width=x_down-x_original;
if(new_width<0){//Mouse moves to the left
new_width=-new_width;
x_point=x_down;
}
new_height=y_down-y_original;
if(new_height<0){ //Mouse moves to the right
new_height=-new_height;
y_point=y_down;
}

if(_annotationLastType == "Freehand")
{
$("#div_"+num).remove();
e=e||window.event;
                       var x = e.pageX-parent_left+nScrollLeft;
                       var y = e.pageY-parent_top+nScrollTop; 
                       x_points[point_index]=x;
					   y_points[point_index]=y;
					   point_index++;

					   var x_min=x_points.min();
						var y_min=y_points.min();
						var x_max=x_points.max();
						var y_max=y_points.max();

						var width_max=x_max-x_min;
						var height_max=y_max-y_min;

						strPoints="";

						for(var i=0;i<x_points.length;i++)
						{
							strPoints+=x_points[i]+","+y_points[i]+";";
						}
						strPoints = strPoints.substring(0,strPoints.length-1);

					   var docAnno = new Annotation({Type: _annotationLastType, Index: num, ShowLeft: x_min, ShowTop: y_min, ShowPoints: strPoints, ViewerLeft: viewer.Left, ViewerTop: viewer.Top, ViewerWidth: viewer.ActualWidth, ViewerHeight: viewer.ActualHeight, ViewerScaleX: viewer.ScaleX, ViewerScaleY: viewer.ScaleY, ThumbViewerScaleX: viewer.ThumbScaleX, ThumbViewerScaleY: viewer.ThumbScaleY, ViewerOne2OneWidth: viewer.OneToOne_Width, ViewerOne2OneHeight: viewer.OneToOne_Height});
	
					    var content = "<div class='new_rect' id='div_"+num+"' style='left:"+x_min+"px;top:"+y_min+"px;"+"width:"+width_max+"px;height:"+height_max+"px'>";
					    content += drawAnnotationWithSVG(docAnno, "annotation", false);
						content += "</div>"
					    $("#draw_canvas").append(content);     
}
else 
{
	_fileDocument[curFileId].Viewers[curPg].Annotations.splice(num,1);

$("#div_"+num).remove();
addImageViewerAnnotation(num, x_point, y_point, new_width, new_height, _annotationLastType, nScrollLeft, nScrollTop);
}
	
}

return false;
};


var MouseUp=function(e){
down_flag=false;
original_flag=true;

if(_annotationLastType == "Freehand")
{
	
$("#div_"+num).remove();

addImageViewerAnnotation(num, x_point, y_point, new_width, new_height, _annotationLastType, nScrollLeft, nScrollTop);

x_points = new Array();
y_points = new Array();
point_index=0;

}

      
		
addThumbViewerAnnotation(_fileDocument[curFileId].Viewers[curPg].Annotations[num], curPg);


$("#div_"+num).draggable({containment:"parent",scroll:false});
$("#div_"+num).mousedown(function(){
$(this).addClass("mousedown");//add shadow
});
$("#div_"+num).mouseup(function(){
$(this).removeClass("mousedown");//delete shadow
});

num++;

$("#draw_canvas").unbind("mousedown",MouseDown);
$("#draw_canvas").unbind("mousemove",MouseMove);
$("#draw_canvas").unbind("mouseup",MouseUp);

_clickNum=0;

		$(".showByDrag").bind("mousedown",dragImageMouseDown);
		$(".showByDrag").bind("mousemove",dragImageMouseMove);
		$(".showByDrag").bind("mouseup",dragImageMouseUp);
		$(".showByDrag").removeClass("mouseStyleAnnotation");
		$(".showByDrag").addClass("mouseStyleMove");

return false;
};


$("#draw_canvas").bind("mousedown",MouseDown);
$("#draw_canvas").bind("mousemove",MouseMove);
$("#draw_canvas").bind("mouseup",MouseUp);
} 
 


function EditText(textAnno)
{
	selectId = (textAnno.id);
	idPosition = selectId.indexOf("_");
	selectTextId = selectId.substring(idPosition + 1);
	oText = document.getElementById("text_"+selectTextId+"");//$("#text_"+selectTextId+"");

	oText.style.visibility = "visible"; 
	
}

function addThumbViewerAnnotation(annotation, pageIndex)
{
	if(annotation.Index == null)
	{return;}
	append_string="<div class='thumb_rect' id='thumbImg_"+annotation.Index+"' style='left:"+annotation.ThumbShowLeft+"px;top:"+annotation.ThumbShowTop+"px;"+"width:"+annotation.ThumbShowWidth+"px;height:"+annotation.ThumbShowHeight+"px'>";
	
	append_string += drawAnnotationWithSVG(annotation, "thumbAnnotation", true);

	append_string += "</div>";

	$("#thumbDiv_"+pageIndex).append(append_string);
	
}


function addImageViewerAnnotation(annoIndex, left, top, width, height, annoType, nScrollLeft, nScrollTop)
{
	pageId=parseInt(_curCorrectPageId);
	pageId = _fileDocument[curFileId].GetPageIndex(pageId);

	zoomId=_zoomIndex;//GetImageViewerByIndex(pageId).ZoomIndex;
	pageSizeType=_pageSizeType;//GetImageViewerByIndex(pageId).PageSizeType;
	rotateId=_fileDocument[curFileId].Viewers[pageId].Rotate;
	var viewer = _fileDocument[curFileId].Viewers[pageId];
	var strPoints="";

	if(annoType == "Freehand" || annoType == "PolygonLines" || annoType == "Polygon" || annoType == "Line")
	{
		var x_min=x_points.min();
		var y_min=y_points.min();
		var x_max=x_points.max();
		var y_max=y_points.max();

		var width_max=x_max-x_min;
		var height_max=y_max-y_min;
		
		for(var i=0;i<x_points.length;i++)
		{
			strPoints+=x_points[i]+","+y_points[i]+";";
		}
		strPoints = strPoints.substring(0,strPoints.length-1);

		left=x_min;
		top=y_min;
		width=width_max;
		height=height_max;
	}
	else if(annoType == "Line")
	{
		//strPoints = left + "," + top + ";" + (width+left) + "," + (height+top);
	}
	
	
	var docAnno = new Annotation({Type: annoType, Index: annoIndex, ShowLeft: left, ShowTop: top, ShowWidth: width, ShowHeight: height, Rotate: rotateId, PageSizeType: pageSizeType, ZoomIndex: zoomId, ShowPoints: strPoints, ViewerLeft: viewer.Left, ViewerTop: viewer.Top, ViewerWidth: viewer.ActualWidth, ViewerHeight: viewer.ActualHeight, ViewerScaleX: viewer.ScaleX, ViewerScaleY: viewer.ScaleY, ThumbViewerScaleX: viewer.ThumbScaleX, ThumbViewerScaleY: viewer.ThumbScaleY, ViewerOne2OneWidth: viewer.OneToOne_Width, ViewerOne2OneHeight: viewer.OneToOne_Height});
	
		

	append_string="<div class='new_rect' id='div_"+annoIndex+"' style='left:"+left+"px;top:"+top+"px;"+"width:"+width+"px;height:"
	+height+"px'><div class='rRightDown' id='rRightDown_"+annoIndex+"'></div><div class='rLeftDown' id='rLeftDown_"
	+annoIndex+"'></div><div class='rRightUp' id='rRightUp_"+annoIndex+"'></div><div class='rLeftUp' id='rLeftUp_"
	+annoIndex+"'></div><div class='rRight' id='rRight_"+annoIndex+"'></div><div class='rLeft' id='rLeft_"
	+annoIndex+"'></div><div class='rUp' id='rUp_"+annoIndex+"'></div><div class='rDown' id='rDown_"+annoIndex+"'></div>";

	
	append_string += drawAnnotationWithSVG(docAnno, "annotation", false);

	append_string+="</div>";
	$("#draw_canvas").append(append_string);

	if(annoType == "Text")
	{	    
		text_shown = $("#text_"+annoIndex+"").val();
		if(text_shown != null)
		{
			//text_shown = escape(text_shown).replace(/\+/g, '%2B').replace(/\"/g,'%22').replace(/\'/g, '%27').replace(/\//g,'%2F');
			docAnno.Text=text_shown;
		}

		var textAnno = document.getElementById("annotation_"+annoIndex+"");//$("#annotation_"+annoIndex);
		textAnno.ondblclick = function()
		{
			EditText(textAnno);
		};
	}
	
	_fileDocument[curFileId].Viewers[pageId].Annotations[annoIndex]=docAnno;
	
	selectDiv = new Array();
	selectDiv.push(annoIndex);	

	BindImageViewerAnnotationClick(annoIndex);

	BindResize(annoIndex);
}

function drawAnnotationWithSVG(Anno, annoTag, isThumb)
{
	var AnnoStr = "<svg id='svg_" + Anno.Index + "' class='svg' width='100%' height='100%' version='1.1' xmlns='http://www.w3.org/2000/svg'>";

	if(Anno.Type == "Highlight")
	{
		if(isThumb)
		{
			AnnoStr += "<rect class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' width='" + Anno.ThumbShowWidth + "' height='" + Anno.ThumbShowHeight + "' style='fill:" + Anno.FillColor + ";stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth/3 + ";opacity:" + Anno.Transparency + "'/>";
		}
		else
		{
			AnnoStr += "<rect class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' width='" + Anno.ShowWidth + "' height='" + Anno.ShowHeight + "' style='fill:" + Anno.FillColor + ";stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
		}
	}
	else if(Anno.Type == "Rectangle")
	{
		if(isThumb)
		{
			AnnoStr += "<rect class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' width='" + Anno.ThumbShowWidth + "' height='" + Anno.ThumbShowHeight + "' style='fill:none;stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth/3 + ";opacity:" + Anno.Transparency + "'/>";
		}
		else
		{
			AnnoStr += "<rect class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' width='" + Anno.ShowWidth + "' height='" + Anno.ShowHeight + "' style='fill:none;stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
		}
	}
	else if(Anno.Type == "FilledRectangle")
	{
		if(isThumb)
		{
			AnnoStr += "<rect class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' width='" + Anno.ThumbShowWidth + "' height='" + Anno.ThumbShowHeight + "' style='fill:" + Anno.FillColor + ";stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth/3 + ";opacity:" + Anno.Transparency + "'/>";
		}
		else
		{
			AnnoStr += "<rect class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' width='" + Anno.ShowWidth + "' height='" + Anno.ShowHeight + "' style='fill:" + Anno.FillColor + ";stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
		}
	}
	else if(Anno.Type == "Ellipse")
	{
		if(isThumb)
		{
			AnnoStr += "<ellipse class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' cx='" + Anno.ThumbShowWidth/2 + "' cy='" + Anno.ThumbShowHeight/2 + "' rx='" + Anno.ThumbShowWidth/2 + "' ry='" + Anno.ThumbShowHeight/2 + "' style='fill:" + Anno.FillColor + ";stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth/3 + ";opacity:" + Anno.Transparency + "'/>";
		}
		else
		{
			AnnoStr += "<ellipse class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' cx='" + Anno.ShowWidth/2 + "' cy='" + Anno.ShowHeight/2 + "' rx='" + Anno.ShowWidth/2 + "' ry='" + Anno.ShowHeight/2 + "' style='fill:" + Anno.FillColor + ";stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
		}
	}
	else if(Anno.Type == "Line")
	{
		strPoints = "";
		if(isThumb)
		{
			strPoints = Anno.ThumbRelativePoints;
			AnnoStr += "<polyline class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' points='" + strPoints + "' style='fill:none;stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth/3 + ";opacity:" + Anno.Transparency + "'/>";
		}
		else
		{
			strPoints = Anno.RelativePoints;
			AnnoStr += "<polyline class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' points='" + strPoints + "' style='fill:none;stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
		}		
		//AnnoStr += "<polyline class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' points='" + strPoints + "' style='fill:none;stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
		//AnnoStr += "<line class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' x1='0' y1='0' x2='" + Anno.ShowWidth + "' y2='" + Anno.ShowHeight + "' style='stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
	}
	else if(Anno.Type == "Freehand" || Anno.Type == "PolygonLines")
	{		
		strPoints = "";
		if(isThumb)
		{
			strPoints = Anno.ThumbRelativePoints;
			AnnoStr += "<polyline class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' points='" + strPoints + "' style='fill:none;stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth/3 + ";opacity:" + Anno.Transparency + "'/>";
		}
		else
		{
			strPoints = Anno.RelativePoints;
			AnnoStr += "<polyline class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' points='" + strPoints + "' style='fill:none;stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
		}				
		//AnnoStr += "<polyline class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' points='" + strPoints + "' style='fill:none;stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
	}
	else if(Anno.Type == "Polygon")
	{
		strPoints = "";
		if(isThumb)
		{
			strPoints = Anno.ThumbRelativePoints;
			AnnoStr += "<polygon  class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' points='" + strPoints + "' style='fill:" + Anno.FillColor + ";stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth/3 + ";opacity:" + Anno.Transparency + "'/>";
		}
		else
		{
			strPoints = Anno.RelativePoints;
			AnnoStr += "<polygon  class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' points='" + strPoints + "' style='fill:" + Anno.FillColor + ";stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
		}				
		//AnnoStr += "<polygon  class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' points='" + strPoints + "' style='fill:" + Anno.FillColor + ";stroke:" + Anno.OutLineColor + ";stroke-width:" + Anno.OutLineWidth + ";opacity:" + Anno.Transparency + "'/>";
	}
	else if(Anno.Type == "Text")
	{
		if(isThumb)
		{
		    var thumbFontSize = _thumbWidth/_docWidth*Anno.TextSize;		
			src = createTextAnnoWidthCanvas(Anno.Text, {fontStyle: Anno.TextStyle, fontName: Anno.TextFont, fontSize: thumbFontSize, fillColor: Anno.FillColor, textColor: Anno.TextColor, width: Anno.ThumbShowWidth, height: Anno.ThumbShowHeight, transparency: Anno.Transparency});
			AnnoStr = "<img  class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' src='" + src + "' style='z-index:1;' />"
		}
		else
		{
			AnnoStr = "<textarea class='text_edit' id='text_"+Anno.Index+"' style='width:"+(Anno.ShowWidth-6)+"px;height:"+(Anno.ShowHeight-6)+"px;font-size:"+Anno.TextSize+"px'>"+Anno.ShowedText+"</textarea>";
			src = createTextAnnoWidthCanvas(Anno.Text, {fontStyle: Anno.TextStyle, fontName: Anno.TextFont, fontSize: Anno.TextSize, fillColor: Anno.FillColor, textColor: Anno.TextColor, width: Anno.ShowWidth, height: Anno.ShowHeight, transparency: Anno.Transparency, rotate: Anno.Rotate});
			AnnoStr += "<img  class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' src='" + src + "' style='z-index:1;' />"
		}
		
		return AnnoStr;
	}
	else if(Anno.Type == "RubberStamp")
	{
		if(isThumb)
		{			
		    var thumbFontSize = _thumbWidth/_docWidth*Anno.TextSize;	
			src = createStampAnnoWidthCanvas(Anno.Text, {fontStyle: Anno.TextStyle, fontName: Anno.TextFont, fontSize: thumbFontSize, fillColor: Anno.FillColor, textColor: Anno.TextColor, width: Anno.ThumbShowWidth, height: Anno.ThumbShowHeight, transparency: Anno.Transparency, rotate: Anno.Rotate});
		}
		else
		{
			src = createStampAnnoWidthCanvas(Anno.Text, {fontStyle: Anno.TextStyle, fontName: Anno.TextFont, fontSize: Anno.TextSize, fillColor: Anno.FillColor, textColor: Anno.TextColor, width: Anno.ShowWidth, height: Anno.ShowHeight, transparency: Anno.Transparency, rotate: Anno.Rotate});
		}
		AnnoStr = "<img  class='" + annoTag + "' id='" + annoTag + "_" + Anno.Index + "' src='" + src + "' />"

		return AnnoStr;
	}

	AnnoStr += "</svg>";

	return AnnoStr;
}

			function createTextAnnoWidthCanvas(text, opt) 
			{
                
                var defaultOptions = {
                    fontStyle: "normal", //normal, bold, italic
                    fontName: "Arial",
                    fontSize: 12, 
                    fillColor: "darkblue",
                    textColor: "white",
                    padding: 4,
                    width: 100,
					height: 80,
					transparency: 0.5,
					rotate: 0,
                };

				function getTrueLength(str)						
				{
					var len = str.length, truelen = 0;
					for(var x = 0; x < len; x++)
					{
						if(str.charCodeAt(x) > 128)
						{
							truelen += 2;
						}
						else
						{
							truelen += 1;
						}
					}
					return truelen;
				}
				function cutString(str, leng)
				{
					var len = str.length, tlen = len, nlen = 0;
					for(var x = 0; x < len; x++)
					{
						if(str.charCodeAt(x) > 128)
						{
							if(nlen + options.fontSize < leng)
							{
								nlen += options.fontSize;
							}
							else
							{
								tlen = x;
								break;
							}
						}
						else
						{
							if(nlen + options.fontSize/2 < leng)
							{
								nlen += options.fontSize/2;
							}
							else
							{
								tlen = x;
								break;
							}
						}
					}
					return tlen;
				}

                options = $.extend(defaultOptions, opt);              

                var canvas = document.createElement("canvas"),

                    context = canvas.getContext("2d");               

                var font = options.fontStyle + " " + options.fontSize + "px " +  options.fontName;                        

                var w = options.width; 
				if(w<options.fontSize)
				{w=2*options.fontSize;}
				var h = options.height;
				if(h<options.fontSize)
				{h=2*options.fontSize;}

                canvas.width = w;
                canvas.height = h;
               
                context.beginPath();                
                                           
                context.textAlign = "left";
                context.fillStyle = options.textColor;
                context.font = font;

				for(var i = 1; getTrueLength(text) > 0; i++)
				{
					var tl = cutString(text, w);
					context.fillText(text.substr(0, tl).replace(/^\s+|\s+$/, ""), 0, i* 20);
					text = text.substr(tl);
				}
				
				context.rect(0, 0, w, h);
				context.globalAlpha = options.transparency;
                context.fillStyle = options.fillColor;
                context.fill();

				context.rotate(options.rotate*90*Math.PI/180);  

				//var metrics = context.measureText(text);

                //context.fillText(text,  metrics.width/2 + options.padding, options.fontSize + options.padding);
                

                return canvas.toDataURL();

            }



function createStampAnnoWidthCanvas(text, opt)
{
	CanvasRenderingContext2D.prototype.roundRect = function (x, y, w, h, r, transparency, fillColor) {
if (w < 2 * r) r = w / 2;
if (h < 2 * r) r = h / 2;
this.beginPath();
this.moveTo(x+r, y);
this.arcTo(x+w, y, x+w, y+h, r);
this.arcTo(x+w, y+h, x, y+h, r);
this.arcTo(x, y+h, x, y, r);
this.arcTo(x, y, x+w, y, r);
this.globalAlpha=transparency;
this.fillStyle = fillColor;	
// this.arcTo(x+r, y);
this.closePath();
return this;
}

	var defaultOptions = {
        fontStyle: "normal", //normal, bold, italic
        fontName: "Arial",
        fontSize: 12, 
        fillColor: "Red",
        outLineColor: "Tomato",
		textColor: "Black",
        padding: 6,
        width: 100,
		height: 80,
		transparency: 0.5,
		rotate:  0,
       };		
	options = $.extend(defaultOptions, opt);    
	var canvas = document.createElement("canvas"),
    context = canvas.getContext("2d"); 
	var font = options.fontStyle + " " + options.fontSize + "px " +  options.fontName;  
	var w = options.width; 
	var h = options.height;

	canvas.width = w;
    canvas.height = h;
               
    context.beginPath();
	     
	context.fillStyle = options.fillColor;
	context.roundRect(0,0,w,h,w/6,options.transparency,options.fillColor).stroke();
	
	context.textAlign = "center";
	context.fillStyle = options.textColor;
	context.font = font;

	var metrics = context.measureText(text);

	
	//if(options.rotate == 1 || options.rotate == 3)
	{
		//context.fillText(text,  h/2 + options.padding, w/2 - metrics.width/2 + options.padding);
	}
	//else 
	{
		context.fillText(text,  w/2 + w/8 - metrics.width/2 + options.padding, h/2 + options.padding);
	}	

	context.globalAlpha = options.transparency;

	context.rotate(options.rotate*90*Math.PI/180);  

	return canvas.toDataURL();
}


function drawImageViewerAnnotations(Pg)
{
	$(".new_rect").remove();
	for(index in _fileDocument[curFileId].Viewers[Pg].Annotations)
	{		
		var annotation = _fileDocument[curFileId].Viewers[Pg].Annotations[index];
		if(annotation == null || annotation.Index != null)
		{
			_fileDocument[curFileId].Viewers[Pg].Annotations[index].ReSetByZoom();
			annotation = _fileDocument[curFileId].Viewers[Pg].Annotations[index];

			drawLeft=annotation.ShowLeft;
			drawTop=annotation.ShowTop;
			drawWidth=annotation.ShowWidth;
			drawHeight=annotation.ShowHeight;

			append_string="<div class='new_rect' id='div_"+annotation.Index+"'style='left:"+drawLeft+"px;top:"+drawTop+"px;"+"width:"+drawWidth+"px;height:"+drawHeight+"px'"
            +"><div class='rRightDown' id='rRightDown_"+annotation.Index+"'></div><div class='rLeftDown'id='rLeftDown_"
			+annotation.Index+"'></div><div class='rRightUp' id='rRightUp_"+annotation.Index+"'></div><div class='rLeftUp' id='rLeftUp_"
			+annotation.Index+"'></div><div class='rRight' id='rRight_"+annotation.Index+"'></div><div class='rLeft' id='rLeft_"
			+annotation.Index+"'></div><div class='rUp' id='rUp_"+annotation.Index+"'></div><div class='rDown' id='rDown_"+annotation.Index+"'></div>";
			
			
			append_string += drawAnnotationWithSVG(annotation, "annotation", false);

			append_string+="</div>";
			$("#draw_canvas").append(append_string);

			if(annotation.Type == "Text")
			{
				(function(arg){
					var textAnno = document.getElementById("annotation_"+annotation.Index+"");//$("#annotation_"+_docAnnos[index].Index);
					textAnno.ondblclick = function()
					{
						EditText(textAnno);
					};

				})(annotation.Index);			 	
			}

			BindImageViewerAnnotationClick(annotation.Index);

			BindResize(annotation.Index);

			$(".rRightDown").css("visibility", "hidden");
			$(".rLeftDown").css("visibility", "hidden");
			$(".rLeftUp").css("visibility", "hidden");
			$(".rRightUp").css("visibility", "hidden");
			$(".rRight").css("visibility", "hidden");
			$(".rLeft").css("visibility", "hidden");
			$(".rUp").css("visibility", "hidden");
			$(".rDown").css("visibility", "hidden");
			
			$("#div_"+annotation.Index).draggable({containment:"parent",scroll:false});
			$("#div_"+annotation.Index).mousedown(function(){
			$(this).addClass("mousedown");
			});
			$("#div_"+annotation.Index).mouseup(function(){
			$(this).removeClass("mousedown");
			});
		}
	}

	drawImageViewerBurnAnnotation(Pg);
}

function drawImageViewerBurnAnnotation(Pg)
{	
		$(".showByDrag").unbind("mousedown",dragImageMouseDown);
		$(".showByDrag").unbind("mousemove",dragImageMouseMove);
		$(".showByDrag").unbind("mouseup",dragImageMouseUp);
		$(".showByDrag").removeClass("mouseStyleMove");

	$(".burn_rect").remove();
	var TempBurnAnnotations = new Array();
	var _AnnoCount = _fileDocument[curFileId].Viewers[Pg].BurnedAnnotations.length;
	for(index in _fileDocument[curFileId].Viewers[Pg].BurnedAnnotations)
	{
	    TempBurnAnnotations[index] = _fileDocument[curFileId].Viewers[Pg].BurnedAnnotations[index];
	}
	for (index in _fileDocument[curFileId].Viewers[Pg].BurnAnnotations)
	{
	    TempBurnAnnotations[_AnnoCount + index] = _fileDocument[curFileId].Viewers[Pg].BurnAnnotations[index];
	}
	for(index in TempBurnAnnotations)
	{		
		var annotation = TempBurnAnnotations[index];
		if(annotation == null || annotation.Index != null)
		{
			TempBurnAnnotations[index].ReSetByZoom();
			annotation = TempBurnAnnotations[index];

			drawLeft=annotation.ShowLeft;
			drawTop=annotation.ShowTop;
			drawWidth=annotation.ShowWidth;
			drawHeight=annotation.ShowHeight;

			

			append_string="<div class='burn_rect' id='div_"+annotation.Index+"'style='left:"+drawLeft+"px;top:"+drawTop+"px;"+"width:"+drawWidth+"px;height:"+drawHeight+"px'>";
			
			
			append_string += drawAnnotationWithSVG(annotation, "annotation", false);

			append_string+="</div>";
			$("#draw_canvas").append(append_string);	
						
			$("#div_"+annotation.Index).addClass("showByDrag");
			
		}
	}
//	for(index in _fileDocument[curFileId].Viewers[Pg].BurnAnnotations)
//	{		
//		var annotation = _fileDocument[curFileId].Viewers[Pg].BurnAnnotations[index];
//		if(annotation == null || annotation.Index != null)
//		{
//			_fileDocument[curFileId].Viewers[Pg].BurnAnnotations[index].ReSetByZoom();
//			annotation = _fileDocument[curFileId].Viewers[Pg].BurnAnnotations[index];

//			drawLeft=annotation.ShowLeft;
//			drawTop=annotation.ShowTop;
//			drawWidth=annotation.ShowWidth;
//			drawHeight=annotation.ShowHeight;

//			

//			append_string="<div class='burn_rect' id='div_"+annotation.Index+"'style='left:"+drawLeft+"px;top:"+drawTop+"px;"+"width:"+drawWidth+"px;height:"+drawHeight+"px'>";
//			
//			
//			append_string += drawAnnotationWithSVG(annotation, "annotation", false);

//			append_string+="</div>";
//			$("#draw_canvas").append(append_string);	
//						
//			$("#div_"+annotation.Index).addClass("showByDrag");
//			
//		}
//	}
		$(".showByDrag").bind("mousedown",dragImageMouseDown);
		$(".showByDrag").bind("mousemove",dragImageMouseMove);
		$(".showByDrag").bind("mouseup",dragImageMouseUp);
		$(".showByDrag").addClass("mouseStyleMove");
}


function BindImageViewerAnnotationClick(annoIndex)
{	
	var parent_left=$("#re_canvas").offset().left;
	var parent_top=$("#re_canvas").offset().top;	
	
	var _isResizeAnnotation = false;	

	var MouseDownChangeSize=function(e)
	{
		_isResizeAnnotation = true;
		
		if(isCtrlDown == false)
		{
			selectDiv = new Array();
		}
		
		selectDiv.push(annoIndex);	
	};
	
	var MouseMoveChangeSize=function(e)
	{		
		
		if(_isResizeAnnotation)
		{
			var nScrollLeft=$("#re_canvas")[0].scrollLeft;
			var nScrollTop=$("#re_canvas")[0].scrollTop;

			pageId=parseInt(_curCorrectPageId);// document.getElementById('currentPageIndex').innerText;
			pageId = _fileDocument[curFileId].GetPageIndex(pageId);
			
 			var zoomScale=_zoomRatio*_zoomIndex;

			pageSizeType=_pageSizeType;
			
			selectId = annoIndex;
			if(isNaN(selectId))
			{return false;}
			reDrawDiv = $("#div_"+selectId);			

			reDraw_left=reDrawDiv.offset().left-parent_left + nScrollLeft;
			reDraw_top=reDrawDiv.offset().top-parent_top + nScrollTop;
			reDraw_width=reDrawDiv.width();
			reDraw_height=reDrawDiv.height();

			
			//ImageViewer Annotation
			_fileDocument[curFileId].Viewers[pageId].Annotations[selectId].ReSetSize(reDraw_left, reDraw_top, reDraw_width, reDraw_height);
			
			
			resizeAnnotationWithSVG("#annotation_", _fileDocument[curFileId].Viewers[pageId].Annotations[selectId], false);
			//ThumbViewer Annotation		
			
			resizeAnnotationWithSVG("#thumbAnnotation_", _fileDocument[curFileId].Viewers[pageId].Annotations[selectId], true);

			$("#thumbImg_"+selectId).css("top",_fileDocument[curFileId].Viewers[pageId].Annotations[selectId].ThumbShowTop);
			$("#thumbImg_"+selectId).css("left",_fileDocument[curFileId].Viewers[pageId].Annotations[selectId].ThumbShowLeft);
			$("#thumbImg_"+selectId).width(_fileDocument[curFileId].Viewers[pageId].Annotations[selectId].ThumbShowWidth).height(_fileDocument[curFileId].Viewers[pageId].Annotations[selectId].ThumbShowHeight);			
			
		}
		else 
		{			
		}
	};
	
	

	$("#div_"+annoIndex).bind("mousedown",MouseDownChangeSize);
	$("#rRightDown_"+annoIndex).bind("mousedown",MouseDownChangeSize);
	$("#rLeftDown_"+annoIndex).bind("mousedown",MouseDownChangeSize);
	$("#rRightUp_"+annoIndex).bind("mousedown",MouseDownChangeSize);
	$("#rLeftUp_"+annoIndex).bind("mousedown",MouseDownChangeSize);
	$("#rRight_"+annoIndex).bind("mousedown",MouseDownChangeSize);
	$("#rLeft_"+annoIndex).bind("mousedown",MouseDownChangeSize);
	$("#rUp_"+annoIndex).bind("mousedown",MouseDownChangeSize);
	$("#rDown_"+annoIndex).bind("mousedown",MouseDownChangeSize);
	$("#div_"+annoIndex).bind("mousemove",MouseMoveChangeSize);

}

function resizeAnnotationWithSVG(annoId, Anno, isThumb)
{
	
	if(Anno.Type == "Highlight" || Anno.Type == "Rectangle" || Anno.Type == "FilledRectangle")
	{
		if(isThumb)
		{
			$(annoId+Anno.Index).attr("width",Anno.ThumbShowWidth);
			$(annoId+Anno.Index).attr("height",Anno.ThumbShowHeight);
		}
		else
		{
			$(annoId+Anno.Index).attr("width",Anno.ShowWidth);
			$(annoId+Anno.Index).attr("height",Anno.ShowHeight);
		}
		
	}
	else if(Anno.Type == "Ellipse")
	{
		if(isThumb)
		{
			$(annoId+Anno.Index).attr("cx",Anno.ThumbShowWidth/2);
			$(annoId+Anno.Index).attr("cy",Anno.ThumbShowHeight/2);
			$(annoId+Anno.Index).attr("rx",Anno.ThumbShowWidth/2);
			$(annoId+Anno.Index).attr("ry",Anno.ThumbShowHeight/2);
		}
		else
		{
			$(annoId+Anno.Index).attr("cx",Anno.ShowWidth/2);
			$(annoId+Anno.Index).attr("cy",Anno.ShowHeight/2);
			$(annoId+Anno.Index).attr("rx",Anno.ShowWidth/2);
			$(annoId+Anno.Index).attr("ry",Anno.ShowHeight/2);
		}
		
	}
	else if(Anno.Type == "Line")
	{		
		if(isThumb)
		{
			$(annoId+Anno.Index).attr("points",Anno.ThumbRelativePoints);
		}
		else
		{
			$(annoId+Anno.Index).attr("points",Anno.RelativePoints);
		}
		
	}
	else if(Anno.Type == "Freehand" || Anno.Type == "PolygonLines" || Anno.Type == "Polygon")
	{
		if(isThumb)
		{
			$(annoId+Anno.Index).attr("points",Anno.ThumbRelativePoints);
		}
		else
		{
			$(annoId+Anno.Index).attr("points",Anno.RelativePoints);
		}
		
	}
	else if(Anno.Type == "Text")
	{
		if(isThumb)
		{
			var thumbFontSize = _thumbWidth/_docWidth*Anno.TextSize;
			src = createTextAnnoWidthCanvas(Anno.Text, {fontStyle: Anno.TextStyle, fontName: Anno.TextFont, fontSize: thumbFontSize, fillColor: Anno.FillColor, textColor: Anno.TextColor, width: Anno.ThumbShowWidth, height: Anno.ThumbShowHeight, transparency: Anno.Transparency, rotate: Anno.Rotate});
			$(annoId+Anno.Index).attr("src",src);
		}
		else
		{
			src = createTextAnnoWidthCanvas(Anno.Text, {fontStyle: Anno.TextStyle, fontName: Anno.TextFont, fontSize: Anno.TextSize, fillColor: Anno.FillColor, textColor: Anno.TextColor, width: Anno.ShowWidth, height: Anno.ShowHeight, transparency: Anno.Transparency, rotate: Anno.Rotate});
			$(annoId+Anno.Index).attr("src",src);
			document.getElementById("text_"+Anno.Index).style.width = Anno.ShowWidth+"px";
			document.getElementById("text_"+Anno.Index).style.height = Anno.ShowHeight+"px";
		}		
		
	}
	else if(Anno.Type == "RubberStamp")
	{
		if(isThumb)
		{
			var thumbFontSize = _thumbWidth/_docWidth*Anno.TextSize;
			src = createStampAnnoWidthCanvas(Anno.Text, {fontStyle: Anno.TextStyle, fontName: Anno.TextFont, fontSize: thumbFontSize, fillColor: Anno.FillColor, textColor: Anno.TextColor, width: Anno.ThumbShowWidth, height: Anno.ThumbShowHeight, transparency: Anno.Transparency, rotate: Anno.Rotate});
			$(annoId+Anno.Index).attr("src",src);	
		}
		else
		{
			src = createStampAnnoWidthCanvas(Anno.Text, {fontStyle: Anno.TextStyle, fontName: Anno.TextFont, fontSize: Anno.TextSize, fillColor: Anno.FillColor, textColor: Anno.TextColor, width: Anno.ShowWidth, height: Anno.ShowHeight, transparency: Anno.Transparency, rotate: Anno.Rotate});
			$(annoId+Anno.Index).attr("src",src);	
		}
			
	}

}

function BindResize(annoIndex)
{
	var rs = new Resize("div_"+annoIndex+"", { Max: true, mxContainer: "_plcBigImg" });

	rs.Set("rRightDown_"+annoIndex+"", "right-down");
	rs.Set("rLeftDown_"+annoIndex+"", "left-down");
	rs.Set("rRightUp_"+annoIndex+"", "right-up");
	rs.Set("rLeftUp_"+annoIndex+"", "left-up");
	rs.Set("rRight_"+annoIndex+"", "right");
	rs.Set("rLeft_"+annoIndex+"", "left");
	rs.Set("rUp_"+annoIndex+"", "up");
	rs.Set("rDown_"+annoIndex+"", "down");
}


function RotateAnnotations(Pg, rotateId)
{	
	thumbViewerWidth = _fileDocument[curFileId].Viewers[Pg].ThumbWidth;
	thumbViewerHeight = _fileDocument[curFileId].Viewers[Pg].ThumbHeight;
	
	if(rotateId == 1 || rotateId == 3)
	{
		$("#thumbDiv_"+Pg).width(thumbViewerHeight).height(thumbViewerWidth);
	}
	else 
	{
		$("#thumbDiv_"+Pg).width(thumbViewerWidth).height(thumbViewerHeight);
	}
	

	for(index in _fileDocument[curFileId].Viewers[Pg].Annotations)
	{
				
		if(_fileDocument[curFileId].Viewers[Pg].Annotations[index].Index != null)
		{
			_fileDocument[curFileId].Viewers[Pg].Annotations[index].ReSetRotate(rotateId);	
			
			var annotation = _fileDocument[curFileId].Viewers[Pg].Annotations[index];
			

			resizeAnnotationWithSVG("#annotation_", annotation, false);


			$("#div_"+annotation.Index).css("top",annotation.ShowTop);
			$("#div_"+annotation.Index).css("left",annotation.ShowLeft);
			$("#div_"+annotation.Index).width(annotation.ShowWidth).height(annotation.ShowHeight);

			resizeAnnotationWithSVG("#thumbAnnotation_", annotation, true);

			$("#thumbImg_"+annotation.Index).css("top",annotation.ThumbShowTop);
			$("#thumbImg_"+annotation.Index).css("left",annotation.ThumbShowLeft);
			$("#thumbImg_"+annotation.Index).width(annotation.ThumbShowWidth).height(annotation.ThumbShowHeight);

		}
	}

	for(index in _fileDocument[curFileId].Viewers[Pg].BurnAnnotations)
	{
				
		if(_fileDocument[curFileId].Viewers[Pg].BurnAnnotations[index].Index != null)
		{
			_fileDocument[curFileId].Viewers[Pg].BurnAnnotations[index].ReSetRotate(rotateId);	
			
			var annotation = _fileDocument[curFileId].Viewers[Pg].BurnAnnotations[index];
			

			resizeAnnotationWithSVG("#annotation_", annotation, false);


			$("#div_"+annotation.Index).css("top",annotation.ShowTop);
			$("#div_"+annotation.Index).css("left",annotation.ShowLeft);
			$("#div_"+annotation.Index).width(annotation.ShowWidth).height(annotation.ShowHeight);

			resizeAnnotationWithSVG("#thumbAnnotation_", annotation, true);

			$("#thumbImg_"+annotation.Index).css("top",annotation.ThumbShowTop);
			$("#thumbImg_"+annotation.Index).css("left",annotation.ThumbShowLeft);
			$("#thumbImg_"+annotation.Index).width(annotation.ThumbShowWidth).height(annotation.ThumbShowHeight);

		}
	}
}