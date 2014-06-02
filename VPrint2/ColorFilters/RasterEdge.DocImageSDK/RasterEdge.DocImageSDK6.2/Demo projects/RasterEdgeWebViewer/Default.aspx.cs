using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Diagnostics;
using RasterEdge.Imaging.WebViewer;
using RasterEdge.Imaging.Basic;
using System.Windows.Forms;
using System.Threading;
using RasterEdge.Imaging.WebViewer.Enum;
using RasterEdge.Imaging.Basic.Core;
using RasterEdge.Imaging.PDF;
using RasterEdge.Imaging.TIFF;
using System.Web.Configuration;
using System.IO;
using RasterEdge.Imaging.MSWordDocx;


public partial class _Default : System.Web.UI.Page
{  
    public string ServerUrl;
    public string SessionId;
    public string OpenFileUrl;

    public float ThumbHeight = 100;
    public float ThumbWidth = 80;
    public float DocHeight = 640;
    public float DocWidth = 819;
    public float PageSizeType = 1;
    public string _cookieName = "RasterEdgeFilePath";
    RasterEdgeLoad _REload = new RasterEdgeLoad();

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ServerUrl = "/RasterEdge_Imaging_Files/WebHandler.ashx";
        this.SessionId = "RasterEdge" + Session.SessionID;
        this.OpenFileUrl = "OpenServerFile.aspx";
        string method = Request.Form["method"];

        if (method == null || method.Equals(""))
        {
            OpenFile();
        }
        else if (method.Equals("OpenFile"))
        {
            OpenFile();
        }
        else if (method.Equals("SaveFile"))
        {
            SaveFile();
        }
    }

   
    public void OpenFile()
    {
        string uploadFile = Request.Form["uploadfile"];

        if (uploadFile == null || uploadFile.Equals(""))
        {
            
            string fileName = Request.QueryString["filename"];
            if (fileName == null || fileName.Equals(""))
            {
                string initFilePath = getFilePath();
                this._REload.LoadFile(initFilePath, this.SessionId);
                //this._REload.LoadFile("/RasterEdge_Demo_Docs/Sample.pdf", this.SessionId);
            }
            else
            {
                this._REload.LoadFile(fileName, this.SessionId);
            }
        }
        else
        {
            string filename = "/RasterEdge_Cache/" + uploadFile;
            this._REload.LoadFile(filename, this.SessionId);

            //load from stream
            //this.REWebViewer1.LoadFile(stream, this.SessionId);
        }        
    }

    public void SaveFile()
    {
        REDocument doc = (REDocument)Session[this.SessionId + "Save"];

        if (doc != null)
        {
            REFile.SaveDocumentFile(doc, "h:/test.pdf", new PDFEncoder());
        }        
    }

    public string getFilePath()
    {
        string filePath = "/RasterEdge_Demo_Docs/Sample.pdf";
        try 
        {
            HttpCookie cookie = Request.Cookies.Get(_cookieName);
            if (cookie != null)
            {
                string value = cookie.Value;
                filePath = "/RasterEdge_Cache/" + value;
                string projectName = System.Web.HttpContext.Current.Request.PhysicalApplicationPath.Replace("\\", "/");
                string path = projectName + "RasterEdge_Cache/" + value;
                if(!File.Exists(path))
                    filePath = "/RasterEdge_Demo_Docs/Sample.pdf";
            }
        }
        catch (Exception ex) { }
        return filePath;
    }
}
