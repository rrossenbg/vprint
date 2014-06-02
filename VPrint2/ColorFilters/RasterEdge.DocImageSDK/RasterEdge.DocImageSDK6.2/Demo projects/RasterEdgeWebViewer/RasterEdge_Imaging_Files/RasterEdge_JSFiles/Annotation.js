var Annotation = Class.create();
Annotation.prototype = 
{

	initialize: function(options) 
	{
		this.SetOptions(options);

		this.Type = this.options.Type;
		this.Index = this.options.Index;		
		this.ShowLeft = this.options.ShowLeft;
		this.ShowTop = this.options.ShowTop;
		this.ShowWidth = this.options.ShowWidth;
		this.ShowHeight = this.options.ShowHeight;
		this.Left = this.options.Left;
		this.Top = this.options.Top;
		this.Width = this.options.Width;
		this.Height = this.options.Height;
		this.ThumbLeft = this.options.ThumbLeft;
		this.ThumbTop = this.options.ThumbTop;
		this.ThumbWidth = this.options.ThumbWidth;
		this.ThumbHeight = this.options.ThumbHeight;
		this.ThumbShowLeft = this.options.ThumbShowLeft;
		this.ThumbShowTop = this.options.ThumbShowTop;
		this.ThumbShowWidth = this.options.ThumbShowWidth;
		this.ThumbShowHeight = this.options.ThumbShowHeight;
		this.Rotate = this.options.Rotate;		
		this.PageSizeType = this.options.PageSizeType;
		this.ZoomIndex = this.options.ZoomIndex;
		this.Text = this.options.Text;
		this.Points = this.options.Points;
		//this.ThumbPoints = this.options.ThumbPoints;
		this.ShowPoints = this.options.ShowPoints;
		this.ThumbShowPoints = this.options.ThumbShowPoints;
		this.RelativePoints = this.options.RelativePoints;
		this.ThumbRelativePoints = this.options.ThumbRelativePoints;
		//this.resizeScaleX = this.options.resizeScaleX;
		//this.resizeScaleY = this.options.resizeScaleY;
		this.ViewerLeft = this.options.ViewerLeft;
		this.ViewerTop = this.options.ViewerTop;
		this.ViewerWidth = this.options.ViewerWidth;
		this.ViewerHeight = this.options.ViewerHeight;
		this.ViewerScaleX = this.options.ViewerScaleX;
		this.ViewerScaleY = this.options.ViewerScaleY;
		this.ThumbViewerScaleX = this.options.ThumbViewerScaleX;
		this.ThumbViewerScaleY = this.options.ThumbViewerScaleY;
		this.ViewerOne2OneWidth = this.options.ViewerOne2OneWidth;
		this.ViewerOne2OneHeight = this.options.ViewerOne2OneHeight;
		

		if(this.Type == "Text")
		{
			this.AddAnnoStyle(TextAnnoStyle);
		}
		else if(this.Type == "Freehand")
		{
			this.AddAnnoStyle(FreehandAnnoStyle);
		}		
		else if(this.Type == "Highlight")
		{
			this.AddAnnoStyle(HighlightAnnoStyle);
		}
		else if(this.Type == "Rectangle")
		{
			this.AddAnnoStyle(RectangleAnnoStyle);
		}
		else if(this.Type == "FilledRectangle")
		{
			this.AddAnnoStyle(FilledRectangleAnnoStyle);
		}
		else if(this.Type == "Ellipse")
		{
			this.AddAnnoStyle(EllipseAnnoStyle);
		}
		else if(this.Type == "RubberStamp")
		{
			this.AddAnnoStyle(RubberStampAnnoStyle);
		}
		else if(this.Type == "PolygonLines")
		{
			this.AddAnnoStyle(PolygonLinesAnnoStyle);
		}
		else if(this.Type == "Polygon")
		{
			this.AddAnnoStyle(PolygonAnnoStyle);
		}
		else if(this.Type == "Line")
		{
			this.AddAnnoStyle(LineAnnoStyle);
		}	
		
		var pointArr = this.ShowPoints.split(";");
		var strPoints = "";
		
		
		this.GetActualPosition();

		
		this.CalculateOnRotate();
		
	},
		
	SetOptions: function(options) 
	{
		this.options = 
		{
			Type:					"",
			Index:					0,
			ShowLeft:				0,
			ShowTop:				0,
			ShowWidth:				0,
			ShowHeight:				0,
			Left:					0,
			Top:					0,
			Width:					0,
			Height:					0,
			ThumbLeft:				0,
			ThumbTop:				0,
			ThumbWidth:				0,
			ThumbHeight:			0,
			ThumbShowLeft:			0,
			ThumbShowTop:			0,
			ThumbShowWidth:			0,
			ThumbShowHeight:		0,
			Rotate:					0,			
			PageSizeType:			_defaultPageSizeType,
			ZoomIndex:				0,
			Text:					"",
			ShowPoints:				"",
			Points:					"",
			//ThumbPoints:			"",
			ThumbShowPoints:		"",
			RelativePoints:			"",
			ThumbRelativePoints:	"",
			resizeScaleX:			1,
			resizeScaleY:			1,
			ViewerLeft:				0,
			ViewerTop:				0,
			ViewerWidth:			0,
			ViewerHeight:			0,
			ViewerScaleX:			1,
			ViewerScaleY:			1,
			ThumbViewerScaleX:		1,
			ThumbViewerScaleY:		1,
			ViewerOne2OneWidth:		0,
			ViewerOne2OneHeight:	0,
					
		};
		Extend(this.options, options || {});
    },

	GetAnnoSrc: function()
	{
		var src = "";
		src += getRootPath()+"/RasterEdge_Imaging_Files/AnnotationView.aspx?View=REAnnotation";
		src += "&AnnotationType=" + this.Type;
		src += "&X=" + this.ShowLeft;
		src += "&Y=" + this.ShowTop;
		src += "&Width=" + this.ShowWidth;
		src += "&Height=" + this.ShowHeight;
		src += "&Points=" + this.Points;		
		if(this.Text == "")
		{
			src += "&TextShown=" + this.ShowedText;
		}
		else
		{
			src += "&TextShown=" + this.Text;
		}
		src += "&OutLineColor=" + this.OutLineColor;	
		src += "&OutLineWidth=" + this.OutLineWidth;	
		src += "&FillColor=" + this.FillColor;	
		src += "&TextColor=" + this.TextColor;	
		src += "&TextFont=" + this.TextFont;	
		src += "&TextSize=" + this.TextSize;	
		src += "&TextStyle=" + this.TextStyle;	
		src += "&Transparency=" + this.Transparency;

		return src;
	},

	GetThumbAnnoSrc: function()
	{
		var src = "";
		src += getRootPath()+"/RasterEdge_Imaging_Files/AnnotationView.aspx?View=REAnnotation";
		src += "&AnnotationType=" + this.Type;
		src += "&X=" + this.ThumbShowLeft;
		src += "&Y=" + this.ThumbShowTop;
		src += "&Width=" + this.ThumbShowWidth;
		src += "&Height=" + this.ThumbShowHeight;
		src += "&Points=" + this.ThumbPoints;		
		if(this.Text == "")
		{
			src += "&TextShown=" + this.ShowedText;
		}
		else
		{
			src += "&TextShown=" + this.Text;
		}
		src += "&OutLineColor=" + this.OutLineColor;	
		src += "&OutLineWidth=" + this.OutLineWidth;	
		src += "&FillColor=" + this.FillColor;	
		src += "&TextColor=" + this.TextColor;	
		src += "&TextFont=" + this.TextFont;	
		src += "&TextSize=1";	
		src += "&TextStyle=" + this.TextStyle;	
		src += "&Transparency=" + this.Transparency;

		return src;
	},
	
	AddAnnoStyle: function(annotationStyle)
	{
		this.OutLineColor = annotationStyle.OutLineColor;
		this.OutLineWidth = annotationStyle.OutLineWidth;
		this.FillColor = annotationStyle.FillColor;
		this.ShowedText = annotationStyle.ShowedText;
		this.Text = this.ShowedText;
		this.TextColor = annotationStyle.TextColor;
		this.TextFont = annotationStyle.TextFont;
		this.TextSize = annotationStyle.TextSize;
		this.TextStyle = annotationStyle.TextStyle;	
		this.Transparency = annotationStyle.Transparency;
	},

	GetActualPosition: function()
	{
		var scaleX = this.ViewerScaleX;
		var scaleY = this.ViewerScaleY;	
		var zoomValue = _zoomRatio*_zoomIndex;
				
		var thumbScaleX = this.ThumbViewerScaleX;
		var thumbScaleY = this.ThumbViewerScaleY;	
		
		var pointScaleX = _thumbWidth/this.ViewerWidth;
		var pointScaleY = _thumbHeight/this.ViewerHeight;
		
		var allWidth = this.ViewerWidth;
		var allHeight = this.ViewerHeight;
		if(allWidth < _docWidth)
		{
			allWidth = _docWidth;
		}
		if(allHeight < _docHeight)
		{
			allHeight = _docHeight;
		}
		
		this.ViewerLeft = 0;
		this.ViewerTop = 0;
		if(this.ViewerWidth < _docWidth)
		{
			this.ViewerLeft = (_docWidth - this.ViewerWidth)/2;
		}
		if(this.ViewerHeight < _docHeight)
		{
			this.ViewerTop = (_docHeight - this.ViewerHeight)/2;
		}

		var pointArr = this.ShowPoints.split(";");			
		//this.ShowPoints = "";
		this.RelativePoints = "";
		this.Points = "";
		this.ThumbPoints = "";
		this.ThumbRelativePoints = "";
	
		if(this.Rotate == 0)//0 degree
		{
			this.Left = (this.ShowLeft - this.ViewerLeft)*scaleX/zoomValue;			
			this.Top = (this.ShowTop - this.ViewerTop)*scaleY/zoomValue;
			this.Width = (this.ShowWidth)*scaleX/zoomValue;
			this.Height = (this.ShowHeight)*scaleY/zoomValue;

			this.ThumbLeft = this.Left*thumbScaleX;
			this.ThumbTop = this.Top*thumbScaleY;
			this.ThumbWidth = this.Width*thumbScaleX;
			this.ThumbHeight = this.Height*thumbScaleY;

			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					x_point = xys[0] - this.ShowLeft;
					y_point = xys[1] - this.ShowTop;
					resizeX = (x_point)*scaleX/zoomValue;
					resizeY = (y_point)*scaleY/zoomValue;
					thumb_x_point = resizeX*thumbScaleX;
					thumb_y_point = resizeY*thumbScaleY;

					//this.ShowPoints += x_point + "," + y_point + ";";
					this.RelativePoints += x_point + "," +  y_point + " ";					
					this.Points += resizeX + "," + resizeY + ";";
					this.ThumbPoints += thumb_x_point + "," + thumb_y_point + ";";
					this.ThumbRelativePoints += thumb_x_point + "," + thumb_y_point + " ";
				}
			
			}				
			
		}
		else if(this.Rotate == 1)//90 degree
		{
			this.Left = (this.ShowTop - this.ViewerTop)*scaleY/zoomValue;
			this.Top = (allWidth - this.ShowLeft - this.ViewerLeft - this.ShowWidth)*scaleX/zoomValue;
			this.Width = (this.ShowHeight)*scaleY/zoomValue;
			this.Height = (this.ShowWidth)*scaleX/zoomValue;

			this.ThumbLeft = _thumbHeight - (this.Top + this.Height)*thumbScaleY;
			this.ThumbTop = this.Left*thumbScaleX;
			this.ThumbWidth = this.Height*thumbScaleY;
			this.ThumbHeight = this.Width*thumbScaleX;

			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					x_point = xys[0] - this.ShowLeft;
					y_point = xys[1] - this.ShowTop;
					resizeX = (y_point)*scaleY/zoomValue;
					resizeY = (this.ShowWidth - x_point)*scaleX/zoomValue;
					thumb_x_point = this.ThumbWidth - resizeY*thumbScaleY;
					thumb_y_point = resizeX*thumbScaleX;

					//this.ShowPoints += x_point + "," + y_point + ";";
					this.RelativePoints += x_point + "," +  y_point + " ";					
					this.Points += resizeX + "," + resizeY + ";";	
					this.ThumbPoints += thumb_x_point + "," + thumb_y_point + ";";
					this.ThumbRelativePoints += thumb_x_point + "," + thumb_y_point + " ";
				}
			
			}		
			
		}
		else if(this.Rotate == 2)//180 degree
		{			
			this.Left = (allWidth - this.ShowLeft - this.ShowWidth - this.ViewerLeft)*scaleX/zoomValue;
			this.Top = (allHeight - this.ShowTop - this.ShowHeight - this.ViewerTop)*scaleY/zoomValue;
			this.Width = this.ShowWidth*scaleX/zoomValue;
			this.Height = this.ShowHeight*scaleY/zoomValue;	

			this.ThumbLeft = _thumbWidth - (this.Left + this.Width)*thumbScaleX;
			this.ThumbTop = _thumbHeight - (this.Top + this.Height)*thumbScaleY;
			this.ThumbWidth = this.Width*thumbScaleX;
			this.ThumbHeight = this.Height*thumbScaleY;
				
			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					x_point = xys[0] - this.ShowLeft;
					y_point = xys[1] - this.ShowTop;
					resizeX = (this.ShowWidth - x_point)*scaleX/zoomValue;
					resizeY = (this.ShowHeight - y_point)*scaleY/zoomValue;
					thumb_x_point = this.ThumbWidth - resizeX*thumbScaleX;
					thumb_y_point = this.ThumbHeight - resizeY*thumbScaleY;
				
					//this.ShowPoints += x_point + "," + y_point + ";";
					this.RelativePoints += x_point + "," +  y_point + " ";					
					this.Points += resizeX + "," + resizeY + ";";	
					this.ThumbPoints += thumb_x_point + "," + thumb_y_point + ";";
					this.ThumbRelativePoints += thumb_x_point + "," + thumb_y_point + " ";
				}
			
			}
		
		}
		else if(this.Rotate == 3)//270 degree
		{			
			this.Left = (allHeight - this.ShowTop - this.ViewerTop - this.ShowHeight)*scaleY/zoomValue;
			this.Top = (this.ShowLeft - this.ViewerLeft)*scaleX/zoomValue;
			this.Width =  (this.ShowHeight)*scaleY/zoomValue;
			this.Height = (this.ShowWidth)*scaleX/zoomValue;

			this.ThumbLeft = this.Top*thumbScaleY;
			this.ThumbTop =  _thumbWidth - (this.Left + this.Width)*thumbScaleX;
			this.ThumbWidth = this.Height*thumbScaleY;
			this.ThumbHeight = this.Width*thumbScaleX;

			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					x_point = xys[0] - this.ShowLeft;
					y_point = xys[1] - this.ShowTop;
					resizeX = (this.ShowHeight - y_point)*scaleY/zoomValue;
					resizeY = (x_point)*scaleX/zoomValue;
					thumb_x_point = resizeY*thumbScaleY; 
					thumb_y_point = this.ThumbHeight - resizeX*thumbScaleX;
				
					//this.ShowPoints += x_point + "," + y_point + ";";
					this.RelativePoints += x_point + "," +  y_point + " ";					
					this.Points += resizeX + "," + resizeY + ";";
					this.ThumbPoints += thumb_x_point + "," + thumb_y_point + ";";
					this.ThumbRelativePoints += thumb_x_point + "," + thumb_y_point + " ";
				}
			
			}
		}	

		//this.ShowPoints = this.ShowPoints.substring(0,this.ShowPoints.length-1);
		this.RelativePoints = this.RelativePoints.substring(0,this.RelativePoints.length-1);
		this.Points = this.Points.substring(0,this.Points.length-1);
		this.ThumbPoints = this.ThumbPoints.substring(0,this.ThumbPoints.length-1);
		this.ThumbRelativePoints = this.ThumbRelativePoints.substring(0,this.ThumbRelativePoints.length-1);
		this.ThumbShowPoints = this.ThumbPoints;

		this.ThumbShowLeft = this.ThumbLeft;
		this.ThumbShowTop =  this.ThumbTop;
		this.ThumbShowWidth = this.ThumbWidth;
		this.ThumbShowHeight = this.ThumbHeight;
		
	},

	ReSetRotate: function(rotate)
	{
		this.Rotate = rotate;	
		if(this.Rotate == 0)
		{
			this.ShowLeft = this.ShowLeft_0;
			this.ShowTop = this.ShowTop_0;
			this.ShowWidth = this.ShowWidth_0;
			this.ShowHeight = this.ShowHeight_0;
			this.RelativePoints = this.RelativePoints_0;
			this.ShowPoints = this.ShowPoints_0;

			this.ThumbShowLeft = this.ThumbShowLeft_0;
			this.ThumbShowTop = this.ThumbShowTop_0;
			this.ThumbShowWidth = this.ThumbShowWidth_0;
			this.ThumbShowHeight = this.ThumbShowHeight_0;
			this.ThumbShowPoints = this.ThumbShowPoints_0;
			this.ThumbRelativePoints = this.ThumbRelativePoints_0;
		}
		else if(this.Rotate == 1)
		{
			this.ShowLeft = this.ShowLeft_90;
			this.ShowTop = this.ShowTop_90;
			this.ShowWidth = this.ShowWidth_90;
			this.ShowHeight = this.ShowHeight_90;
			this.RelativePoints = this.RelativePoints_90;
			this.ShowPoints = this.ShowPoints_90;

			this.ThumbShowLeft = this.ThumbShowLeft_90;
			this.ThumbShowTop = this.ThumbShowTop_90;
			this.ThumbShowWidth = this.ThumbShowWidth_90;
			this.ThumbShowHeight = this.ThumbShowHeight_90;
			this.ThumbShowPoints = this.ThumbShowPoints_90;
			this.ThumbRelativePoints = this.ThumbRelativePoints_90;
		}
		else if(this.Rotate == 2)
		{
			this.ShowLeft = this.ShowLeft_180;
			this.ShowTop = this.ShowTop_180;
			this.ShowWidth = this.ShowWidth_180;
			this.ShowHeight = this.ShowHeight_180;
			this.RelativePoints = this.RelativePoints_180;
			this.ShowPoints = this.ShowPoints_180;

			this.ThumbShowLeft = this.ThumbShowLeft_180;
			this.ThumbShowTop = this.ThumbShowTop_180;
			this.ThumbShowWidth = this.ThumbShowWidth_180;
			this.ThumbShowHeight = this.ThumbShowHeight_180;
			this.ThumbShowPoints = this.ThumbShowPoints_180;
			this.ThumbRelativePoints = this.ThumbRelativePoints_180;

		}
		else if(this.Rotate == 3)
		{
			this.ShowLeft = this.ShowLeft_270;
			this.ShowTop = this.ShowTop_270;
			this.ShowWidth = this.ShowWidth_270;
			this.ShowHeight = this.ShowHeight_270;
			this.RelativePoints = this.RelativePoints_270;
			this.ShowPoints = this.ShowPoints_270;

			this.ThumbShowLeft = this.ThumbShowLeft_270;
			this.ThumbShowTop = this.ThumbShowTop_270;
			this.ThumbShowWidth = this.ThumbShowWidth_270;
			this.ThumbShowHeight = this.ThumbShowHeight_270;
			this.ThumbShowPoints = this.ThumbShowPoints_270;
			this.ThumbRelativePoints = this.ThumbRelativePoints_270;
		}

	},

	ReSetSize: function(left, top, width, height)
	{
		var previousWidth = this.ShowWidth;
		var previousHeight = this.ShowHeight;
		this.resizeScaleX = (width)/previousWidth;
		this.resizeScaleY = (height)/previousHeight;
		
		this.previousLeft = this.ShowLeft;
		this.previousTop = this.ShowTop;
		

		this.ShowLeft = left;
		this.ShowTop = top;
		this.ShowWidth = width;
		this.ShowHeight = height;

		if(this.Type == "Freehand" || this.Type == "PolygonLines" || this.Type == "Polygon"|| this.Type == "Line")
		{	
			this.ReSetPoints();
		}		
				
		this.GetActualPosition();
		
		this.CalculateOnRotate();
		
	},

	ReSetPoints: function()
	{
		var pointArr = this.ShowPoints.split(";");	
		var strPoints = "";
		
		for(var i = 0; i < pointArr.length; i++)
		{
			var xys = pointArr[i].split(",");
			if(typeof(xys) != 'undefined')
			{
				resizeX = (xys[0] - this.previousLeft)*this.resizeScaleX + this.ShowLeft;
				resizeY = (xys[1] - this.previousTop)*this.resizeScaleY + this.ShowTop;	

				strPoints += resizeX + "," + resizeY + ";";				
			}
			
		}
			
		strPoints = strPoints.substring(0,strPoints.length-1);
		this.ShowPoints = strPoints;
	},

		
	ReSetByZoom: function()
	{
		var scaleX = this.ViewerScaleX;
		var scaleY = this.ViewerScaleY;
		var zoomValue = _zoomRatio*_zoomIndex;
		
		var previousWidth = this.ShowWidth;
		var previousHeight = this.ShowHeight;
		this.previousLeft = this.ShowLeft;
		this.previousTop = this.ShowTop;

		if(this.Rotate == 0)//0 degree
		{			
			this.ShowLeft = (this.Left)/scaleX*zoomValue + this.ViewerLeft ;
			this.ShowTop = (this.Top)/scaleY*zoomValue + this.ViewerTop;
			this.ShowWidth = (this.Width)/scaleX*zoomValue;
			this.ShowHeight = this.Height/scaleY*zoomValue;
			
		}
		else if(this.Rotate == 1)//90 degree
		{
			this.ShowLeft = (this.ViewerOne2OneHeight - this.Height - this.Top)/scaleY*zoomValue + this.ViewerLeft;
			this.ShowTop = (this.Left)/scaleX*zoomValue + this.ViewerTop;
			this.ShowWidth = this.Height/scaleY*zoomValue;
			this.ShowHeight = (this.Width)/scaleX*zoomValue;
		}
		else if(this.Rotate == 2)//180 degree
		{
			this.ShowLeft = (this.ViewerOne2OneWidth - this.Width - this.Left)/scaleX*zoomValue + this.ViewerLeft;
			this.ShowTop = (this.ViewerOne2OneHeight - this.Height - this.Top)/scaleY*zoomValue + this.ViewerTop;
			this.ShowWidth = (this.Width)/scaleX*zoomValue;
			this.ShowHeight = this.Height/scaleY*zoomValue;				
		}
		else if(this.Rotate == 3)//270 degree
		{
			this.ShowLeft = (this.Top)/scaleY*zoomValue + this.ViewerLeft;
			this.ShowTop = (this.ViewerOne2OneWidth - this.Width - this.Left)/scaleX*zoomValue + this.ViewerTop;
			this.ShowWidth = this.Height/scaleY*zoomValue;
			this.ShowHeight = (this.Width)/scaleX*zoomValue;
		}

		this.resizeScaleX = this.ShowWidth/previousWidth;
		this.resizeScaleY = this.ShowHeight/previousHeight;
		

		this.ReSetPoints();

		this.GetActualPosition();
		
		this.CalculateOnRotate();
	},
	
	CalculateOnRotate: function()
	{		
		var allWidth = this.ViewerWidth;
		var allHeight = this.ViewerHeight;
		if(allWidth < _docWidth)
		{
			allWidth = _docWidth;
		}
		if(allHeight < _docHeight)
		{
			allHeight = _docHeight;
		}
		allWidth = parseInt(allWidth);
		allHeight = parseInt(allHeight);
		
		rotateLeft = 0;
		rotateTop = 0;
		if(this.ViewerHeight < _docWidth)
		{
			rotateLeft = (_docWidth - this.ViewerHeight)/2;
		}
		if(this.ViewerWidth < _docHeight)
		{
			rotateTop = (_docHeight - this.ViewerWidth)/2;
		}			
		
		

		if(this.Rotate == 0)//0 degree
		{			
			this.ShowLeft_0 = this.ShowLeft;
			this.ShowTop_0 = this.ShowTop;
			this.ShowWidth_0 = this.ShowWidth;
			this.ShowHeight_0 = this.ShowHeight;

			this.ShowLeft_90 = allHeight + rotateLeft - (this.ViewerTop + this.ShowTop_0 + this.ShowHeight_0);
			this.ShowTop_90 = rotateTop + this.ShowLeft_0 - this.ViewerLeft;
			this.ShowWidth_90 = this.ShowHeight_0;
			this.ShowHeight_90 = this.ShowWidth_0;


			this.ShowLeft_180 = allWidth - (this.ShowLeft_0 + this.ShowWidth_0);
			this.ShowTop_180 = allHeight - (this.ShowTop_0 + this.ShowHeight_0);
			this.ShowWidth_180 = this.ShowWidth_0;
			this.ShowHeight_180 = this.ShowHeight_0;

			this.ShowLeft_270 = rotateLeft + this.ShowTop_0 - this.ViewerTop;
			this.ShowTop_270 = rotateTop + allWidth - (this.ViewerLeft + this.ShowLeft_0 + this.ShowWidth_0);
			this.ShowWidth_270 = this.ShowHeight_0;
			this.ShowHeight_270 = this.ShowWidth_0;	
				
			var pointArr = this.ShowPoints.split(";");			
			this.ShowPoints_0 = this.ShowPoints;
			this.ShowPoints_90 = "";
			this.ShowPoints_180 = "";
			this.ShowPoints_270 = "";
			this.RelativePoints_0 = this.RelativePoints;
			this.RelativePoints_90 = "";
			this.RelativePoints_180 = "";
			this.RelativePoints_270 = "";
			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					this.ShowPoints_90 += (this.ShowHeight - (xys[1] - this.ShowTop) + this.ShowLeft_90) + "," + (xys[0] - this.ShowLeft + this.ShowTop_90) + ";";
					this.RelativePoints_90 += (this.ShowHeight - (xys[1] - this.ShowTop)) + "," + (xys[0] - this.ShowLeft) + " ";
					this.ShowPoints_180 += (this.ShowWidth - (xys[0] - this.ShowLeft) + this.ShowLeft_180) + "," + (this.ShowHeight - (xys[1] - this.ShowTop) + this.ShowTop_180) + ";";
					this.RelativePoints_180 += (this.ShowWidth - (xys[0] - this.ShowLeft)) + "," + (this.ShowHeight - (xys[1] - this.ShowTop)) + " ";
					this.ShowPoints_270 += (xys[1] - this.ShowTop + this.ShowLeft_270) + "," + (this.ShowWidth - (xys[0] - this.ShowLeft) + this.ShowTop_270) + ";";
					this.RelativePoints_270 += (xys[1] - this.ShowTop) + "," + (this.ShowWidth - (xys[0] - this.ShowLeft)) + " ";
				} 
			}
			this.ShowPoints_90 = this.ShowPoints_90.substring(0,this.ShowPoints_90.length-1);
			this.RelativePoints_90 = this.RelativePoints_90.substring(0,this.RelativePoints_90.length-1);
			this.ShowPoints_180 = this.ShowPoints_180.substring(0,this.ShowPoints_180.length-1);
			this.RelativePoints_180 = this.RelativePoints_180.substring(0,this.RelativePoints_180.length-1);
			this.ShowPoints_270 = this.ShowPoints_270.substring(0,this.ShowPoints_270.length-1);
			this.RelativePoints_270 = this.RelativePoints_270.substring(0,this.RelativePoints_270.length-1);
		}
		else if(this.Rotate == 1)//90 degree
		{						
			this.ShowLeft_90 = this.ShowLeft;
			this.ShowTop_90 = this.ShowTop;
			this.ShowWidth_90 = this.ShowWidth;
			this.ShowHeight_90 = this.ShowHeight;

			this.ShowLeft_0 = rotateLeft + this.ShowTop_90 - this.ViewerTop;
			this.ShowTop_0 = rotateTop + allWidth - (this.ViewerLeft + this.ShowLeft_90 + this.ShowWidth_90);
			this.ShowWidth_0 = this.ShowHeight_90;
			this.ShowHeight_0 = this.ShowWidth_90;
							
			this.ShowLeft_180 = rotateLeft + allHeight - this.ShowTop - this.ShowHeight - this.ViewerTop;
			this.ShowTop_180 = rotateTop + this.ShowLeft - this.ViewerLeft;
			this.ShowWidth_180 = this.ShowWidth_0;
			this.ShowHeight_180 = this.ShowHeight_0;

			this.ShowLeft_270 = allWidth - this.ShowLeft - this.ShowWidth;
			this.ShowTop_270 = allHeight - this.ShowTop - this.ShowHeight;				
			this.ShowWidth_270 = this.ShowHeight_0;
			this.ShowHeight_270 = this.ShowWidth_0;	
				
			var pointArr = this.ShowPoints.split(";");			
			this.ShowPoints_0 = "";
			this.ShowPoints_90 = this.ShowPoints;
			this.ShowPoints_180 = "";
			this.ShowPoints_270 = "";
			this.RelativePoints_0 = "";
			this.RelativePoints_90 = this.RelativePoints;
			this.RelativePoints_180 = "";
			this.RelativePoints_270 = "";
			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					this.ShowPoints_0 += (xys[1] - this.ShowTop + this.ShowLeft_0) + "," + (this.ShowWidth - (xys[0] - this.ShowLeft) + this.ShowTop_0) + ";";
					this.RelativePoints_0 += (xys[1] - this.ShowTop) + "," + (this.ShowWidth - (xys[0] - this.ShowLeft)) + " ";
					this.ShowPoints_180 += (this.ShowHeight - (xys[1] - this.ShowTop) + this.ShowLeft_180) + "," + (xys[0] - this.ShowLeft + this.ShowTop_180) + ";";
					this.RelativePoints_180 += (this.ShowHeight - (xys[1] - this.ShowTop)) + "," + (xys[0] - this.ShowLeft) + " ";
					this.ShowPoints_270 += (this.ShowWidth - (xys[0] - this.ShowLeft) + this.ShowLeft_270) + "," + (this.ShowHeight - (xys[1] - this.ShowTop) + this.ShowTop_270) + ";";
					this.RelativePoints_270 += (this.ShowWidth - (xys[0] - this.ShowLeft)) + "," + (this.ShowHeight - (xys[1] - this.ShowTop)) + " ";
				} 
			}
			this.ShowPoints_0 = this.ShowPoints_0.substring(0,this.ShowPoints_0.length-1);
			this.RelativePoints_0 = this.RelativePoints_0.substring(0,this.RelativePoints_0.length-1);
			this.ShowPoints_180 = this.ShowPoints_180.substring(0,this.ShowPoints_180.length-1);
			this.RelativePoints_180 = this.RelativePoints_180.substring(0,this.RelativePoints_180.length-1);
			this.ShowPoints_270 = this.ShowPoints_270.substring(0,this.ShowPoints_270.length-1);
			this.RelativePoints_270 = this.RelativePoints_270.substring(0,this.RelativePoints_270.length-1);
		}
		else if(this.Rotate == 2)//180 degree
		{
			this.ShowLeft_180 = this.ShowLeft;
			this.ShowTop_180 = this.ShowTop;
			this.ShowWidth_180 = this.ShowWidth;
			this.ShowHeight_180 = this.ShowHeight;

			this.ShowLeft_0 = allWidth - this.ShowLeft_180 - this.ShowWidth_180;
			this.ShowTop_0 = allHeight - this.ShowTop_180 - this.ShowHeight_180;
			this.ShowWidth_0 = this.ShowWidth_180;
			this.ShowHeight_0 = this.ShowHeight_180;

			this.ShowLeft_90 = allHeight + rotateLeft - (this.ViewerTop + this.ShowTop_0 + this.ShowHeight_0);
			this.ShowTop_90 = rotateTop + this.ShowLeft_0 - this.ViewerLeft;
			this.ShowWidth_90 = this.ShowHeight_0;
			this.ShowHeight_90 = this.ShowWidth_0;

			this.ShowLeft_270 = rotateLeft + this.ShowTop_0 - this.ViewerTop;
			this.ShowTop_270 = rotateTop + allWidth - (this.ViewerLeft + this.ShowLeft_0 + this.ShowWidth_0);
			this.ShowWidth_270 = this.ShowHeight_0;
			this.ShowHeight_270 = this.ShowWidth_0;		

			var pointArr = this.ShowPoints.split(";");			
			this.ShowPoints_0 = "";
			this.ShowPoints_90 = "";
			this.ShowPoints_180 = this.ShowPoints;
			this.ShowPoints_270 = "";
			this.RelativePoints_0 = "";
			this.RelativePoints_90 = "";
			this.RelativePoints_180 = this.RelativePoints;
			this.RelativePoints_270 = "";
			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					this.ShowPoints_0 += (this.ShowWidth - (xys[0] - this.ShowLeft) + this.ShowLeft_0) + "," + (this.ShowHeight - (xys[1] - this.ShowTop) + this.ShowTop_0) + ";";
					this.RelativePoints_0 += (this.ShowWidth - (xys[0] - this.ShowLeft)) + "," + (this.ShowHeight - (xys[1] - this.ShowTop)) + " ";
					this.ShowPoints_90 += (xys[1] - this.ShowTop + this.ShowLeft_90) + "," + (this.ShowWidth - (xys[0] - this.ShowLeft) + this.ShowTop_90) + ";";
					this.RelativePoints_90 += (xys[1] - this.ShowTop) + "," + (this.ShowWidth - (xys[0] - this.ShowLeft)) + " ";
					this.ShowPoints_270 += (this.ShowHeight -(xys[1] - this.ShowTop) + this.ShowLeft_270) + "," + (xys[0] - this.ShowLeft + this.ShowTop_270) + ";";
					this.RelativePoints_270 += (this.ShowHeight -(xys[1] - this.ShowTop)) + "," + (xys[0] - this.ShowLeft) + " ";
				} 
			}
			this.ShowPoints_0 = this.ShowPoints_0.substring(0,this.ShowPoints_0.length-1);
			this.RelativePoints_0 = this.RelativePoints_0.substring(0,this.RelativePoints_0.length-1);
			this.ShowPoints_90 = this.ShowPoints_90.substring(0,this.ShowPoints_90.length-1);
			this.RelativePoints_90 = this.RelativePoints_90.substring(0,this.RelativePoints_90.length-1);
			this.ShowPoints_270 = this.ShowPoints_270.substring(0,this.ShowPoints_270.length-1);
			this.RelativePoints_270 = this.RelativePoints_270.substring(0,this.RelativePoints_270.length-1);
		}
		else if(this.Rotate == 3)//270 degree
		{
			this.ShowLeft_270 = this.ShowLeft;
			this.ShowTop_270 = this.ShowTop;
			this.ShowWidth_270 = this.ShowWidth;
			this.ShowHeight_270 = this.ShowHeight;

			this.ShowLeft_0 = rotateLeft + allHeight - (this.ShowTop_270 + this.ShowHeight_270 + this.ViewerTop);				
			this.ShowTop_0 = rotateTop + this.ShowLeft_270 - this.ViewerLeft;
			this.ShowWidth_0 = this.ShowHeight_270;
			this.ShowHeight_0 = this.ShowWidth_270;

			this.ShowLeft_90 = allWidth - this.ShowLeft - this.ShowWidth;
			this.ShowTop_90 = allHeight - this.ShowTop - this.ShowHeight;
			this.ShowWidth_90 = this.ShowHeight_0;
			this.ShowHeight_90 = this.ShowWidth_0;

			this.ShowLeft_180 = rotateLeft + this.ShowTop - this.ViewerTop;
			this.ShowTop_180 = rotateTop + allWidth - this.ShowLeft - this.ShowWidth - this.ViewerLeft;
			this.ShowWidth_180 = this.ShowWidth_0;
			this.ShowHeight_180 = this.ShowHeight_0;

			var pointArr = this.ShowPoints.split(";");			
			this.ShowPoints_0 = "";
			this.ShowPoints_90 = "";
			this.ShowPoints_180 = "";
			this.ShowPoints_270 = this.ShowPoints;
			this.RelativePoints_0 = "";
			this.RelativePoints_90 = "";
			this.RelativePoints_180 = "";
			this.RelativePoints_270 = this.RelativePoints;
			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					this.ShowPoints_0 += (this.ShowHeight - (xys[1] - this.ShowTop) + this.ShowLeft_0) + "," + (xys[0] - this.ShowLeft + this.ShowTop_0) + ";";
					this.RelativePoints_0 += (this.ShowHeight - (xys[1] - this.ShowTop)) + "," + (xys[0] - this.ShowLeft) + " ";
					this.ShowPoints_90 += (this.ShowWidth - (xys[0] - this.ShowLeft) + this.ShowLeft_90) + "," + (this.ShowHeight - (xys[1] - this.ShowTop) + this.ShowTop_90) + ";";
					this.RelativePoints_90 += (this.ShowWidth - (xys[0] - this.ShowLeft)) + "," + (this.ShowHeight - (xys[1] - this.ShowTop)) + " ";
					this.ShowPoints_180 += (xys[1] - this.ShowTop + this.ShowLeft_180) + "," + (this.ShowWidth - (xys[0] - this.ShowLeft) + this.ShowTop_180) + ";";
					this.RelativePoints_180 += (xys[1] - this.ShowTop) + "," + (this.ShowWidth - (xys[0] - this.ShowLeft)) + " ";
				} 
			}
			this.ShowPoints_0 = this.ShowPoints_0.substring(0,this.ShowPoints_0.length-1);
			this.RelativePoints_0 = this.RelativePoints_0.substring(0,this.RelativePoints_0.length-1);
			this.ShowPoints_90 = this.ShowPoints_90.substring(0,this.ShowPoints_90.length-1);
			this.RelativePoints_90 = this.RelativePoints_90.substring(0,this.RelativePoints_90.length-1);
			this.ShowPoints_180 = this.ShowPoints_180.substring(0,this.ShowPoints_180.length-1);
			this.RelativePoints_180 = this.RelativePoints_180.substring(0,this.RelativePoints_180.length-1);
		}

		//thumb
		if(this.Rotate == 0)
		{
			this.ThumbShowLeft_0 = this.ThumbShowLeft;
			this.ThumbShowTop_0 = this.ThumbShowTop;
			this.ThumbShowWidth_0 = this.ThumbShowWidth;
			this.ThumbShowHeight_0 = this.ThumbShowHeight;

			this.ThumbShowLeft_90 = _thumbHeight - (this.ThumbShowTop_0 + this.ThumbShowHeight_0);
			this.ThumbShowTop_90 = this.ThumbShowLeft_0;
			this.ThumbShowWidth_90 = this.ThumbShowHeight_0;
			this.ThumbShowHeight_90 = this.ThumbShowWidth_0;

			this.ThumbShowLeft_180 = _thumbWidth - (this.ThumbShowLeft_0 + this.ThumbShowWidth_0);
			this.ThumbShowTop_180 = _thumbHeight - (this.ThumbShowTop_0 + this.ThumbShowHeight_0);
			this.ThumbShowWidth_180 = this.ThumbShowWidth_0;
			this.ThumbShowHeight_180 = this.ThumbShowHeight_0;

			this.ThumbShowLeft_270 = this.ThumbShowTop_0;
			this.ThumbShowTop_270 = _thumbWidth - (this.ThumbShowLeft_0 + this.ThumbShowWidth_0);
			this.ThumbShowWidth_270 = this.ThumbShowHeight_0;
			this.ThumbShowHeight_270 = this.ThumbShowWidth_0;

			var pointArr = this.ThumbPoints.split(";");			
			this.ThumbShowPoints_0 = this.ThumbShowPoints;
			this.ThumbShowPoints_90 = "";
			this.ThumbShowPoints_180 = "";
			this.ThumbShowPoints_270 = "";
			this.ThumbRelativePoints_0 = this.ThumbRelativePoints;
			this.ThumbRelativePoints_90 = "";
			this.ThumbRelativePoints_180 = "";
			this.ThumbRelativePoints_270 = "";
			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					this.ThumbShowPoints_90 += (this.ThumbShowHeight - xys[1]) + "," + xys[0] + ";";
					this.ThumbRelativePoints_90 += (this.ThumbShowHeight - xys[1]) + "," + xys[0] + " ";
					this.ThumbShowPoints_180 += (this.ThumbShowWidth - xys[0]) + "," + (this.ThumbShowHeight - xys[1]) + ";";
					this.ThumbRelativePoints_180 += (this.ThumbShowWidth - xys[0]) + "," + (this.ThumbShowHeight - xys[1]) + " ";
					this.ThumbShowPoints_270 += xys[1] + "," + (this.ThumbShowWidth - xys[0]) + ";";
					this.ThumbRelativePoints_270 += xys[1] + "," + (this.ThumbShowWidth - xys[0]) + " ";
				} 
			}
			this.ThumbShowPoints_90 = this.ThumbShowPoints_90.substring(0,this.ThumbShowPoints_90.length-1);
			this.ThumbRelativePoints_90 = this.ThumbRelativePoints_90.substring(0,this.ThumbRelativePoints_90.length-1);
			this.ThumbShowPoints_180 = this.ThumbShowPoints_180.substring(0,this.ThumbShowPoints_180.length-1);
			this.ThumbRelativePoints_180 = this.ThumbRelativePoints_180.substring(0,this.ThumbRelativePoints_180.length-1);
			this.ThumbShowPoints_270 = this.ThumbShowPoints_270.substring(0,this.ThumbShowPoints_270.length-1);
			this.ThumbRelativePoints_270 = this.ThumbRelativePoints_270.substring(0,this.ThumbRelativePoints_270.length-1);
		}
		else if(this.Rotate == 1)
		{
			this.ThumbShowLeft_90 = this.ThumbShowLeft;
			this.ThumbShowTop_90 = this.ThumbShowTop;
			this.ThumbShowWidth_90 = this.ThumbShowWidth;
			this.ThumbShowHeight_90 = this.ThumbShowHeight;
			
			this.ThumbShowLeft_0 = this.ThumbShowTop_90;
			this.ThumbShowTop_0 = _thumbHeight - (this.ThumbShowLeft_90 + this.ThumbShowWidth_90);
			this.ThumbShowWidth_0 = this.ThumbShowHeight_90;
			this.ThumbShowHeight_0 = this.ThumbShowWidth_90;
							
			this.ThumbShowLeft_180 = _thumbWidth - this.ThumbShowTop_90 - this.ThumbShowHeight_90;
			this.ThumbShowTop_180 = this.ThumbShowLeft_90;
			this.ThumbShowWidth_180 = this.ThumbShowWidth_0;
			this.ThumbShowHeight_180 = this.ThumbShowHeight_0;

			this.ThumbShowLeft_270 = _thumbHeight - this.ThumbShowLeft_90 - this.ThumbShowWidth_90;
			this.ThumbShowTop_270 = _thumbWidth - this.ThumbShowTop_90 - this.ThumbShowHeight_90;				
			this.ThumbShowWidth_270 = this.ThumbShowHeight_0;
			this.ThumbShowHeight_270 = this.ThumbShowWidth_0;	

			var pointArr = this.ThumbPoints.split(";");			
			this.ThumbShowPoints_0 = "";
			this.ThumbShowPoints_90 = this.ThumbShowPoints;
			this.ThumbShowPoints_180 = "";
			this.ThumbShowPoints_270 = "";
			this.ThumbRelativePoints_0 = "";
			this.ThumbRelativePoints_90 = this.ThumbRelativePoints;
			this.ThumbRelativePoints_180 = "";
			this.ThumbRelativePoints_270 = "";
			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					this.ThumbShowPoints_0 += (xys[1]) + "," + (this.ThumbShowWidth - (xys[0])) + ";";
					this.ThumbRelativePoints_0 += (xys[1]) + "," + (this.ThumbShowWidth - (xys[0])) + " ";
					this.ThumbShowPoints_180 += (this.ThumbShowHeight - (xys[1])) + "," + (xys[0]) + ";";
					this.ThumbRelativePoints_180 += (this.ThumbShowHeight - (xys[1])) + "," + (xys[0]) + " ";
					this.ThumbShowPoints_270 += (this.ThumbShowWidth - (xys[0])) + "," + (this.ThumbShowHeight - (xys[1])) + ";";
					this.ThumbRelativePoints_270 += (this.ThumbShowWidth - (xys[0])) + "," + (this.ThumbShowHeight - (xys[1])) + " ";
				} 
			}
			this.ThumbShowPoints_0 = this.ThumbShowPoints_0.substring(0,this.ThumbShowPoints_0.length-1);
			this.ThumbRelativePoints_0 = this.ThumbRelativePoints_0.substring(0,this.ThumbRelativePoints_0.length-1);
			this.ThumbShowPoints_180 = this.ThumbShowPoints_180.substring(0,this.ThumbShowPoints_180.length-1);
			this.ThumbRelativePoints_180 = this.ThumbRelativePoints_180.substring(0,this.ThumbRelativePoints_180.length-1);
			this.ThumbShowPoints_270 = this.ThumbShowPoints_270.substring(0,this.ThumbShowPoints_270.length-1);
			this.ThumbRelativePoints_270 = this.ThumbRelativePoints_270.substring(0,this.ThumbRelativePoints_270.length-1);
		}
		else if(this.Rotate == 2)
		{
			this.ThumbShowLeft_180 = this.ThumbShowLeft;
			this.ThumbShowTop_180 = this.ThumbShowTop;
			this.ThumbShowWidth_180 = this.ThumbShowWidth;
			this.ThumbShowHeight_180 = this.ThumbShowHeight;

			this.ThumbShowLeft_0 = _thumbWidth - this.ThumbShowLeft_180 - this.ThumbShowWidth_180;
			this.ThumbShowTop_0 = _thumbHeight - this.ThumbShowTop_180 - this.ThumbShowHeight_180;
			this.ThumbShowWidth_0 = this.ThumbShowWidth_180;
			this.ThumbShowHeight_0 = this.ThumbShowHeight_180;

			this.ThumbShowLeft_90 = _thumbHeight - (this.ThumbShowTop_0 + this.ThumbShowHeight_0);
			this.ThumbShowTop_90 = this.ThumbShowLeft_0 ;
			this.ThumbShowWidth_90 = this.ThumbShowHeight_0;
			this.ThumbShowHeight_90 = this.ThumbShowWidth_0;

			this.ThumbShowLeft_270 = this.ThumbShowTop_0;
			this.ThumbShowTop_270 = _thumbWidth - (this.ThumbShowLeft_0 + this.ThumbShowWidth_0);
			this.ThumbShowWidth_270 = this.ThumbShowHeight_0;
			this.ThumbShowHeight_270 = this.ThumbShowWidth_0;		

			var pointArr = this.ThumbPoints.split(";");			
			this.ThumbShowPoints_0 = "";
			this.ThumbShowPoints_90 = "";
			this.ThumbShowPoints_180 = this.ThumbShowPoints;
			this.ThumbShowPoints_270 = "";
			this.ThumbRelativePoints_0 = "";
			this.ThumbRelativePoints_90 = "";
			this.ThumbRelativePoints_180 = this.ThumbRelativePoints;
			this.ThumbRelativePoints_270 = "";
			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					this.ThumbShowPoints_0 += (this.ThumbShowWidth - (xys[0])) + "," + (this.ThumbShowHeight - (xys[1])) + ";";
					this.ThumbRelativePoints_0 += (this.ThumbShowWidth - (xys[0])) + "," + (this.ThumbShowHeight - (xys[1])) + " ";
					this.ThumbShowPoints_90 += (xys[1]) + "," + (this.ThumbShowWidth - (xys[0])) + ";";
					this.ThumbRelativePoints_90 += (xys[1]) + "," + (this.ThumbShowWidth - (xys[0])) + " ";
					this.ThumbShowPoints_270 += (this.ThumbShowHeight - (xys[1])) + "," + (xys[0]) + ";";
					this.ThumbRelativePoints_270 += (this.ThumbShowHeight -(xys[1])) + "," + (xys[0]) + " ";
				} 
			}
			this.ThumbShowPoints_0 = this.ThumbShowPoints_0.substring(0,this.ThumbShowPoints_0.length-1);
			this.ThumbRelativePoints_0 = this.ThumbRelativePoints_0.substring(0,this.ThumbRelativePoints_0.length-1);
			this.ThumbShowPoints_90 = this.ThumbShowPoints_90.substring(0,this.ThumbShowPoints_90.length-1);
			this.ThumbRelativePoints_90 = this.ThumbRelativePoints_90.substring(0,this.ThumbRelativePoints_90.length-1);
			this.ThumbShowPoints_270 = this.ThumbShowPoints_270.substring(0,this.ThumbShowPoints_270.length-1);
			this.ThumbRelativePoints_270 = this.ThumbRelativePoints_270.substring(0,this.ThumbRelativePoints_270.length-1);
		}
		else if(this.Rotate == 3)//270 degree
		{
			this.ThumbShowLeft_270 = this.ThumbShowLeft;
			this.ThumbShowTop_270 = this.ThumbShowTop;
			this.ThumbShowWidth_270 = this.ThumbShowWidth;
			this.ThumbShowHeight_270 = this.ThumbShowHeight;

			this.ThumbShowLeft_0 = _thumbWidth - (this.ThumbShowTop_270 + this.ThumbShowHeight_270);				
			this.ThumbShowTop_0 = this.ThumbShowLeft_270;
			this.ThumbShowWidth_0 = this.ThumbShowHeight_270;
			this.ThumbShowHeight_0 = this.ThumbShowWidth_270;

			this.ThumbShowLeft_90 = _thumbHeight - this.ThumbShowLeft_270 - this.ThumbShowWidth_270;
			this.ThumbShowTop_90 = _thumbWidth - this.ThumbShowTop_270 - this.ThumbShowHeight_270;
			this.ThumbShowWidth_90 = this.ThumbShowHeight_0;
			this.ThumbShowHeight_90 = this.ThumbShowWidth_0;

			this.ThumbShowLeft_180 = this.ThumbShowTop_270;
			this.ThumbShowTop_180 = _thumbHeight - this.ThumbShowLeft_270 - this.ThumbShowWidth_270;
			this.ThumbShowWidth_180 = this.ThumbShowWidth_0;
			this.ThumbShowHeight_180 = this.ThumbShowHeight_0;

			var pointArr = this.ThumbPoints.split(";");			
			this.ThumbShowPoints_0 = "";
			this.ThumbShowPoints_90 = "";
			this.ThumbShowPoints_180 = "";
			this.ThumbShowPoints_270 = this.ThumbShowPoints;
			this.ThumbRelativePoints_0 = "";
			this.ThumbRelativePoints_90 = "";
			this.ThumbRelativePoints_180 = "";
			this.ThumbRelativePoints_270 = this.ThumbRelativePoints;
			for(var i = 0; i < pointArr.length; i++)
			{
				var xys = pointArr[i].split(",");
				if(typeof(xys) != 'undefined')
				{
					this.ThumbShowPoints_0 += (this.ThumbShowHeight -(xys[1])) + "," + (xys[0]) + ";";
					this.ThumbRelativePoints_0 += (this.ThumbShowHeight - (xys[1])) + "," + (xys[0]) + " ";
					this.ThumbShowPoints_90 += (this.ThumbShowWidth - (xys[0])) + "," + (this.ThumbShowHeight - (xys[1])) + ";";
					this.ThumbRelativePoints_90 += (this.ThumbShowWidth - (xys[0])) + "," + (this.ThumbShowHeight - (xys[1])) + " ";
					this.ThumbShowPoints_180 += (xys[1]) + "," + (this.ThumbShowWidth - (xys[0])) + ";";
					this.ThumbRelativePoints_180 += (xys[1]) + "," + (this.ThumbShowWidth - (xys[0])) + " ";
				} 
			}
			this.ThumbShowPoints_0 = this.ThumbShowPoints_0.substring(0,this.ThumbShowPoints_0.length-1);
			this.ThumbRelativePoints_0 = this.ThumbRelativePoints_0.substring(0,this.ThumbRelativePoints_0.length-1);
			this.ThumbShowPoints_90 = this.ThumbShowPoints_90.substring(0,this.ThumbShowPoints_90.length-1);
			this.ThumbRelativePoints_90 = this.ThumbRelativePoints_90.substring(0,this.ThumbRelativePoints_90.length-1);
			this.ThumbShowPoints_180 = this.ThumbShowPoints_180.substring(0,this.ThumbShowPoints_180.length-1);
			this.ThumbRelativePoints_180 = this.ThumbRelativePoints_180.substring(0,this.ThumbRelativePoints_180.length-1);
		}
			
	},
};