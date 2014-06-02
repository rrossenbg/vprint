var Viewer = Class.create();
Viewer.prototype = 
{

	initialize: function(options) 
	{
		this.SetOptions(options);

		//this.FilePath = this.options.FilePath;
		this.PageIndex = this.options.PageIndex;
		this.ShowIndex = this.options.PageIndex;
		this.Width = this.options.Width;
		this.Height = this.options.Height;
		this.ThumbWidth = this.options.ThumbWidth;
		this.ThumbHeight = this.options.ThumbHeight;
		this.ActualWidth = this.options.ActualWidth;
		this.ActualHeight = this.options.ActualHeight;
		this.BigWidth = this.options.BigWidth;
		this.BigHeight = this.options.BigHeight;
		this.Left = this.options.Left;
		this.Top = this.options.Top;
		this.ZoomIndex = this.options.ZoomIndex;
		this.Rotate = this.options.Rotate;
		this.Sharpen = this.options.Sharpen;
		this.AntiqueEffect = this.options.AntiqueEffect;		
		this.PageSizeType = this.options.PageSizeType;
		this.Annotations = this.options.Annotations;
		this.BurnAnnotations = this.options.BurnAnnotations;
		this.BurnedAnnotations = this.options.BurnedAnnotations;
		this.FirstLeft = this.options.FirstLeft;		
		this.FirstTop = this.options.FirstTop;
		this.FirstActualWidth = this.options.FirstActualWidth;
		this.FirstActualHeight = this.options.FirstActualHeight;
		//this.ScaleX = this.options.ScaleX;
		//this.ScaleY = this.options.ScaleY;
		//this.ThumbScaleX = this.options.ThumbScaleX;
		//this.ThumbScaleY = this.options.ThumbScaleY;
				
		this.Init = false;
		this.isChange = this.options.isChange;
		//this.GetInitialSize();
		
	},
		
	SetOptions: function(options) 
	{
		this.options = 
		{
			//FilePath:			"",
			PageIndex:			0,
			ShowIndex:			0,
			Width:				_docWidth,
			Height:				_docHeight,
			ThumbWidth:			_thumbWidth,
			ThumbHeight:		_thumbHeight,
			ActualWidth:		0,
			ActualHeight:		0,
			BigWidth:			0,
			BigHeight:			0,
			Left:				0,
			Top:				0,
			ZoomIndex:			_defaultZoonIndex,
			Rotate:				0,
			Sharpen:			false,	
			AntiqueEffect:		false,
			PageSizeType:		_defaultPageSizeType,
			Annotations:		new Array(),
			BurnAnnotations:	new Array(),
			BurnedAnnotations:  new Array(),
			FirstLeft:			0,
			FirstTop:			0,
			FirstActualWidth:	0,
			FirstActualHeight:	0,
			isChange:           true,
			//ScaleX:				1,
			//ScaleY:				1,
			//ThumbScaleX:		1,
			//ThumbScaleY:		1,
			
		};
		Extend(this.options, options || {});
    },

	GetInitialSize: function()
	{			
		getCurrentPageSize(this.PageIndex);
		var position = _size.indexOf(",");

		this.OneToOne_Width = _size.substring(0, position);
		this.OneToOne_Height = _size.substring(position + 1);
		this.FitWidth_Width = _docWidth;
		this.FitWidth_Height = _docWidth/this.OneToOne_Width*this.OneToOne_Height;
		this.BestFit_Width = _docHeight/this.OneToOne_Height*this.OneToOne_Width;
		this.BestFit_Height = _docHeight;	

		this.ThumbScaleX = _thumbWidth/this.OneToOne_Width;
		this.ThumbScaleY = _thumbHeight/this.OneToOne_Height;

		this.GetFirstViewerLeftTop();		
	},

	GetFirstViewerLeftTop: function()
	{
		this.FirstLeft = 0;
		this.FirstTop = 0;		
		
		if(_pageSizeType == 0)//fit-width
		{
			this.FirstActualWidth = this.FitWidth_Width;
			this.FirstActualHeight = this.FitWidth_Height;
					
		}
		else if(_pageSizeType == 1)//best-fit
		{
			this.FirstActualWidth = this.BestFit_Width;
			this.FirstActualHeight = this.BestFit_Height;
					
		}
		else if(_pageSizeType == 2)//1:1
		{
			this.FirstActualWidth = this.OneToOne_Width;
			this.FirstActualHeight = this.OneToOne_Height;
		}			
		{
			this.FirstLeft = (_docWidth - this.FirstActualWidth)/2;
		}
		{
			this.FirstTop = (_docHeight - this.FirstActualHeight)/2;
		}	
	},
	
	GetViewerLeftTop: function()
	{
		if(!this.Init)
		{
			this.GetInitialSize();
			this.Init = true;
		}
		if(this.isChange)
		{
		    this.FitWidth_Width = _docWidth;
		    this.FitWidth_Height = _docWidth/this.OneToOne_Width*this.OneToOne_Height;
		
		    this.BestFit_Width = _docHeight/this.OneToOne_Height*this.OneToOne_Width;
		    this.BestFit_Height = _docHeight;
		    this.isChange = false;
		}

		this.Left = 0;
		this.Top = 0;
		var zoomValue = _zoomRatio*_zoomIndex;
		
		if(_pageSizeType == 0)//fit-width
		{
			this.ActualWidth = this.FitWidth_Width*zoomValue;
			this.ActualHeight = this.FitWidth_Height*zoomValue;
			if(this.Rotate == 1 || this.Rotate == 3)
			{
				this.ActualWidth = this.FitWidth_Height*zoomValue;
				this.ActualHeight = this.FitWidth_Width*zoomValue;
			}
			this.ScaleX = this.OneToOne_Width/this.FitWidth_Width;
			this.ScaleY = this.OneToOne_Height/this.FitWidth_Height;
		}
		else if(_pageSizeType == 1)//best-fit
		{
			this.ActualWidth = this.BestFit_Width*zoomValue;
			this.ActualHeight = this.BestFit_Height*zoomValue;
			if(this.Rotate == 1 || this.Rotate == 3)
			{
				this.ActualWidth = this.BestFit_Height*zoomValue;
				this.ActualHeight = this.BestFit_Width*zoomValue;
			}
			this.ScaleX = this.OneToOne_Width/this.BestFit_Width;
			this.ScaleY = this.OneToOne_Height/this.BestFit_Height;
		}
		else if(_pageSizeType == 2)//1:1
		{
			this.ActualWidth = this.OneToOne_Width*zoomValue;
			this.ActualHeight = this.OneToOne_Height*zoomValue;
			if(this.Rotate == 1 || this.Rotate == 3)
			{
				this.ActualWidth = this.OneToOne_Height*zoomValue;
				this.ActualHeight = this.OneToOne_Width*zoomValue;
			}
			this.ScaleX = 1;
			this.ScaleY = 1;
		}	
		
		this.BigWidth = this.ActualWidth;
		this.BigHeight = this.ActualHeight;

		if(this.ActualWidth < _docWidth)
		{
			this.Left = (_docWidth - this.ActualWidth)/2;
			this.BigWidth = _docWidth;
		}
		if(this.ActualHeight < _docHeight)
		{
			this.Top = (_docHeight - this.ActualHeight)/2;
			this.BigHeight = _docHeight;
		}

		this.RefreshViewerInfoToAnnotation();
	},

	RefreshViewerInfoToAnnotation: function()
	{
		for(var i=0; i<this.Annotations.length; i++)
		{
			if(this.Annotations[i] == null || this.Annotations[i].Index == null)
			{continue;}

			this.Annotations[i].ViewerLeft = this.Left;
			this.Annotations[i].ViewerTop = this.Top;
			this.Annotations[i].ViewerWidth = this.ActualWidth;
			this.Annotations[i].ViewerHeight = this.ActualHeight;
			
			this.Annotations[i].ViewerScaleX = this.ScaleX;
			this.Annotations[i].ViewerScaleY = this.ScaleY;
			
			this.Annotations[i].ThumbViewerScaleX = this.ThumbScaleX;
			this.Annotations[i].ThumbViewerScaleY = this.ThumbScaleY;

			this.Annotations[i].ViewerOne2OneWidth = this.OneToOne_Width;
			this.Annotations[i].ViewerOne2OneHeight = this.OneToOne_Height;
		}

		for(var i=0; i<this.BurnAnnotations.length; i++)
		{
			if(this.BurnAnnotations[i] == null || this.BurnAnnotations[i].Index == null)
			{continue;}

			this.BurnAnnotations[i].ViewerLeft = this.Left;
			this.BurnAnnotations[i].ViewerTop = this.Top;
			this.BurnAnnotations[i].ViewerWidth = this.ActualWidth;
			this.BurnAnnotations[i].ViewerHeight = this.ActualHeight;
			
			this.BurnAnnotations[i].ViewerScaleX = this.ScaleX;
			this.BurnAnnotations[i].ViewerScaleY = this.ScaleY;
			
			this.BurnAnnotations[i].ThumbViewerScaleX = this.ThumbScaleX;
			this.BurnAnnotations[i].ThumbViewerScaleY = this.ThumbScaleY;

			this.BurnAnnotations[i].ViewerOne2OneWidth = this.OneToOne_Width;
			this.BurnAnnotations[i].ViewerOne2OneHeight = this.OneToOne_Height;
		}
	},

	GetSrc: function()
	{
		var src = getServerHandlerUrl();
		src += "?View=REImageViewer";
		src += "&FilePath=" + this.FilePath;
		src += "&PageIndex=" + this.PageIndex;
		src += "&DocWidth=" + this.Width;
		src += "&DocHeight=" + this.Height;
		src += "&Width=" + this.OneToOne_Width;
		src += "&Height=" + this.OneToOne_Height;
		src += "&ZoomValue=1";// + _zoomRatio*_zoomIndex;
		src += "&Rotate=0";
		src += "&Sharpen=" + this.Sharpen;
		src += "&AntiqueEffect=" + this.AntiqueEffect;
		src += "&PageSizeType=" + _pageSizeType;
		src += "&IsThumb=false";
		src += "&Rnd=" + _rnd;
		src += "&ScreenWidth=" + screen.width;
		src += "&ScreenHeight=" + screen.height;
		//src += "&Rnd1=" + Math.round(Math.random() * 10000000);
		return src;
	},

	GetThumbSrc: function()
	{
		var src = getServerHandlerUrl();
		src += "?View=REImageViewer";
		src += "&FilePath=" + this.FilePath;
		src += "&PageIndex=" + this.PageIndex;
		src += "&Width=" + this.ThumbWidth;
		src += "&Height=" + this.ThumbHeight;
		src += "&ZoomValue=1";
		src += "&Rotate=" + this.Rotate;
		src += "&Sharpen=" + this.Sharpen;
		src += "&AntiqueEffect=" + this.AntiqueEffect;
		src += "&PageSizeType=" + _pageSizeType;
		src += "&IsThumb=true";
		src += "&Rnd=" + _rnd;
		src += "&Rnd1=" + Math.round(Math.random() * 10000000);

		return src;
	},

	ChangeRotate: function(changeValue)
	{	
		if(changeValue == "1")
		{
			this.Rotate++;
			this.Rotate = this.Rotate%4;
			
		}
		else if(changeValue == "2")
		{
			this.Rotate = this.Rotate + 2;
			this.Rotate = this.Rotate%4;
		}
		else if(changeValue == "-1")
		{
			this.Rotate--;
			if(this.Rotate < 0)
			{
				this.Rotate = 3;
			}
			
		}
	},

	ChangeZoom: function(changeValue)
	{	
		if(changeValue == "1")
		{
			this.ZoomIndex++;
			
		}
		else if(changeValue == "-1")
		{
			this.ZoomIndex--;
			if(this.ZoomIndex < 1)
			{
				this.ZoomIndex = 1;
			}
		}
	},

	BurnAnnos: function()
	{
		for(index in this.Annotations)
		{
			if(this.Annotations[index] == null || this.Annotations[index].Index == null)
			{continue;}

			this.BurnAnnotations[index] = this.Annotations[index];
		}

		this.Annotations = new Array();
	},

	DeleteBurnedAnnos: function()
	{
	    for(var index in this.BurnAnnotations)
	    {
	        this.BurnedAnnotations[index] = this.BurnAnnotations[index];
	    }
		this.BurnAnnotations = new Array();
	},
	
	DeleteBurnedAnnotations: function()
	{
	    this.BurnedAnnotations = new Array();
	},

	BurnAnno: function(annoIndex)
	{
		for(index in this.Annotations)
		{
			if(this.Annotations[index] == null || this.Annotations[index].Index == null)
			{continue;}

			if(this.Annotations[index].Index == annoIndex)
			{
				this.BurnAnnotations[index] = this.Annotations[index];
				this.Annotations.splice(index,1);
				return true;
			}			
		}

		return false;
	},
	
	DeleteAnno: function(annoIndex)
	{
		for(index in this.Annotations)
		{
			if(this.Annotations[index] == null || this.Annotations[index].Index == null)
			{continue;}

			if(this.Annotations[index].Index == annoIndex)
			{			 
				this.Annotations.splice(index,1);
				return true;
			}
		}

		return false;
	},

	IsAnnoBurned: function(annoIndex)
	{
		for(index in this.BurnAnnotations)
		{
			if(this.BurnAnnotations[index] == null || this.BurnAnnotations[index].Index == null)
			{continue;}

			if(this.BurnAnnotations[index].Index == annoIndex)
			{
				return true;
			}
		}

		return false;
	},
};