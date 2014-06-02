var FileDocument = Class.create();
FileDocument.prototype = 
{
	initialize: function(options) 
	{
		this.SetOptions(options);

		this.Index = this.options.Index;
		this.Name = this.options.Name;
		this.Viewers = this.options.Viewers;		
		
	},
		
	SetOptions: function(options) 
	{
		this.options = 
		{
			Index:			0,		
			Name:           "",	
			Viewers:		new Array(),		
		};
		Extend(this.options, options || {});
    },

	GetShowIndex: function(pageIndex)
	{
		for(var i=0; i<this.Viewers.length; i++)
		{
			if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}

			if(this.Viewers[i].PageIndex == pageIndex)
			{
				return parseInt(this.Viewers[i].ShowIndex);
			}
		}
		return null;
	},

	GetPageIndex: function(showIndex)
	{
		for(var i=0; i<this.Viewers.length; i++)
		{
			if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}

			if(this.Viewers[i].ShowIndex == showIndex)
			{
				return parseInt(this.Viewers[i].PageIndex);
			}
		}
		return null;
	},


	DeletePage: function(showIndex)
	{
		var pageIndex = this.GetPageIndex(showIndex);
		//this.Viewers.splice(pageIndex,1);
		this.Viewers[pageIndex] = null;

		for(var i=0; i<this.Viewers.length; i++)
		{
			if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}

			if(this.Viewers[i].ShowIndex > showIndex)
			{
				this.Viewers[i].ShowIndex--;
				$("#thumbPgId_" + this.Viewers[i].PageIndex).text((parseInt(this.Viewers[i].ShowIndex) + 1));
			}
		}
	},

	AddPage: function(showIndex)
	{
		for(var i=0; i<this.Viewers.length; i++)
		{
			if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}

			if(this.Viewers[i].ShowIndex >= showIndex)
			{
				this.Viewers[i].ShowIndex++;
				$("#thumbPgId_" + this.Viewers[i].PageIndex).text((parseInt(this.Viewers[i].ShowIndex) + 1));
			}
		}
		
		var pageIndex = _maxTotalPageCount++;
		var viewer = new Viewer({PageIndex: pageIndex, IsNewPage: true});
		viewer.ShowIndex = showIndex;
		this.Viewers[pageIndex] = viewer;
	},

	SortPage: function(newIndexs)
	{
		for(var i=0; i<this.Viewers.length; i++)
		{
			if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}

			this.Viewers[i].ShowIndex = newIndexs[this.Viewers[i].PageIndex];
			$("#thumbPgId_" + this.Viewers[i].PageIndex).text((parseInt(this.Viewers[i].ShowIndex)) + 1);
		}
	},

	BurnAnnos: function()
	{
		for(var i=0; i<this.Viewers.length; i++)
		{
			if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}

			this.Viewers[i].BurnAnnos();
		}
	},

	DeleteBurnedAnnos: function()
	{
		for(var i=0; i<this.Viewers.length; i++)
		{
			if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}

			this.Viewers[i].DeleteBurnedAnnos();
		}
	},

	DeleteAnno: function(annoIndex)
	{
		for(var i=0; i<this.Viewers.length; i++)
		{
			if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}
			
			if(this.Viewers[i].DeleteAnno(annoIndex))
			{
				return false;
			}			
		}
	},
	
	DeleteBurnedAnnotations: function()
	{
	    for(var i=0; i<this.Viewers.length; i++)
		{
			if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}

			this.Viewers[i].DeleteBurnedAnnotations();
		}
	},

	IsAnnoBurned: function(annoIndex)
	{
		for(var i=0; i<this.Viewers.length; i++)
		{
			if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}
			
			if(this.Viewers[i].IsAnnoBurned(annoIndex))
			{
				return true;
			}			
		}

		return false;
	},
	
	ChangeState:function()
	{
	    for(var i=0; i<this.Viewers.length;i++)
	    {
	        if(this.Viewers[i] == null || this.Viewers[i].ShowIndex == null)
			{continue;}
			this.Viewers[i].isChange = true;
	    }
	},
}