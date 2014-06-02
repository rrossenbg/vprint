using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;

public partial class OpenServerFile : System.Web.UI.Page
{
    private int _count;

    protected void Page_Load(object sender, EventArgs e)
    {      
        string rootPath = Server.MapPath("./RasterEdge_Demo_Docs");
                
        int index = 1;
        _count = 0;

        this.Panel1.Style.Add("margin", "20px 0px 20px 0px");
       
        drawFolder(rootPath, index);

    }
   
    private void drawFolder(string path, int index)
    {
        Panel panel = new Panel();
        this.Panel1.Controls.Add(panel);

        Image image = new Image();
        image.ImageUrl = "./RasterEdge_Imaging_Files/Folder.png";
        image.Width = 20;
        image.Height = 20;
        image.Style.Add("margin-left", index * 20 + "px");
        panel.Controls.Add(image);

        int length = path.Length - (path.LastIndexOf('\\') + 1);
        string name = path.Substring(path.LastIndexOf('\\') + 1, length);

        Label folder = new Label();
        folder.ID = path;
        folder.Text = name;
        panel.Controls.Add(folder);
      
        drawFile(path, index + 1);

        string[] dirs = Directory.GetDirectories(path);

        foreach (string dir in dirs)
        {
            drawFolder(dir, index + 1);
        }
    }

    private void drawFile(string path, int index)
    {
        string[] files = Directory.GetFiles(path);
        
        foreach (string file in files)
        {
            Panel panel = new Panel();
            this.Panel1.Controls.Add(panel);

            FileInfo fileDetail = new FileInfo(file);
            if (fileDetail.Extension.Equals(".pdf") || fileDetail.Extension.Equals(".tif") || fileDetail.Extension.Equals(".tiff"))
            {
                if (fileDetail.Extension.Equals(".pdf"))
                {
                    Image image = new Image();
                    image.ImageUrl = "./RasterEdge_Imaging_Files/pdf.jpg";
                    image.Width = 20;
                    image.Height = 20;
                    image.Style.Add("margin-left", index * 20 + "px");
                    panel.Controls.Add(image);
                }
                else
                {
                    Image image = new Image();
                    image.ImageUrl = "./RasterEdge_Imaging_Files/picture.png";
                    image.Width = 20;
                    image.Height = 20;
                    image.Style.Add("margin-left", index * 20 + "px");
                    panel.Controls.Add(image);
                }

                int length = file.Length - (file.LastIndexOf('\\') + 1);
                string name = file.Substring(file.LastIndexOf('\\') + 1, length);
               
                LinkButton linkFile = new LinkButton();
                //linkFile.ID = file;
                linkFile.ID = "file_" + _count++; 
                linkFile.Text = name;
                linkFile.ToolTip = file;
                linkFile.Click += new EventHandler(linkFile_Click);
                panel.Controls.Add(linkFile);
               
            }
        }
    }

    void linkFile_Click(object sender, EventArgs e)
    {
        LinkButton select = (LinkButton)sender;

        string fileName = select.ToolTip;
        string rootPath = Server.MapPath("./");

        fileName = fileName.Substring(rootPath.Length - 1);
                
        Response.Redirect("Default.aspx?filename=" + fileName);        
    }
  
}
