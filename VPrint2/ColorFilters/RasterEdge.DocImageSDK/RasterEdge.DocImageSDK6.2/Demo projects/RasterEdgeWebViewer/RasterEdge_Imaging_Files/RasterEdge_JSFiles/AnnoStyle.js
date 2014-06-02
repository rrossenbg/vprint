var AnnoStyle = Class.create();
AnnoStyle.prototype = 
{

	initialize: function(options) 
	{
		this.SetOptions(options);

		this.OutLineColor = this.options.OutLineColor;
		this.OutLineWidth = this.options.OutLineWidth;
		this.FillColor = this.options.FillColor;
		this.ShowedText = this.options.ShowedText;	
		this.TextColor = this.options.TextColor;
		this.TextFont = this.options.TextFont;
		this.TextSize = this.options.TextSize;
		this.TextStyle = this.options.TextStyle;
		this.Transparency = this.options.Transparency;
	},
		
	SetOptions: function(options) 
	{
		this.options = 
		{
			OutLineColor:      "",
			OutLineWidth:      0,			
			FillColor:		   "",
			ShowedText:	       "",
			TextColor:         "",			
			TextFont:		   "",
			TextSize:	       0,
			TextStyle:	       "",
			Transparency:      0.4,
		};
		Extend(this.options, options || {});
    },
	
};